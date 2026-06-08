namespace TaskFlowLite.Application.Abstractions;

public interface ICurrentUserContext
{
    bool TryGetUserId(out int userId);
    bool IsInRole(string role);
    int UserId { get; }
    string DisplayName { get; }
}
