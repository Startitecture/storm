CREATE TABLE [dbo].[AggregateEventCompletion] (
    [AggregateEventCompletionId] BIGINT             NOT NULL,
    [EndTime]                    DATETIMEOFFSET (7) NOT NULL,
    [Succeeded]                  BIT                NOT NULL,
    [Result]                     NVARCHAR (MAX)     NOT NULL,
    CONSTRAINT [PK_AggregateEventCompletion] PRIMARY KEY CLUSTERED ([AggregateEventCompletionId] ASC),
    CONSTRAINT [FK_AggregateEventCompletion_AggregateEventStart] FOREIGN KEY ([AggregateEventCompletionId]) REFERENCES [dbo].[AggregateEventStart] ([AggregateEventStartId]) ON DELETE CASCADE
);

