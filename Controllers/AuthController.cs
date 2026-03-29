using GameAPI.Classes;
using GameAPI.Models.Authentification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DBConnection _context;

        public AuthController(DBConnection context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Валидация модели
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // 2. Поиск пользователя
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Nickname == request.NicknameOrEmail ||
                                          u.Email == request.NicknameOrEmail);

            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.Password))
            {
                var errors = new Dictionary<string, string[]>
                {
                    ["General"] = new[] { "Неверный логин или пароль." }
                };
                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Authentication failed"
                };
                return Unauthorized(problemDetails);
            }

            // 3. Успешная авторизация
            string newToken = GenerateRandomToken(100);
            user.Token = newToken;
            await _context.SaveChangesAsync();

            var response = new LoginResponse
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Token = newToken,
                Role = user.Role,
                Email = user.Email,
                RegistrationDate = user.RegistrationDate
            };

            return Ok(response);
        }

        private string GenerateRandomToken(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            // Перемешивание
            for (int i = 0; i < length; i++)
            {
                int swapIndex = random.Next(length);
                (chars[i], chars[swapIndex]) = (chars[swapIndex], chars[i]);
            }

            return new string(chars);
        }
    }
}
