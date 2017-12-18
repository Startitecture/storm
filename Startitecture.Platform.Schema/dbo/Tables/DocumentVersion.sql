CREATE TABLE [dbo].[DocumentVersion] (
    [DocumentVersionId] INT                IDENTITY (1, 1) NOT NULL,
    [DocumentId]        INT                NOT NULL,
    [FileName]          NVARCHAR (260)     NOT NULL,
    [Uri]               NVARCHAR (260)     NOT NULL,
    [Revision]          INT                NOT NULL,
    [RevisionTime]      DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_DocumentVersion] PRIMARY KEY CLUSTERED ([DocumentVersionId] ASC),
    CONSTRAINT [FK_DocumentVersion_Document] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[Document] ([DocumentId]),
    CONSTRAINT [UK_DocumentVersion_Uri] UNIQUE NONCLUSTERED ([Uri] ASC)
);

