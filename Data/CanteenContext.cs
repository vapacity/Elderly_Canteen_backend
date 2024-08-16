using Microsoft.EntityFrameworkCore;
using Elderly_Canteen.Data;
using System.Security.Principal;
using Elderly_Canteen.Data.Entities;
namespace Elderly_Canteen.Data
{
    public class CanteenContext : DbContext
    {
        public CanteenContext(DbContextOptions<CanteenContext> options)
            : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; } // 添加这个属性
    }
}
