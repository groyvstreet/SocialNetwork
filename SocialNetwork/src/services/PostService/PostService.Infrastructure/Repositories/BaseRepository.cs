using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces;
using PostService.Infrastructure.Data;
using System.Linq.Expressions;

namespace PostService.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DataContext _context;
        private readonly DbSet<T> _entities;

        public BaseRepository(DataContext context)
        {
            _context = context;
            _entities = _context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _entities.AsNoTracking().ToListAsync();
        }

        public async Task<List<T>> GetAllByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public void Remove(T entity)
        {
            _entities.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
