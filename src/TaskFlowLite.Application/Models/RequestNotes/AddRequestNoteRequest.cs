using System.ComponentModel.DataAnnotations;

namespace TaskFlowLite.Application.Models.RequestNotes;

public sealed record AddRequestNoteRequest(
    [property: Required, StringLength(2000)] string Body);
