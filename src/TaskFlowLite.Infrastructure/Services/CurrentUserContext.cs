using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskFlowLite.Application.Abstractions;

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

        var claimValue = httpContext?.User.FindFirstValue(TaskFlowClaimTypes.UserId)
            ?? httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(claimValue, out userId) && userId > 0)
        {
            return true;
        }

        var headerValue = httpContext?.Request.Headers["X-TaskFlow-UserId"].FirstOrDefault();
        return int.TryParse(headerValue, out userId) && userId > 0;
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
