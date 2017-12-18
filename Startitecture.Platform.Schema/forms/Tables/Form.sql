CREATE TABLE [forms].[Form] (
    [FormId]   INT           IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (50) NOT NULL,
    [IsActive] BIT           NOT NULL,
    CONSTRAINT [PK_Form] PRIMARY KEY CLUSTERED ([FormId] ASC)
);

