using System.Security.Claims;
using KinoMate.server.Database;
using Microsoft.AspNetCore.Mvc;

namespace KinoMate.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ForumController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpPost("addPost")]
        public async Task<IActionResult> AddPost([FromBody] ForumPostRequest post)
        {
            if (post == null)
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

            var newPost = new ForumPost
            {
                UserId = userId,
                Title = post.Title,
                Description = post.Description,
                CreatedAt = DateTime.Now
            };

            _context.ForumPost.Add(newPost);
            await _context.SaveChangesAsync();
            return Ok(post);
        }

        [HttpGet("getPosts")]
        public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "")
        {
            var posts = _context.ForumPost
                .Where(p => p.Title.Contains(search))  
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = posts.Select(p => new
            {
                p.Id,
                p.UserId,
                p.Title,
                p.Description,
                p.CreatedAt,
                Likes = _context.PostLike
                    .Where(l => l.PostId == p.Id)
                    .Count(),
                Username = _context.Users
                    .Where(u => u.Id == p.UserId)
                    .Select(u => u.Username)
                    .FirstOrDefault(),
            });

            var totalItems = _context.ForumPost
                .Where(p => p.Title.Contains(search))
                .Count();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return Ok(new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Posts = response
            });
        }

        [HttpGet("getPost")]
        public async Task<IActionResult> GetPost([FromQuery] int postId)
        {
            var post = _context.ForumPost
                .Where(p => p.Id == postId)
                .FirstOrDefault();

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            var response = new
            {
                post.Id,
                post.UserId,
                post.Title,
                post.Description,
                post.CreatedAt,
                Likes = _context.PostLike
                    .Where(l => l.PostId == post.Id)
                    .Count(),
                Comments = _context.PostComments
                    .Where(c => c.PostId == post.Id)
                    .Select(c => new
                    {
                        c.Id,
                        c.UserId,
                        c.Description,
                        c.CreatedAt
                    })
            };

            return Ok(response);
        }
        [HttpPost("likePost")]
        public async Task<IActionResult> LikePost([FromBody] PostLikeRequest like)
        {
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

            var post = _context.ForumPost
                .Where(p => p.Id == like.PostId)
                .FirstOrDefault();

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            var existingLike = _context.PostLike
                .Where(l => l.UserId == userId && l.PostId == like.PostId)
                .FirstOrDefault();

            if (existingLike != null)
            {
                _context.PostLike.Remove(existingLike);
                await _context.SaveChangesAsync();
                return Ok();
            }

            var newLike = new PostLike
            {
                UserId = userId,
                PostId = like.PostId
            };

            _context.PostLike.Add(newLike);
            await _context.SaveChangesAsync();
            return Ok(newLike);
        }

        [HttpPost("addComment")]
        public async Task<IActionResult> AddComment([FromBody] PostCommentsRequest comment)
        {
            if (comment == null)
                return BadRequest("Comment is null.");
                
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User ID is missing.");
                
            var userId = _context.Users
                .Where(u => u.Username == username)
                .Select(u => u.Id)
                .FirstOrDefault();

            if (userId == null)
                return NotFound("User not found.");
                
            var newComment = new PostComments
            {
                UserId = userId,
                PostId = comment.PostId,
                Description = comment.Description,
                CreatedAt = DateTime.Now
            };
            _context.PostComments.Add(newComment);
            await _context.SaveChangesAsync();
            return Ok(comment);
        }
    }
}
