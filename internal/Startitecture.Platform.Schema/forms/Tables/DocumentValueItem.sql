CREATE TABLE [forms].[DocumentValueItem] (
    [DocumentValueItemId] BIGINT NOT NULL,
    [DocumentVersionId]   INT    NOT NULL,
    CONSTRAINT [PK_DocumentValueItem] PRIMARY KEY CLUSTERED ([DocumentValueItemId] ASC),
    CONSTRAINT [FK_DocumentValueItem_FieldValueItem] FOREIGN KEY ([DocumentValueItemId]) REFERENCES [forms].[FieldValueItem] ([FieldValueItemId])
);

