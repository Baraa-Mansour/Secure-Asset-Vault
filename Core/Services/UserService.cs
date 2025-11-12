using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class UserService : IUserService
{
    public readonly IUserRepo _userRepo;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepo userRepo , ILogger<UserService> logger)
    {
        _userRepo = userRepo;
        _logger = logger;
    }

    public async Task CheckUserAsync(int userId)
    {
        _logger.LogInformation("Cheack user service start for User Id: {userId}" , userId);

        UserModel? user = await _userRepo.GetUserAsync(userId);

        if (user is null || user.IsDeleted)
        {
            _logger.LogWarning("Cheack user service failed for User Id: {userId} (NotFound)", userId);

            throw new KeyNotFoundException("User not found");
        }
        if (!user.IsActive)
        {
            _logger.LogWarning("Cheack user service failed for User Id: {userId} (UnAuthorized)", userId);

            throw new UnauthorizedAccessException("Account is deactivated");
        }

        _logger.LogInformation("Cheack user service end for User Id: {userId} Successful", userId);
    }
}
