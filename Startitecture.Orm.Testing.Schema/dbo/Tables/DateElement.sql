CREATE TABLE [dbo].[DateElement] (
    [DateElementId] BIGINT             NOT NULL,
    [Value]         DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_DateElement] PRIMARY KEY CLUSTERED ([DateElementId] ASC),
    CONSTRAINT [FK_DateElement_FieldValueElement] FOREIGN KEY ([DateElementId]) REFERENCES [dbo].[FieldValueElement] ([FieldValueElementId]) ON DELETE CASCADE
);

