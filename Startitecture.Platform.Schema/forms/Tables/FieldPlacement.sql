CREATE TABLE [forms].[FieldPlacement] (
    [FieldPlacementId] BIGINT   IDENTITY (1, 1) NOT NULL,
    [FieldInstanceId]  INT      NOT NULL,
    [LayoutAreaId]     INT      NOT NULL,
    [Order]            SMALLINT NOT NULL,
    CONSTRAINT [PK_FieldPlacement] PRIMARY KEY CLUSTERED ([FieldPlacementId] ASC),
    CONSTRAINT [FK_FieldPlacement_FieldInstance] FOREIGN KEY ([FieldInstanceId]) REFERENCES [forms].[FieldInstance] ([FieldInstanceId]),
    CONSTRAINT [FK_FieldPlacement_LayoutArea] FOREIGN KEY ([LayoutAreaId]) REFERENCES [forms].[LayoutArea] ([LayoutAreaId])
);

