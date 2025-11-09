using DataAccess.Models;

namespace DataAccess.Interfaces;

public interface IVaultItemsRepo
{
    public Task<List<VaultItemModel>> GetVaultItemsAsync(int vaultId);
    public Task<VaultItemModel?> GetItemByIdAsync(int itemId);
    public Task<bool> DeleteItemAsync(int itemId);
    public Task<bool> CreateVaultItemAsync(VaultItemModel vaultItemModel);
}
