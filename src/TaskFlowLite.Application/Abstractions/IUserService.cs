using TaskFlowLite.Application.Models.Users;

namespace TaskFlowLite.Application.Abstractions;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetActiveUsersAsync(CancellationToken cancellationToken);
}
