CREATE TABLE [Customer]
(
	[CustId] INT IDENTITY (1,1),
	[FirstName] NVARCHAR (100),
	[LastName] NVARCHAR (100),
	[Email] NVARCHAR (100),

	CONSTRAINT pk_Customer PRIMARY KEY ([CustID])
)