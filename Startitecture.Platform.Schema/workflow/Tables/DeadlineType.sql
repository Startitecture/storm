CREATE TABLE [workflow].[DeadlineType] (
    [DeadlineTypeId] INT          NOT NULL,
    [Name]           VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_DeadlineType] PRIMARY KEY CLUSTERED ([DeadlineTypeId] ASC)
);

