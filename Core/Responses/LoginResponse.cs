
namespace Core.Responses;

public class LoginResponse
{
    public int UserId { get; set; }
    public string JWT { get; set; } = null!;
}

