using Microsoft.Extensions.Configuration;

namespace FileDeletionRecordService
{
    public sealed class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private readonly FileDeletionEventLogCollectService _logCollectSvc;
        private readonly IConfiguration _configuration;
        private Timer _timer;
        private readonly int _timerIntervalInSec;

        public TimedHostedService(FileDeletionEventLogCollectService svc, ILogger<TimedHostedService> logger, IConfiguration configuration)
        {
            _logCollectSvc = svc;
            _logger = logger;
            _configuration = configuration;

            _timerIntervalInSec = _configuration.GetValue<int>("AppConfig:TimerIntervalInSec");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_timerIntervalInSec));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");
            _logCollectSvc.CollectDeletionRecordFromSystemEventLog();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}