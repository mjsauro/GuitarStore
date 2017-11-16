CREATE TABLE [dbo].[Products] (
    [ID]    INT              IDENTITY (1, 1) NOT NULL,
    [ProductTypeName]  NVARCHAR (128)   NULL,
    [MakeName]         NVARCHAR (100)   NULL,
    [Mod]          NVARCHAR (100)   NULL,
    [Description]  NTEXT            NULL,
    [Image]        NVARCHAR (1000)  NULL,
    [Price]        MONEY            NULL,
    [DateCreated]  DATETIME         DEFAULT (getdate()) NULL,
    [DateModified] DATETIME         DEFAULT (getdate()) NULL,
    [Quantity] INT NULL, 
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT [FK_Products_ProductType] FOREIGN KEY (ProductTypeName) REFERENCES ProductType(Name), 
    CONSTRAINT [FK_Products_Make] FOREIGN KEY ([MakeName]) REFERENCES Make([Name])
);

