CREATE TABLE [dbo].[Products] (
    [ProductID]   INT             IDENTITY (1, 1) NOT NULL,
    [ProductType] NVARCHAR (100)  NULL,
    [Make]        NVARCHAR (100)  NULL,
    [Mod]         NVARCHAR (100)  NULL,
    [Description] NVARCHAR (1000) NULL,
    [Image]       NVARCHAR (1000) NULL,
    [Price]       MONEY           NULL,
    [Quantity]    INT             NULL,
    [Color]       NVARCHAR (100)  NULL,
    [Watts]       INT             NULL,
    [Size]        NVARCHAR (100)  NULL,
    [EffectType]  NVARCHAR (100)  NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProductID] ASC)
);

