CREATE TABLE [dbo].[AggregateOption] (
    [AggregateOptionId]     INT             NOT NULL,
    [Name]                  NVARCHAR (50)   NOT NULL,
    [AggregateOptionTypeId] INT             NOT NULL,
    [Value]                 DECIMAL (38, 4) NOT NULL,
    CONSTRAINT [PK_AggregateOption] PRIMARY KEY CLUSTERED ([AggregateOptionId] ASC),
    CONSTRAINT [FK_AggregateOption_AggregateOptionType] FOREIGN KEY ([AggregateOptionTypeId]) REFERENCES [dbo].[AggregateOptionType] ([AggregateOptionTypeId]),
    CONSTRAINT [FK_AggregateOption_DomainAggregate] FOREIGN KEY ([AggregateOptionId]) REFERENCES [dbo].[DomainAggregate] ([DomainAggregateId])
);

