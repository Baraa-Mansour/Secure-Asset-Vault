using Core.Requests;
using Core.Responses;

namespace Core.Interfaces;


public interface IAuthService
{
    public Task<LoginResponse> LoginAsync(LoginRequest userInfo);
    public Task<RegisterResponse> RegisterAsync(RegisterRequest userInfo);
}
