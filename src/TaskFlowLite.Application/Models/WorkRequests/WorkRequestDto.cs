using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Application.Models.WorkRequests;

public sealed record WorkRequestDto(
    int Id,
    string Title,
    string Description,
    Priority Priority,
    WorkRequestStatus Status,
    int RequestedByUserId,
    int? AssignedToUserId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    DateTime? ClosedAtUtc);
