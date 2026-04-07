using GameAPI.Classes;
using GameAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserDataController : ControllerBase
    {
        private readonly DBConnection _context;

        public UserDataController(DBConnection context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все данные пользователя по токену
        /// </summary>
        [HttpGet("load")]
        public async Task<IActionResult> LoadData(string authorization)
        {
            if (string.IsNullOrEmpty(authorization))
                return Unauthorized(new { message = "Токен не предоставлен" });

            // 2. Найти пользователя по токену
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == authorization);
            if (user == null)
                return Unauthorized(new { message = "Неверный или просроченный токен" });

            // 3. Загрузить связанные данные
            var bonuses = await _context.UserBonuses.FirstOrDefaultAsync(b => b.UserId == user.Id);
            var cosmetics = await _context.UserCosmetics
                .Where(c => c.UserId == user.Id)
                .Select(c => c.CosmeticsId)
                .ToListAsync();
            var gifts = await _context.UserGifts.FirstOrDefaultAsync(g => g.UserId == user.Id);
            var scores = await _context.UserScores.FirstOrDefaultAsync(s => s.UserId == user.Id);
            var upgrades = await _context.UserUpgrades.FirstOrDefaultAsync(u => u.UserId == user.Id);

            // 4. Сформировать ответ (если какой-то записи нет – создать объект с значениями по умолчанию)
            var response = new UserDataResponse
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

            return Ok(response);
        }

        /// <summary>
        /// Сохранить (обновить) данные пользователя по токену
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveData(string authorization, [FromBody] UserDataSaveRequest request)
        {
            if (string.IsNullOrEmpty(authorization))
                return Unauthorized(new { message = "Токен не предоставлен" });

            // 2. Найти пользователя
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == authorization);
            if (user == null)
                return Unauthorized(new { message = "Неверный или просроченный токен" });

            // 3. Обновление данных (каждая секция опциональна)

            // ---------- Bonuses ----------
            if (request.Bonuses != null)
            {
                var bonuses = await _context.UserBonuses.FirstOrDefaultAsync(b => b.UserId == user.Id);
                if (bonuses != null)
                {
                    bonuses.BonusStabilizer = request.Bonuses.BonusStabilizer;
                    bonuses.BonusAlignment = request.Bonuses.BonusAlignment;
                    bonuses.BonusInsurance = request.Bonuses.BonusInsurance;
                }
                else
                {
                    // На случай, если записи нет (хотя при регистрации создаётся)
                    _context.UserBonuses.Add(new UserBonuses
                    {
                        UserId = user.Id,
                        BonusStabilizer = request.Bonuses.BonusStabilizer,
                        BonusAlignment = request.Bonuses.BonusAlignment,
                        BonusInsurance = request.Bonuses.BonusInsurance
                    });
                }
            }

            // ---------- Cosmetics (полная замена списка) ----------
            if (request.CosmeticsIds != null)
            {
                // Удалить все текущие записи косметики для пользователя
                var existingCosmetics = _context.UserCosmetics.Where(c => c.UserId == user.Id);
                _context.UserCosmetics.RemoveRange(existingCosmetics);

                // Добавить новые
                foreach (var cosmeticsId in request.CosmeticsIds)
                {
                    _context.UserCosmetics.Add(new UserCosmetics
                    {
                        UserId = user.Id,
                        CosmeticsId = cosmeticsId
                    });
                }
            }

            // ---------- Gifts ----------
            if (request.Gifts != null)
            {
                var gifts = await _context.UserGifts.FirstOrDefaultAsync(g => g.UserId == user.Id);
                if (gifts != null)
                {
                    gifts.LastBonusDT = request.Gifts.LastBonusDT;
                }
                else
                {
                    _context.UserGifts.Add(new UserGifts
                    {
                        UserId = user.Id,
                        LastBonusDT = request.Gifts.LastBonusDT
                    });
                }
            }

            // ---------- Scores ----------
            if (request.Scores != null)
            {
                var scores = await _context.UserScores.FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (scores != null)
                {
                    scores.BestScore = request.Scores.BestScore;
                }
                else
                {
                    _context.UserScores.Add(new UserScores
                    {
                        UserId = user.Id,
                        BestScore = request.Scores.BestScore
                    });
                }
            }

            // ---------- Upgrades ----------
            if (request.Upgrades != null)
            {
                var upgrades = await _context.UserUpgrades.FirstOrDefaultAsync(u => u.UserId == user.Id);
                if (upgrades != null)
                {
                    upgrades.Gold = request.Upgrades.Gold;
                    upgrades.Silver = request.Upgrades.Silver;
                    upgrades.UpCrane = request.Upgrades.UpCrane;
                    upgrades.UpBase = request.Upgrades.UpBase;
                    upgrades.UpExtraGold = request.Upgrades.UpExtraGold;
                    upgrades.UpExtraSilver = request.Upgrades.UpExtraSilver;
                    upgrades.UpExtraMul = request.Upgrades.UpExtraMul;
                }
                else
                {
                    _context.UserUpgrades.Add(new UserUpgrades
                    {
                        UserId = user.Id,
                        Gold = request.Upgrades.Gold,
                        Silver = request.Upgrades.Silver,
                        UpCrane = request.Upgrades.UpCrane,
                        UpBase = request.Upgrades.UpBase,
                        UpExtraGold = request.Upgrades.UpExtraGold,
                        UpExtraSilver = request.Upgrades.UpExtraSilver,
                        UpExtraMul = request.Upgrades.UpExtraMul
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Данные успешно сохранены" });
        }
    }
}
