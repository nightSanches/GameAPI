using GameAPI.Classes;
using GameAPI.Models;
using GameAPI.Models.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            // Обновляем кошелёк
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (wallet == null) return BadRequest("Кошелёк не найден");
            wallet.Money += request.MoneyEarned;

            // Обновляем рекорд
            var score = await _context.UserScores.FirstOrDefaultAsync(s => s.UserId == user.Id);
            bool isNewRecord = false;
            if (score != null && request.Score > score.BestScore)
            {
                score.BestScore = request.Score;
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

            await _context.SaveChangesAsync();

            // Вычисляем новое место игрока
            int rank = 1;
            var scores = await _context.UserScores
                .Select(s => s.BestScore)
                .Distinct()
                .OrderByDescending(s => s)
                .ToListAsync();
            rank = scores.FindIndex(s => s == score.BestScore) + 1;

            // Возвращаем обновлённые данные
            var response = new GameEndResponse
            {
                Money = wallet.Money,
                BestScore = score.BestScore,
                Rank = rank,
                GamesPlayed = stats.GamesPlayedCount,
                BlocksPlaced = stats.BlocksPlacedCount,
                PerfectBlocks = stats.IBlocksPlacedCount,
                IsNewRecord = isNewRecord
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
