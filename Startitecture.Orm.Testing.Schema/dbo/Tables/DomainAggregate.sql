CREATE TABLE [dbo].[DomainAggregate] (
    [DomainAggregateId]              INT                IDENTITY (1, 1) NOT NULL,
    [SubContainerId]                 INT                NOT NULL,
    [TemplateId]                     INT                NOT NULL,
    [CategoryAttributeId]            INT                NOT NULL,
    [Name]                           NVARCHAR (50)      NOT NULL,
    [Description]                    NVARCHAR (MAX)     NOT NULL,
    [CreatedByDomainIdentityId]      INT                NOT NULL,
    [CreatedTime]                    DATETIMEOFFSET (7) NOT NULL,
    [LastModifiedByDomainIdentityId] INT                NOT NULL,
    [LastModifiedTime]               DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_DomainAggregate] PRIMARY KEY CLUSTERED ([DomainAggregateId] ASC),
    CONSTRAINT [FK_DomainAggregate_CategoryAttribute] FOREIGN KEY ([CategoryAttributeId]) REFERENCES [dbo].[CategoryAttribute] ([CategoryAttributeId]),
    CONSTRAINT [FK_DomainAggregate_DomainIdentity] FOREIGN KEY ([CreatedByDomainIdentityId]) REFERENCES [dbo].[DomainIdentity] ([DomainIdentityId]),
    CONSTRAINT [FK_DomainAggregate_DomainIdentity1] FOREIGN KEY ([LastModifiedByDomainIdentityId]) REFERENCES [dbo].[DomainIdentity] ([DomainIdentityId]),
    CONSTRAINT [FK_DomainAggregate_SubContainer] FOREIGN KEY ([SubContainerId]) REFERENCES [dbo].[SubContainer] ([SubContainerId]),
    CONSTRAINT [FK_DomainAggregate_Template] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Template] ([TemplateId])
);

