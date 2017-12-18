CREATE TABLE [forms].[LayoutArea] (
    [LayoutAreaId] INT              IDENTITY (1, 1) NOT NULL,
    [SectionId]    INT              NOT NULL,
    [Identifier]   UNIQUEIDENTIFIER NOT NULL,
    [Order]        SMALLINT         NOT NULL,
    [Style]        VARCHAR (50)     NOT NULL,
    CONSTRAINT [PK_LayoutArea] PRIMARY KEY CLUSTERED ([LayoutAreaId] ASC),
    CONSTRAINT [FK_LayoutArea_Section] FOREIGN KEY ([SectionId]) REFERENCES [forms].[Section] ([SectionId])
);

