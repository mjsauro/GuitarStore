CREATE TABLE [dbo].[Product]
(
	[Id] INT IDENTITY NOT NULL,
	[ProductType] NVARCHAR(100),
	[Make] NVARCHAR(100),
	[Mod] NVARCHAR(100),
	[Description] NTEXT,
	[Image] NVARCHAR(100),
	[Price] DECIMAL (10,2),
	[Quatity] INT

	CONSTRAINT PK_Product PRIMARY KEY(ID)
)
