CREATE TABLE [workflow].[PhaseSignatureOption] (
    [PhaseSignatureOptionId] INT NOT NULL,
    [SignatureTypeId]        INT NOT NULL,
    [RejectionBehaviorId]    INT NOT NULL,
    CONSTRAINT [PK_PhaseSignatureOption] PRIMARY KEY CLUSTERED ([PhaseSignatureOptionId] ASC),
    CONSTRAINT [FK_PhaseSignatureOption_ProcessPhase] FOREIGN KEY ([PhaseSignatureOptionId]) REFERENCES [workflow].[ProcessPhase] ([ProcessPhaseId]),
    CONSTRAINT [FK_PhaseSignatureOption_RejectionBehavior] FOREIGN KEY ([RejectionBehaviorId]) REFERENCES [workflow].[RejectionBehavior] ([RejectionBehaviorId]),
    CONSTRAINT [FK_PhaseSignatureOption_SignatureType] FOREIGN KEY ([SignatureTypeId]) REFERENCES [workflow].[SignatureType] ([SignatureTypeId])
);

