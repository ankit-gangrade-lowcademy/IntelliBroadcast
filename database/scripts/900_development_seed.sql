/* DEVELOPMENT ONLY. Replace the placeholders and generate the password hash using ASP.NET Core PasswordHasher<UserExtended>. */
IF NOT EXISTS (SELECT 1 FROM dbo.NorviaBroadcast WHERE Message = N'Development welcome')
    INSERT dbo.NorviaBroadcast(TenantID, Message, Description, CreatedDate)
    VALUES (0, N'Development welcome', N'Global demo announcement. Do not run this script in production.', SYSUTCDATETIME());

-- A login user is deliberately not seeded with a plaintext/default password.
-- Update an existing development-only UserExtended row with a PasswordHasher-generated value.
