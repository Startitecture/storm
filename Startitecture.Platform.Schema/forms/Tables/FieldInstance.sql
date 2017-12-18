CREATE TABLE [forms].[FieldInstance] (
    [FieldInstanceId]     INT IDENTITY (1, 1) NOT NULL,
    [FieldId]             INT NOT NULL,
    [DataSliceInstanceId] INT NOT NULL,
    CONSTRAINT [PK_FieldInstance] PRIMARY KEY CLUSTERED ([FieldInstanceId] ASC),
    CONSTRAINT [FK_FieldInstance_DataSliceInstance] FOREIGN KEY ([DataSliceInstanceId]) REFERENCES [dbo].[DataSliceInstance] ([DataSliceInstanceId]),
    CONSTRAINT [FK_FieldInstance_Field] FOREIGN KEY ([FieldId]) REFERENCES [forms].[Field] ([FieldId])
);

