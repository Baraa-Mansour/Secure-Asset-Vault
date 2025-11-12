using Core.Interfaces;
using Core.Requests;
using Core.Responses;
using Core.Utilities;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Core.Services;

public class VaultItemsService : IVaultItemsService
{
    private readonly IUserService _userService;
    private readonly IVaultService _vaultService;
    private readonly IVaultItemsRepo _vaultItemsRepo; 
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<VaultItemsService> _logger;

    public VaultItemsService(IVaultService vaultService, IUserService userService , IVaultItemsRepo vaultItemsRepo , IEncryptionService encryptionService 
                             ,Logger<VaultItemsService> logger)
    {
        _userService = userService;
        _vaultService = vaultService;
        _vaultItemsRepo = vaultItemsRepo;
        _encryptionService = encryptionService;
        _logger = logger;
    }


    public async Task<List<VaultItemsResponse>> GetVaultItemsByUserIdAndVaultIdAsync(int userId, int vaultId)
    {
        _logger.LogInformation("Get user items service start for User Id: {userId} from vault id: {vaultId}", userId , vaultId);

        await _userService.CheckUserAsync(userId);
        await _vaultService.CheckVaultExistsAndOwnerAsync(vaultId , userId);

        List<VaultItemModel> items = await _vaultItemsRepo.GetVaultItemsAsync(vaultId);
        List<VaultItemsResponse> itemsResponse = new();
 
        foreach (VaultItemModel item in items)
        {
            itemsResponse.Add(_MapModelToResponse(item));
        }

        _logger.LogInformation("Get user items service end for User Id: {userId} from vault id: {vaultId} Successful", userId, vaultId);

        return itemsResponse;
    }
    public async Task<bool> DeleteVaultItemByUserIdAndVaultIdAndItemIdAsync(int userId, int vaultId, int itemId)
    {
        _logger.LogInformation("Delete user item service start for User Id: {userId} from vault id: {vaultId} on item id: {itemId}", userId, vaultId , itemId);

        await _userService.CheckUserAsync(userId);
        await _vaultService.CheckVaultExistsAndOwnerAsync (vaultId , userId);
        await CheckItemExistsInVaultAsync(vaultId, itemId);

        bool isDeleted = await _vaultItemsRepo.DeleteItemAsync(itemId);

        _logger.LogInformation("Delete user item service end for User Id: {userId} from vault id: {vaultId} on item id: {itemId} Successful", userId, vaultId, itemId);

        return isDeleted;
    }
    public async Task<VaultItemsResponse> CreateVaultItemAsync(int userId, int vaultId, VaultItemRequest request)
    {
        _logger.LogInformation("Create user item service start for User Id: {userId} from vault id: {vaultId}", userId, vaultId);

        await _userService.CheckUserAsync(userId);
        await _vaultService.CheckVaultExistsAndOwnerAsync(vaultId, userId);

        (byte[] key, byte[] IV, byte[] encryptedData) = _encryptionService.Encrypt(request.Data);
        byte[] encryptedKey = _encryptionService.EncryptKey(key,Encoding.UTF8.GetBytes("12345678901234567890123456789012"), IV); // master keey is fixed for learn 

        VaultItemModel item = new VaultItemModel
        {
            Alias = request.Alias,
            VaultId = vaultId,
            EncryptedData = encryptedData,
            EncryptedKey_ForUser = encryptedKey,
            IV = IV
        };

        bool isAdded = await _vaultItemsRepo.CreateVaultItemAsync(item);

        _logger.LogInformation("create user item service end for User Id: {userId} from vault id: {vaultId} Successful, item id: {itemId}", userId, vaultId,item.Id);

        return new VaultItemsResponse
        {
            Id = item.Id,
            Alias = item.Alias
        };
    }
    public async Task<VaultItemsDetailedResponse> GetVaultItemByUserIdAndVaultIdAndItemIdAsync(int userId, int vaultId, int itemId)
    {
        _logger.LogInformation("Get user item service start for User Id: {userId} from vault id: {vaultId} on item id: {itemId}", userId, vaultId , itemId);

        await _userService.CheckUserAsync(userId);
        await _vaultService.CheckVaultExistsAndOwnerAsync(vaultId, userId);

        VaultItemModel vaultItem = await CheckItemExistsInVaultAsync(vaultId, itemId);

        // master keey is fixed for learn 
        byte[] decryptedKey = _encryptionService.DecryptKey(vaultItem.EncryptedKey_ForUser,Encoding.UTF8.GetBytes("12345678901234567890123456789012"),vaultItem.IV);
        string Data = _encryptionService.Decrypt(vaultItem.EncryptedData , decryptedKey , vaultItem.IV);

        _logger.LogInformation("Get user item service end for User Id: {userId} from vault id: {vaultId} on item id: {itemId} Successful", userId, vaultId , itemId);

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
    public async Task<VaultItemModel> CheckItemExistsInVaultAsync(int vaultId ,  int itemId)
    {
        _logger.LogInformation("Check item exists in vault service start for item id: {itemId} in vault id: {vaultId}", itemId, vaultId);

        VaultItemModel? item = await _vaultItemsRepo.GetItemByIdAsync(itemId);

        if(item is null)
        {
            _logger.LogInformation("Check item exists in vault service failed for item id: {itemId} in vault id: {vaultId} (NotFound)", itemId, vaultId);

            throw new KeyNotFoundException("Item not found");
        }

        if(item.VaultId != vaultId)
        {
            _logger.LogWarning("Check item exists in vault service failed for item id: {itemId} in vault id: {vaultId} (UnAuthorized)", itemId, vaultId);

            throw new UnauthorizedAccessException("Item does not belong to this vaultItem");
        }

        _logger.LogInformation("Check item exists in vault service end for item id: {itemId} in vault id: {vaultId} Successful", itemId, vaultId);

        return item;
    }
}
