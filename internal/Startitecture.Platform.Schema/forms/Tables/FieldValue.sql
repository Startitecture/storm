CREATE TABLE [forms].[FieldValue] (
    [FieldValueId]     BIGINT IDENTITY (1, 1) NOT NULL,
    [FormSubmissionId] BIGINT NOT NULL,
    [FieldInstanceId]  INT    NOT NULL,
    CONSTRAINT [PK_FieldValue] PRIMARY KEY CLUSTERED ([FieldValueId] ASC),
    CONSTRAINT [FK_FieldValue_FieldInstance] FOREIGN KEY ([FieldInstanceId]) REFERENCES [forms].[FieldInstance] ([FieldInstanceId]),
    CONSTRAINT [FK_FieldValue_FormSubmission] FOREIGN KEY ([FormSubmissionId]) REFERENCES [forms].[FormSubmission] ([FormSubmissionId])
);

