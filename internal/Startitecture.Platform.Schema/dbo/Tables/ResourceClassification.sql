CREATE TABLE [dbo].[ResourceClassification] (
    [ResourceClassificationId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]                     NVARCHAR (50)  NOT NULL,
    [Description]              NVARCHAR (250) NULL,
    [IsActive]                 BIT            NOT NULL,
    CONSTRAINT [PK_ResourceClassification] PRIMARY KEY CLUSTERED ([ResourceClassificationId] ASC)
);

