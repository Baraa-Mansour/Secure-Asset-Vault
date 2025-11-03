using Core.Interfaces;
using Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/vaults")]
    [ApiController]
    public class VaultsController : ControllerBase
    {
        private readonly IVaultService _vaultService;

        public VaultsController(IVaultService vaultService)
        {
            this._vaultService = vaultService;
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<List<VaultResponse>>> GetUserVaultsAsync(int Id)
        {
            try
            {
                List<VaultResponse> vaults = await _vaultService.GetUserVaultsByIdAsync(Id);



                return Ok(vaults);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Something went wrong: {ex.Message}!");
            }
        }
    }
}
