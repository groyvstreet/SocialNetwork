using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostService.Domain.Entities;

namespace PostService.Infrastructure.Configurations
{
    public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
    {
        public void Configure(EntityTypeBuilder<PostLike> builder)
        {
            builder.HasIndex(pl => new { pl.PostId, pl.UserId })
                   .IsUnique();
        }
    }
}
