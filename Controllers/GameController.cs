using GameAPI.Classes;
using GameAPI.Models;
using GameAPI.Models.Game;
using GameAPI.Models.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static GameAPI.Models.Authentification.FullLoginResponse;

using ScoreByDistrictDto = GameAPI.Models.Authentification.FullLoginResponse.ScoreByDistrictDto;
using DistrictRankDto = GameAPI.Models.Authentification.FullLoginResponse.DistrictRankDto;
using UserAchievementDto = GameAPI.Models.Authentification.FullLoginResponse.UserAchievementDto;
using Newtonsoft.Json;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly DBConnection _context;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера игры.
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public GameController(DBConnection context)
        {
            _context = context;
        }

        /// <summary>
        /// Обрабатывает завершение игровой сессии: обновляет кошелек, рекорды, статистику и достижения игрока.
        /// Возвращает обновленные данные игрока включая ранги по всем районам.
        /// </summary>
        /// <param name="request">Данные о завершении игры (счет, заработанные ресурсы, прогресс достижений)</param>
        /// <returns>Обновленные данные игрока после завершения игры</returns>
        [HttpPost("end")]
        public async Task<IActionResult> EndGame([FromBody] GameEndRequest request)
        {
            var authToken = ExtractAuthToken();
            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { message = "Недействительный токен." });

            // Обновляем кошелёк (Money и Reputation)
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (wallet == null) return BadRequest("Кошелёк не найден");
            wallet.Money += request.MoneyEarned;

            // Обновляем рекорд для указанного района
            var userScore = await _context.UserScores
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.DistrictId == request.DistrictId);
            bool isNewRecord = false;
            if (userScore != null && request.Score > userScore.BestScore)
            {
                userScore.BestScore = request.Score;
                isNewRecord = true;
            }

            // Обновляем статистику
            var stats = await _context.UserStatss.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (stats == null)
            {
                stats = new UserStats { UserId = user.Id };
                _context.UserStatss.Add(stats);
            }
            stats.GamesPlayedCount++;
            stats.BlocksPlacedCount += request.BlocksPlaced;
            stats.IBlocksPlacedCount += request.PerfectBlocks;

            // Обновляем прогресс достижений через отдельный метод
            await UpdateAchievementsAsync(user.Id, request.DistrictId, request.AchievementProgresses, stats, wallet);

            // Обрабатываем использованные бонусы
            if (request.UsedBonuses != null && request.UsedBonuses.Count > 0)
            {
                foreach (var kvp in request.UsedBonuses)
                {
                    int bonusId = kvp.Key;
                    int usedQuantity = kvp.Value;
                    if (usedQuantity <= 0) continue;

                    var userBonus = await _context.UserBonuses
                        .FirstOrDefaultAsync(ub => ub.UserId == user.Id && ub.BonusId == bonusId);

                    if (userBonus != null)
                    {
                        userBonus.Quantity = Math.Max(0, userBonus.Quantity - usedQuantity);
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Получаем актуальные счета по районам и вычисляем ранги для каждого района
            var allDistricts = await _context.Districts.ToListAsync();
            var scoresByDistrict = new List<ScoreByDistrictDto>();
            var districtRanks = new List<DistrictRankDto>();

            foreach (var district in allDistricts)
            {
                var userScoreRecord = await _context.UserScores
                    .FirstOrDefaultAsync(s => s.UserId == user.Id && s.DistrictId == district.Id);
                var bestScoreForDistrict = userScoreRecord?.BestScore ?? 0;

                scoresByDistrict.Add(new ScoreByDistrictDto
                {
                    DistrictId = district.Id,
                    BestScore = bestScoreForDistrict
                });

                // Вычисляем dense rank для этого района
                int rankForDistrict = 1;
                if (bestScoreForDistrict > 0)
                {
                    var allScoresForDistrict = await _context.UserScores
                        .Where(s => s.DistrictId == district.Id)
                        .OrderByDescending(s => s.BestScore)
                        .Select(s => s.BestScore)
                        .Distinct()
                        .ToListAsync();
                    rankForDistrict = allScoresForDistrict.TakeWhile(s => s > bestScoreForDistrict).Count() + 1;
                }
                else
                {
                    var hasAnyScore = await _context.UserScores
                        .Where(s => s.DistrictId == district.Id && s.BestScore > 0)
                        .AnyAsync();
                    rankForDistrict = hasAnyScore 
                        ? await _context.UserScores.Where(s => s.DistrictId == district.Id).Select(s => s.BestScore).Distinct().CountAsync() + 1 
                        : 1;
                }

                districtRanks.Add(new DistrictRankDto
                {
                    DistrictId = district.Id,
                    BestScore = bestScoreForDistrict,
                    Rank = rankForDistrict
                });
            }

            // Возвращаем обновлённые данные
            var response = new GameEndResponse
            {
                Money = wallet.Money,
                Reputation = wallet.Reputation,
                GamesPlayed = stats.GamesPlayedCount,
                BlocksPlaced = stats.BlocksPlacedCount,
                PerfectBlocks = stats.IBlocksPlacedCount,
                IsNewRecord = isNewRecord,
                ScoresByDistrict = scoresByDistrict,
                DistrictRanks = districtRanks,
                Achievements = await _context.UserAchievements
                    .Where(ua => ua.UserId == user.Id)
                    .Select(ua => new UserAchievementDto
                    {
                        AchievementId = ua.AchievementId,
                        CurrentProgress = ua.CurrentProgress,
                        IsUnlocked = ua.IsUnlocked
                    })
                    .ToListAsync(),
                Bonuses = await _context.UserBonuses
                    .Where(ub => ub.UserId == user.Id)
                    .Select(ub => new Models.Game.UserBonusDto
                    {
                        BonusId = ub.BonusId,
                        Quantity = ub.Quantity
                    })
                    .ToListAsync()
            };

            return Ok(response);
        }

        /// <summary>
        /// Сохраняет прогресс достижений без завершения игровой сессии.
        /// Используется при выходе через окно паузы.
        /// Не обновляет кошелёк, рекорды или статистику — только достижения.
        /// Также обрабатывает использованные бонусы, списывая их из БД.
        /// </summary>
        [HttpPost("save-achievements")]
        public async Task<IActionResult> SaveAchievements([FromBody] SaveAchievementsRequest request)
        {
            var authToken = ExtractAuthToken();
            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { message = "Недействительный токен." });

            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (wallet == null) return BadRequest("Кошелёк не найден");

            var stats = await _context.UserStatss.FirstOrDefaultAsync(s => s.UserId == user.Id);

            // Обновляем прогресс достижений через существующий метод
            await UpdateAchievementsAsync(user.Id, request.DistrictId, request.AchievementProgresses, stats, wallet);

            // Обрабатываем использованные бонусы (списываем их)
            if (request.UsedBonuses != null && request.UsedBonuses.Count > 0)
            {
                foreach (var kvp in request.UsedBonuses)
                {
                    int bonusId = kvp.Key;
                    int usedQuantity = kvp.Value;
                    if (usedQuantity <= 0) continue;
                    var userBonus = await _context.UserBonuses
                        .FirstOrDefaultAsync(ub => ub.UserId == user.Id && ub.BonusId == bonusId);
                    if (userBonus != null)
                    {
                        userBonus.Quantity = Math.Max(0, userBonus.Quantity - usedQuantity);
                    }
                }
            }


            await _context.SaveChangesAsync();

            // Возвращаем обновлённый список достижений
            var achievements = await _context.UserAchievements
                .Where(ua => ua.UserId == user.Id)
                .Select(ua => new UserAchievementDto
                {
                    AchievementId = ua.AchievementId,
                    CurrentProgress = ua.CurrentProgress,
                    IsUnlocked = ua.IsUnlocked
                })
                .ToListAsync();

            // Возвращаем обновлённый список бонусов (после списания использованных)
            var bonuses = await _context.UserBonuses
                .Where(ub => ub.UserId == user.Id)
                .Select(ub => new Models.Authentification.FullLoginResponse.UserBonusDto
                {
                    BonusId = ub.BonusId,
                    Quantity = ub.Quantity
                })
                .ToListAsync();

            return Ok(new SaveAchievementsResponse
            {
                Achievements = achievements,
                Reputation = wallet.Reputation,
                Bonuses = bonuses
            });
        }

        /// <summary>
        /// Синхронизирует отложенные запросы, отправленные клиентом в офлайн-режиме.
        /// Принимает массив запросов и обрабатывает их последовательно.
        /// Используется при восстановлении соединения после игры без интернета.
        /// </summary>
        [HttpPost("sync-offline")]
        public async Task<IActionResult> SyncOfflineRequests([FromBody] List<OfflineSyncRequest> requests)
        {
            var authToken = ExtractAuthToken();
            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { message = "Недействительный токен." });
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (wallet == null) return BadRequest("Кошелёк не найден");
            var stats = await _context.UserStatss.FirstOrDefaultAsync(s => s.UserId == user.Id);
            var errors = new List<string>();
            foreach (var request in requests)
            {
                try
                {
                    switch (request.RequestType)
                    {
                        case "GameEnd":
                            await ProcessGameEndRequest(user, wallet, stats, request);
                            break;
                        case "SaveAchievements":
                            await ProcessSaveAchievementsRequest(user, wallet, stats, request);
                            break;
                        case "Purchase":
                            await ProcessPurchaseRequest(user, wallet, request);
                            break;
                        default:
                            errors.Add($"Unknown request type: {request.RequestType}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to process {request.RequestType}: {ex.Message}");
                }
            }
            await _context.SaveChangesAsync();
            return Ok(new { processed = requests.Count - errors.Count, errors });
        }
        private async Task ProcessGameEndRequest(User user, UserWallet wallet, UserStats stats, OfflineSyncRequest request)
        {
            var gameEndData = JsonConvert.DeserializeObject<GameEndRequest>(request.JsonBody);
            if (gameEndData == null) return;
            // Обновляем кошелёк
            wallet.Money += gameEndData.MoneyEarned;
            // Обновляем рекорд для указанного района
            var userScore = await _context.UserScores
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.DistrictId == gameEndData.DistrictId);
            if (userScore != null && gameEndData.Score > userScore.BestScore)
            {
                userScore.BestScore = gameEndData.Score;
            }
            // Обновляем статистику
            if (stats != null)
            {
                stats.GamesPlayedCount++;
                stats.BlocksPlacedCount += gameEndData.BlocksPlaced;
                stats.IBlocksPlacedCount += gameEndData.PerfectBlocks;
            }
            // Обновляем прогресс достижений
            await UpdateAchievementsAsync(user.Id, gameEndData.DistrictId, gameEndData.AchievementProgresses, stats, wallet);
            // Обрабатываем использованные бонусы
            if (gameEndData.UsedBonuses != null && gameEndData.UsedBonuses.Count > 0)
            {
                foreach (var kvp in gameEndData.UsedBonuses)
                {
                    int bonusId = kvp.Key;
                    int usedQuantity = kvp.Value;
                    if (usedQuantity <= 0) continue;
                    var userBonus = await _context.UserBonuses
                        .FirstOrDefaultAsync(ub => ub.UserId == user.Id && ub.BonusId == bonusId);
                    if (userBonus != null)
                    {
                        userBonus.Quantity = Math.Max(0, userBonus.Quantity - usedQuantity);
                    }
                }
            }
        }
        private async Task ProcessSaveAchievementsRequest(User user, UserWallet wallet, UserStats stats, OfflineSyncRequest request)
        {
            var saveData = JsonConvert.DeserializeObject<SaveAchievementsRequest>(request.JsonBody);
            if (saveData == null) return;
            await UpdateAchievementsAsync(user.Id, saveData.DistrictId, saveData.AchievementProgresses, stats, wallet);
            // Обрабатываем использованные бонусы (списываем их)
            if (saveData.UsedBonuses != null && saveData.UsedBonuses.Count > 0)
            {
                foreach (var kvp in saveData.UsedBonuses)
                {
                    int bonusId = kvp.Key;
                    int usedQuantity = kvp.Value;
                    if (usedQuantity <= 0) continue;
                    var userBonus = await _context.UserBonuses
                        .FirstOrDefaultAsync(ub => ub.UserId == user.Id && ub.BonusId == bonusId);
                    if (userBonus != null)
                    {
                        userBonus.Quantity = Math.Max(0, userBonus.Quantity - usedQuantity);
                    }
                }
            }
        }

        /// <summary>
        /// Обновляет прогресс достижений игрока на основе переданных значений прогресса.
        /// Метод работает с любыми записями достижений, используя тип условия (ConditionType) для определения логики обновления.
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="districtId">ID района, для которого обновляются достижения</param>
        /// <param name="achievementProgresses">Словарь с прогрессом по типам условий (ключ - ConditionType, значение - текущий прогресс)</param>
        /// <param name="stats">Статистика пользователя</param>
        /// <param name="wallet">Кошелёк пользователя (для начисления награды при разблокировке достижения)</param>
        private async Task UpdateAchievementsAsync(int userId, int districtId, Dictionary<string, int> achievementProgresses, UserStats stats, UserWallet wallet)
        {
            // Получаем все достижения для указанного района
            var achievements = await _context.Achievements
                .Where(a => a.DistrictId == districtId)
                .ToListAsync();

            // Получаем все достижения пользователя
            var userAchievements = await _context.UserAchievements
                .Where(ua => ua.UserId == userId)
                .ToDictionaryAsync(ua => ua.AchievementId);

            foreach (var achievement in achievements)
            {
                // Пропускаем уже разблокированные достижения
                if (userAchievements.TryGetValue(achievement.Id, out var userAchievement) && userAchievement.IsUnlocked)
                    continue;

                // Если достижения ещё нет у пользователя, создаём новую запись
                if (userAchievement == null)
                {
                    userAchievement = new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = achievement.Id,
                        CurrentProgress = 0,
                        IsUnlocked = false
                    };
                    _context.UserAchievements.Add(userAchievement);
                    userAchievements[achievement.Id] = userAchievement;
                }

                // Получаем новый прогресс из переданного словаря по типу условия
                int newProgress = 0;
                if (achievementProgresses.TryGetValue(achievement.ConditionType, out var progressValue))
                {
                    newProgress = progressValue;
                }

                // Обновляем прогресс только если он больше текущего
                if (newProgress > userAchievement.CurrentProgress)
                {
                    userAchievement.CurrentProgress = newProgress;

                    // Проверяем, достигнуто ли условие
                    if (userAchievement.CurrentProgress >= achievement.ConditionValue)
                    {
                        userAchievement.IsUnlocked = true;
                        // Начисляем награду за достижение
                        wallet.Reputation += achievement.RewardRep;
                    }
                }
            }
        }

        /// <summary>
        /// Обрабатывает отложенный запрос покупки из офлайн-режима.
        /// Выполняет ту же логику, что и StoreController.Buy.
        /// </summary>
        private async Task ProcessPurchaseRequest(User user, UserWallet wallet, OfflineSyncRequest request)
        {
            var purchaseData = JsonConvert.DeserializeObject<PurchaseRequest>(request.JsonBody);
            if (purchaseData == null) return;
            switch (purchaseData.ItemType.ToLower())
            {
                case "bonus":
                    var bonus = await _context.Bonuses.FindAsync(purchaseData.ItemId);
                    if (bonus == null) return;
                    if (wallet.Money < bonus.PriceMoney) return;
                    wallet.Money -= bonus.PriceMoney;
                    var userBonus = await _context.UserBonuses
                        .FirstOrDefaultAsync(b => b.UserId == user.Id && b.BonusId == purchaseData.ItemId);
                    if (userBonus == null)
                    {
                        userBonus = new UserBonus { UserId = user.Id, BonusId = purchaseData.ItemId, Quantity = 1 };
                        _context.UserBonuses.Add(userBonus);
                    }
                    else
                    {
                        userBonus.Quantity++;
                    }
                    break;
                case "upgrade":
                    var upgradeLevel = await _context.UpgradesCosts
                        .FirstOrDefaultAsync(l => l.UpgradeId == purchaseData.ItemId && l.Level == purchaseData.Level);
                    if (upgradeLevel == null) return;
                    if (wallet.Money < upgradeLevel.PriceMoney) return;
                    var userUpgrade = await _context.UserUpgrades
                        .FirstOrDefaultAsync(u => u.UserId == user.Id && u.UpgradeId == purchaseData.ItemId);
                    if (userUpgrade == null) return;
                    if (purchaseData.Level != userUpgrade.Level + 1) return;
                    wallet.Money -= upgradeLevel.PriceMoney;
                    userUpgrade.Level = purchaseData.Level;
                    break;
            }
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
            if (string.IsNullOrWhiteSpace(authToken))
                return null;
            return await _context.Users.FirstOrDefaultAsync(u => u.Token == authToken);
        }
    }
}
