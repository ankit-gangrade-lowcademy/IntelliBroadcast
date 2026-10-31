/* Idempotent compatibility setup for the shared Norvia database. Review before running. */
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.NorviaBroadcast', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NorviaBroadcast
    (
        ID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_NorviaBroadcast PRIMARY KEY,
        TenantID int NULL,
        Message nvarchar(500) NOT NULL,
        Description nvarchar(max) NULL,
        CreatedDate datetime2(3) NOT NULL CONSTRAINT DF_NorviaBroadcast_CreatedDate DEFAULT SYSUTCDATETIME()
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_NorviaBroadcast_Tenant_Created' AND object_id = OBJECT_ID(N'dbo.NorviaBroadcast'))
    CREATE INDEX IX_NorviaBroadcast_Tenant_Created ON dbo.NorviaBroadcast(TenantID, CreatedDate DESC, ID DESC) INCLUDE(Message);

IF COL_LENGTH(N'dbo.NorviaProducts', N'ProductHash') IS NULL
    ALTER TABLE dbo.NorviaProducts ADD ProductHash uniqueidentifier NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_NorviaProducts_ProductHash' AND object_id = OBJECT_ID(N'dbo.NorviaProducts'))
    CREATE UNIQUE INDEX UX_NorviaProducts_ProductHash ON dbo.NorviaProducts(ProductHash) WHERE ProductHash IS NOT NULL;

COMMIT TRANSACTION;
