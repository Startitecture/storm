CREATE TYPE [dbo].[FieldValueTableType] AS TABLE
(
	[FieldValueId]			BIGINT NULL,
	[FieldId]				INT NOT NULL,
	[LastModifiedByDomainIdentifierId] INT NOT NULL,
	[LastModifiedTime]		DATETIMEOFFSET(7) NOT NULL
)
