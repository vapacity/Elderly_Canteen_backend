using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace Elderly_Canteen.Data.Repos
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(object id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task UpdateAsync(Expression<Func<T, bool>> predicate, Action<T> updateAction);
        Task DeleteAsync(object id);
        Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
        Task<List<T>> GetWithIncludesAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<T> FindByCompositeKeyAsync<T>(params object[] keyValues) where T : class;
        Task DeleteByCompositeKeyAsync<T>(params object[] keyValues) where T : class;
        Task<bool> DeleteByConditionAsync(Expression<Func<T, bool>> expression);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}
