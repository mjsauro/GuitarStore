CREATE TABLE [dbo].[CustomerAddress] (
    [CustID]    INT            NOT NULL,
    [AddressID] INT            NOT NULL,
    [Address1]  NVARCHAR (100) NULL,
    [Address2]  NVARCHAR (100) NULL,
    [City]      NVARCHAR (100) NULL,
    [State]     NVARCHAR (2)   NULL,
    [Zip]       NVARCHAR (5)   NULL,
    CONSTRAINT [PK_CustomerAddress] PRIMARY KEY CLUSTERED ([CustID] ASC, [AddressID] ASC),
    CONSTRAINT [FK_Customer_Address] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([AddressId]),
    CONSTRAINT [FK_CustomerAddress_EmpID] FOREIGN KEY ([CustID]) REFERENCES [dbo].[Customer] ([CustId])
);

