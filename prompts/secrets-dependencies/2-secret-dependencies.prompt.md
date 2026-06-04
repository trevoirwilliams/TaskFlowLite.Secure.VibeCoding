Apply only the approved configuration and secret-management fixes below.

Approved fixes:
- Do not store real secrets in appsettings.json or appsettings.Development.json.
- Keep only safe placeholders in committed configuration files.
- Add documentation comments or README notes showing which keys must be supplied locally.
- Update .gitignore to exclude local-only secret files such as .env and generated logs.
- Do not introduce a secret key store yet.
- Do not change authentication, authorization, database schema, endpoints, or business logic.

After changes, summarize every modified file and explain why the change was made.