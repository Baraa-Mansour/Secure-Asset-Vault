
using DataAccess.Models;

namespace DataAccess.Interfaces;

public interface IVaultRepo
{
    public Task<List<VaultModel>> GetVaultsByUserIdAsync(int userId);
    public Task<VaultModel> GetVaultsByIdAsync(int Id);
    public Task<bool> CreateVaultByUserIdAsync(VaultModel vault);
    public Task<bool> UpdateVaultAsync(VaultModel vault);
    public Task<bool> DeleteVaultAsync(int id);
}
