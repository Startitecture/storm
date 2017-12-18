CREATE TABLE [forms].[DateValueItem] (
    [DateValueItemId] BIGINT             NOT NULL,
    [Value]           DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_DateValueItem] PRIMARY KEY CLUSTERED ([DateValueItemId] ASC),
    CONSTRAINT [FK_DateValueItem_FieldValueItem] FOREIGN KEY ([DateValueItemId]) REFERENCES [forms].[FieldValueItem] ([FieldValueItemId])
);

