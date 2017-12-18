CREATE TABLE [workflow].[RejectionBehavior] (
    [RejectionBehaviorId] INT          NOT NULL,
    [Name]                VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_RejectionBehavior] PRIMARY KEY CLUSTERED ([RejectionBehaviorId] ASC)
);

