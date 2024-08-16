using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elderly_Canteen.Data.Entities;
namespace Elderly_Canteen.Data.Repos
{
    public interface IDonateRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<List<Donation>> GetListAsync();

        Task AddAsync(Donation donation);
    }
}
