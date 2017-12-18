CREATE TABLE [workflow].[Attachment] (
    [AttachmentId]             BIGINT         IDENTITY (1, 1) NOT NULL,
    [WorkflowFormSubmissionId] BIGINT         NOT NULL,
    [Subject]                  NVARCHAR (100) NOT NULL,
    [Order]                    INT            NOT NULL,
    CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED ([AttachmentId] ASC),
    CONSTRAINT [FK_Attachment_WorkflowFormSubmission] FOREIGN KEY ([WorkflowFormSubmissionId]) REFERENCES [workflow].[WorkflowFormSubmission] ([WorkflowFormSubmissionId])
);

