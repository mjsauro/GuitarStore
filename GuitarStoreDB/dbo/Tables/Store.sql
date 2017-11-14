CREATE TABLE [dbo].[Store] (
    [StoreID] INT            IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (100) NULL,
    CONSTRAINT [pk_Store] PRIMARY KEY CLUSTERED ([StoreID] ASC)
);

