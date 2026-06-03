using Microsoft.EntityFrameworkCore;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.Users;
using TaskFlowLite.Domain.Entities;
using TaskFlowLite.Infrastructure.Persistence;

namespace TaskFlowLite.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IQueryable<ApplicationUser> _users;

    public UserService(TaskFlowLiteDbContext dbContext)
    {
        _users = dbContext.Users;
    }

    public async Task<IReadOnlyList<UserDto>> GetActiveUsersAsync(CancellationToken cancellationToken)
    {
        return await _users
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayName)
            .Select(x => new UserDto(x.Id, x.DisplayName, x.Email ?? string.Empty))
            .ToListAsync(cancellationToken);
    }
}
