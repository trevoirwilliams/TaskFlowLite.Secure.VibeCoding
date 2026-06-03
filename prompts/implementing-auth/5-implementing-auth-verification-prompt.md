Review the completed Identity-only authentication cleanup as a secure-code reviewer.

Verify:
1. ApplicationUser is the only user entity.
2. ApplicationUser contains required profile fields such as DisplayName and IsActive.
3. There is no DomainUserId anywhere.
4. There is no old domain User entity.
5. There is no DbSet<User>.
6. There is no _dbContext.Users usage.
7. CurrentUserContext uses only authenticated claims.
8. X-TaskFlow-UserId is not trusted anywhere.
9. AuthService uses Identity only for registration and login.
10. JWTs contain only necessary non-sensitive claims.
11. WorkRequest and RequestNote foreign keys point to ApplicationUser IDs.
12. User listing uses ApplicationUser safely.
13. Seed data creates Identity users directly.
14. JWT validation still checks issuer, audience, lifetime, and signing key.
15. Login failure messages do not leak account availability.

Return findings grouped as:
- Must fix now
- Should fix before merge
- Acceptable for this course checkpoint

Do not suggest bringing back a separate user table.