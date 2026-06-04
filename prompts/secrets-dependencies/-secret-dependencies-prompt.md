Review the outdated NuGet package output below and update this .NET 10 solution to use the latest secure package versions.

Goal:
Update outdated packages to the latest stable, secure versions while preserving the current architecture and behavior of the application.

Outdated package output:
#file:dependency-report.txt 

Instructions:
1. Inspect all .csproj files in the solution.
2. Update direct NuGet package references to the latest stable versions shown in the outdated package report.
3. Prefer stable releases only. Do not use preview, alpha, beta, RC, nightly, or unofficial packages unless the project already intentionally uses preview packages.
4. Keep package versions compatible with .NET 10.
5. Do not add new packages unless absolutely required to resolve a compatibility issue.
6. Do not remove packages unless they are clearly unused and you can prove the project still builds and tests successfully without them.
7. Do not change application behavior, API routes, authentication logic, authorization rules, database models, migrations, business logic, or test intent.
8. If a package has a major version upgrade, review likely breaking changes before applying it. If the change is risky, update it only if the code can be adjusted safely and explain the adjustment.
9. After updating packages, run:
   - dotnet restore
   - dotnet build
   - dotnet test
   - dotnet package list --vulnerable --include-transitive
   - dotnet package list --outdated
10. Fix any compile errors or test failures caused by package updates.
11. Do not suppress warnings unless there is a clear and justified reason.
12. If package lock files are present, update them through dotnet restore. Do not manually edit lock files unless necessary.

Return a summary with:
- Packages updated
- Old version and new version
- Whether each update was patch, minor, or major
- Any breaking changes found
- Any code changes required
- Vulnerability scan result after update
- Remaining outdated packages, if any
- Final restore/build/test status

Important:
Do not claim the solution is secure just because packages were updated. State only what was verified.