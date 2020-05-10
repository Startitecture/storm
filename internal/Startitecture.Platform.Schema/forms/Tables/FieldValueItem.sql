CREATE TABLE [forms].[FieldValueItem] (
    [FieldValueItemId] BIGINT   IDENTITY (1, 1) NOT NULL,
    [FieldValueId]     BIGINT   NOT NULL,
    [Order]            SMALLINT NOT NULL,
    CONSTRAINT [PK_FieldValueItem] PRIMARY KEY CLUSTERED ([FieldValueItemId] ASC),
    CONSTRAINT [FK_FieldValueItem_FieldValue] FOREIGN KEY ([FieldValueId]) REFERENCES [forms].[FieldValue] ([FieldValueId])
);

