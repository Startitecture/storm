CREATE TABLE [forms].[Page] (
    [PageId]        INT           IDENTITY (1, 1) NOT NULL,
    [FormVersionId] INT           NOT NULL,
    [Name]          NVARCHAR (50) NOT NULL,
    [Order]         SMALLINT      NOT NULL,
    CONSTRAINT [PK_Page] PRIMARY KEY CLUSTERED ([PageId] ASC),
    CONSTRAINT [FK_Page_FormVersion] FOREIGN KEY ([FormVersionId]) REFERENCES [forms].[FormVersion] ([FormVersionId])
);

