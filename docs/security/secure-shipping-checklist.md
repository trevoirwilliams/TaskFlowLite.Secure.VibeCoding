# TaskFlow Lite Secure Shipping Checklist

## Purpose

This checklist is used before merging, releasing, or marking security-sensitive TaskFlow Lite changes as ready.

TaskFlow Lite is a .NET 10 ASP.NET Core Web API that uses ASP.NET Core Web API, Entity Framework Core, SQLite, ASP.NET Core Identity, JWT bearer authentication, and a layered API/Application/Domain/Infrastructure structure.

Use this checklist together with:

- `.github/copilot-instructions.md`
- `.github/instructions/security-review-agent.md`
- `.github/instructions/secure-coding-agent.agent.md`

This checklist does not replace developer judgment. It helps organize security review evidence, AI-assisted findings, remediation decisions, test results, dependency review results, and release readiness.

---

## Secure Shipping Principle

Do not claim code is ready, secure, production-ready, or safe to ship unless the claim is supported by evidence.

Evidence may include:

- Code review notes
- Security Review Agent findings
- Secure Coding Agent triage or implementation notes
- Build output
- Test output
- Vulnerable package scan output
- Outdated package review output
- Confirmed business rules
- Manual verification notes

If evidence is missing, document it honestly.

## Security Review Package Index

| Artifact | Path | Purpose |
|---|---|---|
| Checklist | `docs/security/secure-shipping-checklist.md` | Master secure shipping gate |
| Review Record | `docs/security/reviews/2026-06-security-checklist-review.md` | Branch-specific review summary |
| Triage Register | `docs/security/triage/security-triage-register.md` | Tracks findings, owners, status, and merge gate |
| Status Decision | `docs/security/decisions/SEC-001-status-update-authorization-decision.md` | Documents status update authorization rule |
| Assignment Decision | `docs/security/decisions/SEC-003-assignment-authorization-decision.md` | Documents assignment authorization rule |
| SEC-001 Plan | `docs/security/remediation/SEC-001-status-resource-authorization.md` | Fix plan for status authorization |
| SEC-002 Plan | `docs/security/remediation/SEC-002-status-enum-validation.md` | Fix plan for status enum validation |
| SEC-003 Plan | `docs/security/remediation/SEC-003-assignment-authorization-scope.md` | Plan for assignment authorization |
| SEC-004 Plan | `docs/security/remediation/SEC-004-request-note-service-hardening.md` | Follow-up plan for note service hardening |
| Evidence Pack | `docs/security/evidence/security-checklist/` | Restore/build/test/package evidence |
| PR Comment | `docs/security/pr/security-review-comment.md` | Pull request review summary |

---

## 1. Repository and Branch Readiness

Complete before reviewing or shipping the branch.

- [ ] I am on the intended working branch.
- [ ] The working tree is clean or all pending changes are understood.
- [ ] The changed files match the intended security work.
- [ ] No generated build artifacts are being committed accidentally.
- [ ] No local-only files are being committed accidentally.
- [ ] The repository-wide AI coding standards file is present.
- [ ] The Security Review Agent file is present.
- [ ] The Secure Coding Agent file is present.

Expected files:

```text
.github/copilot-instructions.md
.github/instructions/security-review-agent.md
.github/instructions/secure-coding-agent.agent.md
```

Evidence commands:

```powershell
git status
git branch --show-current
```

Notes:

```text

```

---

## 2. Architecture Review

Confirm that the change preserves the existing application structure.

- [ ] Controllers remain thin.
- [ ] Business logic remains in services or appropriate domain objects.
- [ ] Domain behavior remains in domain entities where appropriate.
- [ ] Dependency injection is used instead of manual service construction.
- [ ] Async EF Core APIs are used for database work.
- [ ] No unjustified raw SQL was introduced.
- [ ] Security-sensitive logic was not moved to the client.
- [ ] Swagger or local development tooling was not used as a substitute for server-side security.

Notes:

```text

```

---

## 3. Authentication Review

Confirm that authentication behavior was not weakened.

- [ ] Protected business endpoints require authentication.
- [ ] Anonymous access is limited to intentional authentication endpoints.
- [ ] Existing `[Authorize]` attributes were not removed.
- [ ] Existing `[AllowAnonymous]` usage remains intentional.
- [ ] Authentication failures remain generic.
- [ ] Token handling was not weakened.
- [ ] Password handling still relies on ASP.NET Core Identity.
- [ ] No custom password hashing was introduced.
- [ ] No authentication bypass was introduced for testing or convenience.

