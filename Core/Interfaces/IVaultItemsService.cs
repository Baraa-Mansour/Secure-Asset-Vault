using Core.Requests;
using Core.Responses;

namespace Core.Interfaces;


public interface IVaultItemsService
{
    public Task<List<VaultItemsResponse>> GetVaultItemsByUserIdAndVaultIdAsync(int userId , int vaultId);
    public Task<VaultItemsDetailedResponse> GetVaultItemByUserIdAndVaultIdAndItemIdAsync(int userId, int vaultId , int itemId);
    public Task<bool> DeleteVaultItemByUserIdAndVaultIdAndItemIdAsync(int userId, int vaultId, int itemId);
    public Task<VaultItemsResponse> CreateVaultItemAsync(int userId , int vaultId , VaultItemRequest request);
}
