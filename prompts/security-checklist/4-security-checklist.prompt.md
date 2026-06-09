Use the Secure Coding Agent instructions in .github/instructions/secure-coding-agent.agent.md and the repository-wide coding standards in .github/copilot-instructions.md.

Implement a secure fix for the status update authorization issue.

Finding:
WorkRequestService.UpdateStatusAsync currently updates a work request status after loading the work request by ID. The controller has role checks, but the service must enforce resource-level authorization before mutation.

Business rule:
- Managers can update the status of any work request.
- Workers can update the status only for work requests assigned to them.
- Requesters cannot update status unless they are also assigned or have a permitted role.
- Unauthenticated users must not reach this service path.
- If the caller is not allowed to access the work request, return a safe failure result without changing the record.

Implementation requirements:
- Preserve the existing controller role checks.
- Do not remove or weaken [Authorize].
- Use ICurrentUserContext to determine the current user.
- Apply the authorization check before calling ChangeStatus.
- Do not trust any user ID from the request body.
- Preserve existing domain status transition rules.
- Make the smallest code change that addresses the root cause.
- Keep controllers thin and keep business logic in services.
- Do not invent unrelated features.

Likely files:
- src/TaskFlowLite.Infrastructure/Services/WorkRequestService.cs
- src/TaskFlowLite.Api/Controllers/WorkRequestsController.cs only if response handling must change
- Tests only if a test project already exists

Before changing code:
1. Summarize your implementation plan.
2. List the files you expect to modify.
3. State any security assumptions.

After changing code:
1. Summarize the files changed.
2. Explain how resource-level authorization is enforced.
3. Explain what tests should be run.
4. List any missing evidence.