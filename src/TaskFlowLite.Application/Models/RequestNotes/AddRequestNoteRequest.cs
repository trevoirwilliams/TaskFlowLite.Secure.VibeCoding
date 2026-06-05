using System.ComponentModel.DataAnnotations;

namespace TaskFlowLite.Application.Models.RequestNotes;

public sealed record AddRequestNoteRequest(
    [param: Required, StringLength(2000)] string Body);
