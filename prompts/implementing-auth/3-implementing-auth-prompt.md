Apply the Identity-only user cleanup.

This is a focused refactor. Do not redesign unrelated features.

Required end state:
- ApplicationUser is the only user entity in the application.
- ApplicationUser inherits from IdentityUser<int>.
- ApplicationUser includes DisplayName, IsActive, and CreatedAtUtc.
- Remove DomainUserId from ApplicationUser.
- Delete the old TaskFlowLite.Domain.Entities.User entity.
- Remove DbSet<User> from TaskFlowLiteDbContext.
- Remove all model configuration for the old User entity.
- Update WorkRequest navigation properties so RequestedByUser and AssignedToUser reference ApplicationUser.
- Update RequestNote.AuthorUser to reference ApplicationUser.
- Keep RequestedByUserId, AssignedToUserId, and AuthorUserId as int foreign keys.
- Configure the WorkRequest and RequestNote relationships against ApplicationUser in TaskFlowLiteDbContext.
- Update UserService so it queries Identity users/ApplicationUser, not the removed Users DbSet.
- Update AuthService so registration creates only ApplicationUser.
- Update login so it uses only Identity/ApplicationUser and checks ApplicationUser.IsActive directly.
- Update token generation so it uses ApplicationUser.Id as the only user ID.
- Remove all DomainUserId usage from tokens, claims, services, seeders, and DTO mapping.
- Remove the X-TaskFlow-UserId fallback from CurrentUserContext.
- CurrentUserContext must only read the authenticated user ID from claims.
- If no valid authenticated claim exists, TryGetUserId must return false.
- Do not keep backwards compatibility for header-based auth.
- Do not add a replacement fake user header.
- Do not create a separate UserProfile table.
- Do not add a DomainUserId claim.

Seeding requirements:
- Seed ApplicationUser records directly through UserManager<ApplicationUser>.
- Seed the three existing sample users as Identity users with DisplayName, Email, IsActive, and CreatedAtUtc.
- Seed work requests and notes using those ApplicationUser IDs.
- Do not seed the old Users table because it must no longer exist.

Security requirements:
- Do not store plaintext passwords.
- Do not write custom password hashing.
- Keep Identity responsible for password hashing and login validation.
- Keep JWT validation for issuer, audience, lifetime, and signing key.
- Do not include secrets or password-related fields in JWT claims or API responses.
- Keep generic login failure messages.
- Keep authentication middleware and authorization middleware in the correct order.

After changes:
- Summarize each changed file.
- List every removed legacy artifact.
- List every remaining reference to User, DomainUserId, or X-TaskFlow-UserId if any remain.
- Run or tell me to run dotnet build.