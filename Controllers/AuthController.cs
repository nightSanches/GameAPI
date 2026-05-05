using GameAPI.Classes;
using GameAPI.Interfaces;
using GameAPI.Models;
using GameAPI.Models.Authentification;
using GameAPI.Models.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Вход в систему по никнейму или email
        /// </summary>
        [HttpPost("login")]
        [EnableRateLimiting("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Поиск пользователя по никнейму или email
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
                    Title = "Ошибка аутентификации"
                };
                return Unauthorized(problemDetails);
            }

            // Генерация нового токена сессии
            user.Token = GenerateRandomToken(100);
            await _context.SaveChangesAsync();

            var response = new LoginResponse
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Token = user.Token,
                Role = user.Role,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                RegistrationDate = user.RegistrationDate
            };

            return Ok(response);
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        [HttpPost("register")]
        [EnableRateLimiting("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Проверка уникальности никнейма
            var existingUserByNickname = await _context.Users
                .FirstOrDefaultAsync(u => u.Nickname == request.Nickname);
            if (existingUserByNickname != null)
            {
                ModelState.AddModelError("Nickname", "Пользователь с таким никнеймом уже существует.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Если указан email, проверяем, что он не подтверждён другим пользователем
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var confirmedUserWithEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.EmailConfirmed);
                if (confirmedUserWithEmail != null)
                {
                    ModelState.AddModelError("Email", "Пользователь с таким email уже существует.");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }
            }

            // Создание пользователя
            var user = new User
            {
                Nickname = request.Nickname,
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email,
                Password = PasswordHasher.HashPassword(request.Password),
                Role = "player",
                RegistrationDate = DateTime.UtcNow,
                Token = null // будет сгенерирован позже
            };

            // Генерация токена подтверждения, если email указан
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                user.EmailConfirmationToken = GenerateEmailConfirmationToken();
                user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            }

            // Сохраняем пользователя, чтобы получить Id
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Генерация токена сессии
            user.Token = GenerateRandomToken(100);
            await _context.SaveChangesAsync();

            // Отправка письма подтверждения, если email указан
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?token={user.EmailConfirmationToken}";
                await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);
            }

            // Создание связанных записей в других таблицах
            await CreateDefaultUserData(user.Id);

            var response = new LoginResponse
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Token = user.Token,
                Role = user.Role,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                RegistrationDate = user.RegistrationDate
            };

            return Ok(response);
        }

        /// <summary>
        /// Подтверждение email по токену из письма (возвращает HTML-страницу)
        /// </summary>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
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

            // Проверяем, не подтверждён ли email другим пользователем
            var confirmedUserWithSameEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email && u.EmailConfirmed && u.Id != user.Id);
            if (confirmedUserWithSameEmail != null)
            {
                return Content(GetErrorHtml("Этот email уже подтверждён другим пользователем. Выберите другой email."), "text/html");
            }

            // Обнуляем email у других неподтверждённых пользователей с этим email
            var otherUnconfirmedUsers = await _context.Users
                .Where(u => u.Email == user.Email && !u.EmailConfirmed && u.Id != user.Id)
                .ToListAsync();
            foreach (var other in otherUnconfirmedUsers)
            {
                other.Email = null;
                other.EmailConfirmationToken = null;
                other.EmailConfirmationTokenExpires = null;
            }

            // Подтверждаем email
            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpires = null;
            await _context.SaveChangesAsync();

            // Отправляем уведомление (fire-and-forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendEmailConfirmedNotificationAsync(user.Email);
                }
                catch { /* логирование ошибки */ }
            });

            return Content(GetSuccessHtml("Email успешно подтверждён!"), "text/html");
        }

        /// <summary>
        /// Повторная отправка письма подтверждения email
        /// </summary>
        [HttpPost("resend-confirmation")]
        [EnableRateLimiting("ResendConfirmation")]
        public async Task<IActionResult> ResendConfirmation([FromQuery] string authToken)
        {
            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { message = "Недействительный токен." });

            if (user.EmailConfirmed)
                return BadRequest(new { message = "Email уже подтверждён." });

            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest(new { message = "У вас не указан email." });

            user.EmailConfirmationToken = GenerateEmailConfirmationToken();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();

            var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?token={user.EmailConfirmationToken}";
            await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

            return Ok(new { message = "Письмо отправлено повторно. Проверьте почту." });
        }

        /// <summary>
        /// Добавление или смена email для текущего пользователя
        /// </summary>
        [HttpPost("add-email")]
        public async Task<IActionResult> AddEmail([FromQuery] string authToken, [FromBody] AddEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { message = "Недействительный токен." });

            // Проверяем, не подтверждён ли email другим пользователем
            var confirmedUserWithEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.EmailConfirmed);
            if (confirmedUserWithEmail != null)
                return BadRequest(new { message = "Этот email уже подтверждён другим пользователем." });

            // Если email уже подтверждён у текущего пользователя, запрещаем менять
            if (user.EmailConfirmed)
                return BadRequest(new { message = "Ваш email уже подтверждён и не может быть изменён." });

            // Устанавливаем новый email
            user.Email = request.Email;
            user.EmailConfirmed = false;
            user.EmailConfirmationToken = GenerateEmailConfirmationToken();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();

            var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?token={user.EmailConfirmationToken}";
            await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

            return Ok(new { message = "Письмо с подтверждением отправлено на указанный email." });
        }

        // ==================== Вспомогательные методы ====================

        /// <summary>
        /// Создаёт все необходимые записи в связанных таблицах для нового пользователя
        /// </summary>
        private async Task CreateDefaultUserData(int userId)
        {
            // Кошелёк
            _context.UserWallets.Add(new UserWallet { UserId = userId, Gold = 0, Silver = 0 });

            // Счёт
            _context.UserScores.Add(new UserScore { UserId = userId, BestScore = 0 });

            // Подарки
            _context.UserGifts.Add(new UserGift { UserId = userId, LastBonusDt = null });

            // Бонусы – для каждого бонуса из справочника создаём запись с количеством 0
            var allBonuses = await _context.Bonuses.ToListAsync();
            foreach (var bonus in allBonuses)
            {
                _context.UserBonuses.Add(new UserBonus
                {
                    UserId = userId,
                    BonusId = bonus.Id,
                    Quantity = 0
                });
            }

            // Улучшения – для каждого улучшения создаём запись с уровнем 0
            var allUpgrades = await _context.Upgrades.ToListAsync();
            foreach (var upgrade in allUpgrades)
            {
                _context.UserUpgrades.Add(new UserUpgrade
                {
                    UserId = userId,
                    UpgradeId = upgrade.Id,
                    Level = 0
                });
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Получение пользователя по токену (поддерживает "Bearer ..." или чистый токен)
        /// </summary>
        private async Task<User?> GetUserByToken(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
                return null;

            // Убираем префикс "Bearer " если есть
            var token = authToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authToken.Substring(7)
                : authToken;

            return await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
        }

        // Генерация случайного токена (из примера)
        private string GenerateRandomToken(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = validChars[random.Next(validChars.Length)];
            // Перемешивание
            for (int i = 0; i < length; i++)
            {
                int swapIndex = random.Next(length);
                (chars[i], chars[swapIndex]) = (chars[swapIndex], chars[i]);
            }
            return new string(chars);
        }

        // Генерация токена подтверждения email (из примера)
        private string GenerateEmailConfirmationToken()
        {
            var guid = Guid.NewGuid().ToString("N");
            var randomPart = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("+", "").Replace("/", "").Replace("=", "");
            return guid + randomPart;
        }

        // HTML-страницы для подтверждения email
        private string GetSuccessHtml(string message) => $@"
<!DOCTYPE html>
<meta charset=""UTF-8"">
<html lang=""ru"">
<head><title>Подтверждение email</title>
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

        private string GetErrorHtml(string message) => $@"
<!DOCTYPE html>
<meta charset=""UTF-8"">
<html lang=""ru"">
<head><title>Ошибка подтверждения</title>
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
}