CREATE TABLE [dbo].[AggregateSubmission] (
    [AggregateSubmissionId] INT NOT NULL,
    [DomainAggregateId]     INT NOT NULL,
    CONSTRAINT [PK_AggregateSubmission] PRIMARY KEY CLUSTERED ([AggregateSubmissionId] ASC),
    CONSTRAINT [FK_AggregateSubmission_DomainAggregate] FOREIGN KEY ([DomainAggregateId]) REFERENCES [dbo].[DomainAggregate] ([DomainAggregateId]),
    CONSTRAINT [FK_AggregateSubmission_GenericSubmission] FOREIGN KEY ([DomainAggregateId]) REFERENCES [dbo].[GenericSubmission] ([GenericSubmissionId]) ON DELETE CASCADE
);

