USE Store2

CREATE TABLE [Customer]
(
	[CustID] INT IDENTITY (1,1),
	[FirstName] NVARCHAR(100),
	[LastName] NVARCHAR(100),
	[Email] NVARCHAR(100),

	CONSTRAINT PK_Customer PRIMARY KEY ([CustID])
	
)

CREATE TABLE [Store]
(
	[StoreID] INT IDENTITY (1,1),
	[Name] NVARCHAR(100),

	CONSTRAINT PK_Store PRIMARY KEY ([StoreID])
)

CREATE TABLE [Address]
(
	[AddressID] INT IDENTITY(1,1),
	[Address1] NVARCHAR(100),
	[Address2] NVARCHAR(100),
	[City] NVARCHAR(100),
	[State] NVARCHAR(2),
	[Zip] NVARCHAR(5),
	[CustID] INT
	CONSTRAINT PK_Address PRIMARY KEY(AddressID),
	CONSTRAINT FK_CustID FOREIGN KEY (CustID) REFERENCES Customer(CustID),

)

ALTER TABLE [Address]
ADD CONSTRAINT FK_Manufacturer FOREIGN KEY ([ManufacturerName]) REFERENCES Manufacturer([NAME])

ALTER TABLE [Address]
ADD CONSTRAINT MustHaveSingleID
CHECK (
	(CustID IS NOT NULL AND StoreID IS NULL AND EmpID IS NULL AND ManufacturerName IS NULL)
	OR (CustID IS NULL AND StoreID IS NOT NULL AND EmpID IS NULL AND ManufacturerName IS NULL)
	OR (CustID IS NULL AND StoreID IS NULL AND EmpID IS NOT NULL AND ManufacturerName IS NULL)
	OR (CustID IS NULL AND StoreID IS NULL AND EmpID IS NULL AND ManufacturerName IS NOT NULL)
)


SELECT Customer.CustID, Customer.FirstName, Customer.LastName, [Address].City, [Address].[State], [Address].[Zip]
FROM Customer
INNER JOIN [Address] ON Customer.CustID=[Address].CustID

SELECT Store.StoreID, Store.[Name], [Address].[Address1], [Address].Address2, [Address].City, [Address].[State], [Address].[Zip]
FROM Store
INNER JOIN [Address] ON Store.StoreID=[Address].StoreID

SELECT Employee.EmpID, Employee.[FirstName], Employee.[LastName], [Address].[Address1], [Address].Address2, [Address].City, [Address].[State], [Address].[Zip]
FROM Employee
INNER JOIN [Address] ON Employee.EmpID=[Address].EmpID