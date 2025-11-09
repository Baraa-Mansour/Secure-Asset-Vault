using Core.Interfaces;
using Core.Requests;
using Core.Responses;
using Core.Utilities;
using DataAccess.Interfaces;
using DataAccess.Models;
using System.Text;

namespace Core.Services;

public class VaultItemsService : IVaultItemsService
{
    private readonly IUserService _userService;
    private readonly IVaultService _vaultService;
    private readonly IVaultItemsRepo _vaultItemsRepo; 
    private readonly IEncryptionService _encryptionService;

    public VaultItemsService(IVaultService vaultService, IUserService userService , IVaultItemsRepo vaultItemsRepo , IEncryptionService encryptionService)
    {
        _userService = userService;
        _vaultService = vaultService;
        _vaultItemsRepo = vaultItemsRepo;
        _encryptionService = encryptionService;
    }


    public async Task<List<VaultItemsResponse>> GetVaultItemsByUserIdAndVaultIdAsync(int userId, int vaultId)
    {
        await _userService.CheckUserAsync(userId);
        await _vaultService.CheckVaultExistsAndOwnerAsync(vaultId , userId);

        List<VaultItemModel> items = await _vaultItemsRepo.GetVaultItemsAsync(vaultId);
        List<VaultItemsResponse> itemsResponse = new();
 
        foreach (VaultItemModel item in items)
        {
            itemsResponse.Add(_MapModelToResponse(item));
        }

        return itemsResponse;
    }
    public async Task<bool> DeleteVaultItemByUserIdAndVaultIdAndItemIdAsync(int userId, int vaultId, int itemId)
    {
        await _userService.CheckUserAsync(userId);
        await _vaultService.CheckVaultExistsAndOwnerAsync (vaultId , userId);
        await ItemExistsAndInVaultAsync(vaultId, itemId);

        bool isDeleted = await _vaultItemsRepo.DeleteItemAsync(itemId);

        return isDeleted;
    }
    public async Task<VaultItemsResponse> CreateVaultItemAsync(int userId, int vaultId, VaultItemRequest request)
    {
        await _userService.CheckUserAsync(userId);
        await _vaultService.CheckVaultExistsAndOwnerAsync(vaultId, userId);

        (byte[] key, byte[] IV, byte[] encryptedData) = _encryptionService.Encrypt(request.Data);
        byte[] encryptedKey = _encryptionService.EncryptKey(key, BitConverter.GetBytes(userId), IV); // master keey is userId->byte[] for learn 

        VaultItemModel item = new VaultItemModel
        {
            Alias = request.Alias,
            VaultId = vaultId,
            EncryptedData = encryptedData,
            EncryptedKey_ForUser = encryptedKey,
            IV = IV
        };

        bool isAdded = await _vaultItemsRepo.CreateVaultItemAsync(item);

        return new VaultItemsResponse
        {
            Id = item.Id,
            Alias = item.Alias
        };
    }
    public async Task<VaultItemsDetailedResponse> GetVaultItemByUserIdAndVaultIdAndItemIdAsync(int userId, int vaultId, int itemId)
    {
        await _userService.CheckUserAsync(userId);
        await _vaultService.CheckVaultExistsAndOwnerAsync(vaultId, userId);

        VaultItemModel vaultItem = await ItemExistsAndInVaultAsync(vaultId, itemId);

        // master keey is userId->byte[] for learn 
        byte[] decryptedKey = _encryptionService.DecryptKey(vaultItem.EncryptedKey_ForUser, BitConverter.GetBytes(userId), vaultItem.IV);
        string Data = _encryptionService.Decrypt(vaultItem.EncryptedData , decryptedKey , vaultItem.IV);

        return new VaultItemsDetailedResponse
        {
            Id = vaultItem.Id,
            Alias = vaultItem.Alias,
            Data = Data
        };
    }

    private VaultItemsResponse _MapModelToResponse(VaultItemModel item)
    {
        return new VaultItemsResponse
        {
            Id = item.Id,
            Alias = item.Alias
        };
    }
    public async Task<VaultItemModel> ItemExistsAndInVaultAsync(int vaultId ,  int itemId)
    {
        VaultItemModel? item = await _vaultItemsRepo.GetItemByIdAsync(itemId);

        if(item is null)
        {
            throw new KeyNotFoundException("Item not found");
        }

        if(item.VaultId != vaultId)
        {
            throw new UnauthorizedAccessException("Item does not belong to this vaultItem");
        }

        return item;
    }
}
