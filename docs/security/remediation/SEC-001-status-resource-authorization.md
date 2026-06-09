# Remediation Plan: SEC-001 Status Resource Authorization

## Classification

Fix Now

## Finding

`WorkRequestService.UpdateStatusAsync` changes workflow state after loading a work request by ID.

## Risk

A role check at the controller does not prove the caller can mutate the specific work request.

## Current Evidence

`UpdateStatusAsync` loads the entity by ID before calling `ChangeStatus`.

## Approved Business Rule

See:

`docs/security/decisions/SEC-001-status-update-authorization-decision.md`

## Secure Fix Requirements

- Preserve controller role checks.
- Use the current authenticated user.
- Authorize before calling `ChangeStatus`.
- Workers should not update unrelated requests unless explicitly approved.
- Managers should follow the approved business rule.
- Preserve domain status transition rules.
- Return a safe response when access is denied.

## Likely Files

- `src/TaskFlowLite.Infrastructure/Services/WorkRequestService.cs`
- Possibly `src/TaskFlowLite.Api/Controllers/WorkRequestsController.cs`
- Security tests

## Secure Coding Agent Prompt

```text
Use the Secure Coding Agent instructions in .github/instructions/secure-coding-agent.agent.md and the repository-wide coding standards in .github/copilot-instructions.md.

Implement the approved fix for SEC-001: Status Resource Authorization.

Approved business rule:
[paste approved decision from docs/security/decisions/SEC-001-status-update-authorization-decision.md]

Requirements:
- Preserve existing controller role checks.
- Use ICurrentUserContext.
- Enforce resource-level authorization before calling ChangeStatus.
- Do not trust route IDs or request body data as authorization evidence.
- Preserve existing domain transition rules.
- Make the smallest safe change.
- Add or update tests if the test project exists.

Before changing code, provide the implementation plan and files likely to change.
After changing code, summarize files changed, tests added, commands to run, and missing evidence.
````

## Required Tests

* Worker can update assigned request.
* Worker cannot update unrelated request.
* Manager behavior matches approved rule.
* Unauthorized mutation does not change status.
* Existing invalid transition rules still apply.

## Evidence Required

* Build output
* Test output
* Vulnerable package scan output
* Outdated package review output
