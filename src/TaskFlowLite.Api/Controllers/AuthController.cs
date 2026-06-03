using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.Auth;

namespace TaskFlowLite.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromServices] IAuthService authService,
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                ["register"] = result.Errors.ToArray()
            })
            {
                Title = "Registration failed."
            });
        }

        return Ok(result.Response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromServices] IAuthService authService,
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed.",
                Detail = "Invalid email or password.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(result.Response);
    }
}
