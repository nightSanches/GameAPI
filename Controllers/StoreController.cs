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

        public StoreController(DBConnection context)
        {
            _context = context;
        }

        [HttpPost("buy")]
        public async Task<IActionResult> Buy([FromQuery] string authToken, [FromBody] PurchaseRequest request)
        {
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
                    if (wallet.Gold < bonus.PriceGold) return BadRequest("Недостаточно золота");

                    wallet.Gold -= bonus.PriceGold;
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

                case "cosmetic":
                    var cosmetic = await _context.Cosmetics.FindAsync(request.ItemId);
                    if (cosmetic == null) return NotFound("Скин не найден");
                    if (wallet.Silver < cosmetic.PriceSilver) return BadRequest("Недостаточно серебра");

                    wallet.Silver -= cosmetic.PriceSilver;
                    bool alreadyOwned = await _context.UserCosmetics
                        .AnyAsync(c => c.UserId == user.Id && c.CosmeticId == request.ItemId);
                    if (alreadyOwned) return BadRequest("У вас уже есть этот скин");
                    _context.UserCosmetics.Add(new UserCosmetic { UserId = user.Id, CosmeticId = request.ItemId });
                    break;

                case "upgrade":
                    var upgradeLevel = await _context.UpgradesCosts
                        .FirstOrDefaultAsync(l => l.UpgradeId == request.ItemId && l.Level == request.Level);
                    if (upgradeLevel == null) return NotFound("Уровень улучшения не найден");
                    if (wallet.Gold < upgradeLevel.PriceGold || wallet.Silver < upgradeLevel.PriceSilver)
                        return BadRequest("Недостаточно валюты");

                    var userUpgrade = await _context.UserUpgrades
                        .FirstOrDefaultAsync(u => u.UserId == user.Id && u.UpgradeId == request.ItemId);
                    if (userUpgrade == null) return BadRequest("Улучшение не найдено у пользователя");
                    if (request.Level != userUpgrade.Level + 1)
                        return BadRequest("Нельзя перескочить уровень");

                    wallet.Gold -= upgradeLevel.PriceGold;
                    wallet.Silver -= upgradeLevel.PriceSilver;
                    userUpgrade.Level = request.Level;
                    break;

                default:
                    return BadRequest("Неизвестный тип предмета");
            }

            await _context.SaveChangesAsync();

            // Формируем обновлённый профиль (только нужные поля)
            var response = new PurchaseResponse
            {
                Gold = wallet.Gold,
                Silver = wallet.Silver,
                Bonuses = await _context.UserBonuses
                    .Where(b => b.UserId == user.Id)
                    .Select(b => new UserBonusDto { BonusId = b.BonusId, Quantity = b.Quantity })
                    .ToListAsync(),
                Upgrades = await _context.UserUpgrades
                    .Where(u => u.UserId == user.Id)
                    .Select(u => new UserUpgradeDto { UpgradeId = u.UpgradeId, Level = u.Level })
                    .ToListAsync(),
                OwnedSkinIds = await _context.UserCosmetics
                    .Where(c => c.UserId == user.Id)
                    .Select(c => c.CosmeticId)
                    .ToListAsync()
            };

            return Ok(response);
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
