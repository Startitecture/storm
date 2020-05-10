CREATE TABLE [forms].[FormSubmission] (
    [FormSubmissionId]    BIGINT             IDENTITY (1, 1) NOT NULL,
    [FormVersionId]       INT                NOT NULL,
    [SubmissionTime]      DATETIMEOFFSET (7) NOT NULL,
    [SubmittedByPersonId] INT                NOT NULL,
    CONSTRAINT [PK_FormSubmission] PRIMARY KEY CLUSTERED ([FormSubmissionId] ASC),
    CONSTRAINT [FK_FormSubmission_FormVersion] FOREIGN KEY ([FormVersionId]) REFERENCES [forms].[FormVersion] ([FormVersionId]),
    CONSTRAINT [FK_FormSubmission_Person] FOREIGN KEY ([SubmittedByPersonId]) REFERENCES [dbo].[Person] ([PersonId])
);

