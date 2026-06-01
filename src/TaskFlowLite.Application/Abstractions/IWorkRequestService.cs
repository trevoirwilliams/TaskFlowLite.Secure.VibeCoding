using TaskFlowLite.Application.Models.WorkRequests;
using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Application.Abstractions;

public interface IWorkRequestService
{
    Task<IReadOnlyList<WorkRequestDto>> GetAsync(
        WorkRequestStatus? status,
        Priority? priority,
        int? assignedToUserId,
        string? search,
        CancellationToken cancellationToken);

    Task<WorkRequestDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<WorkRequestDto> CreateAsync(CreateWorkRequestRequest request, CancellationToken cancellationToken);
    Task<WorkRequestDto?> UpdateAsync(int id, UpdateWorkRequestRequest request, CancellationToken cancellationToken);
    Task<WorkRequestDto?> AssignAsync(int id, AssignWorkRequestRequest request, CancellationToken cancellationToken);
    Task<WorkRequestDto?> UpdateStatusAsync(int id, UpdateWorkRequestStatusRequest request, CancellationToken cancellationToken);
}
