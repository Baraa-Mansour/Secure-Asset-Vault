using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Services;

public class UserService : IUserService
{
    public readonly IUserRepo _userRepo;

    public UserService(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task CheckUser(int userId)
    {
        UserModel? user = await _userRepo.GetUserAsync(userId);

        if (user is null || user.IsDeleted)
        {
            throw new KeyNotFoundException("User not found");
        }
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is deactivated");
        }

    }
}
