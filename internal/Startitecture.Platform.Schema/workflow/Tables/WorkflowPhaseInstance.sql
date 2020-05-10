CREATE TABLE [workflow].[WorkflowPhaseInstance] (
    [WorkflowPhaseInstanceId] BIGINT IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId]      INT    NOT NULL,
    [ProcessPhaseId]          INT    NOT NULL,
    [PhaseStateId]            INT    NOT NULL,
    CONSTRAINT [PK_WorkflowPhaseInstance] PRIMARY KEY CLUSTERED ([WorkflowPhaseInstanceId] ASC),
    CONSTRAINT [FK_WorkflowPhaseInstance_PhaseState] FOREIGN KEY ([PhaseStateId]) REFERENCES [workflow].[PhaseState] ([PhaseStateId]),
    CONSTRAINT [FK_WorkflowPhaseInstance_ProcessPhase] FOREIGN KEY ([ProcessPhaseId]) REFERENCES [workflow].[ProcessPhase] ([ProcessPhaseId]),
    CONSTRAINT [FK_WorkflowPhaseInstance_WorkflowInstance] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [workflow].[WorkflowInstance] ([WorkflowInstanceId])
);

