CREATE TABLE [dbo].[Field] (
    [FieldId]     INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Field] PRIMARY KEY CLUSTERED ([FieldId] ASC)
);

