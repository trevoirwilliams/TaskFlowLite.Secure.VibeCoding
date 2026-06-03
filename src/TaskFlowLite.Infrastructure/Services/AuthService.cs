using Microsoft.AspNetCore.Identity;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.Auth;
using TaskFlowLite.Domain.Entities;

namespace TaskFlowLite.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var trimmedDisplayName = request.DisplayName.Trim();

        var existing = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existing is not null)
        {
            return AuthResult.Failure("An account with that email already exists.");
        }

        var identityUser = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            EmailConfirmed = true,
            DisplayName = trimmedDisplayName,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(identityUser, request.Password);
        if (!createResult.Succeeded)
        {
            return AuthResult.Failure(createResult.Errors.Select(x => x.Description));
        }

        var token = _tokenService.CreateAccessToken(identityUser.Id, normalizedEmail, identityUser.DisplayName);
        return AuthResult.Success(token);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var identityUser = await _userManager.FindByEmailAsync(normalizedEmail);

        if (identityUser is null)
        {
            return AuthResult.Failure("Invalid email or password.");
        }

        var passwordResult = await _signInManager.CheckPasswordSignInAsync(identityUser, request.Password, lockoutOnFailure: true);
        if (!passwordResult.Succeeded)
        {
            return AuthResult.Failure("Invalid email or password.");
        }

        if (!identityUser.IsActive)
        {
            return AuthResult.Failure("Invalid email or password.");
        }

        var token = _tokenService.CreateAccessToken(identityUser.Id, identityUser.Email ?? normalizedEmail, identityUser.DisplayName);
        return AuthResult.Success(token);
    }
}
