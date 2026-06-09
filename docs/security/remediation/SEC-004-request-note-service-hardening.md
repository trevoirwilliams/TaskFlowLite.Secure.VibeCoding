# Remediation Plan: SEC-004 Request Note Service Hardening

## Classification

Follow Up

## Finding

`RequestNoteService` accepts a raw `workRequestId` and relies on the controller to check parent work request access before service methods are called.

## Risk

The current controller flow is protected, but future callers could misuse the service if they bypass the controller pre-check.

## Current Evidence

`RequestNoteService.GetForWorkRequestAsync` retrieves notes by `WorkRequestId`.

`RequestNoteService.AddAsync` checks that the work request exists before adding a note.

## Secure Fix Options

### Option A: Service validates parent access internally

- Stronger defense-in-depth.
- May duplicate work request access logic.

### Option B: Service requires a pre-authorized parent context

- Makes caller responsibility explicit.
- Avoids duplicated authorization queries.
- Requires clearer method contracts.

## Recommended Priority

Follow-up after Fix Now items are resolved.

## Secure Coding Agent Planning Prompt

```text
Use the Secure Coding Agent instructions in .github/instructions/secure-coding-agent.agent.md.

Plan a follow-up hardening change for SEC-004.

Do not modify code.

Compare:
1. Service validates parent access internally
2. Service requires a pre-authorized parent context

Return:
- Recommended approach
- Files likely to change
- Tests needed
- Risks of overengineering
- Whether this should block merge
````

## Evidence Required

* Follow-up issue
* Design decision if implementation is deferred
