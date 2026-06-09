# TaskFlow Lite AI Coding Standards

TaskFlow Lite is a .NET 10 ASP.NET Core Web API for managing work requests, assignments, statuses, and request notes.

The project uses:

- ASP.NET Core Web API
- Entity Framework Core
- SQLite for local development
- ASP.NET Core Identity
- JWT bearer authentication
- Layered project structure with API, Application, Domain, and Infrastructure projects

Follow these instructions for all AI-assisted code suggestions, reviews, refactoring, and tests.

## Architecture Rules

- Preserve the existing layered structure.
- Keep controllers thin.
- Keep business logic in application or infrastructure services.
- Keep domain behavior in domain entities where appropriate.
- Use dependency injection instead of manually constructing services.
- Use async EF Core APIs for database operations.
- Do not introduce raw SQL unless explicitly requested and justified.
- Do not move security-sensitive logic into the client or into Swagger-only workflows.

## Authentication and Authorization Rules

- Do not weaken authentication.
- Do not remove `[Authorize]` attributes from protected controllers or actions.
- Keep anonymous access limited to intentional authentication endpoints.
- Use role-based authorization only when the business rule requires it.
- Apply resource-level authorization before reading, updating, assigning, closing, or deleting protected records.
- Do not trust client-supplied user IDs as proof of access.
- Use the current authenticated user context for user-scoped operations.
- Return `NotFound`, `Forbid`, or another safe response when the caller should not access a resource.

## Resource-Level Access Rules

When working with work requests or notes:

- Scope read operations to the current authenticated user.
- Scope mutation operations at least as carefully as read operations.
- Do not update a work request by ID alone unless the business rule explicitly allows that role to modify any request.
- Check ownership, assignment, or role-based authority before changing protected records.
- Be especially careful with assignment and status changes because they modify workflow state.

## Input Validation Rules

- Treat all request data as untrusted.
- Use request DTOs for API input.
- Use data annotations for basic shape validation.
- Use length limits on strings.
- Trim and normalize strings before persistence where appropriate.
- Do not remove validation attributes to make a request easier to process.
- Avoid overposting by accepting only the fields needed for the operation.
- Validate enum and ID values against the intended business rule.

## Secrets and Configuration Rules

- Do not hardcode production secrets.
- Do not commit JWT signing keys, passwords, API keys, connection secrets, or tokens.
- Use configuration, environment variables, user secrets, or deployment secret stores for sensitive values.
- Do not log passwords, JWTs, signing keys, API keys, or connection strings.
- Treat development defaults as development-only.
- Required production security settings should fail fast when missing or invalid.

## Error Handling and Logging Rules

- Keep authentication failures generic.
- Do not expose stack traces or sensitive implementation details in API responses.
- Use validation errors for user-correctable input problems.
- Do not include passwords, tokens, signing keys, or secrets in logs.
- Prefer clear internal logs and safe external responses.

## Build, Test, and Dependency Evidence Rules

Before claiming that code is ready:

- Run or request evidence for restore, build, and tests.
- Run or request evidence for vulnerable package checks.
- Run or request evidence for outdated package review.
- Do not claim that a vulnerability scan passed unless scan output is provided.
- Do not claim that tests passed unless test output is provided.
- Document known risks separately from completed fixes.