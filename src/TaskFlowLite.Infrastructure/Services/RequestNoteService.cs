using Microsoft.EntityFrameworkCore;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.RequestNotes;
using TaskFlowLite.Domain.Entities;
using TaskFlowLite.Infrastructure.Persistence;

namespace TaskFlowLite.Infrastructure.Services;

public class RequestNoteService : IRequestNoteService
{
    private readonly TaskFlowLiteDbContext _dbContext;
    private readonly ICurrentUserContext _currentUserContext;

    public RequestNoteService(TaskFlowLiteDbContext dbContext, ICurrentUserContext currentUserContext)
    {
        _dbContext = dbContext;
        _currentUserContext = currentUserContext;
    }

    public async Task<IReadOnlyList<RequestNoteDto>> GetForWorkRequestAsync(int workRequestId, CancellationToken cancellationToken)
    {
        return await _dbContext.RequestNotes
            .AsNoTracking()
            .Where(x => x.WorkRequestId == workRequestId)
            .OrderBy(x => x.CreatedAtUtc)
            .Select(x => new RequestNoteDto(x.Id, x.WorkRequestId, x.AuthorUserId, x.Body, x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<RequestNoteDto?> AddAsync(int workRequestId, AddRequestNoteRequest request, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.WorkRequests.AnyAsync(x => x.Id == workRequestId, cancellationToken);
        if (!exists)
        {
            return null;
        }

        var note = new RequestNote
        {
            WorkRequestId = workRequestId,
            AuthorUserId = _currentUserContext.UserId,
            Body = request.Body.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.RequestNotes.Add(note);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RequestNoteDto(note.Id, note.WorkRequestId, note.AuthorUserId, note.Body, note.CreatedAtUtc);
    }
}
