using GameAPI.Classes;
using GameAPI.Interfaces;
using GameAPI.Models;
using GameAPI.Models.Authentification;
using GameAPI.Models.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using static GameAPI.Models.Authentification.FullLoginResponse;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DBConnection _context;
        private readonly IEmailService _emailService;

        private readonly IConfiguration _configuration;

        public AuthController(DBConnection context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
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

            var fullResponse = await BuildFullLoginResponse(user);
            return Ok(fullResponse);
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
                var publicBaseUrl = _configuration["AppSettings:PublicBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
                var confirmationLink = $"{publicBaseUrl.TrimEnd('/')}/api/auth/confirm-email?token={user.EmailConfirmationToken}";
                await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);
            }

            // Создание связанных записей в других таблицах
            await CreateDefaultUserData(user.Id);

            var fullResponse = await BuildFullLoginResponse(user);
            return Ok(fullResponse);
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
                return Unauthorized(new { status = -1, message = "Недействительный токен." });

            if (user.EmailConfirmed)
                return BadRequest(new { status = 2, message = "Email уже подтверждён." });

            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest(new { status = -2, message = "У вас не указан email." });

            if (user.EmailConfirmationTokenExpires.HasValue)
            {
                DateTime lastSendTime = user.EmailConfirmationTokenExpires.Value.AddHours(-24);
                double secondsSinceLastSend = (DateTime.UtcNow - lastSendTime).TotalSeconds;
                if (secondsSinceLastSend < 60)
                {
                    int retryAfter = 60 - (int)secondsSinceLastSend;
                    return BadRequest(new
                    {
                        status = 1,
                        message = $"Повторная отправка возможна через {retryAfter} сек.",
                        retryAfterSeconds = retryAfter
                    });
                }
            }

            user.EmailConfirmationToken = GenerateEmailConfirmationToken();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();

            var publicBaseUrl = _configuration["AppSettings:PublicBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
            var confirmationLink = $"{publicBaseUrl.TrimEnd('/')}/api/auth/confirm-email?token={user.EmailConfirmationToken}";
            await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

            return Ok(new { status = 0, message = "Письмо отправлено повторно. Проверьте почту." });
        }

        /// <summary>
        /// Добавление или смена email для текущего пользователя
        /// </summary>
        //[HttpPost("add-email")]
        //public async Task<IActionResult> AddEmail([FromQuery] string authToken, [FromBody] AddEmailRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(new ValidationProblemDetails(ModelState));

        //    var user = await GetUserByToken(authToken);
        //    if (user == null)
        //        return Unauthorized(new { message = "Недействительный токен." });

        //    // Проверяем, не подтверждён ли email другим пользователем
        //    var confirmedUserWithEmail = await _context.Users
        //        .FirstOrDefaultAsync(u => u.Email == request.Email && u.EmailConfirmed);
        //    if (confirmedUserWithEmail != null)
        //        return BadRequest(new { message = "Этот email уже подтверждён другим пользователем." });

        //    // Если email уже подтверждён у текущего пользователя, запрещаем менять
        //    if (user.EmailConfirmed)
        //        return BadRequest(new { message = "Ваш email уже подтверждён и не может быть изменён." });

        //    // Устанавливаем новый email
        //    user.Email = request.Email;
        //    user.EmailConfirmed = false;
        //    user.EmailConfirmationToken = GenerateEmailConfirmationToken();
        //    user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
        //    await _context.SaveChangesAsync();

        //    var publicBaseUrl = _configuration["AppSettings:PublicBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
        //    var confirmationLink = $"{publicBaseUrl.TrimEnd('/')}/api/auth/confirm-email?token={user.EmailConfirmationToken}";
        //    await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

        //    return Ok(new { message = "Письмо с подтверждением отправлено на указанный email." });
        //}

        // ==================== Вспомогательные методы ====================

        /// <summary>
        /// Создаёт все необходимые записи в связанных таблицах для нового пользователя
        /// </summary>
        private async Task CreateDefaultUserData(int userId)
        {
            // Кошелёк
            _context.UserWallets.Add(new UserWallet { UserId = userId, Money = 0, Reputation = 0 });

            // Счёт для каждого района
            var allDistricts = await _context.Districts.ToListAsync();
            foreach (var district in allDistricts)
            {
                _context.UserScores.Add(new UserScore 
                { 
                    UserId = userId, 
                    DistrictId = district.Id, 
                    BestScore = 0 
                });
            }

            // Подарки
            _context.UserGifts.Add(new UserGift { UserId = userId, LastBonusDt = null });

            // Статистика
            _context.UserStatss.Add(new UserStats { UserId = userId, GamesPlayedCount = 0, BlocksPlacedCount = 0, IBlocksPlacedCount = 0 });

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

            // Достижения – для каждого достижения создаём запись с прогрессом 0
            var allAchievements = await _context.Achievements.ToListAsync();
            foreach (var achievement in allAchievements)
            {
                _context.UserAchievements.Add(new UserAchievement
                {
                    UserId = userId,
                    AchievementId = achievement.Id,
                    CurrentProgress = 0,
                    IsUnlocked = false
                });
            }

            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Формирует полный профиль игрока для ответа
        /// </summary>
        private async Task<FullLoginResponse> BuildFullLoginResponse(User user)
        {
            // Подгружаем связанные данные, если они не включены
            if (user.Wallet == null)
                user.Wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            
            // Счета по всем районам
            var userScores = await _context.UserScores
                .Where(s => s.UserId == user.Id)
                .ToListAsync();
            
            if (user.Gift == null)
                user.Gift = await _context.UserGifts.FirstOrDefaultAsync(g => g.UserId == user.Id);

            // Статистика
            var stats = await _context.UserStatss.FirstOrDefaultAsync(s => s.UserId == user.Id);

            // Общий лучший рекорд среди всех районов
            int bestScoreOverall = userScores.Any() ? userScores.Max(s => s.BestScore) : 0;

            // Ранг игрока (dense rank) по лучшему рекорду
            int rank = 1;
            if (userScores.Any())
            {
                var allBestScores = await _context.UserScores
                    .GroupBy(s => s.UserId)
                    .Select(g => g.Max(s => s.BestScore))
                    .Distinct()
                    .OrderByDescending(s => s)
                    .ToListAsync();
                rank = allBestScores.FindIndex(s => s == bestScoreOverall) + 1;
            }

            // Бонусы
            var bonuses = await _context.UserBonuses
                .Where(b => b.UserId == user.Id)
                .Select(b => new UserBonusDto { BonusId = b.BonusId, Quantity = b.Quantity })
                .ToListAsync();

            // Улучшения
            var upgrades = await _context.UserUpgrades
                .Where(u => u.UserId == user.Id)
                .Select(u => new UserUpgradeDto { UpgradeId = u.UpgradeId, Level = u.Level })
                .ToListAsync();

            // Достижения
            var achievements = await _context.UserAchievements
                .Where(a => a.UserId == user.Id)
                .Select(a => new UserAchievementDto 
                { 
                    AchievementId = a.AchievementId, 
                    CurrentProgress = a.CurrentProgress,
                    IsUnlocked = a.IsUnlocked
                })
                .ToListAsync();

            // Районы (конфигурация)
            var districts = await _context.Districts
                .OrderBy(d => d.SortOrder)
                .Select(d => new DistrictDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    UnlockRepReq = d.UnlockRepReq,
                    DifficultyMultiplier = d.DifficultyMultiplier
                })
                .ToListAsync();

            // Доступность подарка: email подтверждён и прошло >=8 часа с последнего получения
            bool giftAvailable = user.EmailConfirmed && (user.Gift?.LastBonusDt == null ||
                (DateTime.UtcNow - user.Gift.LastBonusDt.Value).TotalHours >= 8);

            int secondsUntilNextGift = -1;
            if (user.EmailConfirmed && user.Gift != null)
            {
                if (user.Gift.LastBonusDt == null)
                    secondsUntilNextGift = 0; // можно забирать сразу
                else
                {
                    var nextTime = user.Gift.LastBonusDt.Value.AddHours(8);
                    var delta = nextTime - DateTime.UtcNow;
                    secondsUntilNextGift = delta.TotalSeconds > 0 ? (int)delta.TotalSeconds : 0;
                }
            }

            // Конфигурация магазина
            var storeConfig = new StoreConfigDto
            {
                Bonuses = await _context.Bonuses.Select(b => new BonusConfigDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    PriceMoney = b.PriceMoney
                }).ToListAsync(),
                UpgradeLevels = await _context.UpgradesCosts.Select(ul => new UpgradeLevelConfigDto
                {
                    UpgradeId = ul.UpgradeId,
                    Level = ul.Level,
                    PriceMoney = ul.PriceMoney
                }).ToListAsync(),
                Upgrades = await _context.Upgrades.Select(u => new UpgradeConfigDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Description = u.Description
                }).ToListAsync()
            };

            return new FullLoginResponse
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Token = user.Token,
                Role = user.Role,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                RegistrationDate = user.RegistrationDate,
                Money = user.Wallet?.Money ?? 0,
                Reputation = user.Wallet?.Reputation ?? 0,
                BestScore = bestScoreOverall,
                Rank = rank,
                GamesPlayed = stats?.GamesPlayedCount ?? 0,
                BlocksPlaced = stats?.BlocksPlacedCount ?? 0,
                PerfectBlocks = stats?.IBlocksPlacedCount ?? 0,
                Bonuses = bonuses,
                Upgrades = upgrades,
                Achievements = achievements,
                Districts = districts,
                SecondsUntilNextGift = secondsUntilNextGift,
                GiftAvailable = giftAvailable,
                StoreConfig = storeConfig
            };
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