CREATE TABLE [dbo].[Make]
(
	[Name] nvarchar(100) NOT NULL PRIMARY KEY,
	[ManufacturerName] NVARCHAR (100) NOT NULL, 
    CONSTRAINT [FK_Make_Manufacturer] FOREIGN KEY (ManufacturerName) REFERENCES Manufacturer([Name])
)
