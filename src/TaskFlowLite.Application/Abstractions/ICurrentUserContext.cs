namespace TaskFlowLite.Application.Abstractions;

public interface ICurrentUserContext
{
    bool TryGetUserId(out int userId);
    int UserId { get; }
    string DisplayName { get; }
}
