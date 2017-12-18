CREATE TABLE [dbo].[ResourceAttachment] (
    [ResourceAttahcmentId] BIGINT NOT NULL,
    [ExternalResourceId]   INT    NOT NULL,
    CONSTRAINT [PK_ResourceAttachment] PRIMARY KEY CLUSTERED ([ResourceAttahcmentId] ASC),
    CONSTRAINT [FK_ResourceAttachment_Attachment] FOREIGN KEY ([ResourceAttahcmentId]) REFERENCES [workflow].[Attachment] ([AttachmentId]),
    CONSTRAINT [FK_ResourceAttachment_ExternalResource] FOREIGN KEY ([ExternalResourceId]) REFERENCES [dbo].[ExternalResource] ([ExternalResourceId])
);

