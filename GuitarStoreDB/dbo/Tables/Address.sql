CREATE TABLE [dbo].[Address] (
    [AddressId] INT            IDENTITY (1, 1) NOT NULL,
    [Address1]  NVARCHAR (100) NOT NULL,
    [Address2]  NVARCHAR (100) NULL,
    [City]      NVARCHAR (100) NOT NULL,
    [State]     NVARCHAR (2)   NOT NULL,
    [Zip]       NVARCHAR (5)   NOT NULL,
    CONSTRAINT [pk_Address] PRIMARY KEY CLUSTERED ([AddressId] ASC)
);

