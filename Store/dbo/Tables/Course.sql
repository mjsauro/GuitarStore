CREATE TABLE [dbo].[Course] (
    [Name]        NVARCHAR (100)  NOT NULL,
    [Description] NVARCHAR (1000) NULL,
    CONSTRAINT [PK_Course] PRIMARY KEY CLUSTERED ([Name] ASC)
);

