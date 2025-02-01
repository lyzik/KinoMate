using KinoMate.server.Database.Auth;
using Microsoft.EntityFrameworkCore;

namespace KinoMate.server.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ForumPost> ForumPost { get; set; }
        public DbSet<PostLike> PostLike { get; set; }
        public DbSet<PostComments> PostComments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
