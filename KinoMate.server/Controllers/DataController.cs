using KinoMate.server.Database;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using KinoMate.server.Models;
using Microsoft.EntityFrameworkCore;

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
        [HttpPost("toggleFavoriteMovie/{movieId}")]
        public async Task<IActionResult> ToggleFavoriteMovie(int movieId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User ID is missing.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var favorites = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id);
            if (favorites == null)
            {
                favorites = new Favorites { UserId = user.Id, MoviesId = new List<int>(), SeriesId = new List<int>() };
                _context.Favorites.Add(favorites);
            }

            if (favorites.MoviesId.Contains(movieId))
            {
                favorites.MoviesId.Remove(movieId);
            }
            else
            {
                favorites.MoviesId.Add(movieId);
            }

            await _context.SaveChangesAsync();
            return Ok(favorites);
        }

        [HttpPost("toggleFavoriteSeries/{seriesId}")]
        public async Task<IActionResult> ToggleFavoriteSeries(int seriesId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User ID is missing.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var favorites = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id);
            if (favorites == null)
            {
                favorites = new Favorites { UserId = user.Id, MoviesId = new List<int>(), SeriesId = new List<int>() };
                _context.Favorites.Add(favorites);
            }

            if (favorites.SeriesId.Contains(seriesId))
            {
                favorites.SeriesId.Remove(seriesId);
            }
            else
            {
                favorites.SeriesId.Add(seriesId);
            }

            await _context.SaveChangesAsync();
            return Ok(favorites);
        }

        [HttpPost("toggleMovieNotification/{movieId}")]
        public async Task<IActionResult> ToggleMovieNotification(int movieId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User ID is missing.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var favorites = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id);
            if (favorites == null)
            {
                favorites = new Favorites { UserId = user.Id, MoviesId = new List<int>(), SeriesId = new List<int>(), MoviesNotificationId = new List<int>(), SeriesNotificationId = new List<int>() };
                _context.Favorites.Add(favorites);
            }

            if (favorites.MoviesNotificationId.Contains(movieId))
            {
                favorites.MoviesNotificationId.Remove(movieId);
            }
            else
            {
                favorites.MoviesNotificationId.Add(movieId);
            }

            await _context.SaveChangesAsync();
            return Ok(favorites);
        }

        [HttpPost("toggleSeriesNotification/{seriesId}")]
        public async Task<IActionResult> ToggleSeriesNotification(int seriesId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User ID is missing.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var favorites = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id);
            if (favorites == null)
            {
                favorites = new Favorites { UserId = user.Id, MoviesId = new List<int>(), SeriesId = new List<int>(), MoviesNotificationId = new List<int>(), SeriesNotificationId = new List<int>() };
                _context.Favorites.Add(favorites);
            }

            if (favorites.SeriesNotificationId.Contains(seriesId))
            {
                favorites.SeriesNotificationId.Remove(seriesId);
            }
            else
            {
                favorites.SeriesNotificationId.Add(seriesId);
            }

            await _context.SaveChangesAsync();
            return Ok(favorites);
        }
        [HttpGet("favoriteMovies")]
        public async Task<IActionResult> GetFavoriteMovies()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User ID is missing.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var favorites = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .Select(f => f.MoviesId)
                .FirstOrDefaultAsync();

            if (favorites == null || !favorites.Any())
            {
                return Ok(new List<int>());
            }

            return Ok(favorites);
        }

    }
}
