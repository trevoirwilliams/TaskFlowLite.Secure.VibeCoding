using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowLite.Application.Abstractions;

namespace TaskFlowLite.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetActiveUsers(
        [FromServices] IUserService userService,
        CancellationToken cancellationToken)
    {
        var users = await userService.GetActiveUsersAsync(cancellationToken);
        return Ok(users);
    }
}
