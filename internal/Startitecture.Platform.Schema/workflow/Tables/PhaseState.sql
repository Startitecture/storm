CREATE TABLE [workflow].[PhaseState] (
    [PhaseStateId] INT          NOT NULL,
    [Name]         VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_PhaseState] PRIMARY KEY CLUSTERED ([PhaseStateId] ASC)
);

