using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Domain.Entities;

public class WorkRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public WorkRequestStatus Status { get; private set; } = WorkRequestStatus.New;
    public int RequestedByUserId { get; set; }
    public int? AssignedToUserId { get; private set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? ClosedAtUtc { get; private set; }

    public ApplicationUser RequestedByUser { get; set; } = null!;
    public ApplicationUser? AssignedToUser { get; private set; }
    public ICollection<RequestNote> Notes { get; set; } = new List<RequestNote>();

    public void UpdateDetails(string title, string description, Priority priority)
    {
        Title = title;
        Description = description;
        Priority = priority;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void AssignTo(int? assignedToUserId)
    {
        AssignedToUserId = assignedToUserId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeStatus(WorkRequestStatus newStatus)
    {
        if (Status is WorkRequestStatus.Done or WorkRequestStatus.Cancelled && newStatus is WorkRequestStatus.InProgress)
        {
            throw new InvalidOperationException("A completed or cancelled request cannot move back to in progress.");
        }

        Status = newStatus;
        UpdatedAtUtc = DateTime.UtcNow;
        ClosedAtUtc = newStatus is WorkRequestStatus.Done or WorkRequestStatus.Cancelled
            ? DateTime.UtcNow
            : null;
    }
}
