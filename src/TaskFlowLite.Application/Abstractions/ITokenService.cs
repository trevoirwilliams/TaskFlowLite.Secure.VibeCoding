using TaskFlowLite.Application.Models.Auth;

namespace TaskFlowLite.Application.Abstractions;

public interface ITokenService
{
    AuthResponse CreateAccessToken(int identityUserId, int domainUserId, string email, string displayName);
}
