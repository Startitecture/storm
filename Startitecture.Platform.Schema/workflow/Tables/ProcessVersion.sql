CREATE TABLE [workflow].[ProcessVersion] (
    [ProcessVersionId] INT           IDENTITY (1, 1) NOT NULL,
    [ProcessId]        INT           NOT NULL,
    [Revision]         INT           NOT NULL,
    [Name]             NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ProcessVersion] PRIMARY KEY CLUSTERED ([ProcessVersionId] ASC),
    CONSTRAINT [FK_ProcessVersion_Process] FOREIGN KEY ([ProcessId]) REFERENCES [workflow].[Process] ([ProcessId])
);

