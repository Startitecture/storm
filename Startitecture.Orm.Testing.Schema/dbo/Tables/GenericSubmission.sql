CREATE TABLE [dbo].[GenericSubmission] (
    [GenericSubmissionId]          INT                IDENTITY (1, 1) NOT NULL,
    [Subject]                      NVARCHAR (50)      NOT NULL,
    [SubmittedByDomainIdentifierId] INT                NOT NULL,
    [SubmittedTime]                DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_GenericSubmission] PRIMARY KEY CLUSTERED ([GenericSubmissionId] ASC)
);

