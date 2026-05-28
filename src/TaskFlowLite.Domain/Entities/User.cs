namespace TaskFlowLite.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<WorkRequest> RequestedWorkRequests { get; set; } = new List<WorkRequest>();
    public ICollection<WorkRequest> AssignedWorkRequests { get; set; } = new List<WorkRequest>();
    public ICollection<RequestNote> RequestNotes { get; set; } = new List<RequestNote>();
}
