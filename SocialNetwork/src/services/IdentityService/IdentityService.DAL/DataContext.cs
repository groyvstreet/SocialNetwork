using IdentityService.BL.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
