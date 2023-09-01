using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.UserProfileInterfaces;
using PostService.Domain.Entities;

namespace PostService.Infrastructure.Data
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly DataContext context;

        public UserProfileRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<UserProfile?> GetUserProfileByIdAsync(Guid id)
        {
            return await context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(up => up.Id == id);
        }
    }
}
