We are updating the TaskFlow Lite ASP.NET Core Web API list endpoint.

Endpoint:
GET /api/workrequests

Task:
Add an optional search query parameter safely.

Required behavior:
- Add a nullable string query parameter named search to GET /api/workrequests.
- Pass search through IWorkRequestService.GetAsync.
- Trim the search value before applying it.
- Treat null, empty, or whitespace-only search as no search.
- Reject or ignore search values longer than 100 characters. Recommend the safer API behavior and explain it before coding.
- Search only WorkRequest.Title and WorkRequest.Description.
- Apply search only after the query has been scoped to the current user.
- Keep status, priority, and assignedToUserId as narrowing filters.
- Use EF Core query composition.
- Do not use raw SQL.
- Do not return EF Core entities.
- Preserve WorkRequestDto projection.
- Do not add roles, managers, administrators, policies, or a new authentication system.

Before writing code:
1. Summarize the implementation plan.
2. State where search will be added in the query order.
3. State how oversized search input will be handled.
4. State what files will change.

Then provide the focused code changes.