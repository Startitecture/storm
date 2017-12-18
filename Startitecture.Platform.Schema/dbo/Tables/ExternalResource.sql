CREATE TABLE [dbo].[ExternalResource] (
    [ExternalResourceId]       INT            NOT NULL,
    [ResourceClassificationId] INT            NOT NULL,
    [Name]                     NVARCHAR (260) NOT NULL,
    [Uri]                      NVARCHAR (260) NOT NULL,
    CONSTRAINT [PK_ExternalResource] PRIMARY KEY CLUSTERED ([ExternalResourceId] ASC),
    CONSTRAINT [FK_ExternalResource_ResourceClassification] FOREIGN KEY ([ResourceClassificationId]) REFERENCES [dbo].[ResourceClassification] ([ResourceClassificationId]),
    CONSTRAINT [UK_ExternalResource_Uri] UNIQUE NONCLUSTERED ([Uri] ASC)
);

