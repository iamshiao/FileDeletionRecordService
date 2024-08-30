using FileDeletionRecordService;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddWindowsService(options =>
        {
            options.ServiceName = "FileDeletionRecordService";
        });

        LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services);

        services.AddSingleton<FileDeletionEventLogCollectService>();
        services.AddHostedService<TimedHostedService>();
    })
    .Build();

await host.RunAsync();
