using Core.Requests;
using Core.Responses;

namespace Core.Interfaces;

public interface IVaultService
{
    public Task<List<VaultResponse>> GetVaultsByUserIdAsync(int userId);
    public Task<VaultResponse> CreateVaultByUserIdAsync(VaultRequest vault , int userId);
    public Task<bool> UpdateVaultByUserIdAsync(int vaultId , int userId  , VaultRequest vault);
    public Task<bool> DeleteVaultByUserIdAsync(int vaultId, int userId);
}
