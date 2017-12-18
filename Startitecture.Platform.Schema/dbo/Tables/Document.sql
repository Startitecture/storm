CREATE TABLE [dbo].[Document] (
    [DocumentId]               INT            IDENTITY (1, 1) NOT NULL,
    [ContainerId]              INT            NOT NULL,
    [ResourceClassificationId] INT            NOT NULL,
    [Name]                     NVARCHAR (260) NOT NULL,
    CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED ([DocumentId] ASC),
    CONSTRAINT [FK_Document_Container] FOREIGN KEY ([ContainerId]) REFERENCES [dbo].[Container] ([ContainerId]),
    CONSTRAINT [FK_Document_ResourceClassification] FOREIGN KEY ([ResourceClassificationId]) REFERENCES [dbo].[ResourceClassification] ([ResourceClassificationId])
);

