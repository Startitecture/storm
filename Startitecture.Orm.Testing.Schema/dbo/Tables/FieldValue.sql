CREATE TABLE [dbo].[FieldValue] (
    [FieldValueId]                     BIGINT             IDENTITY (1, 1) NOT NULL,
    [FieldId]                          INT                NOT NULL,
    [LastModifiedByDomainIdentifierId] INT                NOT NULL,
    [LastModifiedTime]                 DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_FieldValue] PRIMARY KEY CLUSTERED ([FieldValueId] ASC),
    CONSTRAINT [FK_FieldValue_DomainIdentity] FOREIGN KEY ([LastModifiedByDomainIdentifierId]) REFERENCES [dbo].[DomainIdentity] ([DomainIdentityId]),
    CONSTRAINT [FK_FieldValue_Field] FOREIGN KEY ([FieldId]) REFERENCES [dbo].[Field] ([FieldId])
);

