using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces;
using System.Linq.Expressions;

namespace PostService.Infrastructure.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext context;
        private readonly DbSet<T> dbSet;

        public Repository(DataContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAsync()
        {
            return await dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<List<T>> GetByAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultByAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
