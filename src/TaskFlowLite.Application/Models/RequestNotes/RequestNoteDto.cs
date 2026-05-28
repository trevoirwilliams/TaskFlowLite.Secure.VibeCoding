namespace TaskFlowLite.Application.Models.RequestNotes;

public sealed record RequestNoteDto(
    int Id,
    int WorkRequestId,
    int AuthorUserId,
    string Body,
    DateTime CreatedAtUtc);
