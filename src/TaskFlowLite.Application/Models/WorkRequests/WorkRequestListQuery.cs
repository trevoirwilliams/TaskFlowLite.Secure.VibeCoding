using System.ComponentModel.DataAnnotations;
using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Application.Models.WorkRequests;

public sealed class WorkRequestListQuery
{
    [StringLength(100)]
    public string? Search { get; set; }

    public WorkRequestStatus? Status { get; set; }

    public Priority? Priority { get; set; }

    public int? AssignedToUserId { get; set; }

    public string? NormalizedSearch => string.IsNullOrWhiteSpace(Search)
        ? null
        : Search.Trim();
}