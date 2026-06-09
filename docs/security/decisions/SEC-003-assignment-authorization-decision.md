# Decision Record: SEC-003 Assignment Authorization Scope

## Finding

The assignment endpoint is restricted to Managers at the controller level, but assignment behavior needs an explicit service-level business rule.

## Decision Needed

Define whether managers can assign all work requests or only scoped work requests.

## Options

### Option A: Managers can assign any work request

- The Manager role is treated as a global assignment authority.
- Service logic should still make this rule explicit and testable.
- Assignment should not proceed if the request is in a terminal state, if that rule is approved.

### Option B: Managers can assign only scoped requests

- Managers are limited by team, tenant, department, ownership, or another scope.
- Additional data model support may be required.
- The service must enforce the scope before assignment.

### Option C: Requesters can self-assign

- Requesters may assign themselves to their own work requests.
- Additional checks are required to avoid privilege escalation.

## Approved Decision

```text
Pending.
````

## Implementation Impact

Until the decision is approved, assignment remediation should not invent the missing rule.

## Required Tests

* Manager assignment succeeds within approved scope.
* Assignment fails outside approved scope.
* Non-manager assignment is rejected.
* Terminal-state reassignment behavior matches approved rule.
