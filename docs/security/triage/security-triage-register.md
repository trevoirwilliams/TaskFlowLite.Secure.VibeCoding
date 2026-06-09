# Security Triage Register

## Purpose

This register tracks security findings discovered during the secure shipping review.

Each finding must have:

- A stable ID
- A classification
- A decision
- An owner
- Related files
- Required evidence
- Current status

---

## Triage Categories

| Category | Meaning |
|---|---|
| Fix Now | Must be remediated before production-style secure shipping |
| Needs Business Decision | Requires a documented business rule before implementation |
| Needs Evidence | Cannot be approved until command/test/scan output exists |
| Follow Up | Defense-in-depth or non-blocking hardening item |

---

## Current Findings

| ID | Finding | Classification | Owner | Status | Related Plan |
|---|---|---|---|---|---|
| SEC-001 | Status update by ID without resource-level authorization check | Fix Now |  | Open | `docs/security/remediation/SEC-001-status-resource-authorization.md` |
| SEC-002 | Status enum membership not validated at API/domain boundary | Fix Now |  | Open | `docs/security/remediation/SEC-002-status-enum-validation.md` |
| SEC-003 | Assignment by ID without service-level authorization guard | Needs Business Decision |  | Open | `docs/security/remediation/SEC-003-assignment-authorization-scope.md` |
| SEC-004 | Request note service relies on controller pre-checks | Follow Up |  | Open | `docs/security/remediation/SEC-004-request-note-service-hardening.md` |

---

## Merge Gate

The branch cannot be marked production-ready while any `Fix Now` item remains open.

Current merge gate result:

~~~text
Changes required.
~~~
