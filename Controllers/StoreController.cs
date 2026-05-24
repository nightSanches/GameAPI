using GameAPI.Models.Shop;
using GameAPI.Models;
using Microsoft.AspNetCore.Mvc;
using static GameAPI.Models.Authentification.FullLoginResponse;
using GameAPI.Classes;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly DBConnection _context;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера магазина.
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public StoreController(DBConnection context)
        {
            _context = context;
        }

        /// <summary>
        /// Выполняет покупку предмета в магазине (бонус или улучшение).
        /// Проверяет баланс, списывает средства и добавляет предмет пользователю.
        /// </summary>
        /// <param name="request">Данные о покупке (тип предмета, ID, уровень для улучшений)</param>
        /// <returns>Обновленные данные кошелька, бонусов и улучшений пользователя</returns>
        [HttpPost("buy")]
        public async Task<IActionResult> Buy([FromBody] PurchaseRequest request)
        {
            var authToken = ExtractAuthToken();
            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { message = "Недействительный токен." });

            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (wallet == null) return BadRequest("Кошелёк не найден");

            // Выполняем покупку в зависимости от типа предмета
            switch (request.ItemType.ToLower())
            {
                case "bonus":
                    var bonus = await _context.Bonuses.FindAsync(request.ItemId);
                    if (bonus == null) return NotFound("Бонус не найден");
                    if (wallet.Money < bonus.PriceMoney) return BadRequest("Недостаточно монет");

                    wallet.Money -= bonus.PriceMoney;
                    var userBonus = await _context.UserBonuses
                        .FirstOrDefaultAsync(b => b.UserId == user.Id && b.BonusId == request.ItemId);
                    if (userBonus == null)
                    {
                        userBonus = new UserBonus { UserId = user.Id, BonusId = request.ItemId, Quantity = 1 };
                        _context.UserBonuses.Add(userBonus);
                    }
                    else
                    {
                        userBonus.Quantity++;
                    }
                    break;

                case "upgrade":
                    var upgradeLevel = await _context.UpgradesCosts
                        .FirstOrDefaultAsync(l => l.UpgradeId == request.ItemId && l.Level == request.Level);
                    if (upgradeLevel == null) return NotFound("Уровень улучшения не найден");
                    if (wallet.Money < upgradeLevel.PriceMoney)
                        return BadRequest("Недостаточно валюты");

                    var userUpgrade = await _context.UserUpgrades
                        .FirstOrDefaultAsync(u => u.UserId == user.Id && u.UpgradeId == request.ItemId);
                    if (userUpgrade == null) return BadRequest("Улучшение не найдено у пользователя");
                    if (request.Level != userUpgrade.Level + 1)
                        return BadRequest("Нельзя перескочить уровень");

                    wallet.Money -= upgradeLevel.PriceMoney;
                    userUpgrade.Level = request.Level;
                    break;

                default:
                    return BadRequest("Неизвестный тип предмета");
            }

            await _context.SaveChangesAsync();

            // Формируем обновлённый профиль (только нужные поля)
            var response = new PurchaseResponse
            {
                Money = wallet.Money,
                Bonuses = await _context.UserBonuses
                    .Where(b => b.UserId == user.Id)
                    .Select(b => new UserBonusDto { BonusId = b.BonusId, Quantity = b.Quantity })
                    .ToListAsync(),
                Upgrades = await _context.UserUpgrades
                    .Where(u => u.UserId == user.Id)
                    .Select(u => new UserUpgradeDto { UpgradeId = u.UpgradeId, Level = u.Level })
                    .ToListAsync()
            };

            return Ok(response);
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
