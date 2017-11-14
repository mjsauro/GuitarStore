CREATE TABLE [dbo].[Customer] (
    [CustId]    INT            IDENTITY (1, 1) NOT NULL,
    [FirstName] NVARCHAR (100) NULL,
    [LastName]  NVARCHAR (100) NULL,
    [Email]     NVARCHAR (100) NULL,
    CONSTRAINT [pk_Customer] PRIMARY KEY CLUSTERED ([CustId] ASC)
);

