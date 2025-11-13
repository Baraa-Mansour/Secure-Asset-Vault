
using System.ComponentModel.DataAnnotations;

namespace Core.Requests;


public class VaultRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }
}

