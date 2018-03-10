CREATE TABLE [dbo].[SubContainer] (
    [SubContainerId] INT           IDENTITY (1, 1) NOT NULL,
    [TopContainerId] INT           NOT NULL,
    [Name]           NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_SubContainer] PRIMARY KEY CLUSTERED ([SubContainerId] ASC),
    CONSTRAINT [FK_SubContainer_TopContainer] FOREIGN KEY ([TopContainerId]) REFERENCES [dbo].[TopContainer] ([TopContainerId])
);

