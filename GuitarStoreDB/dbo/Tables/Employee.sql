CREATE TABLE [dbo].[Employee] (
    [EmpID]     INT            IDENTITY (1, 1) NOT NULL,
    [FirstName] NVARCHAR (100) NOT NULL,
    [LastName]  NVARCHAR (100) NOT NULL,
    [DOB]       DATE           NOT NULL,
    [Wage]      MONEY          NOT NULL,
    CONSTRAINT [pk_Employee] PRIMARY KEY CLUSTERED ([EmpID] ASC)
);

