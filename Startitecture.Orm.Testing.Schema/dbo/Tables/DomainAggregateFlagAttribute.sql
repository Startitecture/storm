CREATE TABLE [dbo].[DomainAggregateFlagAttribute] (
    [DomainAggregateId] INT NOT NULL,
    [FlagAttributeId]   INT NOT NULL,
    CONSTRAINT [PK_DomainAggregateFlagAttribute] PRIMARY KEY CLUSTERED ([DomainAggregateId] ASC, [FlagAttributeId] ASC),
    CONSTRAINT [FK_DomainAggregateFlagAttribute_DomainAggregate] FOREIGN KEY ([DomainAggregateId]) REFERENCES [dbo].[DomainAggregate] ([DomainAggregateId]),
    CONSTRAINT [FK_DomainAggregateFlagAttribute_FlagAttribute] FOREIGN KEY ([FlagAttributeId]) REFERENCES [dbo].[FlagAttribute] ([FlagAttributeId])
);

