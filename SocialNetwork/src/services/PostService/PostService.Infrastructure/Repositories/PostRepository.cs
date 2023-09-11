using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        private readonly DataContext _context;

        public PostRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        /*public async Task<List<Post>> GetPostsAsync()
        {
            return await _context.Posts.AsNoTracking().ToListAsync();
        }*/

        /*public async Task<Post?> GetPostByIdAsync(Guid id)
        {
            return await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }*/

        public async Task<List<Post>> GetPostsByUserIdAsync(Guid userId)
        {
            return await _context.Posts.AsNoTracking().Where(p => p.UserId == userId).ToListAsync();
        }

        /*public async Task AddPostAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
        }*/

        /*public async Task UpdatePostAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }*/

        /*public async Task RemovePostAsync(Post post)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }*/
    }
}
