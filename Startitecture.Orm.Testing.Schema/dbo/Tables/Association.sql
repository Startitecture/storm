CREATE TABLE [dbo].[Association] (
    [OtherAggregateId]  INT NOT NULL,
    [DomainAggregateId] INT NOT NULL,
    CONSTRAINT [PK_Association] PRIMARY KEY CLUSTERED ([OtherAggregateId] ASC),
    CONSTRAINT [FK_Association_DomainAggregate] FOREIGN KEY ([DomainAggregateId]) REFERENCES [dbo].[DomainAggregate] ([DomainAggregateId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Association_OtherAggregate] FOREIGN KEY ([OtherAggregateId]) REFERENCES [dbo].[OtherAggregate] ([OtherAggregateId]) ON DELETE CASCADE,
    CONSTRAINT [UK_AggregateLink_DomainAggregateId] UNIQUE NONCLUSTERED ([OtherAggregateId] ASC)
);

