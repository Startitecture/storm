CREATE TABLE [dbo].[CurrentAggregateSubmission] (
    [CurrentAggregateSubmissionId] INT NOT NULL,
    [DomainAggregateId]            INT NOT NULL,
    CONSTRAINT [PK_CurrentAggregateSubmission] PRIMARY KEY CLUSTERED ([CurrentAggregateSubmissionId] ASC),
    CONSTRAINT [FK_CurrentAggregateSubmission_DomainAggregate] FOREIGN KEY ([DomainAggregateId]) REFERENCES [dbo].[DomainAggregate] ([DomainAggregateId]),
    CONSTRAINT [FK_CurrentAggregateSubmission_GenericSubmission] FOREIGN KEY ([CurrentAggregateSubmissionId]) REFERENCES [dbo].[GenericSubmission] ([GenericSubmissionId]),
    CONSTRAINT [UK_CurrentAggregateSubmission_DomainAggregateId] UNIQUE NONCLUSTERED ([DomainAggregateId] ASC)
);

