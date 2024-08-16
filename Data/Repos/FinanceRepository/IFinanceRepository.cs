using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elderly_Canteen.Data.Entities;
namespace Elderly_Canteen.Data.Repos
{
    public interface IFinanceRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task AddAsync(Finance finance);
    }
}
