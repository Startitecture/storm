CREATE TABLE [dbo].[Person] (
    [PersonId]           INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]          NVARCHAR (50)  NOT NULL,
    [MiddleName]         NVARCHAR (50)  NOT NULL,
    [LastName]           NVARCHAR (50)  NOT NULL,
    [SecurityIdentifier] NVARCHAR (254) NOT NULL,
    CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([PersonId] ASC),
    CONSTRAINT [UK_Person_SecurityIdentifier] UNIQUE NONCLUSTERED ([SecurityIdentifier] ASC) WITH (FILLFACTOR = 80)
);

