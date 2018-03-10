CREATE TABLE [dbo].[Child] (
    [ChildId]           INT           IDENTITY (1, 1) NOT NULL,
    [DomainAggregateId] INT           NOT NULL,
    [Name]              NVARCHAR (50) NOT NULL,
    [Value]             MONEY         NOT NULL,
    CONSTRAINT [PK_Child] PRIMARY KEY CLUSTERED ([ChildId] ASC),
    CONSTRAINT [FK_Child_DomainAggregate] FOREIGN KEY ([DomainAggregateId]) REFERENCES [dbo].[DomainAggregate] ([DomainAggregateId]) ON DELETE CASCADE
);

