CREATE TABLE [workflow].[WorkflowState] (
    [WorkflowStateId] INT          NOT NULL,
    [Name]            VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_WorkflowState] PRIMARY KEY CLUSTERED ([WorkflowStateId] ASC)
);

