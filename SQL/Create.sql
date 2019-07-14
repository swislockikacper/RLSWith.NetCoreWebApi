IF (OBJECT_ID('dbo.FK_Client_Tenant', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE [dbo].[Client] DROP CONSTRAINT [FK_Client_Tenant]
END

ALTER SECURITY POLICY [dbo].[TenantAccessPolicy] DROP FILTER PREDICATE ON [dbo].[Client]
GO

ALTER SECURITY POLICY [dbo].[TenantAccessPolicy] DROP BLOCK PREDICATE ON [dbo].[Client]
GO

DROP SECURITY POLICY IF EXISTS [dbo].[TenantAccessPolicy]
GO

DROP FUNCTION IF EXISTS [dbo].[TenantAccessPredicate]
GO

DROP TABLE IF EXISTS [dbo].[Client]
GO

DROP TABLE IF EXISTS [dbo].[Tenant]
GO

DROP LOGIN [adminlogin]
GO

DROP LOGIN [userlogin]
GO

DROP USER [admin]
GO

DROP USER [user]
GO

CREATE TABLE [dbo].[Tenant]
(
	[TenantId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY CLUSTERED,
	[ApiKey] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(200) NOT NULL
)
GO

CREATE TABLE [dbo].[Client]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY CLUSTERED,
	[TenantId] UNIQUEIDENTIFIER DEFAULT CAST(SESSION_CONTEXT(N'TenantId') AS UNIQUEIDENTIFIER),
	[FullName] NVARCHAR(150) NOT NULL,
	[Age] smallint NULL,
	CONSTRAINT [FK_Client_Tenant] FOREIGN KEY ([TenantId]) REFERENCES dbo.Tenant([TenantId])
)
GO


INSERT INTO [dbo].[Tenant]([TenantId], [ApiKey], [Name])
VALUES('ad974f04-f31f-4a15-b655-0fa068aed3e8', 'a15fb65e-b347-4d75-9b30-2369e0afb8b8','Tenant 1')
GO

INSERT INTO [dbo].[Tenant]([TenantId], [ApiKey], [Name])
VALUES('f870e20b-addf-4777-8211-9e2ed75b4f71', 'c6835aac-8969-44a7-8b9f-a71835fe6951','Tenant 2')
GO

INSERT [dbo].[Client]([Id], [TenantId], [FullName], [Age]) 
VALUES ('87837c6c-2ea3-4f27-b0c7-79760b443d16', 'ad974f04-f31f-4a15-b655-0fa068aed3e8', 'Test Client 1', 20)
GO

INSERT [dbo].[Client]([Id], [TenantId], [FullName], [Age]) 
VALUES ('98d0203c-216c-4bc3-a021-78f5129a4685', 'ad974f04-f31f-4a15-b655-0fa068aed3e8', 'Test Client 2', 50)
GO

INSERT [dbo].[Client]([Id], [TenantId], [FullName], [Age]) 
VALUES ('964a8488-6806-4c3a-a62f-c10701da5cb1', 'f870e20b-addf-4777-8211-9e2ed75b4f71', 'Test Client 3', 34)
GO

INSERT [dbo].[Client]([Id], [TenantId], [FullName], [Age]) 
VALUES ('354fa3c0-dce4-42b6-89bd-075f33da476f', 'f870e20b-addf-4777-8211-9e2ed75b4f71', 'Test Client 4', 28)
GO

INSERT [dbo].[Client]([Id], [TenantId], [FullName], [Age]) 
VALUES ('7b7e6650-c5dd-481f-a5bf-29f109d3303e', 'f870e20b-addf-4777-8211-9e2ed75b4f71', 'Test Client 5', 45)
GO

CREATE LOGIN [adminlogin] WITH PASSWORD = 'sup3rus3r'
GO

CREATE USER [admin] FOR LOGIN [adminlogin]
GO

EXEC sp_addrolemember N'db_owner', N'admin'
GO

CREATE LOGIN [userlogin] WITH PASSWORD = 'us3r'
GO

CREATE USER [user] FOR LOGIN [userlogin]
GO

EXEC sp_addrolemember N'db_datareader', N'user'
EXEC sp_addrolemember N'db_datawriter', N'user'
GO


CREATE FUNCTION [dbo].[TenantAccessPredicate] (@TenantId uniqueidentifier)
    RETURNS TABLE
    WITH SCHEMABINDING
AS
    RETURN SELECT 1 AS TenantAccessPredicateResult 
	WHERE (@TenantId = CAST(SESSION_CONTEXT(N'TenantId') AS uniqueidentifier))
		  OR
		  (DATABASE_PRINCIPAL_ID() = DATABASE_PRINCIPAL_ID('admin'))
GO

CREATE SECURITY POLICY [dbo].[TenantAccessPolicy]
ADD FILTER PREDICATE [dbo].[TenantAccessPredicate]([TenantId]) ON [dbo].[Client],
ADD BLOCK PREDICATE [dbo].[TenantAccessPredicate]([TenantId]) ON [dbo].[Client]
GO

--Test connections
-- Tenant 1
EXEC sp_set_session_context @key=N'TenantId', @value='ad974f04-f31f-4a15-b655-0fa068aed3e8'
GO
SELECT * FROM [dbo].[Client]
GO

-- Tenant 2
EXEC sp_set_session_context @key=N'TenantId', @value='f870e20b-addf-4777-8211-9e2ed75b4f71'
GO
SELECT * FROM [dbo].[Client]
GO
