CREATE TABLE [forms].[TextValueItem] (
    [TextValueItemId] BIGINT         NOT NULL,
    [Value]           NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_TextValueItem] PRIMARY KEY CLUSTERED ([TextValueItemId] ASC),
    CONSTRAINT [FK_TextValueItem_FieldValueItem] FOREIGN KEY ([TextValueItemId]) REFERENCES [forms].[FieldValueItem] ([FieldValueItemId])
);

