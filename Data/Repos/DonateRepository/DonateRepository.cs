using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elderly_Canteen.Data;
using Elderly_Canteen.Data.Entities;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Elderly_Canteen.Data.Repos
{
    public class DonateRepository<T> : IDonateRepository<T> where T : class
    {
        private readonly ModelContext _context;
        private readonly DbSet<T> _dbSet;

        public DonateRepository(ModelContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        // 实现GetListAsync方法，用于查询所有捐赠信息
        public async Task<List<Donation>> GetListAsync()
        {
            return await _context.Donations
            .Include(d => d.Finance)  // 在Donate实体中定义了导航属性Finance
            .ToListAsync();
        }
        // 增加捐赠信息
        public async Task AddAsync(Donation donation)
        {
            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();
        }
    }
}
