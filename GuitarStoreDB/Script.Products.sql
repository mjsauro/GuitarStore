/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--INSERT INTO ProductType(Name) VALUES ('Amplifier'), ('Effect')

--INSERT INTO Manufacturer(Name) VALUES ('Gibson'), ('Marshall'), ('Vox'), ('Dunlop'), ('Boss'), ('Ibanez')

----INSERT INTO Make(Name, ManufacturerName) VALUES ('Boss', 'Boss')

----INSERT INTO Products (ProductTypeName, [MakeName], [Model], [Description], [Image], Price ) VALUES  
----('Effect', 'Boss', 'DS-1', 'Classic distortion sound.', '/images/distortionpedal.jpg', 39.00)

--DECLARE @guitarId int
--SET @guitarId = (SELECT TOP(1) ID FROM Products WHERE Model = 'Tube Screamer')


----INSERT INTO ProductTypeProperty(ProductTypeName, Property) VALUES
----('Amplifier', 'Watts'), ('Amplifier', 'Size'), ('Effect', 'Effect Type')

--INSERT INTO ProductTypePropertyValues(ProductID, ProductTypeName, Property, Value) VALUES
--(@guitarId, 'Effect', 'Effect Type', 'Overdrive')

--SELECT * FROM Products

--SELECT * FROM ProductType

--SELECT * FROM ProductTypeProperty