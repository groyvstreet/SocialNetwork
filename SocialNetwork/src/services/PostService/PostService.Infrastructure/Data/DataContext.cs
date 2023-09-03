using Microsoft.EntityFrameworkCore;
using PostService.Domain.Entities;

namespace PostService.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<PostLike> PostLikes { get; set; }

        public DbSet<CommentLike> CommentLikes { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
