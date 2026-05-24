using GameAPI.Classes;
using GameAPI.Models;
using GameAPI.Models.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static GameAPI.Models.Authentification.FullLoginResponse;

using ScoreByDistrictDto = GameAPI.Models.Authentification.FullLoginResponse.ScoreByDistrictDto;
using DistrictRankDto = GameAPI.Models.Authentification.FullLoginResponse.DistrictRankDto;
using UserAchievementDto = GameAPI.Models.Authentification.FullLoginResponse.UserAchievementDto;

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
