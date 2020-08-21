CREATE TABLE [dbo].[FloatElement] (
    [FloatElementId] BIGINT     NOT NULL,
    [Value]          FLOAT (53) NOT NULL,
    CONSTRAINT [PK_FloatElement] PRIMARY KEY CLUSTERED ([FloatElementId] ASC),
    CONSTRAINT [FK_FloatElement_FieldValueElement] FOREIGN KEY ([FloatElementId]) REFERENCES [dbo].[FieldValueElement] ([FieldValueElementId]) ON DELETE CASCADE
);

