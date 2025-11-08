
namespace DataAccess.Models;

public class VaultModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
