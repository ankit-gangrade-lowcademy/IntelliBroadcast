# IntelliBroadcast

ASP.NET Core MVC application targeting .NET 10, backed by an existing SQL Server/Azure SQL Norvia identity and broadcast database.

## Configuration and local setup

1. Install the .NET 10 SDK and obtain access to the shared database.
2. Supply `ConnectionStrings:DefaultConnection` in `src/IntelliBroadcast.Web/appsettings.json`, or preferably use user-secrets: `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=...;User Id=...;Password=...;Encrypt=True;TrustServerCertificate=False;" --project src/IntelliBroadcast.Web`.
3. Run `database/setup.sql` against the target database. It creates only `NorviaBroadcast` when missing. The app maps the existing shared identity/product tables.
4. Run `dotnet run --project src/IntelliBroadcast.Web` and use its HTTPS URL.

`ProjectHash` is fixed at `4f0d7c72-9a31-4c68-b526-2e8f5a1d93b7`. Norvia Backoffice Admin must provision the IntelliBroadcast `NorviaProducts` row with exactly this hash, then assign each user an available non-zero `ProductUserRoleID` through `UserExtendedRoleMapping` for the same tenant.

Passwords in `UserExtended.PasswordHash` use ASP.NET Core Identity `PasswordHasher<T>` PBKDF2 format. Plaintext is unsupported. The app verifies active credentials and the tenant/product/role mapping before issuing a secure cookie or server session. Every failure displays `Invalid Login`.

## Build and test

```sh
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

## Azure App Service

1. Create an App Service with .NET 10 and enable **HTTPS Only**.
2. Add an application setting named `ConnectionStrings__DefaultConnection` containing the SQL/Azure SQL connection string (or use an App Service connection string named `DefaultConnection`). Keep credentials out of files.
3. Set `BaseURL` to the public HTTPS App Service URL (for example, `https://my-app.azurewebsites.net`) or provide it through the `BaseURL` App Service application setting. Keep the committed fixed `ProjectHash`; do not make it browser-controlled.
4. Configure SQL networking and least-privilege database access.
5. Publish with `dotnet publish src/IntelliBroadcast.Web -c Release -o ./publish`, then deploy that directory with the Azure portal, pipeline, or `az webapp deploy`.
6. Verify `/health`, login, tenant/global filtering, and logout. For multiple instances, configure a shared distributed session cache and persistent Data Protection keys.

See `src/IntelliBroadcast.Web/appsettings.Production.json.example` for the credential-free production template.
