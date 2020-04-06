CREATE TYPE [dbo].[FieldValueDataType] AS TABLE
(
	[FieldValueId]			BIGINT NULL,
	[FieldId]				INT NOT NULL,
	[LastModifiedByDomainIdentifierId] INT NOT NULL,
	[LastModifiedTime]		DATETIMEOFFSET(7) NOT NULL
)
