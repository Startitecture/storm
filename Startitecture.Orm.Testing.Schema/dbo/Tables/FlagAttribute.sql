CREATE TABLE [dbo].[FlagAttribute] (
    [FlagAttributeId] INT           NOT NULL IDENTITY(1, 1),
    [Name]            NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_FlagAttribute] PRIMARY KEY CLUSTERED ([FlagAttributeId] ASC)
);

