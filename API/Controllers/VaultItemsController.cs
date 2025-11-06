using Core.Interfaces;
using Core.Requests;
using Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers;


[Route("api/vaults/{vaultId}/items")]
[ApiController]
[Authorize]
public class VaultItemsController : ControllerBase
{
    private readonly IVaultItemsService _vaultItemsService;

    public VaultItemsController(IVaultItemsService vaultItemsService)
    {
        _vaultItemsService = vaultItemsService;
    }

    // <start>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<VaultItemsResponse>) , StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<VaultItemsResponse>>> GetVaultItemsAsync([FromRoute]int vaultId)
    {
        try
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value!);

            List<VaultItemsResponse> vaultItems = await _vaultItemsService.GetVaultItemsByUserIdAndVaultIdAsync(userId , vaultId);

            return Ok(vaultItems);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    // </end>

    // <start>
    [HttpGet("{itemId}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(VaultItemsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VaultItemsResponse>> GetVaultItemByIdAsync([FromRoute] int vaultId , [FromRoute] int itemId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value!);

            VaultItemsResponse vaultItems = await _vaultItemsService.GetVaultItemByUserIdAndVaultIdAndItemIdAsync(userId, vaultId , itemId);

            return Ok(vaultItems);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    // </end>

    // <start>
    [HttpDelete("{itemId}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteVaultItemByIdAsync([FromRoute] int vaultId, [FromRoute] int itemId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value!);

            bool isDeleted = await _vaultItemsService.DeleteVaultItemByUserIdAndVaultIdAndItemIdAsync(userId, vaultId, itemId);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    // </end>

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(VaultItemsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VaultItemsResponse>> CreateVaultItemAsync([FromRoute] int vaultId , [FromBody]VaultItemRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value!);

            VaultItemsResponse response = await _vaultItemsService.CreateVaultItemAsync(userId, vaultId, request);

            return Created("", response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    // </end>
}
