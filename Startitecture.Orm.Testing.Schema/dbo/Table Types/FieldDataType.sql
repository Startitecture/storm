CREATE TYPE [dbo].[FieldDataType] AS TABLE
(
	[FieldId] INT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL
)
