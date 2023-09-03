using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Domain.Entities;

namespace PostService.Infrastructure.Data
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContext context;

        public CommentRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<List<Comment>> GetCommentsAsync()
        {
            return await context.Comments.AsNoTracking().ToListAsync();
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid id)
        {
            return await context.Comments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId)
        {
            return await context.Comments.AsNoTracking().Where(c => c.PostId == postId).ToListAsync();
        }

        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment> UpdateCommentAsync(Comment comment)
        {
            context.Comments.Update(comment);
            await context.SaveChangesAsync();

            return comment;
        }

        public async Task RemoveCommentAsync(Comment comment)
        {
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
        }
    }
}
