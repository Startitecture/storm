CREATE TABLE [dbo].[OtherAggregate] (
    [OtherAggregateId]      INT           IDENTITY (1, 1) NOT NULL,
    [Name]                  NVARCHAR (50) NOT NULL,
    [AggregateOptionTypeId] INT           NOT NULL,
    CONSTRAINT [PK_OtherAggregate] PRIMARY KEY CLUSTERED ([OtherAggregateId] ASC),
    CONSTRAINT [FK_OtherAggregate_AggregateOptionType] FOREIGN KEY ([AggregateOptionTypeId]) REFERENCES [dbo].[AggregateOptionType] ([AggregateOptionTypeId])
);

