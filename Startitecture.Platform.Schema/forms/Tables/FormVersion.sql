CREATE TABLE [forms].[FormVersion] (
    [FormVersionId] INT           IDENTITY (1, 1) NOT NULL,
    [FormId]        INT           NOT NULL,
    [Name]          NVARCHAR (50) NOT NULL,
    [Revision]      INT           NOT NULL,
    [IsActive]      BIT           NOT NULL,
    CONSTRAINT [PK_FormVersion] PRIMARY KEY CLUSTERED ([FormVersionId] ASC),
    CONSTRAINT [FK_FormVersion_Form] FOREIGN KEY ([FormId]) REFERENCES [forms].[Form] ([FormId])
);

