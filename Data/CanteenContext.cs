using Microsoft.EntityFrameworkCore;
using Elderly_Canteen.Data;
using System.Security.Principal;
namespace Elderly_Canteen.Data
{
    public class CanteenContext : DbContext
    {
        public CanteenContext(DbContextOptions<CanteenContext> options)
            : base(options)
        {
        }
        
    }
}
