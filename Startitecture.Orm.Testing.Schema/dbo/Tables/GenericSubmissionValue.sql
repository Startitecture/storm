CREATE TABLE [dbo].[GenericSubmissionValue] (
    [GenericSubmissionValueId] BIGINT NOT NULL,
    [GenericSubmissionId]      INT    NOT NULL,
    CONSTRAINT [PK_GenericSubmissionValue] PRIMARY KEY CLUSTERED ([GenericSubmissionValueId] ASC),
    CONSTRAINT [FK_GenericSubmissionValue_FieldValue] FOREIGN KEY ([GenericSubmissionValueId]) REFERENCES [dbo].[FieldValue] ([FieldValueId]) ON DELETE CASCADE,
    CONSTRAINT [FK_GenericSubmissionValue_GenericSubmission] FOREIGN KEY ([GenericSubmissionId]) REFERENCES [dbo].[GenericSubmission] ([GenericSubmissionId])
);

