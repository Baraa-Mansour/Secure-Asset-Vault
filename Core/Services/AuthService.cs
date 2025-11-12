using Core.Interfaces;
using Core.Requests;
using Core.Responses;
using Core.Utilities;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepo _userRepo;
    private readonly IHashService _hasher;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepo userRepo , IHashService hasher , IJwtService jwtService , ILogger<AuthService> logger)
    {
        _userRepo = userRepo;
        _hasher = hasher;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest userInfo)
    {
        _logger.LogInformation("User registration attempted: {Username}", userInfo.Username);

        if(await _userRepo.IsUsernameOrEmailExistsAsync(userInfo.Username , userInfo.Email))
        {
            _logger.LogWarning("Failed register attempt: {username}", userInfo.Username);

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

        _logger.LogInformation("User registered successfully: {UserId}", model.Id);

        return new RegisterResponse
        {
            Id = model.Id,
            Username = userInfo.Username,
            Email = userInfo.Email
        };
    }
    public async Task<LoginResponse?> LoginAsync(LoginRequest userInfo)
    {
        _logger.LogInformation("Login attempted: {UsernameOrEmail}", userInfo.UsernameOrEmail);

        UserModel? user = await _userRepo.FindUserByUsernameOrEmailAsync(userInfo.UsernameOrEmail);

        if(user == null || !_hasher.Verify(userInfo.Password, user.PasswordHash, user.PasswordSalt))
        {
            _logger.LogWarning("Failed login attempt: {UsernameOrEmail}", userInfo.UsernameOrEmail);

            return null;
        }

        _logger.LogInformation("Successful login: {UserId}", user.Id);

        return new LoginResponse
        {
            UserId = user.Id,
            JWT = _jwtService.GetJwtToken(user.Id, user.Username)
        };
    }
}
