CREATE TABLE [dbo].[AggregateSubmissionId] (
    [AggregateSubmissionId] INT NOT NULL,
    [DomainAggregateId]     INT NOT NULL,
    CONSTRAINT [PK_AggregateSubmissionId] PRIMARY KEY CLUSTERED ([AggregateSubmissionId] ASC),
    CONSTRAINT [FK_AggregateSubmissionId_DomainAggregate] FOREIGN KEY ([DomainAggregateId]) REFERENCES [dbo].[DomainAggregate] ([DomainAggregateId]),
    CONSTRAINT [FK_AggregateSubmissionId_GenericSubmission] FOREIGN KEY ([DomainAggregateId]) REFERENCES [dbo].[GenericSubmission] ([GenericSubmissionId]) ON DELETE CASCADE
);

