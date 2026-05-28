using TaskFlowLite.Application.Models.RequestNotes;

namespace TaskFlowLite.Application.Abstractions;

public interface IRequestNoteService
{
    Task<IReadOnlyList<RequestNoteDto>> GetForWorkRequestAsync(int workRequestId, CancellationToken cancellationToken);
    Task<RequestNoteDto?> AddAsync(int workRequestId, AddRequestNoteRequest request, CancellationToken cancellationToken);
}
