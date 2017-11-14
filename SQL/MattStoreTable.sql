USE MattsStore

DROP TABLE Store

CREATE TABLE Store
(
	StoreID INT IDENTITY(1,1),
	[Name] NVARCHAR(100),
	CONSTRAINT PK_Store PRIMARY KEY (StoreID)
)

DROP TABLE Customer

CREATE TABLE Customer
(
	CustID INT IDENTITY(1,1),
	[FirstName] NVARCHAR(100),
	[LastName] NVARCHAR(100),
	[Email] NVARCHAR(100),
	CONSTRAINT PK_Customer PRIMARY KEY (CustID)
)

DROP TABLE Employee

CREATE TABLE Employee 
(
	EmpID INT IDENTITY (1,1),
	[FirstName] NVARCHAR(100),
	[LastName] NVARCHAR(100),
	[DOB] DATE,
	[Wage] FLOAT (2),
	CONSTRAINT PK_Employee PRIMARY KEY (EmpID)

)

DROP TABLE [Shift]

CREATE TABLE [Shift]
(
	[Start] TIME,
	[End] TIME,
)

DROP TABLE ProductType

CREATE TABLE ProductType
(
	[ProductName] NVARCHAR(100),
	CONSTRAINT PK_ProductsType PRIMARY KEY([ProductName])
)

DROP TABLE Product


CREATE TABLE Product
(
	[UPC] CHAR(12),
	[Name] NVARCHAR(100),
	[Price] FLOAT(4),

	CONSTRAINT PK_Product PRIMARY KEY([UPC])
)

DROP TABLE Manufacturer

CREATE TABLE Manufacturer
(
	[Name] NVARCHAR(100),

	CONSTRAINT PK_Manufacturer PRIMARY KEY([Name])

)

DROP TABLE [Address]

CREATE TABLE [Address]
(
	[AddressID] INT IDENTITY (1,1),
	[Address1] NVARCHAR(100),
	[Address2] NVARCHAR(100),
	[City] NVARCHAR(100),
	[State] NVARCHAR(2),
	[Zip] CHAR(5)
	CONSTRAINT PK_Address PRIMARY KEY([AddressID])
)

--RelationTables


--Address Relations

DROP TABLE [StoreAddress]

CREATE TABLE [StoreAddress]
(
	StoreID INT IDENTITY(1,1),
	[AddressID] INT,
	[Name] NVARCHAR(100),
	[Address1] NVARCHAR(100),
	[Address2] NVARCHAR(100),
	[City] NVARCHAR(100),
	[State] NVARCHAR(2),
	[Zip] CHAR(5)
	CONSTRAINT PK_StoreAddress PRIMARY KEY (StoreID, [AddressID]),
	CONSTRAINT FK_StoreAddress_StoreID FOREIGN KEY (StoreID) REFERENCES Store(StoreID),
	CONSTRAINT FK_StoreAddress_Address FOREIGN KEY ([AddressID]) REFERENCES [Address]([AddressID])
)

DROP TABLE [EmployeeAddress]

CREATE TABLE [EmployeeAddress]
(
	EmpID INT IDENTITY (1,1),
	[AddressID] INT,
	[Address1] NVARCHAR(100),
	[Address2] NVARCHAR(100),
	[City] NVARCHAR(100),
	[State] NVARCHAR(2),
	[Zip] CHAR(5)
	CONSTRAINT PK_EmployeeAddress PRIMARY KEY (EmpID, [AddressID]),
	CONSTRAINT FK_StoreAddress_EmpID FOREIGN KEY (EmpID) REFERENCES Employee(EmpID),
	CONSTRAINT FK_StoreAddress_Address FOREIGN KEY ([AddressID]) REFERENCES [Address]([AddressID])
)

DROP TABLE [ManufacturerAddress]

CREATE TABLE [ManufacturerAddress]
(
	[Name] NVARCHAR(100),
	[AddressID] INT,
	[Address1] NVARCHAR(100),
	[Address2] NVARCHAR(100),
	[City] NVARCHAR(100),
	[State] NVARCHAR(2),
	[Zip] CHAR(5)
	CONSTRAINT PK_ManufacturerAddress PRIMARY KEY ([Name], [Address1]),
	CONSTRAINT FK_ManufacturerAddress_Name FOREIGN KEY ([Name]) REFERENCES [Manufacturer]([Name]),
	CONSTRAINT FK_ManufacturerAddress_Address FOREIGN KEY ([AddressID]) REFERENCES [Address]([AddressID])
)

--