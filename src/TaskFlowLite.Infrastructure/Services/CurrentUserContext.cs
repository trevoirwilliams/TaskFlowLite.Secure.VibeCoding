using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskFlowLite.Application.Abstractions;
using System.IdentityModel.Tokens.Jwt;

namespace TaskFlowLite.Infrastructure.Services;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool TryGetUserId(out int userId)
    {
        userId = default;
        var httpContext = _httpContextAccessor.HttpContext;

        var claimValue = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? httpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return int.TryParse(claimValue, out userId) && userId > 0;
    }

    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        return _httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
    }

    public int UserId
    {
        get
        {
            if (TryGetUserId(out var userId))
            {
                return userId;
            }

            throw new InvalidOperationException("Current user context is missing or invalid.");
        }
    }

    public string DisplayName =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
        ?? "TaskFlow Local User";
}
