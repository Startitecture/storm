SET NOCOUNT ON
 
MERGE INTO [AggregateOptionType] AS Target
USING (VALUES
  (1,'OptionA')
 ,(2,'OptionB')
 ,(3,'OptionC')
 ,(4,'OptionD')
 ,(5,'OptionE')
) AS Source ([AggregateOptionTypeId],[Name])
ON (Target.[AggregateOptionTypeId] = Source.[AggregateOptionTypeId])
WHEN MATCHED AND (Target.[Name] <> Source.[Name]) THEN
 UPDATE SET
 [Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([AggregateOptionTypeId],[Name])
 VALUES(Source.[AggregateOptionTypeId],Source.[Name])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;
