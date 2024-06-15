using Chefster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Chefster.Controllers;

[Authorize]
[Route("api/gordon")]
[ApiController]
public class GordonController(GordonService gordonService) : ControllerBase
{
    private readonly GordonService _gordonService = gordonService;

    /// <summary>
    /// FOR TESTING ONLY
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreatedGordonResponse([FromBody] string request)
    {
        if (request.IsNullOrEmpty())
        {
            return BadRequest("Request must not be empty!");
        }
        var response = await _gordonService.GetMessageResponse(request);

        if (response.Success != true)
        {
            return BadRequest($"Failed to get Gordon response. Error: {response.Error}");
        }

        return Ok(response.Data);
    }
}
