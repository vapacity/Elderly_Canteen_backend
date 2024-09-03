using Elderly_Canteen.Services.Interfaces;

namespace Elderly_Canteen.Services.Implements
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public TimedHostedService(IServiceProvider serviceProvider, ILogger<TimedHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            // 设定定时器在每天晚上22点触发
            var now = DateTime.Now;
            var scheduledTime = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0); // 22点
            if (now > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1); // 如果当前时间已过22点，定时到明天晚上22点
            }

            var initialDelay = scheduledTime - now;

            _timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromHours(24)); // 每24小时调用一次
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Hosted Service is working.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var stockService = scope.ServiceProvider.GetRequiredService<IRepoService>();

                // 调用每日库存补充函数
                stockService.ReplenishDailyStockAsync().GetAwaiter().GetResult();

                // 调用检查和删除过期食材函数
                stockService.CheckAndRemoveExpiredIngredientsAsync().GetAwaiter().GetResult();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
