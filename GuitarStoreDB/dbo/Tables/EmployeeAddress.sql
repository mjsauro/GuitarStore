CREATE TABLE [dbo].[EmployeeAddress] (
    [EmpID]     INT            IDENTITY (1, 1) NOT NULL,
    [AddressID] INT            NOT NULL,
    [Address1]  NVARCHAR (100) NULL,
    [Address2]  NVARCHAR (100) NULL,
    [City]      NVARCHAR (100) NULL,
    [State]     NVARCHAR (2)   NULL,
    [Zip]       CHAR (5)       NULL,
    CONSTRAINT [PK_EmployeeAddress] PRIMARY KEY CLUSTERED ([EmpID] ASC, [AddressID] ASC),
    CONSTRAINT [FK_Employee_Address] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([AddressId]),
    CONSTRAINT [FK_EpmloyeeAddress_EmpID] FOREIGN KEY ([EmpID]) REFERENCES [dbo].[Employee] ([EmpID])
);

