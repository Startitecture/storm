CREATE TABLE [dbo].[ResourceAttachment] (
    [ResourceAttachmentId] BIGINT NOT NULL,
    [ExternalResourceId]   INT    NOT NULL,
    CONSTRAINT [PK_ResourceAttachment] PRIMARY KEY CLUSTERED ([ResourceAttachmentId] ASC),
    CONSTRAINT [FK_ResourceAttachment_Attachment] FOREIGN KEY ([ResourceAttachmentId]) REFERENCES [workflow].[Attachment] ([AttachmentId]),
    CONSTRAINT [FK_ResourceAttachment_ExternalResource] FOREIGN KEY ([ExternalResourceId]) REFERENCES [dbo].[ExternalResource] ([ExternalResourceId])
);

