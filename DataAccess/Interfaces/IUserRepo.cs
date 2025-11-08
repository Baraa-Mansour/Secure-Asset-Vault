using DataAccess.Models;

namespace DataAccess.Interfaces;

public interface IUserRepo
{
    public Task<bool> IsUsernameOrEmailExistsAsync(string userName , string email);
    public Task<bool> AddUserAsync(UserModel user);
    public Task<UserModel?> FindUserByUsernameOrEmailAsync(string userNameOrEmail);
    public Task<UserModel?> GetUserAsync(int id);
}
