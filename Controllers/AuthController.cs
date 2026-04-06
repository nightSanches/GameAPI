using GameAPI.Classes;
using GameAPI.Interfaces;
using GameAPI.Models;
using GameAPI.Models.Authentification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DBConnection _context;
        private readonly IEmailService _emailService;

        public AuthController(DBConnection context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Валидация модели
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // 2. Поиск пользователя
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Nickname == request.NicknameOrEmail ||
                                          u.Email == request.NicknameOrEmail);

            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.Password))
            {
                var errors = new Dictionary<string, string[]>
                {
                    ["General"] = new[] { "Неверный логин или пароль." }
                };
                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Authentication failed"
                };
                return Unauthorized(problemDetails);
            }

            // 3. Успешная авторизация
            string newToken = GenerateRandomToken(100);
            user.Token = newToken;
            await _context.SaveChangesAsync();

            var response = new LoginResponse
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Token = newToken,
                Role = user.Role,
                Email = user.Email,
                RegistrationDate = user.RegistrationDate
            };

            return Ok(response);
        }

        [HttpPost("register")]
        [EnableRateLimiting("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. Валидация модели
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // 2. Проверка существования пользователя с таким никнеймом
            var existingUserByNickname = await _context.Users
                .FirstOrDefaultAsync(u => u.Nickname == request.Nickname);
            if (existingUserByNickname != null)
            {
                ModelState.AddModelError("Nickname", "Пользователь с таким никнеймом уже существует.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // 3. Если email указан, проверить его уникальность
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var confirmedUserWithEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.EmailConfirmed == true);
                if (confirmedUserWithEmail != null)
                {
                    ModelState.AddModelError("Email", "Пользователь с таким email уже существует.");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }
            }

            // 4. Создание нового пользователя
            var user = new User
            {
                Nickname = request.Nickname,
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email,
                Password = PasswordHasher.HashPassword(request.Password), // хэшируем
                Role = "player",
                RegistrationDate = DateTime.UtcNow,
                Token = null // пока нет токена, сгенерируем после сохранения
            };

            // Если email указан, генерируем токен подтверждения
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                user.EmailConfirmationToken = GenerateEmailConfirmationToken();
                user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            }

            // 5. Сохранение в БД
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 6. Генерация токена и обновление записи
            string newToken = GenerateRandomToken(100);
            user.Token = newToken;
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?token={user.EmailConfirmationToken}";
                await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);
            }

            // 7. Формирование ответа
            var response = new LoginResponse
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Token = newToken,
                Role = user.Role,
                Email = user.Email,
                RegistrationDate = user.RegistrationDate
            };

            // 8. Создание записей в других таблицах для нового пользователя
            var bonuses = new UserBonuses
            {
                UserId = user.Id
            };
            _context.UserBonuses.Add(bonuses);

            var gift = new UserGifts
            {
                UserId = user.Id
            };
            _context.UserGifts.Add(gift);

            var score = new UserScores
            {
                UserId = user.Id,
                BestScore = 0
            };
            _context.UserScores.Add(score);

            var upgrades = new UserUpgrades
            {
                UserId = user.Id
            };
            _context.UserUpgrades.Add(upgrades);

            await _context.SaveChangesAsync();

            return Ok(response);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token is required.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);

            if (user == null)
                return Content(GetErrorHtml("Недействительный токен подтверждения."), "text/html");

            if (user.EmailConfirmationTokenExpires < DateTime.UtcNow)
                return Content(GetErrorHtml("Срок действия ссылки истёк. Запросите новое письмо."), "text/html");

            if (user.EmailConfirmed)
                return Content(GetSuccessHtml("Email уже был подтверждён."), "text/html");

            // Проверяем, не подтвердил ли уже кто-то этот email
            var confirmedUserWithSameEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email && u.EmailConfirmed == true && u.Id != user.Id);
            if (confirmedUserWithSameEmail != null)
            {
                return Content(GetErrorHtml("Этот email уже подтверждён другим пользователем. Выберите другой email."), "text/html");
            }

            // Обнуляем email у других неподтверждённых пользователей с этим email
            var otherUnconfirmedUsers = await _context.Users
                .Where(u => u.Email == user.Email && u.EmailConfirmed == false && u.Id != user.Id)
                .ToListAsync();
            foreach (var other in otherUnconfirmedUsers)
            {
                other.Email = null;
                other.EmailConfirmationToken = null;
                other.EmailConfirmationTokenExpires = null;
            }

            // Подтверждаем email текущему пользователю
            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpires = null;
            await _context.SaveChangesAsync();

            // Отправляем уведомление о подтверждении (асинхронно, не блокируя ответ)
            try
            {
                await _emailService.SendEmailConfirmedNotificationAsync(user.Email);
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем процесс
                // Для продакшена можно использовать ILogger
                Console.WriteLine($"Failed to send confirmation notification: {ex.Message}");
            }

            return Content(GetSuccessHtml("Email успешно подтверждён!"), "text/html");
        }

        private string GetSuccessHtml(string message)
        {
            return $@"
    <!DOCTYPE html>
    <meta charset=""UTF-8"">
    <html lang=""ru"">
    <head>
        <title>Подтверждение email</title>
        <style>
            body {{ font-family: Arial, sans-serif; text-align: center; margin-top: 100px; }}
            .container {{ max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ccc; border-radius: 10px; background: #f9f9f9; }}
            h1 {{ color: #4CAF50; }}
            p {{ font-size: 18px; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h1>✅ Успешно</h1>
            <p>{message}</p>
        </div>
    </body>
    </html>";
        }

        private string GetErrorHtml(string message)
        {
            return $@"
    <!DOCTYPE html>
    <meta charset=""UTF-8"">
    <html lang=""ru"">
    <head>
        <title>Ошибка подтверждения</title>
        <style>
            body {{ font-family: Arial, sans-serif; text-align: center; margin-top: 100px; }}
            .container {{ max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ccc; border-radius: 10px; background: #f9f9f9; }}
            h1 {{ color: #f44336; }}
            p {{ font-size: 18px; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h1>❌ Ошибка</h1>
            <p>{message}</p>
        </div>
    </body>
    </html>";
        }

        [HttpPost("resend-confirmation")]
        [EnableRateLimiting("ResendConfirmation")]
        public async Task<IActionResult> ResendConfirmation(string authToken)
        {
            // Поиск пользователя по токену
            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { message = "Недействительный токен." });

            // Если email уже подтверждён
            if (user.EmailConfirmed)
                return BadRequest(new { message = "Email уже подтверждён." });

            // Если email не задан
            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest(new { message = "У вас не указан email." });

            // Генерируем новый токен и отправляем письмо
            user.EmailConfirmationToken = GenerateEmailConfirmationToken();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();

            var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?token={user.EmailConfirmationToken}";
            await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

            return Ok(new { message = "Письмо отправлено повторно. Проверьте почту." });
        }

        public class AddEmailRequest
        {
            [Required]
            [EmailAddress(ErrorMessage = "Неверный формат email.")]
            public string Email { get; set; }
        }

        [HttpPost("add-email")]
        public async Task<IActionResult> AddEmail(string authToken,
            [FromBody] AddEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { message = "Недействительный токен." });

            // Проверка, что email не подтверждён другим пользователем
            var confirmedUserWithEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.EmailConfirmed == true);
            if (confirmedUserWithEmail != null)
                return BadRequest(new { message = "Этот email уже подтверждён другим пользователем." });

            // Если у пользователя уже есть email и он не подтверждён, можно заменить
            // Если email уже подтверждён – запрещаем (но это не должно случиться, т.к. мы проверили выше)
            if (user.EmailConfirmed)
                return BadRequest(new { message = "Ваш email уже подтверждён." });

            // Обновляем email
            user.Email = request.Email;
            user.EmailConfirmed = false;
            user.EmailConfirmationToken = GenerateEmailConfirmationToken();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();

            // Отправляем письмо
            var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?token={user.EmailConfirmationToken}";
            await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

            return Ok(new { message = "Письмо с подтверждением отправлено на указанный email." });
        }

        private async Task<User> GetUserByToken(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
                return null;

            // Предполагаем, что токен передаётся как "Bearer <token>" или просто токен
            var token = authToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authToken.Substring(7)
                : authToken;

            return await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
        }

        private string GenerateRandomToken(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            // Перемешивание
            for (int i = 0; i < length; i++)
            {
                int swapIndex = random.Next(length);
                (chars[i], chars[swapIndex]) = (chars[swapIndex], chars[i]);
            }

            return new string(chars);
        }

        private string GenerateEmailConfirmationToken()
        {
            // Генерируем уникальный токен (Guid + случайные символы)
            var guid = Guid.NewGuid().ToString("N");
            var randomPart = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("+", "").Replace("/", "").Replace("=", ""); // убираем не-URL символы
            return guid + randomPart; // 32 + ~22 символов
        }
    }
}
