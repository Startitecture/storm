CREATE TABLE [dbo].[Container] (
    [ContainerId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (260) NOT NULL,
    [Path]        NVARCHAR (260) NOT NULL,
    CONSTRAINT [PK_Container] PRIMARY KEY CLUSTERED ([ContainerId] ASC)
);

