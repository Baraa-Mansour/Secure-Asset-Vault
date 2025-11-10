
namespace DataAccess.Models;

public class AuditLogsModel
{
    public int Id { get; set; }
    public int? UserId { get; set; } = null;
    public string Action { get; set; } = null!;
    public int? ResorceId { get; set; } = null;
    public string? Message { get; set; } = null;
    public string? IpAddress { get; set; } = null;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public string Severity { get; set; } = null!; // info - warning - error;
}
