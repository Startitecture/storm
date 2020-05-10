CREATE TABLE [workflow].[WorkflowInstance] (
    [WorkflowInstanceId] INT            IDENTITY (1, 1) NOT NULL,
    [ProcessVersionId]   INT            NOT NULL,
    [WorkflowStateId]    INT            NOT NULL,
    [Subject]            NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_WorkflowInstance] PRIMARY KEY CLUSTERED ([WorkflowInstanceId] ASC),
    CONSTRAINT [FK_WorkflowInstance_ProcessVersion] FOREIGN KEY ([ProcessVersionId]) REFERENCES [workflow].[ProcessVersion] ([ProcessVersionId]),
    CONSTRAINT [FK_WorkflowInstance_WorkflowState] FOREIGN KEY ([WorkflowStateId]) REFERENCES [workflow].[WorkflowState] ([WorkflowStateId])
);

