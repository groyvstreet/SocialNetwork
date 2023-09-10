using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;

        public UserRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await context.Users.AsNoTracking().FirstOrDefaultAsync(up => up.Id == id);
        }
    }
}
