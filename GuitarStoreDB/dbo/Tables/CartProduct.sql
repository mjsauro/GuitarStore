CREATE TABLE [dbo].[CartProduct]
(
	[CartID] UNIQUEIDENTIFIER NOT NULL,
	[ProductID] INT NOT NULL,
	[Quantity] INT NOT NULL DEFAULT 1,
	[DateCreated] DATETIME DEFAULT GetDate() NOT NULL,
	[DateModified] DATETIME DEFAULT GetDate() NOT NULL,

	CONSTRAINT pk_CartProduct PRIMARY KEY ([CartID], [ProductID]),
	CONSTRAINT fk_CartProduct_Cart FOREIGN KEY ([CartID]) REFERENCES Cart([ID]),
	CONSTRAINT [fk_CartProduct_Product] FOREIGN KEY (ProductID) REFERENCES Products(ID)
)
