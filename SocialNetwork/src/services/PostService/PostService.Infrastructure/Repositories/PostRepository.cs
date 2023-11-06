using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        public PostRepository(DataContext context) : base(context) { }

        public async Task<Post?> GetPostWithCommentsByIdAsync(Guid id)
        {
            return await _context.Posts.AsNoTracking().Include(post => post.Comments).FirstOrDefaultAsync(post => post.Id == id);
        }
    }
}
