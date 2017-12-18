CREATE TABLE [dbo].[DataSliceInstance] (
    [DataSliceInstanceId] INT              IDENTITY (1, 1) NOT NULL,
    [DataSliceId]         INT              NOT NULL,
    [Identifier]          UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_DataSliceInstance] PRIMARY KEY CLUSTERED ([DataSliceInstanceId] ASC),
    CONSTRAINT [FK_DataSliceInstance_DataSlice] FOREIGN KEY ([DataSliceId]) REFERENCES [dbo].[DataSlice] ([DataSliceId])
);

