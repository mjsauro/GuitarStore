CREATE TABLE [dbo].[OrderProduct]
(
	[OrderID] INT NOT NULL,
	[ProductID] INT NOT NULL,
	
	CONSTRAINT pk_OrderProduct PRIMARY KEY ([ProductID], [OrderID]),
	CONSTRAINT fk_OrderProduct_Order FOREIGN KEY ([OrderID]) REFERENCES [Order]([ID]),
	CONSTRAINT fk_OrderProduct_Product FOREIGN KEY ([ProductID]) REFERENCES [Products](ID),

	[Quantity] INT NOT NULL DEFAULT 1,
	[PlacedName] NVARCHAR (100) NULL,
	[PlacedUnitName] MONEY NOT NULL,
	[DateCreated] DATETIME DEFAULT GetDate(),
	[DateModified] DATETIME DEFAULT GetDate(),
	
)
