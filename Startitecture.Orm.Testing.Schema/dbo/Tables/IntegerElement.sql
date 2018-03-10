CREATE TABLE [dbo].[IntegerElement] (
    [IntegerElementId] BIGINT NOT NULL,
    [Value]            INT    NOT NULL,
    CONSTRAINT [PK_IntegerElement] PRIMARY KEY CLUSTERED ([IntegerElementId] ASC),
    CONSTRAINT [FK_IntegerElement_FieldValueElement] FOREIGN KEY ([IntegerElementId]) REFERENCES [dbo].[FieldValueElement] ([FieldValueElementId])
);

