CREATE TABLE [dbo].[FieldValueElement] (
    [FieldValueElementId] BIGINT NOT NULL,
    [FieldValueId]        BIGINT NOT NULL,
    [Order]               INT    NOT NULL,
    CONSTRAINT [PK_FieldValueElement] PRIMARY KEY CLUSTERED ([FieldValueElementId] ASC),
    CONSTRAINT [FK_FieldValueElement_FieldValue1] FOREIGN KEY ([FieldValueId]) REFERENCES [dbo].[FieldValue] ([FieldValueId])
);

