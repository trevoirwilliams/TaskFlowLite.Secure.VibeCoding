using Microsoft.AspNetCore.Http;
using TaskFlowLite.Application.Abstractions;

namespace TaskFlowLite.Infrastructure.Services;

public class CurrentUserContext : ICurrentUserContext
{
    private const int DefaultUserId = 1;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var headerValue = _httpContextAccessor.HttpContext?.Request.Headers["X-TaskFlow-UserId"].FirstOrDefault();
            return int.TryParse(headerValue, out var userId) ? userId : DefaultUserId;
        }
    }

    public string DisplayName => "TaskFlow Local User";
}
