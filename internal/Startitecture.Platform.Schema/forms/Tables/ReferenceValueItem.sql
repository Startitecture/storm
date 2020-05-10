CREATE TABLE [forms].[ReferenceValueItem] (
    [ReferenceValueItemId] BIGINT         NOT NULL,
    [ItemType]             NVARCHAR (250) NOT NULL,
    [ItemId]               BIGINT         NOT NULL,
    CONSTRAINT [PK_ReferenceValueItem] PRIMARY KEY CLUSTERED ([ReferenceValueItemId] ASC),
    CONSTRAINT [FK_ReferenceValueItem_FieldValueItem] FOREIGN KEY ([ReferenceValueItemId]) REFERENCES [forms].[FieldValueItem] ([FieldValueItemId])
);

