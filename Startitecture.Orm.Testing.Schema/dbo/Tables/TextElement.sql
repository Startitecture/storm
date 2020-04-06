CREATE TABLE [dbo].[TextElement]
(
	[TextElementId] BIGINT NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_TextElement] PRIMARY KEY CLUSTERED ([TextElementId] ASC), 
    CONSTRAINT [FK_TextElement_FieldValueElement] FOREIGN KEY ([TextElementId]) REFERENCES [FieldValueElement](FieldValueElementId)
)
