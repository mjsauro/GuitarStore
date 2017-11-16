CREATE TABLE [dbo].[Order]
(
	[ID] INT IDENTITY NOT NULL,
	[TrackingNumber] CHAR(8) NOT NULL DEFAULT left(newid(), 8),
	[Email] NVARCHAR(100) NOT NULL,
	[PurchaserName] NVARCHAR (512) NOT NULL,
	[ShippingAddress1] NVARCHAR (1000) NOT NULL,
	[ShippingAddress2] NVARCHAR (1000) NULL,
	[ShippingCity] NVARCHAR (1000) NOT NULL,
	[ShippingState] NVARCHAR (100) NOT NULL,
	[ShippingPostalCode] NVARCHAR (10) NOT NULL,
	[SubTotal] MONEY NOT NULL,
	[ShippingAndHandling] MONEY NOT NULL,
	[Tax] MONEY NOT NULL,
	[DateCreated] DATETIME DEFAULT GetDate() NOT NULL,
	[DateModified] DATETIME DEFAULT GetDate() NOT NULL,
	[ShipDate] DATETIME NULL,

	CONSTRAINT pk_Order PRIMARY KEY ([ID])

)
