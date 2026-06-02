using TaskFlowLite.Application.Models.WorkRequests;

namespace TaskFlowLite.Application.Abstractions;

public interface IWorkRequestService
{
    Task<IReadOnlyList<WorkRequestDto>> GetAsync(
        WorkRequestListQuery query,
        CancellationToken cancellationToken);

    Task<WorkRequestDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<WorkRequestDto> CreateAsync(CreateWorkRequestRequest request, CancellationToken cancellationToken);
    Task<WorkRequestDto?> UpdateAsync(int id, UpdateWorkRequestRequest request, CancellationToken cancellationToken);
    Task<WorkRequestDto?> AssignAsync(int id, AssignWorkRequestRequest request, CancellationToken cancellationToken);
    Task<WorkRequestDto?> UpdateStatusAsync(int id, UpdateWorkRequestStatusRequest request, CancellationToken cancellationToken);
}
