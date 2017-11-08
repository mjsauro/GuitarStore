CREATE TABLE [dbo].[Student] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [FirstName] NVARCHAR (100) NULL,
    [LastName]  NVARCHAR (100) NULL,
    CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED ([ID] ASC)
);

