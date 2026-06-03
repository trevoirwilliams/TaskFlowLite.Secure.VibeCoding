using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.Auth;
using TaskFlowLite.Infrastructure.Authentication;

namespace TaskFlowLite.Infrastructure.Services;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AuthResponse CreateAccessToken(int identityUserId, int domainUserId, string email, string displayName)
    {
        var now = DateTime.UtcNow;
        var expiresAtUtc = now.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identityUserId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(ClaimTypes.NameIdentifier, domainUserId.ToString()),
            new(ClaimTypes.Name, displayName),
            new(TaskFlowClaimTypes.UserId, domainUserId.ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AuthResponse(
            AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
            TokenType: "Bearer",
            ExpiresAtUtc: expiresAtUtc,
            UserId: domainUserId,
            Email: email,
            DisplayName: displayName);
    }
}
