using Core.Interfaces;
using Core.Requests;
using Core.Responses;
using Core.Utilities;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepo _userRepo;
    private readonly IHasher _hasher;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepo userRepo , IHasher hasher , IJwtService jwtService)
    {
        _userRepo = userRepo;
        _hasher = hasher;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest userInfo)
    {
        if(await _userRepo.IsUsernameOrEmailExistsAsync(userInfo.Username , userInfo.Email))
        {
            throw new InvalidDataException("Username/Email already exists");
        }

        (byte[] hash, byte[] salt) = _hasher.Hash(userInfo.Password);

        UserModel model = new UserModel
        {
            Username = userInfo.Username,
            Email = userInfo.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
        };
        
        bool isAdded = await _userRepo.AddUserAsync(model);

        return new RegisterResponse
        {
            Id = model.Id,
            Username = userInfo.Username,
            Email = userInfo.Email
        };
    }
    public async Task<LoginResponse?> LoginAsync(LoginRequest userInfo)
    {
        UserModel? user = await _userRepo.FindUserByUsernameOrEmailAsync(userInfo.UsernameOrEmail);

        if(user == null|| !_hasher.Verify(userInfo.Password, user.PasswordHash, user.PasswordSalt))
        {
            return null;
        }

        return new LoginResponse
        {
            UserId = user.Id,
            JWT = _jwtService.GetJwtToken(user.Id, user.Username)
        };
    }
}
