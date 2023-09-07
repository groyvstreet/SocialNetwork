using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
