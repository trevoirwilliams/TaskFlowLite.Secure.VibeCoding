namespace TaskFlowLite.Application.Models.Users;

public sealed record UserDto(
    int Id,
    string DisplayName,
    string Email);
