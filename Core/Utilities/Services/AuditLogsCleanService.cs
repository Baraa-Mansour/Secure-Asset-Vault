using DataAccess.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Utilities.Services;

public class AuditLogsCleanService : IHostedService , IDisposable
{
    private readonly ILogger<AuditLogsCleanService> _logger;
    private readonly IAuditLogsRepo _logsRepo;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly Timer _timer;

    public AuditLogsCleanService(IAuditLogsRepo logsRepo , ILogger<AuditLogsCleanService> logger)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        _logsRepo = logsRepo;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void DoWork(object? state)
    {
        // Prevent overlapping executions
        if (_semaphore.Wait(0))
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _logsRepo.ClearOldLogsAsync(DateTime.UtcNow.AddDays(-30));
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Audit log cleanup failed: {ex.Message}" , ex.Message);
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }


    public void Dispose()
    {
        _timer?.Dispose();
    }
}
