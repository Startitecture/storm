CREATE TABLE [dbo].[Template] (
    [TemplateId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Template] PRIMARY KEY CLUSTERED ([TemplateId] ASC)
);

