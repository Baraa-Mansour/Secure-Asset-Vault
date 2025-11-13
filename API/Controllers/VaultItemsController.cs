using Core.Interfaces;
using Core.Requests;
using Core.Responses;
using Core.Utilities.Attributes;
using Core.Utilities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
    // workflow
    [LogAction]
    [ServiceFilter(typeof(LogActionFilter))]
    public async Task<ActionResult<IEnumerable<VaultItemsResponse>>> GetVaultItemsAsync([FromRoute]int vaultId)
    {
        try
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

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
    [HttpGet("{itemId:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(VaultItemsDetailedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // workflow
    [LogAction]
    [ServiceFilter(typeof(LogActionFilter))]
    public async Task<ActionResult<VaultItemsDetailedResponse>> GetVaultItemByIdAsync([FromRoute] int vaultId , [FromRoute] int itemId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            VaultItemsDetailedResponse vaultItems = await _vaultItemsService.GetVaultItemByUserIdAndVaultIdAndItemIdAsync(userId, vaultId , itemId);

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
    [HttpDelete("{itemId:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // workflow
    [LogAction]
    [ServiceFilter(typeof(LogActionFilter))]
    public async Task<ActionResult> DeleteVaultItemByIdAsync([FromRoute] int vaultId, [FromRoute] int itemId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

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

    // <start>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(VaultItemsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // workflow
    [LogAction]
    [ServiceFilter(typeof(LogActionFilter))]
    public async Task<ActionResult<VaultItemsResponse>> CreateVaultItemAsync([FromRoute] int vaultId , [FromBody]VaultItemRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

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
