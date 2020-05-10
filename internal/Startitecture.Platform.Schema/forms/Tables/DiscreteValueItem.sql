CREATE TABLE [forms].[DiscreteValueItem] (
    [DiscreteValueItemId] BIGINT NOT NULL,
    [Value]               BIGINT NOT NULL,
    CONSTRAINT [PK_DiscreteValueItem] PRIMARY KEY CLUSTERED ([DiscreteValueItemId] ASC),
    CONSTRAINT [FK_DiscreteValueItem_FieldValueItem] FOREIGN KEY ([DiscreteValueItemId]) REFERENCES [forms].[FieldValueItem] ([FieldValueItemId])
);

