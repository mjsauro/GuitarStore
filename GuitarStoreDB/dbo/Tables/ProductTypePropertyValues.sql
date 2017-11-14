CREATE TABLE [dbo].[ProductTypePropertyValues]
(
	[ProductID] INT NOT NULL,
	[ProductTypeName] nvarchar(128) NOT NULL,
	[Property] nvarchar(128) NOT NULL, 
    CONSTRAINT [PK_ProductTypePropertyValues] PRIMARY KEY ([Property], [ProductID],ProductTypeName), 
	[Value] NTEXT, 
    CONSTRAINT [FK_ProductTypePropertyValues_Products] FOREIGN KEY (ProductID) REFERENCES Products(ID), 
    CONSTRAINT [FK_ProductTypePropertyValues_ProductTypeProperty] FOREIGN KEY (ProductTypeName, Property) REFERENCES ProductTypeProperty(ProductTypeName, Property)
)
