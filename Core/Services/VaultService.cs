using Core.Interfaces;
using Core.Requests;
using Core.Responses;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class VaultService : IVaultService
{
    private readonly IVaultRepo _vaultRepo;
    private readonly IUserService _userService;
    private readonly ILogger _logger;

    public VaultService(IVaultRepo vaultRepo, IUserService userService , ILogger<VaultService> logger)
    {
        _vaultRepo = vaultRepo;
        _userService = userService;
        _logger = logger;
    }

    public async Task<List<VaultResponse>> GetVaultsByUserIdAsync(int userId)
    {
        _logger.LogInformation("Get User vaults service start for user id: {userId}" , userId);

        await _userService.CheckUserAsync(userId);

        List<VaultModel> vaults = await _vaultRepo.GetVaultsByUserIdAsync(userId);
        List<VaultResponse> result = new();

        foreach(VaultModel vault in vaults)
        {
            result.Add(_MapVaultModelToResponse(vault));
        }

        _logger.LogInformation("Get User vaults service end for user id: {userId} Successful", userId);

        return result;
    }
    public async Task<VaultResponse> CreateVaultByUserIdAsync(VaultRequest vault, int userId)
    {
        _logger.LogInformation("Create User vault service start for user id: {userId}", userId);

        await _userService.CheckUserAsync(userId);

        VaultModel vaultModel = _MapVaultRequestAndUserIdToVaultModel(vault, userId);

        bool isAdded = await _vaultRepo.CreateVaultByUserIdAsync(vaultModel);

        _logger.LogInformation("Create User vault service end for user id: {userId} Successful, Vault Id: {vaultId}", userId , vaultModel.Id);

        return _MapVaultModelToResponse(vaultModel);
    }
    public async Task<bool> UpdateVaultByUserIdAsync(int vaultId, int userId, VaultRequest vault)
    {
        _logger.LogInformation("Update User vault service start for user id: {userId} on vault id: {vaultId}", userId , vaultId);

        await _userService.CheckUserAsync(userId);

        VaultModel vaultModel = await CheckVaultExistsAndOwnerAsync(vaultId, userId); 

        vaultModel.Name = vault.Name;
        vaultModel.Description = vault.Description;

        bool isUpdated = await _vaultRepo.UpdateVaultAsync(vaultModel);

        _logger.LogInformation("Update User vault service end for user id: {userId} on vault id: {vaultId} Successful", userId, vaultId);

        return isUpdated;
    }
    public async Task<bool> DeleteVaultByUserIdAsync(int vaultId, int userId)
    {
        _logger.LogInformation("Delete User vault service start for user id: {userId} on vault id: {vaultId}", userId, vaultId);

        await _userService.CheckUserAsync(userId);

        await CheckVaultExistsAndOwnerAsync(vaultId, userId);

        bool isDeleted = await _vaultRepo.DeleteVaultAsync(vaultId);

        _logger.LogInformation("Delete User vault service end for user id: {userId} on vault id: {vaultId} Successful", userId, vaultId);

        return isDeleted;
    }

 
    public async Task<VaultModel> CheckVaultExistsAndOwnerAsync(int vaultId , int userId)
    {
        _logger.LogInformation("Check User vault exists service start for user id: {userId} on vault id: {vaultId}", userId, vaultId);

        VaultModel? vaultModel = await _vaultRepo.GetVaultsByIdAsync(vaultId);

        if (vaultModel is null)
        {
            _logger.LogInformation("Check User vault exists service failed for user id: {userId} on vault id: {vaultId} (Not Found)", userId, vaultId);

            throw new KeyNotFoundException("Vault not found");
        }
        if (vaultModel.UserId != userId)
        {
            _logger.LogInformation("Check User vault exists service failed for user id: {userId} on vault id: {vaultId} (UnAuthorized)", userId, vaultId);

            throw new UnauthorizedAccessException("Access denied");
        }

        _logger.LogInformation("Check User vault exists service end for user id: {userId} on vault id: {vaultId} Successful", userId, vaultId);

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
