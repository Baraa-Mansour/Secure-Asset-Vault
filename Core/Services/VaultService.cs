using Core.Interfaces;
using Core.Requests;
using Core.Responses;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Services;

public class VaultService : IVaultService
{
    private readonly IVaultRepo _vaultRepo;
    private readonly IUserService _userService;

    public VaultService(IVaultRepo vaultRepo, IUserService userService)
    {
        _vaultRepo = vaultRepo;
        _userService = userService;
    }

    public async Task<List<VaultResponse>> GetVaultsByUserIdAsync(int userId)
    {
        await _userService.CheckUserAsync(userId);

        List<VaultModel> vaults = await _vaultRepo.GetVaultsByUserIdAsync(userId);
        List<VaultResponse> result = new();

        foreach(VaultModel vault in vaults)
        {
            result.Add(_MapVaultModelToResponse(vault));
        }

        return result;
    }
    public async Task<VaultResponse> CreateVaultByUserIdAsync(VaultRequest vault, int userId)
    {
        await _userService.CheckUserAsync(userId);

        VaultModel vaultModel = _MapVaultRequestAndUserIdToVaultModel(vault, userId);

        bool isAdded = await _vaultRepo.CreateVaultByUserIdAsync(vaultModel);

        return _MapVaultModelToResponse(vaultModel);
    }
    public async Task<bool> UpdateVaultByUserIdAsync(int vaultId, int userId, VaultRequest vault)
    {
        await _userService.CheckUserAsync(userId);

        VaultModel vaultModel = await CheckVaultExistsAndOwnerAsync(vaultId, userId); 

        vaultModel.Name = vault.Name;
        vaultModel.Description = vault.Description;

        bool isUpdated = await _vaultRepo.UpdateVaultAsync(vaultModel);

        return isUpdated;
    }
    public async Task<bool> DeleteVaultByUserIdAsync(int vaultId, int userId)
    {
        await _userService.CheckUserAsync(userId);

        await CheckVaultExistsAndOwnerAsync(vaultId, userId);


        bool isDeleted = await _vaultRepo.DeleteVaultAsync(vaultId);

        return isDeleted;
    }

 
    public async Task<VaultModel> CheckVaultExistsAndOwnerAsync(int vaultId , int userId)
    {
        VaultModel vaultModel = await _vaultRepo.GetVaultsByIdAsync(vaultId);

        if (vaultModel is null)
        {
            throw new KeyNotFoundException("Vault not found");
        }
        if (vaultModel.UserId != userId)
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        return vaultModel;
    }
    private VaultResponse _MapVaultModelToResponse(VaultModel vault)
    {
        return new VaultResponse
        {
            Id = vault.Id,
            Name = vault.Name,
            Description = vault.Description,
            CreatedAt = vault.CreatedAt
        };
    }
    private VaultModel _MapVaultRequestAndUserIdToVaultModel(VaultRequest vault , int userId)
    {
        return new VaultModel
        {
            Name = vault.Name,
            UserId = userId,
            Description = vault.Description
        };
    }
}
