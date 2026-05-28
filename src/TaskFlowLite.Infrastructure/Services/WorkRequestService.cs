using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskFlowLite.Application.Abstractions;
using TaskFlowLite.Application.Models.WorkRequests;
using TaskFlowLite.Domain.Entities;
using TaskFlowLite.Domain.Enums;
using TaskFlowLite.Infrastructure.Persistence;

namespace TaskFlowLite.Infrastructure.Services;

public class WorkRequestService : IWorkRequestService
{
    private readonly TaskFlowLiteDbContext _dbContext;
    private readonly ICurrentUserContext _currentUserContext;

    public WorkRequestService(TaskFlowLiteDbContext dbContext, ICurrentUserContext currentUserContext)
    {
        _dbContext = dbContext;
        _currentUserContext = currentUserContext;
    }

    public async Task<IReadOnlyList<WorkRequestDto>> GetAsync(
        WorkRequestStatus? status,
        Priority? priority,
        int? assignedToUserId,
        CancellationToken cancellationToken)
    {
        IQueryable<WorkRequest> query = _dbContext.WorkRequests.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(x => x.Priority == priority.Value);
        }

        if (assignedToUserId.HasValue)
        {
            query = query.Where(x => x.AssignedToUserId == assignedToUserId.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(ToDto())
            .ToListAsync(cancellationToken);
    }

    public async Task<WorkRequestDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.WorkRequests
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(ToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WorkRequestDto> CreateAsync(CreateWorkRequestRequest request, CancellationToken cancellationToken)
    {
        var workRequest = new WorkRequest
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Priority = request.Priority,
            RequestedByUserId = _currentUserContext.UserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        workRequest.AssignTo(request.AssignedToUserId);

        _dbContext.WorkRequests.Add(workRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(workRequest.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created work request could not be reloaded.");
    }

    public async Task<WorkRequestDto?> UpdateAsync(int id, UpdateWorkRequestRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.WorkRequests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.UpdateDetails(request.Title.Trim(), request.Description.Trim(), request.Priority);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<WorkRequestDto?> AssignAsync(int id, AssignWorkRequestRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.WorkRequests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.AssignTo(request.AssignedToUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<WorkRequestDto?> UpdateStatusAsync(int id, UpdateWorkRequestStatusRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.WorkRequests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.ChangeStatus(request.Status);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    private static Expression<Func<WorkRequest, WorkRequestDto>> ToDto()
    {
        return x => new WorkRequestDto(
            x.Id,
            x.Title,
            x.Description,
            x.Priority,
            x.Status,
            x.RequestedByUserId,
            x.AssignedToUserId,
            x.CreatedAtUtc,
            x.UpdatedAtUtc,
            x.ClosedAtUtc);
    }
}
