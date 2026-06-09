# Remediation Plan: SEC-003 Assignment Authorization Scope

## Classification

Needs Business Decision

## Finding

Assignment mutates a work request after loading by ID, while the business rule for manager assignment scope is not documented.

## Risk

If managers are not globally authorized, assignment by ID may allow modification outside the intended scope.

## Approved Business Rule

See:

`docs/security/decisions/SEC-003-assignment-authorization-decision.md`

## Secure Fix Requirements

- Do not invent assignment scope.
- Preserve controller role checks.
- Make manager assignment rule explicit.
- Add service-layer guard if manager scope is limited.
- Consider terminal-state assignment rules if approved.

## Secure Coding Agent Planning Prompt

```text
Use the Secure Coding Agent instructions in .github/instructions/secure-coding-agent.agent.md.

Review SEC-003 using docs/security/decisions/SEC-003-assignment-authorization-decision.md.

Do not modify code.

Return:
1. Whether the approved business rule is sufficient for implementation
2. Files likely to change
3. Tests required
4. Unsafe fixes to avoid
5. Whether this remains a merge blocker
````

## Evidence Required

* Approved business decision
* Tests matching the decision
* Build output

