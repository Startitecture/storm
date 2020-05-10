CREATE TABLE [forms].[CurrencyValueItem] (
    [CurrencyValueItemId] BIGINT          NOT NULL,
    [Value]               DECIMAL (32, 4) NOT NULL,
    CONSTRAINT [PK_CurrencyValueItem] PRIMARY KEY CLUSTERED ([CurrencyValueItemId] ASC),
    CONSTRAINT [FK_CurrencyValueItem_FieldValueItem] FOREIGN KEY ([CurrencyValueItemId]) REFERENCES [forms].[FieldValueItem] ([FieldValueItemId])
);

