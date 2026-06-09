# Decision Record: SEC-001 Status Update Authorization

## Finding

Status updates mutate workflow state and must enforce resource-level authorization before the mutation occurs.

## Decision Needed

Define who can update the status of a work request.

## Options

### Option A: Managers can update any request; workers can update assigned requests only

- Managers may update status for any work request.
- Workers may update status only when `AssignedToUserId` matches the current authenticated user.
- Requesters cannot update status unless they are also assigned or have an allowed role.

### Option B: Managers and workers can update only scoped requests

- Managers are limited by team, department, tenant, or ownership scope.
- Workers are limited to assigned requests.
- Additional scope fields may be needed before implementation.

### Option C: Only managers can update status

- Workers cannot update status directly.
- Workers must request status changes through another workflow.

## Approved Decision

```text
Pending.
````

## Implementation Impact

Until this decision is approved, secure remediation cannot be finalized.

## Required Tests

* Worker can update assigned request.
* Worker cannot update unrelated request.
* Manager behavior matches approved rule.
* Status is unchanged when access is denied.
