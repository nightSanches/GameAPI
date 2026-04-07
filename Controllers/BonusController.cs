using GameAPI.Classes;
using GameAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BonusController : ControllerBase
    {
        private readonly DBConnection _context;

        public BonusController(DBConnection context)
        {
            _context = context;
        }

        /// <summary>
        /// Запрос на получение бонуса (раз в 8 часов, только при подтверждённом email)
        /// </summary>
        [HttpPost("claim")]
        public async Task<IActionResult> ClaimBonus(string authorization)
        {
            if (string.IsNullOrEmpty(authorization))
                return Unauthorized(new { message = "Токен не предоставлен" });

            // 2. Найти пользователя
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == authorization);
            if (user == null)
                return Unauthorized(new { message = "Неверный или просроченный токен" });

            // 3. Проверить подтверждение email
            if (!user.EmailConfirmed)   // предполагается свойство bool EmailConfirmed в модели User
            {
                return Ok(new BonusClaimResponse
                {
                    Success = false,
                    Message = "Email не подтверждён. Подтвердите email, чтобы получать бонусы."
                });
            }

            // 4. Получить запись о подарках/бонусах пользователя
            var userGifts = await _context.UserGifts.FirstOrDefaultAsync(g => g.UserId == user.Id);
            if (userGifts == null)
            {
                // Если записи нет (страховка – при регистрации она создаётся), создаём
                userGifts = new UserGifts { UserId = user.Id, LastBonusDT = null };
                _context.UserGifts.Add(userGifts);
                await _context.SaveChangesAsync();
            }

            // 5. Проверка времени
            DateTime nowUtc = DateTime.UtcNow;
            if (userGifts.LastBonusDT.HasValue)
            {
                var hoursSinceLast = (nowUtc - userGifts.LastBonusDT.Value).TotalHours;
                if (hoursSinceLast < 8)
                {
                    var hoursLeft = 8 - hoursSinceLast;
                    return Ok(new BonusClaimResponse
                    {
                        Success = false,
                        Message = $"Бонус пока недоступен. Попробуйте через {Math.Ceiling(hoursLeft)} час(ов).",
                        LastBonusDT = userGifts.LastBonusDT
                    });
                }
            }

            // 6. Выдать бонус: обновить дату последнего получения
            userGifts.LastBonusDT = nowUtc;
            await _context.SaveChangesAsync();

            // Здесь при необходимости можно добавить начисление ресурсов (золото, серебро и т.п.)

            return Ok(new BonusClaimResponse
            {
                Success = true,
                Message = "Бонус успешно получен!",
                LastBonusDT = nowUtc
            });
        }
    }
}
