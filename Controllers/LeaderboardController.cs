using GameAPI.Models.Leaderboard;
using GameAPI.Models;
using Microsoft.AspNetCore.Mvc;
using GameAPI.Classes;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly DBConnection _context;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера таблицы лидеров.
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public LeaderboardController(DBConnection context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить таблицу лидеров для конкретного района (топ-50 + текущий игрок)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLeaderboard([FromQuery] int districtId = 1)
        {
            // 1. Все пользователи с рекордами для указанного района, сортировка: BestScore DESC, Nickname ASC
            var allUsersQuery = _context.Users
                .Join(_context.UserScores.Where(s => s.DistrictId == districtId),
                    u => u.Id,
                    s => s.UserId,
                    (u, s) => new { User = u, s.BestScore })
                .OrderByDescending(x => x.BestScore)
                .ThenBy(x => x.User.Nickname);

            // 2. Определяем текущего пользователя, если передан токен в заголовке Authorization
            User currentUser = null;
            var authToken = ExtractAuthToken();
            if (!string.IsNullOrWhiteSpace(authToken))
            {
                currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Token == authToken);
            }

            // 3. Вычисляем dense rank (группируем по одинаковым BestScore)
            var rankedQuery = allUsersQuery.AsEnumerable() // переходим в память для сложной группировки
                .GroupBy(x => x.BestScore)
                .OrderByDescending(g => g.Key)
                .Select((group, index) => new
                {
                    Score = group.Key,
                    Position = index + 1, // dense rank
                    Players = group.OrderBy(p => p.User.Nickname).ToList()
                });

            // 4. Формируем TopPlayers (макс 50 записей)
            var topPlayers = new List<LeaderboardEntry>();
            int count = 0;

            foreach (var group in rankedQuery)
            {
                if (count >= 50) break;

                LeaderboardEntry entry;
                bool isCurrentUserInGroup = currentUser != null &&
                    group.Players.Any(p => p.User.Id == currentUser.Id);

                if (isCurrentUserInGroup)
                {
                    // Показываем только текущего игрока на этой позиции
                    var player = group.Players.First(p => p.User.Id == currentUser.Id);
                    entry = new LeaderboardEntry
                    {
                        Position = group.Position,
                        Nickname = player.User.Nickname,
                        BestScore = player.BestScore,
                        IsCurrentUser = true
                    };
                    topPlayers.Add(entry);
                    count++;
                }
                else
                {
                    // Показываем первого по алфавиту из группы
                    var first = group.Players.First();
                    entry = new LeaderboardEntry
                    {
                        Position = group.Position,
                        Nickname = first.User.Nickname,
                        BestScore = first.BestScore,
                        IsCurrentUser = false
                    };
                    topPlayers.Add(entry);
                    count++;
                }
            }

            // 5. Формируем CurrentPlayerEntry для текущего игрока
            LeaderboardEntry currentPlayerEntry = null;
            if (currentUser != null)
            {
                var userScore = await _context.UserScores
                    .FirstOrDefaultAsync(s => s.UserId == currentUser.Id && s.DistrictId == districtId);
                if (userScore != null)
                {
                    // Вычисляем позицию текущего игрока (dense rank по всем)
                    var allScores = await _context.UserScores
                        .Where(s => s.DistrictId == districtId)
                        .OrderByDescending(s => s.BestScore)
                        .Select(s => s.BestScore)
                        .ToListAsync();

                    int currentPlayerScore = userScore.BestScore;
                    int currentPlayerPos = allScores
                        .Distinct()
                        .OrderByDescending(s => s)
                        .TakeWhile(s => s > currentPlayerScore)
                        .Count() + 1;

                    currentPlayerEntry = new LeaderboardEntry
                    {
                        Position = currentPlayerPos,
                        Nickname = currentUser.Nickname,
                        BestScore = currentPlayerScore,
                        IsCurrentUser = true
                    };
                }
            }

            var response = new LeaderboardResponse
            {
                DistrictId = districtId,
                TopPlayers = topPlayers,
                CurrentPlayerEntry = currentPlayerEntry
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
    }
}
