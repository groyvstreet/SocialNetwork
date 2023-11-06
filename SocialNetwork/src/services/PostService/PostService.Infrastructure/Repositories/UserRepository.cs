using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context) { }

        public async Task<User?> GetUserWithPostsByIdAsync(Guid id)
        {
            return await _context.Users.AsNoTracking().Include(user => user.Posts).FirstOrDefaultAsync(user => user.Id == id);
        }
    }
}
