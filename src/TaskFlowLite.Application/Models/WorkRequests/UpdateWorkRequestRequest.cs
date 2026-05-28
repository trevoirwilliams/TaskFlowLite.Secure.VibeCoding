using System.ComponentModel.DataAnnotations;
using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Application.Models.WorkRequests;

public sealed record UpdateWorkRequestRequest(
    [property: Required, StringLength(120)] string Title,
    [property: Required, StringLength(2000)] string Description,
    Priority Priority);