Notes:

```text

```

---

## 4. Authorization Review

Confirm that authorization is enforced on the server.

- [ ] Existing role checks were not removed.
- [ ] Existing role checks were not weakened.
- [ ] Role checks are paired with resource-level authorization where the operation affects a protected record.
- [ ] Authorization decisions were not moved to the UI or client.
- [ ] Route IDs are not treated as authorization evidence.
- [ ] Request body user IDs are not treated as authorization evidence.
- [ ] Current authenticated user context is used for scoped operations.
- [ ] Unauthorized access returns a safe response.
- [ ] Authorization behavior is covered by tests or documented as missing evidence.

Notes:

```text

```

---

## 5. Resource-Level Access Review

Confirm that protected records are scoped correctly before being read or mutated.

- [ ] Work request list operations are scoped to the current authenticated user.
- [ ] Work request detail operations are scoped to the current authenticated user.
- [ ] Work request update operations are scoped before mutation.
- [ ] Work request status changes are authorized before mutation.
- [ ] Work request assignment changes are authorized before mutation.
- [ ] Request notes are accessible only when the caller can access the parent work request.
- [ ] Mutations are scoped at least as carefully as read operations.
- [ ] Protected records are not mutated by ID alone unless the business rule explicitly allows that role to modify any matching record.
- [ ] Any broad role behavior is documented as a business decision.

Notes:

```text

```

---

## 6. TaskFlow Lite Work Request Decisions

Complete this section before approving work request workflow changes.

### 6.1 Status Update Authorization

- [ ] Manager status update behavior is documented.
- [ ] Worker status update behavior is documented.
- [ ] Workers cannot update unrelated requests unless explicitly allowed.
- [ ] Status update authorization is checked before calling domain state-change behavior.
- [ ] Status updates do not rely only on route-level role checks.
- [ ] Status update tests cover allowed and forbidden scenarios.

Business decision:

```text
Managers can update the status of:
[ ] Any work request
[ ] Only scoped/team-owned work requests
[ ] Other:

Workers can update the status of:
[ ] Assigned work requests only
[ ] Any work request
[ ] No work requests
[ ] Other:
```

Decision notes:

```text

```

### 6.2 Assignment Authorization

- [ ] Manager assignment scope is documented.
- [ ] Assignment does not rely only on route-level role checks unless global manager assignment is approved.
- [ ] Assignment authorization is checked before mutation when manager scope is limited.
- [ ] Assignment behavior for completed or cancelled requests is documented.
- [ ] Assignment tests cover allowed and forbidden scenarios.

Business decision:

```text
Managers can assign:
[ ] Any work request
[ ] Only scoped/team-owned work requests
[ ] Other:

Requesters can self-assign:
[ ] Yes
[ ] No
[ ] Other:

Completed or cancelled requests can be reassigned:
[ ] Yes
[ ] No
[ ] Other:
```

Decision notes:

```text

```

### 6.3 Request Note Access

- [ ] Request notes require access to the parent work request.
- [ ] Controller-level parent access checks are present.
- [ ] Service-level note access hardening has been considered.
- [ ] Note creation uses the current authenticated user as the author.
- [ ] Note access tests exist or are planned.
- [ ] Any manager-wide note visibility rule is documented.

Business decision:

```text
Managers can read notes for:
[ ] Any work request
[ ] Only scoped/team-owned work requests
[ ] Other:

Requesters can read notes for:
[ ] Their own requests
[ ] Assigned requests only
[ ] Other:

Assigned workers can read notes for:
[ ] Assigned requests
[ ] Any request
[ ] Other:
```

Decision notes:

```text

```

---

## 7. Input Validation Review

Confirm that API input is constrained before use.

- [ ] Request DTOs are used for API input.
- [ ] Required fields are validated.
- [ ] String length limits are present where needed.
- [ ] Inputs are trimmed or normalized where appropriate.
- [ ] Overposting risks were reviewed.
- [ ] Route IDs are validated through the intended access path.
- [ ] Body IDs are not trusted without server-side validation.
- [ ] Validation attributes were not removed to make implementation easier.
- [ ] Invalid input produces safe validation responses.

