using System.ComponentModel.DataAnnotations;

namespace Core.Requests;


public class LoginRequest
{
    [Required]
    [StringLength(maximumLength : 255)]
    public string UsernameOrEmail { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
