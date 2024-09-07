using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Elderly_Canteen.Services.Implements
{
    public class LogService : ILogService
    {
        private readonly IGenericRepository<SystemLog> _systemLogRepository;

        public LogService(IGenericRepository<SystemLog> systemLogRepository)
        {
            _systemLogRepository = systemLogRepository;
        }

        public async Task LogAsync(string logLevel,string message)
        {
            var log = new SystemLog
            {
                LogLevel = logLevel,
                Message = message,
                CreatedAt = DateTime.Now
            };
            await _systemLogRepository.AddAsync(log);
        }

        public async Task<List<SystemLog>> GetLogsAsync()
        {
            var logs = await _systemLogRepository.GetAllAsync();
            return logs.OrderByDescending(log => log.CreatedAt).ToList();
        }
    }
}
