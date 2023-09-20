using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(DataContext context) : base(context) { }
    }
}
