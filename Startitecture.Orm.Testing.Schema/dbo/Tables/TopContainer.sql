CREATE TABLE [dbo].[TopContainer] (
    [TopContainerId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_TopContainer] PRIMARY KEY CLUSTERED ([TopContainerId] ASC)
);

