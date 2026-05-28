using Microsoft.EntityFrameworkCore;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.Users;
using TaskFlowLite.Infrastructure.Persistence;

namespace TaskFlowLite.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly TaskFlowLiteDbContext _dbContext;

    public UserService(TaskFlowLiteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<UserDto>> GetActiveUsersAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayName)
            .Select(x => new UserDto(x.Id, x.DisplayName, x.Email))
            .ToListAsync(cancellationToken);
    }
}
