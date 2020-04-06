CREATE TYPE [dbo].[FieldValueElementDataType] AS TABLE
(
    [FieldValueElementId] BIGINT NULL,
    [FieldValueId]        BIGINT NOT NULL,
    [Order]               INT    NOT NULL,
    [DateElement]         DATETIMEOFFSET(7) NULL,
    [FloatElement]        FLOAT  NULL,
    [IntegerElement]      BIGINT NULL,
    [MoneyElement]        MONEY  NULL,
    [TextElement]         NVARCHAR(MAX) NULL
)