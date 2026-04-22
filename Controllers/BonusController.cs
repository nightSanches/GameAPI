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

        [HttpPost("claim")]
        public async Task<IActionResult> ClaimBonus(string authorization)
        {
            if (string.IsNullOrEmpty(authorization))
                return Unauthorized(new { message = "Токен не предоставлен" });

            //найти пользователя по токену
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == authorization);
            if (user == null)
                return Unauthorized(new { message = "Неверный токен" });

            //проверить подтверждение email
            if (!user.EmailConfirmed)
            {
                return Ok(new BonusClaimResponse
                {
                    Success = false,
                    Message = "Для получения бонуса необходимо подтвердить email."
                });
            }

            //получить или создать запись в UserGifts
            var gifts = await _context.UserGifts.FirstOrDefaultAsync(g => g.UserId == user.Id);
            if (gifts == null)
            {
                gifts = new UserGifts { UserId = user.Id, LastBonusDT = null };
                _context.UserGifts.Add(gifts);
                await _context.SaveChangesAsync(); // сохраняем, чтобы иметь Id
            }

            //проверить, можно ли получить бонус (последний раз был null или прошло >= 8 часов)
            bool canClaim = false;
            if (gifts.LastBonusDT == null)
            {
                canClaim = true;
            }
            else
            {
                var hoursSinceLast = (DateTime.UtcNow - gifts.LastBonusDT.Value).TotalHours;
                if (hoursSinceLast >= 8)
                    canClaim = true;
            }

            if (!canClaim)
            {
                return Ok(new BonusClaimResponse
                {
                    Success = false,
                    Message = "Бонус ещё не доступен. Попробуйте позже."
                });
            }

            //сгенерировать случайные ресурсы
            int goldEarned = Random.Shared.Next(10, 31);   // 10..30
            int silverEarned = Random.Shared.Next(3, 11); // 3..10

            //получить или создать запись в UserUpgrades
            var upgrades = await _context.UserUpgrades.FirstOrDefaultAsync(u => u.UserId == user.Id);
            if (upgrades == null)
            {
                upgrades = new UserUpgrades
                {
                    UserId = user.Id,
                    Gold = 0,
                    Silver = 0,
                    UpCrane = 0,
                    UpBase = 0,
                    UpExtraGold = 0,
                    UpExtraSilver = 0,
                    UpExtraMul = 0
                };
                _context.UserUpgrades.Add(upgrades);
            }

            //начислить ресурсы и обновить дату получения
            upgrades.Gold += goldEarned;
            upgrades.Silver += silverEarned;
            gifts.LastBonusDT = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            //получить актуальные данные пользователя (аналогично методу LoadData)
            var userData = await GetUserDataResponse(user.Id);

            return Ok(new BonusClaimResponse
            {
                Success = true,
                GoldEarned = goldEarned,
                SilverEarned = silverEarned,
                UserData = userData
            });
        }

        /// <summary>
        /// Формирует полный ответ с данными пользователя (используется в предыдущем контроллере)
        /// </summary>
        private async Task<UserDataResponse> GetUserDataResponse(int userId)
        {
            var bonuses = await _context.UserBonuses.FirstOrDefaultAsync(b => b.UserId == userId);
            var cosmetics = await _context.UserCosmetics
                .Where(c => c.UserId == userId)
                .Select(c => c.CosmeticsId)
                .ToListAsync();
            var gifts = await _context.UserGifts.FirstOrDefaultAsync(g => g.UserId == userId);
            var scores = await _context.UserScores.FirstOrDefaultAsync(s => s.UserId == userId);
            var upgrades = await _context.UserUpgrades.FirstOrDefaultAsync(u => u.UserId == userId);

            return new UserDataResponse
            {
                Bonuses = bonuses != null
                    ? new UserBonusesDto
                    {
                        BonusStabilizer = bonuses.BonusStabilizer,
                        BonusAlignment = bonuses.BonusAlignment,
                        BonusInsurance = bonuses.BonusInsurance
                    }
                    : new UserBonusesDto { BonusStabilizer = 0, BonusAlignment = 0, BonusInsurance = 0 },

                CosmeticsIds = cosmetics,

                Gifts = gifts != null
                    ? new UserGiftsDto { LastBonusDT = gifts.LastBonusDT }
                    : new UserGiftsDto { LastBonusDT = null },

                Scores = scores != null
                    ? new UserScoresDto { BestScore = scores.BestScore }
                    : new UserScoresDto { BestScore = 0 },

                Upgrades = upgrades != null
                    ? new UserUpgradesDto
                    {
                        Gold = upgrades.Gold,
                        Silver = upgrades.Silver,
                        UpCrane = upgrades.UpCrane,
                        UpBase = upgrades.UpBase,
                        UpExtraGold = upgrades.UpExtraGold,
                        UpExtraSilver = upgrades.UpExtraSilver,
                        UpExtraMul = upgrades.UpExtraMul
                    }
                    : new UserUpgradesDto
                    {
                        Gold = 0,
                        Silver = 0,
                        UpCrane = 0,
                        UpBase = 0,
                        UpExtraGold = 0,
                        UpExtraSilver = 0,
                        UpExtraMul = 0
                    }
            };
        }
    }
}