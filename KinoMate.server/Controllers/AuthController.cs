using KinoMate.server.Database;
using KinoMate.server.Database.Auth;
using KinoMate.server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KinoMate.server.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public AuthController(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin model)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == model.Username || u.Email.ToLower() == model.Username.ToLower());
            if (user != null && VerifyPassword(user, model.Password))
            {
                var token = GenerateJwtToken(user.Username);
                return Ok(new
                {
                    User = new { user.Username, user.Email },
                    Token = token
                });
            }

            return Unauthorized("Incorrect username or password.");
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegister model)
        {
            var existingUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == model.Username);
            var existingEmail = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());
            if (existingUser != null)
                return BadRequest(new { message = "The specified username occupied." });
            if (existingEmail != null)
                return BadRequest(new { message = "The account with this email address is taken." });

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();


            EmailServices emailServices = new EmailServices();
            emailServices.SendEmailAsync(model.Email, "Welcome to KinoMate!", "EmailConfirm", null);

            return Ok("The user has been successfully registered.");
        }
        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null || username == null)
                return NotFound("User not found.");

            return Ok(new
            {
                Username = user.Username,
                Email = user.Email
            });
        }
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (!VerifyPassword(user, model.CurrentPassword))
            {
                return BadRequest("Current password is incorrect.");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _dbContext.SaveChangesAsync();
            return Ok("Password changed successfully.");
        }

        private bool VerifyPassword(User user, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [AllowAnonymous]
        [HttpPost("sendemail")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailModel model)
        {
            EmailServices emailServices = new EmailServices();
            emailServices.SendEmailAsync(model.Email, model.Subject, "EmailConfirm", null);
            return Ok("Email has been sent.");
        }

        public class SendEmailModel
        {
            public string Email { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }
        }

        public class ChangePasswordModel
        {
            public string CurrentPassword {get;set;}
            public string NewPassword {get;set;}
        }

    }
}
