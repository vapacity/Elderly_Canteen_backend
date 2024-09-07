using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface ILogService
    {
        Task LogAsync(string logLevel, string message);
        Task<List<SystemLog>> GetLogsAsync();
    }
}
