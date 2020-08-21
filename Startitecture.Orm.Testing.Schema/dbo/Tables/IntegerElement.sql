CREATE TABLE [dbo].[IntegerElement] (
    [IntegerElementId] BIGINT NOT NULL,
    [Value]            BIGINT    NOT NULL,
    CONSTRAINT [PK_IntegerElement] PRIMARY KEY CLUSTERED ([IntegerElementId] ASC),
    CONSTRAINT [FK_IntegerElement_FieldValueElement] FOREIGN KEY ([IntegerElementId]) REFERENCES [dbo].[FieldValueElement] ([FieldValueElementId]) ON DELETE CASCADE
);

