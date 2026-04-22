using GameAPI.Classes;
using GameAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly DBConnection _context;

        public LeaderboardController(DBConnection context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить топ-50 игроков и место текущего пользователя
        /// </summary>
        [HttpGet("top")]
        public async Task<IActionResult> GetLeaderboard(string authorization)
        {
            User currentUser = null;
            if (!string.IsNullOrEmpty(authorization))
            {
                currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Token == authorization);
            }

            // Получить 50 лучших игроков
            var top50 = await _context.UserScores
                .OrderByDescending(s => s.BestScore)
                .Take(50)
                .Select(s => new LeaderboardEntry
                {
                    UserId = s.UserId,
                    BestScore = s.BestScore
                })
                .ToListAsync();

            var response = new LeaderboardResponse
            {
                Top50 = top50
            };

            // Если пользователь авторизован — получить его рекорд и место
            if (currentUser != null)
            {
                var userScore = await _context.UserScores
                    .FirstOrDefaultAsync(s => s.UserId == currentUser.Id);

                if (userScore != null)
                {
                    response.UserBestScore = userScore.BestScore;

                    // Подсчёт места: количество игроков с результатом строго больше, +1
                    // (одинаковые счета делят место, следующее место пропускается)
                    var place = await _context.UserScores
                        .CountAsync(s => s.BestScore > userScore.BestScore) + 1;

                    response.UserPlace = place;
                }
            }

            return Ok(response);
        }
    }
}
