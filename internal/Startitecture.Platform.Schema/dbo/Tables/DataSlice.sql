CREATE TABLE [dbo].[DataSlice] (
    [DataSliceId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_DataSlice] PRIMARY KEY CLUSTERED ([DataSliceId] ASC)
);

