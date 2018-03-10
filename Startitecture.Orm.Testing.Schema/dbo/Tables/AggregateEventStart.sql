CREATE TABLE [dbo].[AggregateEventStart] (
    [AggregateEventStartId] BIGINT             IDENTITY (1, 1) NOT NULL,
    [DomainAggregateId]     INT                NOT NULL,
    [GlobalIdentifier]      UNIQUEIDENTIFIER   NOT NULL,
    [DomainIdentityId]      INT                NOT NULL,
    [StartTime]             DATETIMEOFFSET (7) NOT NULL,
    [EventName]             NVARCHAR (50)      NOT NULL,
    [EventDescription]      NVARCHAR (MAX)     NOT NULL,
    CONSTRAINT [PK_AggregateEventStart] PRIMARY KEY CLUSTERED ([AggregateEventStartId] ASC),
    CONSTRAINT [FK_AggregateEventStart_DomainAggregate] FOREIGN KEY ([DomainAggregateId]) REFERENCES [dbo].[DomainAggregate] ([DomainAggregateId]) ON DELETE CASCADE,
    CONSTRAINT [FK_AggregateEventStart_DomainIdentity] FOREIGN KEY ([DomainIdentityId]) REFERENCES [dbo].[DomainIdentity] ([DomainIdentityId])
);

