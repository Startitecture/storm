CREATE TABLE [dbo].[MoneyElement] (
    [MoneyElementId] BIGINT NOT NULL,
    [Value]          MONEY  NOT NULL,
    CONSTRAINT [PK_MoneyElement] PRIMARY KEY CLUSTERED ([MoneyElementId] ASC),
    CONSTRAINT [FK_MoneyElement_FieldValueElement] FOREIGN KEY ([MoneyElementId]) REFERENCES [dbo].[FieldValueElement] ([FieldValueElementId]) ON DELETE CASCADE 
);

