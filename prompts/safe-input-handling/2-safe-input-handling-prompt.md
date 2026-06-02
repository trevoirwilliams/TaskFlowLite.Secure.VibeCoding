We are improving the TaskFlow Lite work request list endpoint after search has already been added safely.

Task:
Refactor the list query inputs into a dedicated request model.

Requirements:
- Create a WorkRequestListQuery model under the WorkRequests application models namespace.
- Include Status, Priority, AssignedToUserId, and Search properties.
- Apply a 100-character validation rule to Search using DataAnnotations.
- Provide a safe way to access a trimmed/normalized search value.
- Update WorkRequestsController.Get to bind the query model from the query string.
- Update IWorkRequestService.GetAsync to accept the query model.
- Update WorkRequestService.GetAsync to use the query model.
- Preserve the current query order:
  1. Resolve current-user context.
  2. Scope records to requester-or-assignee.
  3. Apply status, priority, and assignedToUserId filters.
  4. Apply search inside the scoped query.
  5. Order and project to WorkRequestDto.
- Do not add new features.
- Do not add paging, sorting, roles, policies, or a new authentication system.
- Do not use raw SQL.
- Do not return EF Core entities.

Before writing code:
1. Summarize the refactoring plan.
2. List the files that will change.
3. Explain how validation behavior will be preserved.
4. Explain how the access-control query order will be preserved.

Then provide the focused code changes.