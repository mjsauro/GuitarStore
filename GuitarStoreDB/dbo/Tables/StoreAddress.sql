CREATE TABLE [dbo].[StoreAddress] (
    [StoreID]   INT            IDENTITY (1, 1) NOT NULL,
    [AddressID] INT            NOT NULL,
    [Name]      NVARCHAR (100) NULL,
    [Address1]  NVARCHAR (100) NULL,
    [Address2]  NVARCHAR (100) NULL,
    [City]      NVARCHAR (100) NULL,
    [State]     NVARCHAR (2)   NULL,
    [Zip]       CHAR (5)       NULL,
    CONSTRAINT [PK_StoreAddress] PRIMARY KEY CLUSTERED ([StoreID] ASC, [AddressID] ASC),
    CONSTRAINT [FK_StoreAddress_Address] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([AddressId]),
    CONSTRAINT [FK_StoreAddress_StoreID] FOREIGN KEY ([StoreID]) REFERENCES [dbo].[Store] ([StoreID])
);

