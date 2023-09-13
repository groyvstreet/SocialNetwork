using Microsoft.EntityFrameworkCore;
using PostService.Domain.Entities;
using PostService.Infrastructure.Configurations;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PostLikeConfiguration()).ApplyConfiguration(new CommentLikeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
