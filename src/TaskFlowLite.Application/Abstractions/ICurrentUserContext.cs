namespace TaskFlowLite.Application.Abstractions;

public interface ICurrentUserContext
{
    int UserId { get; }
    string DisplayName { get; }
}
