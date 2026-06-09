Use the repository coding standards, Security Review Agent, Secure Coding Agent, secure shipping checklist, review record, triage register, remediation plans, decision records, and evidence pack.

Draft a pull request security review comment.

Use this structure:
1. Review scope
2. Positive controls
3. Merge blockers
4. Business decisions required
5. Follow-up items
6. Evidence status
7. Merge recommendation

Do not invent build results, test results, package scan results, business rules, or production readiness claims.

Use these files:
- docs/security/secure-shipping-checklist.md
- docs/security/reviews/2026-06-security-checklist-review.md
- docs/security/triage/security-triage-register.md
- docs/security/remediation/
- docs/security/decisions/
- docs/security/evidence/security-checklist/
```

Expected output:

```markdown
# Pull Request Security Review

## Scope

Reviewed the TaskFlow Lite work request and request note flows, including controller authorization, service-layer mutation paths, status validation, request note access, repository AI standards, and the security review agents.

## Positive Controls

- Work request endpoints require authentication.
- Assignment and status endpoints include role checks.
- Work request read paths are scoped to the current user.
- Request note controller checks parent work request access before note operations.
- AI agent instructions require evidence and prohibit unsupported readiness claims.

## Merge Blockers

- SEC-001: Status update resource-level authorization must be remediated.
- SEC-002: Status enum membership validation must be remediated.

## Business Decisions Required

- SEC-003: Assignment authorization scope must be documented before implementation.

## Follow-Up

- SEC-004: Request note service hardening should be tracked as defense-in-depth.

## Evidence Status

Review the evidence pack under:

`docs/security/evidence/security-checklist/`

Do not claim build, test, or package readiness unless the captured output supports it.

## Merge Recommendation

Request changes.

The branch should not be marked production-ready while Fix Now findings remain open or required evidence is missing.
