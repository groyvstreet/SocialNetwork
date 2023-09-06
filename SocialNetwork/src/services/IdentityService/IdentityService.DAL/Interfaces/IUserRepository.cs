using IdentityService.DAL.Entities;

namespace IdentityService.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersAsync();

        Task<User?> GetUserByIdAsync(string id);

        Task<User?> GetUserByEmailAsync(string email);

        Task<User?> GetUserByEmailAndPasswordAsync(string emaill, string password);

        Task<List<string>> GetUserRolesAsync(User user);

        Task AddUserAsync(User user, string password);

        Task UpdateUserAsync(User user);

        Task RemoveUserAsync(User user);
    }
}
