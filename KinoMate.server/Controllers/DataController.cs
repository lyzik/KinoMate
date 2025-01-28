using KinoMate.server.Database;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace KinoMate.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("addComment")]
        public async Task<IActionResult> AddComment([FromBody] Comment comment)
        {
            if (comment == null)
            {
                return BadRequest("Comment is null.");
            }
            
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User ID is missing.");
            }

            var userId = _context.Users
                .Where(u => u.Username == username)
                .Select(u => u.Id)
                .FirstOrDefault();

            if (userId == null)
            {
                return NotFound("User not found.");
            }

            comment.UserId = userId;
            comment.CreatedAt = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }
    }
}
