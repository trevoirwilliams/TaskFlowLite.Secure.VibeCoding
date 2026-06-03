We are working in the TaskFlow Lite ASP.NET Core Web API.

Before writing code, inspect the current project and create an implementation plan to add ASP.NET Core Identity and JWT bearer authentication.

Current context:
- The API uses .NET 10.
- The app already has EF Core and SQLite.
- The app already has a domain User entity used by WorkRequest and RequestNote relationships.
- The current user context currently relies on X-TaskFlow-UserId, but this should not be trusted as real authentication.
- Program.cs currently has controllers, Swagger, infrastructure registration, HTTPS redirection, and controller mapping, but no authentication or authorization middleware.

Plan the safest incremental implementation.

Requirements:
1. Use ASP.NET Core Identity for user account and password management.
2. Use JWT bearer tokens for API authentication.
3. Add register and login endpoints.
4. Generate signed JWT access tokens after login.
5. Configure JWT bearer validation for issuer, audience, expiration, and signing key.
6. Configure Swagger to support bearer tokens.
7. Avoid hardcoding production secrets.
8. Return a file-by-file implementation plan, risks, and validation steps.

Do not modify files yet.