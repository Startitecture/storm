GRANT USAGE ON SCHEMA dbo TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."AggregateEventStartId_AggregateEventStartId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."CategoryAttribute_CategoryAttributeId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."Child_ChildId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."DomainAggregate_DomainAggregateId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."DomainIdentity_DomainIdentityId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."FieldValueElement_FieldValueElementId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."FieldValue_FieldValueId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."Field_FieldId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."FlagAttribute_FlagAttributeId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."GenericSubmission_GenericSubmissionId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."OtherAggregate_OtherAggregateId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."SubContainer_SubContainerId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."Template_TemplateId_seq" TO storm_dev_user;

GRANT ALL ON SEQUENCE dbo."TopContainer_TopContainerId_seq" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."AggregateEventCompletion" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."AggregateEventStart" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."AggregateOption" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."AggregateOptionType" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."AggregateSubmission" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."Association" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."CategoryAttribute" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."Child" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."CurrentAggregateSubmission" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."DateElement" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."DomainAggregate" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."DomainAggregateFlagAttribute" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."DomainIdentity" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."Field" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."FieldValue" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."FieldValueElement" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."FlagAttribute" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."FloatElement" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."GenericSubmission" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."GenericSubmissionValue" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."IntegerElement" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."MoneyElement" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."OtherAggregate" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."SubContainer" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."Template" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."TextElement" TO storm_dev_user;

GRANT SELECT, UPDATE, INSERT, DELETE, TRUNCATE ON TABLE dbo."TopContainer" TO storm_dev_user;

