CREATE TABLE [workflow].[FormPhaseInstance] (
    [FormPhaseInstanceId] INT      IDENTITY (1, 1) NOT NULL,
    [FormVersionId]       INT      NOT NULL,
    [ProcessPhaseId]      INT      NOT NULL,
    [Order]               SMALLINT NOT NULL,
    CONSTRAINT [PK_FormPhaseInstance] PRIMARY KEY CLUSTERED ([FormPhaseInstanceId] ASC),
    CONSTRAINT [FK_FormInstance_FormVersion] FOREIGN KEY ([FormVersionId]) REFERENCES [forms].[FormVersion] ([FormVersionId]),
    CONSTRAINT [FK_FormPhaseInstance_ProcessPhase] FOREIGN KEY ([ProcessPhaseId]) REFERENCES [workflow].[ProcessPhase] ([ProcessPhaseId])
);

