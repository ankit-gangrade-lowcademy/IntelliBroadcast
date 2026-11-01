/* Idempotent supplemental setup. Existing shared identity/product tables are not recreated. */
IF OBJECT_ID(N'dbo.NorviaBroadcast', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NorviaBroadcast (
        ID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_NorviaBroadcast PRIMARY KEY,
        TenantID int NOT NULL, Message nvarchar(500) NOT NULL,
        Description nvarchar(4000) NULL,
        CreatedDate datetime2 NOT NULL CONSTRAINT DF_NorviaBroadcast_CreatedDate DEFAULT SYSUTCDATETIME()
    );
END;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_NorviaBroadcast_TenantID_CreatedDate' AND object_id=OBJECT_ID(N'dbo.NorviaBroadcast'))
    CREATE INDEX IX_NorviaBroadcast_TenantID_CreatedDate ON dbo.NorviaBroadcast(TenantID, CreatedDate DESC);

-- NorviaProducts.ProductHash for IntelliBroadcast must equal:
-- 4f0d7c72-9a31-4c68-b526-2e8f5a1d93b7
