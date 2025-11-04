
namespace Core.Responses;

public class LoginResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string JWT { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

