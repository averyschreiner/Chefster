using Chefster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult> CreatedGordonResponse([FromBody] string question)
    {
        var response = await _gordonService.GetMessageResponse(question);

        if (response.Success != true)
        {
            return BadRequest($"Failed to get Gordon response");
        }

        return Ok(response);
    }
}