using System.ComponentModel.DataAnnotations;

namespace Core.Requests;

public class VaultItemRequest
{
    [Required]
    [StringLength(200)]
    public string Alias { get; set; } = null!;
    [Required]
    public string Data { get; set; } = null!;
}
