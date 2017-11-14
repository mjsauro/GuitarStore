CREATE TABLE [dbo].[ProductTypeProperty]
(
	[ProductTypeName] nvarchar(128) NOT NULL,
	[Property] nvarchar(128) NOT NULL, 
    CONSTRAINT [FK_ProductTypeProperty_ProductType] FOREIGN KEY (ProductTypeName) REFERENCES ProductType(Name), 
    CONSTRAINT [PK_ProductTypeProperty] PRIMARY KEY ([ProductTypeName], [Property])
)
