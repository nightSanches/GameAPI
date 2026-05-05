namespace GameAPI.Models.Authentification
{
    // Ответ после успешного входа/регистрации
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }
}
