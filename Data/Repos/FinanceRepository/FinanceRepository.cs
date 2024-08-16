using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elderly_Canteen.Data;
using Elderly_Canteen.Data.Entities;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Elderly_Canteen.Data.Repos
{
    public class FinanceRepository<T> : IFinanceRepository<T> where T : class
    {
        private readonly ModelContext _context;
        private readonly DbSet<T> _dbSet;

        public FinanceRepository(ModelContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        // 增加财务信息
        public async Task AddAsync(Finance finance)
        {
            _context.Finances.Add(finance);
            await _context.SaveChangesAsync();
        }
    }
}
