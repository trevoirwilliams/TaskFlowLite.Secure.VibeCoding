We have an approved access-control plan for the TaskFlow Lite ASP.NET Core Web API request details endpoint.

Implement the plan with the smallest safe code change.

Feature:
GET /api/workrequests/{id}

Implementation constraints:
- Do not add a full authentication system in this change.
- Do not introduce roles, admin overrides, or manager behavior.
- Do not return EF Core entities from the API.
- Keep WorkRequestsController thin if possible.
- Enforce the access rule in the service/data-access path.
- Preserve the explicit WorkRequestDto response shape.
- Keep the change focused and easy to review.

Please update the relevant code and briefly summarize:
1. Which files changed
2. How missing or invalid current-user context is handled
3. Where the access rule is enforced
4. Why the endpoint still returns a safe 404
5. What tests should be added next