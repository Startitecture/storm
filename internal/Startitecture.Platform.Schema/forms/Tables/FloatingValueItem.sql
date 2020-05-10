CREATE TABLE [forms].[FloatingValueItem] (
    [FloatingValueItemId] BIGINT     NOT NULL,
    [Value]               FLOAT (53) NOT NULL,
    CONSTRAINT [PK_FloatingValueItem] PRIMARY KEY CLUSTERED ([FloatingValueItemId] ASC),
    CONSTRAINT [FK_FloatingValueItem_FieldValueItem] FOREIGN KEY ([FloatingValueItemId]) REFERENCES [forms].[FieldValueItem] ([FieldValueItemId])
);

