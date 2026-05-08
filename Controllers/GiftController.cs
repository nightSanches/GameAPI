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

        public GiftController(DBConnection context)
        {
            _context = context;
        }

        [HttpPost("claim")]
        public async Task<IActionResult> Claim([FromQuery] string authToken)
        {
            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { message = "Недействительный токен." });

            if (!user.EmailConfirmed)
                return BadRequest(new { message = "Необходимо подтвердить email" });

            var gift = await _context.UserGifts.FirstOrDefaultAsync(g => g.UserId == user.Id);
            if (gift == null) return BadRequest("Запись о подарках не найдена");

            bool canClaim = gift.LastBonusDt == null ||
                (DateTime.UtcNow - gift.LastBonusDt.Value).TotalHours >= 24;

            if (!canClaim)
                return BadRequest(new { message = "Подарок можно получать раз в 24 часа" });

            // Начисляем случайные монеты (можно настроить)
            var random = new Random();
            int goldReward = random.Next(50, 150);

            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            wallet.Gold += goldReward;
            gift.LastBonusDt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            int secondsUntilNextGift = -1;
            if (user.EmailConfirmed && user.Gift != null)
            {
                if (user.Gift.LastBonusDt == null)
                    secondsUntilNextGift = 0; // можно забирать сразу
                else
                {
                    var nextTime = user.Gift.LastBonusDt.Value.AddHours(24);
                    var delta = nextTime - DateTime.UtcNow;
                    secondsUntilNextGift = delta.TotalSeconds > 0 ? (int)delta.TotalSeconds : 0;
                }
            }

            // Возвращаем обновлённый профиль (основные поля)
            var profile = new GiftResponse
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Token = user.Token,
                Role = user.Role,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                RegistrationDate = user.RegistrationDate,
                Gold = wallet.Gold,
                SecondsUntilNextGift = secondsUntilNextGift,
                GiftAvailable = false
                // Остальные поля не заполняем т.к. не нужны в данном ответе (или можно заполнить, если понадобится)
            };

            return Ok(profile);
        }

        private async Task<User> GetUserByToken(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken)) return null;
            var token = authToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authToken.Substring(7) : authToken;
            return await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
        }
    }
}
