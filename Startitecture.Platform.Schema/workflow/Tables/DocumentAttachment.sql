CREATE TABLE [workflow].[DocumentAttachment] (
    [DocumentAttachmentId] BIGINT NOT NULL,
    [DocumentVersionId]    INT    NOT NULL,
    CONSTRAINT [PK_DocumentAttachment] PRIMARY KEY CLUSTERED ([DocumentAttachmentId] ASC),
    CONSTRAINT [FK_DocumentAttachment_Attachment] FOREIGN KEY ([DocumentAttachmentId]) REFERENCES [workflow].[Attachment] ([AttachmentId]),
    CONSTRAINT [FK_DocumentAttachment_DocumentVersion] FOREIGN KEY ([DocumentVersionId]) REFERENCES [dbo].[DocumentVersion] ([DocumentVersionId])
);

