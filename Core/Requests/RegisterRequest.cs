
using System.ComponentModel.DataAnnotations;

namespace Core.Requests;


public class RegisterRequest
{
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = null!;
    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;

}
