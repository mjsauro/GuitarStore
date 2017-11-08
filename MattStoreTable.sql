USE MattsStore

CREATE TABLE Store
(
	StoreID INT IDENTITY(1,1),
	[Name] NVARCHAR(100),
	CONSTRAINT PK_Store PRIMARY KEY (StoreID)
)

CREATE TABLE Customer
(
	CustID INT IDENTITY(1,1),
	[FirstName] NVARCHAR(100),
	[LastName] NVARCHAR(100),
	[Email] NVARCHAR(100),
	CONSTRAINT PK_Customer PRIMARY KEY (CustID)
)

CREATE TABLE Employee 
(
	EmpID INT IDENTITY (1,1),
	SupID INT IDENTITY(1,1),
	[FirstName] NVARCHAR(100),
	[LastName] NVARCHAR(100),
	[DOB] DATE,
	[Wage] FLOAT (2),
	CONSTRAINT PK_Employee PRIMARY KEY (EmpID)

)

CREATE TABLE ProductType
(
	[ProductName] NVARCHAR(100),
	CONSTRAINT PK_ProductsType PRIMARY KEY([ProductName])
)
CREATE TABLE [Shift]
(
	[Start] TIME,
	[End] TIME,
)

CREATE TABLE Product
(
	[UPC] INT(12),
	[Name] NVARCHAR(100),
	[Price] FLOAT(4),

	CONSTRAINT PK_Product PRIMARY KEY([UPC])
)

CREATE TABLE Manufacturer
(
	[Name] NVARCHAR(100),

	CONSTRAINT PK_Manufacturer PRIMARY KEY([Name])

)

CREATE TABLE [Address]
(
	[Address1] NVARCHAR(100),
	[Address2] NVARCHAR(100),
	[City] NVARCHAR(100),
	[State] NVARCHAR(2),
	[Zip] INT(5)
)

--RelationTables


--Address Relations
CREATE TABLE [StoreAddress]
(
	StoreID INT IDENTITY(1,1),
	[Name] NVARCHAR(100),
	[Address1] NVARCHAR(100),
	[Address2] NVARCHAR(100),
	[City] NVARCHAR(100),
	[State] NVARCHAR(2),
	[Zip] INT(5)
	CONSTRAINT PK_StoreAddress PRIMARY KEY (StoreID, [Address1]),
	CONSTRAINT FK_StoreAddress_StoreID FOREIGN KEY (StoreID) REFERENCES Store(StoreID),
	CONSTRAINT FK_StoreAddress_Address FOREIGN KEY ([Address1]) REFERENCES [Address]([Address1])
)

CREATE TABLE [EmployeeAddress]
(
	EmpID INT IDENTITY (1,1),
	[Address1] NVARCHAR(100),
	[Address2] NVARCHAR(100),
	[City] NVARCHAR(100),
	[State] NVARCHAR(2),
	[Zip] INT(5)
	CONSTRAINT PK_EmployeeAddress PRIMARY KEY (EmpID, [Address1]),
	CONSTRAINT FK_StoreAddress_EmpID FOREIGN KEY (EmpID) REFERENCES Employee(EmpID),
	CONSTRAINT FK_StoreAddress_Address FOREIGN KEY ([Address1]) REFERENCES [Address]([Address1])
)

CREATE TABLE [ManufacturerAddress]
(
	[Name] NVARCHAR(100),
	[Address1] NVARCHAR(100),
	[Address2] NVARCHAR(100),
	[City] NVARCHAR(100),
	[State] NVARCHAR(2),
	[Zip] INT(5)
	CONSTRAINT PK_ManufacturerAddress PRIMARY KEY ([Name], [Address1]),
	CONSTRAINT FK_ManufacturerAddress_Name FOREIGN KEY ([Name]) REFERENCES [Manufacturer]([Name]),
	CONSTRAINT FK_ManufacturerAddress_Address FOREIGN KEY ([Address1]) REFERENCES [Address]([Address1])
)

--