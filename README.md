# IntelliBroadcast

Production-oriented .NET 10 MVC portal for tenant and global Norvia announcements. It uses ASP.NET Core MVC/Razor, EF Core with SQL Server/Azure SQL, secure cookie authentication, server-side sessions, and Bootstrap 5.

## Prerequisites and local setup

1. Install the .NET 10 SDK and have access to the shared Norvia SQL Server database.
2. Keep credentials out of tracked files. Set the shared connection string with user secrets:

   ```bash
   dotnet user-secrets set --project src/IntelliBroadcast "ConnectionStrings:DefaultConnection" "<shared Norvia SQL connection string>"
   ```

   The expected configuration key and placeholder format are in `src/IntelliBroadcast/appsettings.Example.json`. The database is the same database used by NorviaBackoffice; do not copy its committed password into this repository.
3. Ensure active users in `dbo.UserExtended` have `Email`, `Tenant_ID`, `IsActive = 1`, and an ASP.NET Core `PasswordHasher<UserExtended>` password hash in `PasswordHash`.
4. Review and run `database/scripts/001_intellibroadcast_setup.sql` against the target database. Existing shared tables are mapped with `ExcludeFromMigrations`, so deployment does not attempt to own or drop Norvia tables.
5. Optionally run `database/scripts/900_development_seed.sql` only in a development database.
6. Run `dotnet restore`, `dotnet build --no-restore`, `dotnet test --no-build`, then `dotnet run --project src/IntelliBroadcast`.

The health endpoint is `/health`. The login page is `/Account/Login`. Announcements, requirements, and architecture routes require both a valid auth cookie and matching server-side session.

## Fixed product identity and provisioning handoff

`ProjectIdentity` is the single source of truth for `ProjectHash`:

`4f0d7c72-9a31-4c68-b526-2e8f5a1d93b7`

It is registered once as the singleton `IProjectIdentity`. Authorised outbound HTTP calls use `ProjectIdentityHeaderHandler`, which replaces any caller-provided value and sends the exact GUID in `X-Project-Hash`. It is intentionally not stored in configuration, accepted from browser input, displayed in the UI, or logged.

**Provisioning handoff:** give the exact GUID above to the Norvia Backoffice Admin. They must store it in `NorviaProducts.ProductHash` for the **IntelliBroadcast** product. The values in both systems must match exactly.

## Database behavior

- Reads `dbo.NorviaBroadcast`: `ID`, `TenantID`, `Message`, `Description`, and `CreatedDate`.
- Returns only the authenticated user's server-resolved tenant and tenant `0` (global).
- Sorts by `CreatedDate DESC, ID DESC` and fetches 10 rows per page asynchronously.
- Never accepts a tenant identifier from the browser.

## Azure App Service deployment

1. Create a Linux or Windows Azure App Service using the .NET 10 runtime (or deploy self-contained if the runtime is unavailable in the selected region).
2. Create/configure Azure SQL firewall access and run `database/scripts/001_intellibroadcast_setup.sql` once after review.
3. In **App Service > Configuration > Connection strings**, add `DefaultConnection` as type `SQLAzure`. Azure exposes it to ASP.NET Core as `ConnectionStrings__DefaultConnection`.
4. Set `ASPNETCORE_ENVIRONMENT=Production`. Replace `<APP-NAME>` in `appsettings.Production.json` or set `AllowedHosts` through configuration. No App Service setting is needed for `ProjectHash`.
5. Publish and deploy:

   ```bash
   dotnet publish src/IntelliBroadcast/IntelliBroadcast.csproj -c Release -o ./publish
   az webapp deploy --resource-group <RESOURCE_GROUP> --name <APP_NAME> --src-path ./publish --type zip
   ```

6. Require HTTPS, keep TLS 1.2 or newer, enable Always On, and verify `https://<APP_NAME>.azurewebsites.net/health`.
7. For scaled-out App Service instances, replace in-memory session storage with a shared `IDistributedCache` provider such as Azure Cache for Redis and persist Data Protection keys to a shared protected store. A single instance works with the included implementation.
8. Complete the Norvia Backoffice Admin provisioning handoff described above.

Application logs intentionally omit passwords, connection strings, email addresses, tenant IDs, and the project hash.
