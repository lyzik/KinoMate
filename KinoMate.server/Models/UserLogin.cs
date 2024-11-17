namespace KinoMate.server.Models
{
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class UserRegister : UserLogin
    {
        public string Email { get; set; }
    }
}
