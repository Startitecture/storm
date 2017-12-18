CREATE TABLE [forms].[Section] (
    [SectionId] INT           IDENTITY (1, 1) NOT NULL,
    [PageId]    INT           NOT NULL,
    [Name]      NVARCHAR (50) NOT NULL,
    [Order]     SMALLINT      NOT NULL,
    CONSTRAINT [PK_Section] PRIMARY KEY CLUSTERED ([SectionId] ASC),
    CONSTRAINT [FK_Section_Page] FOREIGN KEY ([PageId]) REFERENCES [forms].[Page] ([PageId])
);

