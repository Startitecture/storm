CREATE TABLE [dbo].[CategoryAttribute] (
    [CategoryAttributeId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (50) NOT NULL,
    [IsSystem]            BIT           NOT NULL,
    [IsActive]            BIT           NOT NULL,
    CONSTRAINT [PK_CategoryAttribute] PRIMARY KEY CLUSTERED ([CategoryAttributeId] ASC)
);

