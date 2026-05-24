using GameAPI.Models.Authentification;
using GameAPI.Models;
using Microsoft.AspNetCore.Mvc;
using GameAPI.Classes;
using Microsoft.EntityFrameworkCore;
using GameAPI.Models.Gift;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiftController : ControllerBase
    {
        private readonly DBConnection _context;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера подарков.
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public GiftController(DBConnection context)
        {
            _context = context;
        }

        /// <summary>
        /// Позволяет пользователю получить ежедневный подарок (бонусные монеты).
        /// Подарок доступен раз в 8 часов только для пользователей с подтвержденным email.
        /// </summary>
        /// <returns>Обновленный профиль пользователя с новым балансом и временем до следующего подарка</returns>
        [HttpPost("claim")]
        public async Task<IActionResult> Claim()
        {
            var authToken = ExtractAuthToken();
            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { status = -1, message = "Недействительный токен." });

            if (!user.EmailConfirmed)
                return Ok(new { status = 2, message = "Необходимо подтвердить email" });

            var gift = await _context.UserGifts.FirstOrDefaultAsync(g => g.UserId == user.Id);
            if (gift == null)
                return BadRequest(new { status = -2, message = "Запись о подарках не найдена" });

            bool canClaim = gift.LastBonusDt == null ||
                (DateTime.UtcNow - gift.LastBonusDt.Value).TotalHours >= 8;

            if (!canClaim)
                return Ok(new { status = 1, message = "Подарок можно получать раз в 8 часов" });

            // Начисляем монеты
            var random = new Random();
            int moneyReward = random.Next(50, 150);

            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            wallet.Money += moneyReward;
            gift.LastBonusDt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            int secondsUntilNextGift = 0;
            if (user.EmailConfirmed && gift.LastBonusDt != null)
            {
                var nextTime = gift.LastBonusDt.Value.AddHours(8);
                var delta = nextTime - DateTime.UtcNow;
                secondsUntilNextGift = delta.TotalSeconds > 0 ? (int)delta.TotalSeconds : 0;
            }

            var profile = new GiftResponse
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Token = user.Token,
                Role = user.Role,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                RegistrationDate = user.RegistrationDate,
                Money = wallet.Money,
                SecondsUntilNextGift = secondsUntilNextGift,
                GiftAvailable = false
            };

            return Ok(new { status = 0, data = profile });
        }

        /// <summary>
        /// Извлекает токен из заголовка Authorization.
        /// </summary>
        private string ExtractAuthToken()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader))
                return null;
            return authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader.Substring(7).Trim()
                : authHeader.Trim();
        }

        /// <summary>
        /// Получает пользователя по токену аутентификации.
        /// </summary>
        /// <param name="authToken">Токен аутентификации</param>
        /// <returns>Объект пользователя или null, если токен недействителен</returns>
        private async Task<User> GetUserByToken(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken)) return null;
            return await _context.Users.FirstOrDefaultAsync(u => u.Token == authToken);
        }
    }
}
