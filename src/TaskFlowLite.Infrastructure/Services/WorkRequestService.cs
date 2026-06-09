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
        WorkRequestListQuery query,
        CancellationToken cancellationToken)
    {
        if (!_currentUserContext.TryGetUserId(out var currentUserId))
        {
            return [];
        }

        IQueryable<WorkRequest> scopedQuery = _dbContext.WorkRequests
            .AsNoTracking()
            .Where(x => x.RequestedByUserId == currentUserId || x.AssignedToUserId == currentUserId);

        if (query.Status.HasValue)
        {
            scopedQuery = scopedQuery.Where(x => x.Status == query.Status.Value);
        }

        if (query.Priority.HasValue)
        {
            scopedQuery = scopedQuery.Where(x => x.Priority == query.Priority.Value);
        }

        if (query.AssignedToUserId.HasValue)
        {
            scopedQuery = scopedQuery.Where(x => x.AssignedToUserId == query.AssignedToUserId.Value);
        }

        var normalizedSearch = query.NormalizedSearch;
        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            scopedQuery = scopedQuery.Where(x => x.Title.Contains(normalizedSearch) || x.Description.Contains(normalizedSearch));
        }

        return await scopedQuery
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(ToDto())
            .ToListAsync(cancellationToken);
    }

    public async Task<WorkRequestDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (!_currentUserContext.TryGetUserId(out var currentUserId))
        {
            return null;
        }

        return await _dbContext.WorkRequests
            .AsNoTracking()
            .Where(x => x.Id == id
                && (x.RequestedByUserId == currentUserId || x.AssignedToUserId == currentUserId))
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
        if (!_currentUserContext.TryGetUserId(out var currentUserId))
        {
            return null;
        }

        var entity = await _dbContext.WorkRequests.FirstOrDefaultAsync(
            x => x.Id == id && (x.RequestedByUserId == currentUserId || x.AssignedToUserId == currentUserId),
            cancellationToken);

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
        if (!_currentUserContext.TryGetUserId(out var currentUserId))
        {
            return null;
        }

        var canManageAnyRequest = _currentUserContext.IsInRole("Manager");
        var canUpdateAssignedRequests = _currentUserContext.IsInRole("Worker");

        if (!canManageAnyRequest && !canUpdateAssignedRequests)
        {
            return null;
        }

        IQueryable<WorkRequest> statusScope = _dbContext.WorkRequests.Where(x => x.Id == id);
        if (!canManageAnyRequest)
        {
            statusScope = statusScope.Where(x => x.AssignedToUserId == currentUserId);
        }

        var entity = await statusScope.FirstOrDefaultAsync(cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.ChangeStatus(request.Status);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new WorkRequestDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Priority,
            entity.Status,
            entity.RequestedByUserId,
            entity.AssignedToUserId,
            entity.CreatedAtUtc,
            entity.UpdatedAtUtc,
            entity.ClosedAtUtc);
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
