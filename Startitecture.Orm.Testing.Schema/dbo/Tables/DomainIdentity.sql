CREATE TABLE [dbo].[DomainIdentity] (
    [DomainIdentityId] INT            NOT NULL IDENTITY,
    [UniqueIdentifier] NVARCHAR (254) NOT NULL,
    [FirstName]        NVARCHAR (50)  NOT NULL,
    [MiddleName]       NVARCHAR (50)  NULL,
    [LastName]         NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_DomainIdentity] PRIMARY KEY CLUSTERED ([DomainIdentityId] ASC)
);

