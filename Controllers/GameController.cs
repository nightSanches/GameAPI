using GameAPI.Classes;
using GameAPI.Models;
using GameAPI.Models.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static GameAPI.Models.Authentification.FullLoginResponse;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly DBConnection _context;

        public GameController(DBConnection context)
        {
            _context = context;
        }

        [HttpPost("end")]
        public async Task<IActionResult> EndGame([FromQuery] string authToken, [FromBody] GameEndRequest request)
        {
            var user = await GetUserByToken(authToken);
            if (user == null)
                return Unauthorized(new { message = "Недействительный токен." });

            // Обновляем кошелёк (Money и Reputation)
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (wallet == null) return BadRequest("Кошелёк не найден");
            wallet.Money += request.MoneyEarned;
            wallet.Reputation += request.ReputationEarned;

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

            // Обновляем прогресс достижений
            var achievements = await _context.Achievements
                .Where(a => a.DistrictId == request.DistrictId)
                .ToListAsync();

            var userAchievements = await _context.UserAchievements
                .Where(ua => ua.UserId == user.Id)
                .ToListAsync();

            foreach (var achievement in achievements)
            {
                var userAchievement = userAchievements.FirstOrDefault(ua => ua.AchievementId == achievement.Id);
                if (userAchievement == null || userAchievement.IsUnlocked)
                    continue;

                int newProgress = 0;
                switch (achievement.ConditionType.ToLower())
                {
                    case "max_floor":
                        newProgress = request.MaxFloor;
                        break;
                    case "perfect_streak":
                        newProgress = request.PerfectStreak;
                        break;
                    case "games_played":
                        newProgress = stats.GamesPlayedCount;
                        break;
                }

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

            await _context.SaveChangesAsync();

            // Вычисляем новое место игрока (dense rank по лучшему счету среди всех районов)
            var allUserScores = await _context.UserScores
                .Where(s => s.UserId == user.Id)
                .ToListAsync();
            int bestScoreOverall = allUserScores.Any() ? allUserScores.Max(s => s.BestScore) : 0;

            int rank = 1;
            if (bestScoreOverall > 0)
            {
                var allBestScores = await _context.UserScores
                    .GroupBy(s => s.UserId)
                    .Select(g => g.Max(s => s.BestScore))
                    .Distinct()
                    .OrderByDescending(s => s)
                    .ToListAsync();
                rank = allBestScores.FindIndex(s => s == bestScoreOverall) + 1;
            }

            // Получаем актуальные счета по районам
            var scoresByDistrict = await _context.UserScores
                .Where(s => s.UserId == user.Id)
                .Select(s => new ScoreByDistrictDto
                {
                    DistrictId = s.DistrictId,
                    BestScore = s.BestScore
                })
                .ToListAsync();

            // Возвращаем обновлённые данные
            var response = new GameEndResponse
            {
                Money = wallet.Money,
                Reputation = wallet.Reputation,
                BestScore = bestScoreOverall,
                Rank = rank,
                GamesPlayed = stats.GamesPlayedCount,
                BlocksPlaced = stats.BlocksPlacedCount,
                PerfectBlocks = stats.IBlocksPlacedCount,
                IsNewRecord = isNewRecord,
                ScoresByDistrict = scoresByDistrict,
                Achievements = await _context.UserAchievements
                    .Where(ua => ua.UserId == user.Id)
                    .Select(ua => new UserAchievementDto
                    {
                        AchievementId = ua.AchievementId,
                        CurrentProgress = ua.CurrentProgress,
                        IsUnlocked = ua.IsUnlocked
                    })
                    .ToListAsync()
            };

            return Ok(response);
        }

        private async Task<User> GetUserByToken(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
                return null;
            var token = authToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authToken.Substring(7)
                : authToken;
            return await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
        }
    }
}
