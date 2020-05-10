CREATE TABLE [workflow].[Process] (
    [ProcessId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Process] PRIMARY KEY CLUSTERED ([ProcessId] ASC)
);

