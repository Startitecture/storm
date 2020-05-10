CREATE TABLE [forms].[Field] (
    [FieldId]     INT            IDENTITY (1, 1) NOT NULL,
    [DataSliceId] INT            NOT NULL,
    [FieldTypeId] INT            NOT NULL,
    [ValueTypeId] INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Label]       NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Field] PRIMARY KEY CLUSTERED ([FieldId] ASC),
    CONSTRAINT [FK_Field_DataSlice] FOREIGN KEY ([DataSliceId]) REFERENCES [dbo].[DataSlice] ([DataSliceId]),
    CONSTRAINT [FK_Field_FieldType] FOREIGN KEY ([FieldTypeId]) REFERENCES [forms].[FieldType] ([FieldTypeId]),
    CONSTRAINT [FK_Field_ValueType] FOREIGN KEY ([ValueTypeId]) REFERENCES [forms].[ValueType] ([ValueTypeId])
);

