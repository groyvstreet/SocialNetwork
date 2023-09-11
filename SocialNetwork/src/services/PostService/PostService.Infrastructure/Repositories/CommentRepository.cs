using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        private readonly DataContext _context;

        public CommentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        /*public async Task<List<Comment>> GetCommentsAsync()
        {
            return await _context.Comments.AsNoTracking().ToListAsync();
        }*/

        /*public async Task<Comment?> GetCommentByIdAsync(Guid id)
        {
            return await _context.Comments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }*/

        public async Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId)
        {
            return await _context.Comments.AsNoTracking().Where(c => c.PostId == postId).ToListAsync();
        }

        /*public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }*/

        /*public async Task UpdateCommentAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }*/

        /*public async Task RemoveCommentAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }*/
    }
}
