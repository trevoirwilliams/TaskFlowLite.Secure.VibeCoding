# TaskFlowLite

## Local Configuration and Secrets

Committed configuration files must contain only non-secret placeholders.

Set the following values locally before running the API:

- `Jwt:SigningKey` (minimum 32 characters; do not commit real values)
- `DevSeed:Password` (if you use development seeding)
- `ConnectionStrings:TaskFlowLite` (if your local DB location differs)

Recommended local-only configuration options:

- Environment variables (preferred for local secrets)
- `.env` / `.env.*` files kept out of source control
- `appsettings.Local.json` or `appsettings.Development.Local.json` (ignored by git)

Do not commit real secrets to `appsettings.json` or `appsettings.Development.json`.
