
namespace DataAccess.Models;

public class VaultItemModel
{
    public int Id { get; set; } = -1;
    public int VaultId { get; set; } = -1;
    public string Alias { get; set; } = null!;
    public byte[] EncryptedData { get; set; } = null!;
    public byte[] EncryptedKey_ForUser { get; set; } = null!;
    public byte[] IV { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
