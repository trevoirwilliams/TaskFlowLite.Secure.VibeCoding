using System.ComponentModel.DataAnnotations;
using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Application.Models.WorkRequests;

public sealed record CreateWorkRequestRequest(
    [param: Required, StringLength(120)] string Title,
    [param: Required, StringLength(2000)] string Description,
    Priority Priority,
    int? AssignedToUserId);
