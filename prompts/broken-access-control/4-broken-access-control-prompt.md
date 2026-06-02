We completed a security review of the TaskFlow Lite list endpoint.

Endpoint:
GET /api/workrequests

Current issue:
WorkRequestService.GetAsync starts from all work requests and applies optional filters afterward. It does not scope the result set to the current user before filters are applied.

Related behavior:
GET /api/workrequests/{id} already uses ICurrentUserContext.TryGetUserId(out int userId) and only returns records where the current user is the requester or assigned user.

Approved access rule for list results:
A caller can list only work requests where the caller is the requester or assigned user.

Required behavior:
- Missing or invalid current-user context must return no work requests.
- status can narrow the scoped result set.
- priority can narrow the scoped result set.
- assignedToUserId can narrow the scoped result set.
- assignedToUserId must not expand visibility beyond requester-or-assignee records.
- Do not add search yet.
- Do not add roles, managers, administrators, policies, or a new authentication system.
- Do not return EF Core entities.
- Preserve WorkRequestDto projection.

Required query order:
1. Resolve current-user context using TryGetUserId.
2. If invalid, return an empty list.
3. Start from work requests visible to the current user:
   - RequestedByUserId == currentUserId OR AssignedToUserId == currentUserId.
4. Apply optional status, priority, and assignedToUserId filters.
5. Order by CreatedAtUtc descending.
6. Project to WorkRequestDto.

Propose the smallest safe code change to harden the list endpoint before search is added.

Include:
1. Files to change
2. Expected code changes
3. Manual verification steps
4. Tests to add next
5. Remaining limitations