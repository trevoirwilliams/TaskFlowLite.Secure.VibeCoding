using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.Application.Models.WorkRequests;

public sealed record UpdateWorkRequestStatusRequest(WorkRequestStatus Status);
