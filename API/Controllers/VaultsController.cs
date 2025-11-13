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


[Route("api/vaults")]
[ApiController]
[Authorize]
public class VaultsController : ControllerBase
{
    private readonly IVaultService _vaultService;

    public VaultsController(IVaultService vaultService)
    {
        this._vaultService = vaultService;
    }


    // <start>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<VaultResponse>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // workflow
    [LogAction]
    [ServiceFilter(typeof(LogActionFilter))]
    public async Task<ActionResult<IEnumerable<VaultResponse>>> GetUserVaultsAsync()
    {
        try
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            List<VaultResponse> vaults = await _vaultService.GetVaultsByUserIdAsync(userId);

            return Ok(vaults);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch(UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Something went wrong: {ex.Message}!");
        }
    }
    // </end>

    // <start>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(VaultResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // workflow
    [LogAction]
    [ServiceFilter(typeof(LogActionFilter))]
    public async Task<ActionResult<VaultResponse>> CreateUserVaultAsync([FromBody]VaultRequest request)
    {
        try
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            VaultResponse response = await _vaultService.CreateVaultByUserIdAsync(request , userId);

            return StatusCode(201,response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch(InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500 , ex.Message);
        }
    }
    // </end>

    // <start>
    [HttpPut("{vaultId:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // workflow
    [LogAction]
    [ServiceFilter(typeof(LogActionFilter))]
    public async Task<ActionResult> UpdateUserVaultAsync([FromRoute]int vaultId, [FromBody]VaultRequest request)
    {
        try
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            bool isUpdated = await _vaultService.UpdateVaultByUserIdAsync(vaultId, userId, request);

            if (!isUpdated)
            {
                return StatusCode(500, "Something went wrong while proccess!");
            }
            return NoContent();
        }
        catch(UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch(InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch(Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    // </end>

    // <start>
    [HttpDelete("{vaultId:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // workflow
    [LogAction]
    [ServiceFilter(typeof(LogActionFilter))]
    public async Task<ActionResult> DeleteUserVaultAsync([FromRoute] int vaultId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            bool isDeleted = await _vaultService.DeleteVaultByUserIdAsync(vaultId, userId);

            if (!isDeleted)
            {
                return StatusCode(500, "Something went wrong while proccess!");
            }
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    // </end>
}