Notes:

```text

```

---

## 8. TaskFlow Lite Status Validation Decisions

Complete this section before approving status workflow changes.

- [ ] Undefined `WorkRequestStatus` enum values are rejected at the API boundary.
- [ ] Undefined `WorkRequestStatus` enum values are rejected in the domain model.
- [ ] Invalid numeric enum payloads are covered by tests.
- [ ] Valid status values still follow domain transition rules.
- [ ] Existing invalid transition behavior remains intact.
- [ ] Status validation does not bypass domain rules.

Business decision:

```text
The public API accepts:
[ ] Only defined WorkRequestStatus enum values
[ ] A restricted subset of WorkRequestStatus values
[ ] Other:

Internal-only statuses exist:
[ ] Yes
[ ] No
[ ] Not applicable
```

Decision notes:

```text

```

---

## 9. Data Access Review

Confirm that data access does not expose or mutate protected records incorrectly.

- [ ] Queries do not expose more data than required.
- [ ] Read paths and mutation paths use consistent access rules.
- [ ] Protected records are not loaded by ID alone before mutation unless explicitly justified.
- [ ] EF Core parameterized queries are preserved.
- [ ] No unsafe string-built SQL was introduced.
- [ ] No direct data access bypasses service-level authorization rules.
- [ ] Query filters do not trust client-supplied user IDs as proof of access.
- [ ] Sensitive operations are reviewed for horizontal access control issues.

Notes:

```text

```

---

## 10. Secrets and Configuration Review

Confirm that secrets and security-sensitive settings are handled safely.

- [ ] No JWT signing keys were committed.
- [ ] No passwords were committed.
- [ ] No API keys were committed.
- [ ] No tokens were committed.
- [ ] No connection secrets were committed.
- [ ] Sensitive values are expected to come from safe configuration sources.
- [ ] Secrets are not logged.
- [ ] Development defaults are documented as development-only.
- [ ] Production-required security settings fail safely when missing or invalid.
- [ ] Empty placeholders in source-controlled configuration are intentional.

Notes:

```text

```

---

## 11. Error Handling and Logging Review

Confirm that errors and logs do not leak sensitive information.

- [ ] Authentication errors remain generic.
- [ ] API responses do not expose stack traces.
- [ ] API responses do not expose sensitive implementation details.
- [ ] Validation errors are safe and user-correctable.
- [ ] Logs do not include passwords.
- [ ] Logs do not include JWTs or access tokens.
- [ ] Logs do not include signing keys.
- [ ] Logs do not include API keys.
- [ ] Logs do not include connection strings.
- [ ] Security-sensitive operations have enough logging for investigation without leaking secrets.

Notes:

```text

```

---

## 12. Test Evidence

Record test evidence before claiming readiness.

- [ ] Unit tests were run.
- [ ] Integration tests were run.
- [ ] Security-sensitive tests were added or updated.
- [ ] Authorization tests cover allowed access.
- [ ] Authorization tests cover forbidden access.
- [ ] Status transition tests cover valid transitions.
- [ ] Status transition tests cover invalid transitions.
- [ ] Enum validation tests cover invalid numeric values.
- [ ] Request note tests cover inaccessible parent requests.
- [ ] Missing test coverage is documented.

Suggested commands:

```powershell
dotnet test TaskFlowLite.slnx -c Debug
dotnet test TaskFlowLite.UnitTests.csproj -c Debug
dotnet test TaskFlowLite.IntegrationTests.csproj -c Debug
```

Actual command output summary:

```text

```

Missing test evidence:

```text

```

---

## 13. Build and Dependency Evidence

Record build and dependency evidence before claiming readiness.

- [ ] Restore completed.
- [ ] Build completed.
- [ ] Vulnerable package check was run.
- [ ] Outdated package review was run.
- [ ] Dependency findings were reviewed.
- [ ] Dependency updates were not applied blindly without compatibility review.
- [ ] Missing dependency evidence is documented.

Suggested commands:

```powershell
dotnet restore TaskFlowLite.slnx
dotnet build TaskFlowLite.slnx -c Debug
dotnet list TaskFlowLite.slnx package --vulnerable --include-transitive
dotnet list TaskFlowLite.slnx package --outdated --include-transitive
```

Actual command output summary:

```text

```

Missing build or dependency evidence:

