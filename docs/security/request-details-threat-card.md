# Threat Model Scope

Feature under review:
GET /api/workrequests/{id}

Current behavior:
The endpoint retrieves a single work request by integer ID and returns a WorkRequestDto if found.

Primary security concern:
An authenticated or unauthenticated caller may be able to retrieve a work request they should not be allowed to view by changing the route ID or spoofing the current-user header.

## Current Baseline

The endpoint exists in WorkRequestsController and calls IWorkRequestService.GetByIdAsync.
The service queries WorkRequests by ID and returns a WorkRequestDto.
The current implementation does not apply an ownership check inside GetByIdAsync.
The application currently uses X-TaskFlow-UserId as a temporary current-user mechanism.

## Caller-Controlled Inputs

- Route value: id
- Header value: X-TaskFlow-UserId
- Request frequency and enumeration pattern

## Primary Threats

| Threat | Current Risk | Security Question |
|---|---|---|
| ID tampering | A caller can change the route ID | Should the service filter by ownership or assignment? |
| User spoofing | Caller can change X-TaskFlow-UserId | Should this be replaced or guarded before relying on it? |
| Missing authentication | Controller does not require an authenticated principal | Should the endpoint reject unauthenticated callers? |
| Broken horizontal access control | Query filters only by work request ID | Can user 2 retrieve user 1's request? |
| Excessive data exposure | DTO exposes raw user IDs | Does the client need raw IDs or safer display data? |
| Notes exposure | Notes can be fetched by workRequestId | Should notes require the same access rule as request details? |
| Enumeration | Sequential IDs may be guessed | Should unauthorized records return 403 or safe 404? |
| Default user fallback | Missing header becomes user 1 | Could callers accidentally or maliciously gain user 1 context? |

## Required Security Decisions

1. Decide whether this endpoint should require formal authentication now or remain header-based for the current checkpoint.
2. Decide whether a caller can view a request only when they are the requester or assignee.
3. Decide whether administrators or managers exist in the current scope.
4. Decide whether unauthorized access should return 403 or a safe 404.
5. Decide whether `RequestedByUserId` and `AssignedToUserId` should remain in the response.
6. Decide whether notes should be included in request details or remain a separate endpoint.
7. Decide whether notes need the same authorization rule as the parent request.

## Acceptance Criteria

1. The endpoint must not return a work request solely because the caller knows the ID.
2. The endpoint must validate access using server-side current-user context.
3. A requester can view their own work request.
4. An assigned user can view a work request assigned to them.
5. An unrelated user must not be able to view the request.
6. Missing records must return a safe 404.
7. Unauthorized records must return either 403 or safe 404 based on the chosen API policy.
8. The response must not expose fields that the client does not need.
9. The implementation must avoid returning EF Core entities directly.
10. Notes, if added to request details, must follow the same access rule as the parent request.

## Test Ideas

1. GET existing request as requester returns 200.
2. GET existing request as assigned user returns 200.
3. GET existing request as unrelated user is blocked.
4. GET missing request returns 404.
5. GET request without X-TaskFlow-UserId does not silently grant unsafe access.
6. GET request with invalid X-TaskFlow-UserId does not silently grant unsafe access.
7. Response does not include unexpected navigation properties.
8. Response does not expose user email addresses.
9. Notes cannot be retrieved for a request the caller cannot view.
10. Repeated ID probing does not reveal sensitive differences in response details.