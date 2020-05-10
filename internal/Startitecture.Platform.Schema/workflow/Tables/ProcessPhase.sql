CREATE TABLE [workflow].[ProcessPhase] (
    [ProcessPhaseId]   INT           IDENTITY (1, 1) NOT NULL,
    [ProcessVersionId] INT           NOT NULL,
    [PhaseTypeId]      INT           NOT NULL,
    [Name]             NVARCHAR (50) NOT NULL,
    [Order]            SMALLINT      NOT NULL,
    CONSTRAINT [PK_ProcessPhase] PRIMARY KEY CLUSTERED ([ProcessPhaseId] ASC),
    CONSTRAINT [FK_ProcessPhase_PhaseType] FOREIGN KEY ([PhaseTypeId]) REFERENCES [workflow].[PhaseType] ([PhaseTypeId]),
    CONSTRAINT [FK_ProcessPhase_ProcessVersion] FOREIGN KEY ([ProcessVersionId]) REFERENCES [workflow].[ProcessVersion] ([ProcessVersionId])
);

