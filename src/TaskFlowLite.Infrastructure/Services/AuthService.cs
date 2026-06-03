using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.Auth;
using TaskFlowLite.Domain.Entities;
using TaskFlowLite.Infrastructure.Identity;
using TaskFlowLite.Infrastructure.Persistence;

namespace TaskFlowLite.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly TaskFlowLiteDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        TaskFlowLiteDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
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

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var domainUser = new User
        {
            DisplayName = trimmedDisplayName,
            Email = normalizedEmail,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Users.Add(domainUser);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var identityUser = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            EmailConfirmed = true,
            DomainUserId = domainUser.Id
        };

        var createResult = await _userManager.CreateAsync(identityUser, request.Password);
        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AuthResult.Failure(createResult.Errors.Select(x => x.Description));
        }

        await transaction.CommitAsync(cancellationToken);

        var token = _tokenService.CreateAccessToken(identityUser.Id, domainUser.Id, normalizedEmail, domainUser.DisplayName);
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

        var domainUser = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == identityUser.DomainUserId, cancellationToken);

        if (domainUser is null || !domainUser.IsActive)
        {
            return AuthResult.Failure("This account is unavailable.");
        }

        var token = _tokenService.CreateAccessToken(identityUser.Id, domainUser.Id, domainUser.Email, domainUser.DisplayName);
        return AuthResult.Success(token);
    }
}
