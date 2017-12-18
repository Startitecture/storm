CREATE TABLE [workflow].[WorkflowFormSubmission] (
    [WorkflowFormSubmissionId] BIGINT NOT NULL,
    [WorkflowPhaseInstanceId]  BIGINT NOT NULL,
    [FormPhaseInstanceId]      INT    NOT NULL,
    CONSTRAINT [PK_WorkflowFormSubmission] PRIMARY KEY CLUSTERED ([WorkflowFormSubmissionId] ASC),
    CONSTRAINT [FK_WorkflowFormSubmission_FormPhaseInstance] FOREIGN KEY ([FormPhaseInstanceId]) REFERENCES [workflow].[FormPhaseInstance] ([FormPhaseInstanceId]),
    CONSTRAINT [FK_WorkflowFormSubmission_FormSubmission] FOREIGN KEY ([WorkflowFormSubmissionId]) REFERENCES [forms].[FormSubmission] ([FormSubmissionId]),
    CONSTRAINT [FK_WorkflowFormSubmission_WorkflowPhaseInstance] FOREIGN KEY ([WorkflowPhaseInstanceId]) REFERENCES [workflow].[WorkflowPhaseInstance] ([WorkflowPhaseInstanceId])
);

