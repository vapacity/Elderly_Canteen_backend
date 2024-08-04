using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elderly_Canteen.Data;
using Elderly_Canteen.Data.Entities;
namespace Elderly_Canteen.Data.Repos
{
    namespace Elderly_Canteen.Data.Repos
    {
        public class GenericRepository<T> : IGenericRepository<T> where T : class
        {
            private readonly ModelContext _context;
            private readonly DbSet<T> _dbSet;

            public GenericRepository(ModelContext context)
            {
                _context = context;
                _dbSet = context.Set<T>();
            }

            public IQueryable<T> GetAll()
            {
                return _dbSet;
            }

            public async Task<IEnumerable<T>> GetAllAsync()
            {
                return await _dbSet.ToListAsync();
            }

            public async Task<T> GetByIdAsync(object id)
            {
                return await _dbSet.FindAsync(id);
            }

            public async Task AddAsync(T entity)
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateAsync(T entity)
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(object id)
            {
                T entity = await _dbSet.FindAsync(id);
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
