CREATE TABLE [workflow].[PhaseActionDeadline] (
    [PhaseActionDeadlineId] INT      NOT NULL,
    [DeadlineTypeId]        INT      NOT NULL,
    [RoutingTypeId]         INT      NOT NULL,
    [DeadlineDays]          SMALLINT NOT NULL,
    CONSTRAINT [PK_PhaseActionDeadline] PRIMARY KEY CLUSTERED ([PhaseActionDeadlineId] ASC),
    CONSTRAINT [FK_PhaseActionDeadline_DeadlineType] FOREIGN KEY ([DeadlineTypeId]) REFERENCES [workflow].[DeadlineType] ([DeadlineTypeId]),
    CONSTRAINT [FK_PhaseActionDeadline_ProcessPhase] FOREIGN KEY ([PhaseActionDeadlineId]) REFERENCES [workflow].[ProcessPhase] ([ProcessPhaseId]),
    CONSTRAINT [FK_PhaseActionDeadline_RoutingType] FOREIGN KEY ([RoutingTypeId]) REFERENCES [workflow].[RoutingType] ([RoutingTypeId])
);

