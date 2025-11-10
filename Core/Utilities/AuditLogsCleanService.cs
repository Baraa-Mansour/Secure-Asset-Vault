using DataAccess.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Core.Utilities;

public class AuditLogsCleanService : IHostedService , IDisposable
{
    private readonly IAuditLogsRepo _logsRepo;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly Timer _timer;

    public AuditLogsCleanService(IAuditLogsRepo logsRepo)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        _logsRepo = logsRepo;
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
                    // Use proper logging instead of Console
                    Console.WriteLine($"Audit log cleanup failed: {ex.Message}");
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
        this.Dispose();
        return Task.CompletedTask;
    }


    public void Dispose()
    {
        _timer?.Dispose();
    }
}
