We are reviewing the TaskFlow Lite ASP.NET Core Web API request details feature for a security hardening checkpoint.

Feature under review:
GET /api/workrequests/{id}

Current behavior:
- WorkRequestsController.GetById calls IWorkRequestService.GetByIdAsync(id).
- WorkRequestService.GetByIdAsync currently queries WorkRequests by Id only.
- The application uses ICurrentUserContext to represent the current user.
- The current-user context is temporarily backed by a request header in the local/internal development environment.
- We are not replacing the identity mechanism in this change. That limitation must be documented.

Security objective:
A caller must not receive a work request simply because they know or guess the request Id.

Access rule:
- The requester can view the work request.
- The assigned user can view the work request.
- An unrelated user must not view the work request.
- Missing records should return a safe 404.
- Unauthorized records should use a consistent API behavior: either 403 or safe 404. Recommend one and explain the tradeoff.
- Missing or invalid current-user context must not silently grant access.

Do not write code yet.

Create an access-control plan with the following sections:
1. Current risk
2. Recommended authorization rule
3. Where the rule should be enforced
4. Recommended API response behavior
5. Data exposure concerns in the response DTO
6. Changes needed in service/controller/current-user handling
7. Tests required before accepting the change
8. Risks or limitations that remain after this checkpoint