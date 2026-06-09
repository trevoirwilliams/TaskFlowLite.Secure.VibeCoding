# Remediation Plan: SEC-002 Status Enum Validation

## Classification

Fix Now

## Finding

Status values should be validated at the API and domain boundary so undefined numeric enum values cannot enter the status workflow.

## Risk

Undefined enum values can create invalid workflow states if they are accepted by model binding or domain methods.

## Current Evidence

`UpdateWorkRequestStatusRequest` accepts `WorkRequestStatus Status` directly.

`WorkRequest.ChangeStatus` applies transition logic but does not visibly reject undefined enum values before assigning the new status.

## Approved Business Rule

The public API accepts only defined `WorkRequestStatus` enum values.

## Secure Fix Requirements

- Add API boundary validation for undefined enum values.
- Add domain-level guard for undefined enum values.
- Preserve existing transition rules.
- Do not convert to a string-based API unless explicitly approved.
- Add tests for invalid numeric enum values.

## Likely Files

- `src/TaskFlowLite.Application/Models/WorkRequests/UpdateWorkRequestStatusRequest.cs`
- `src/TaskFlowLite.Domain/Entities/WorkRequest.cs`
- Security or domain tests

## Secure Coding Agent Prompt

```text
Use the Secure Coding Agent instructions in .github/instructions/secure-coding-agent.agent.md and the repository-wide coding standards in .github/copilot-instructions.md.

Implement the approved fix for SEC-002: Status Enum Validation.

Business rule:
The public API and domain model must reject undefined WorkRequestStatus values.

Requirements:
- Validate enum membership at the API boundary.
- Add a domain-level guard in WorkRequest.ChangeStatus.
- Preserve existing transition rules.
- Do not weaken validation.
- Add or update tests if the test project exists.
- Make the smallest safe change.

Before changing code, provide the implementation plan and files likely to change.
After changing code, summarize files changed, tests added, commands to run, and missing evidence.
````

## Required Tests

* Invalid numeric status payload is rejected.
* Domain model rejects undefined status values.
* Valid status transitions still work.
* Existing invalid transition rule still works.

## Evidence Required

* Build output
* Test output