```text

```

---

## 14. AI Review Evidence

Record how AI was used in the review.

- [ ] Repository-wide coding standards were used.
- [ ] Security Review Agent was used.
- [ ] Secure Coding Agent was used where implementation triage or remediation was needed.
- [ ] AI findings were reviewed by the developer.
- [ ] Unsupported AI claims were challenged.
- [ ] AI did not invent test results.
- [ ] AI did not invent scan results.
- [ ] AI did not invent business rules.
- [ ] AI did not claim production readiness without evidence.
- [ ] Final decisions were made by the developer.

Prompt or review notes:

```text

```

---

## 15. Current Security Triage

Use this section to track the current findings.

| ID | Finding | Classification | Decision | Evidence | Status |
|---|---|---|---|---|---|
| SEC-001 | Status update by ID without resource-level authorization check | Fix Now | Must be remediated before secure shipping | Security review and code inspection | Pending |
| SEC-002 | Status enum membership not validated at API/domain boundary | Fix Now | Must be remediated before secure shipping | Security review and code inspection | Pending |
| SEC-003 | Assignment by ID without service-level authorization guard | Needs Business Decision | Confirm manager assignment scope before implementation | Security review and code inspection | Pending |
| SEC-004 | Request note service relies on controller pre-checks | Follow Up | Add service defense-in-depth after priority fixes | Security review and code inspection | Pending |

Triage notes:

```text

```

---

## 16. Fix Now Items

These items must be resolved before a production-style secure shipping claim.

### SEC-001: Status Update Resource-Level Authorization

- [ ] Business rule confirmed.
- [ ] Service-layer authorization implemented.
- [ ] Controller role checks preserved.
- [ ] Worker unrelated-request case blocked.
- [ ] Manager behavior matches confirmed rule.
- [ ] Tests added or updated.
- [ ] Build/test evidence captured.

Notes:

```text

```

### SEC-002: Status Enum Membership Validation

- [ ] API boundary rejects undefined status enum values.
- [ ] Domain model rejects undefined status enum values.
- [ ] Existing transition rules preserved.
- [ ] Invalid numeric enum payload test added.
- [ ] Domain guard test added.
- [ ] Build/test evidence captured.

Notes:

```text

```

---

## 17. Business Decision Items

These items require an explicit decision before implementation or release approval.

### SEC-003: Assignment Authorization Scope

Decision required:

- Can managers assign any request globally?
- Are managers limited to scoped/team-owned requests?
- Can requesters self-assign?
- Can completed or cancelled requests be reassigned?

Decision:

```text

```

Required follow-up:

- [ ] Document decision.
- [ ] Implement authorization guard if required.
- [ ] Add allowed and forbidden assignment tests.
- [ ] Capture build/test evidence.

---

## 18. Follow-Up Items

These items improve defense-in-depth but do not automatically block the current release unless the risk changes.

### SEC-004: Request Note Service Hardening

- [ ] Decide whether RequestNoteService should validate parent access internally.
- [ ] Decide whether RequestNoteService should accept a pre-authorized parent context instead of raw `workRequestId`.
- [ ] Add tests for direct service misuse if service-level hardening is implemented.
- [ ] Preserve existing controller parent-access checks.

Notes:

```text

```

---

## 19. Unsupported Claims

List claims that must not be made until evidence exists.

- [ ] "The build passes."
- [ ] "All tests pass."
- [ ] "No vulnerable packages exist."
- [ ] "Dependencies are current."
- [ ] "The application is secure."
- [ ] "The branch is production-ready."
- [ ] "Managers can modify all requests."
- [ ] "Workers can update any request."
- [ ] "Request note access is fully protected at all layers."

Unsupported claims identified during review:

```text

```

---

## 20. Release Recommendation

Choose one:

- [ ] Approved
- [ ] Approved with follow-up
- [ ] Changes required

Decision:

```text
Changes required before production-style secure shipping.
```

Rationale:

```text
The current review identified two Fix Now findings: status update resource-level authorization and status enum validation. Assignment authorization requires a business decision, and request note service hardening should be tracked as follow-up defense-in-depth. Build, test, vulnerable package, and outdated package evidence must also be captured before any readiness claim.
```

---

## 21. Sign-Off

Reviewer:

```text

```

Date:

```text

```

Summary:

```text

```

Final decision:

```text

```
