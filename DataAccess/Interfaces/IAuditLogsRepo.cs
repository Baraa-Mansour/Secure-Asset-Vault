
using DataAccess.Models;

namespace DataAccess.Interfaces;

public interface IAuditLogsRepo
{
    public Task ClearOldLogsAsync(DateTime wantedDate);
    public Task AddLogAsync(AuditLogsModel logsModel);
}
