namespace TaskFlowLite.Domain.Entities;

public class RequestNote
{
    public int Id { get; set; }
    public int WorkRequestId { get; set; }
    public int AuthorUserId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public WorkRequest WorkRequest { get; set; } = null!;
    public ApplicationUser AuthorUser { get; set; } = null!;
}
