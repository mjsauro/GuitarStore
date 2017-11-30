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

INSERT INTO [dbo].[Manufacturer] ([Name]) VALUES (N'Boss')
INSERT INTO [dbo].[Manufacturer] ([Name]) VALUES (N'Dunlop')
INSERT INTO [dbo].[Manufacturer] ([Name]) VALUES (N'Fender')
INSERT INTO [dbo].[Manufacturer] ([Name]) VALUES (N'Gibson')
INSERT INTO [dbo].[Manufacturer] ([Name]) VALUES (N'Ibanez')
INSERT INTO [dbo].[Manufacturer] ([Name]) VALUES (N'Marshall')
INSERT INTO [dbo].[Manufacturer] ([Name]) VALUES (N'Vox')


INSERT INTO [dbo].[Make] ([Name], [ManufacturerName]) VALUES (N'Boss', N'Boss')
INSERT INTO [dbo].[Make] ([Name], [ManufacturerName]) VALUES (N'Dunlop', N'Dunlop')
INSERT INTO [dbo].[Make] ([Name], [ManufacturerName]) VALUES (N'Fender', N'Fender')
INSERT INTO [dbo].[Make] ([Name], [ManufacturerName]) VALUES (N'Gibson', N'Gibson')
INSERT INTO [dbo].[Make] ([Name], [ManufacturerName]) VALUES (N'Ibanez', N'Ibanez')
INSERT INTO [dbo].[Make] ([Name], [ManufacturerName]) VALUES (N'Marshall', N'Marshall')
INSERT INTO [dbo].[Make] ([Name], [ManufacturerName]) VALUES (N'Vox', N'Vox')


INSERT INTO [dbo].[ProductType] ([Name]) VALUES (N'Amplifier')
INSERT INTO [dbo].[ProductType] ([Name]) VALUES (N'Effect')
INSERT INTO [dbo].[ProductType] ([Name]) VALUES (N'Guitar')

INSERT INTO [dbo].[ProductTypeProperty] ([ProductTypeName], [Property]) VALUES (N'Amplifier', N'Size')
INSERT INTO [dbo].[ProductTypeProperty] ([ProductTypeName], [Property]) VALUES (N'Amplifier', N'Watts')
INSERT INTO [dbo].[ProductTypeProperty] ([ProductTypeName], [Property]) VALUES (N'Effect', N'Effect Type')
INSERT INTO [dbo].[ProductTypeProperty] ([ProductTypeName], [Property]) VALUES (N'Guitar', N'Color')
INSERT INTO [dbo].[ProductTypeProperty] ([ProductTypeName], [Property]) VALUES (N'Guitar', N'PickupStyle')


SET IDENTITY_INSERT [dbo].[Products] ON
INSERT INTO [dbo].[Products] ([ID], [ProductTypeName], [MakeName], [Mod], [Description], [Image], [Price], [DateCreated], [DateModified], [Quantity]) VALUES (1, N'Guitar', N'Fender', N'Stratocaster', N'A true classic from the 1950s.', N'/images/strat.jpg', 599.0000, '20171114 14:10:51.190', '20171114 14:10:51.190', NULL)
INSERT INTO [dbo].[Products] ([ID], [ProductTypeName], [MakeName], [Mod], [Description], [Image], [Price], [DateCreated], [DateModified], [Quantity]) VALUES (38, N'Guitar', N'Gibson', N'Les Paul', N'A perfect balance of clean and distorted tones.', N'/images/lespaul.jpg', 699.0000, '20171114 15:01:27.023', '20171114 15:01:27.023', NULL)
INSERT INTO [dbo].[Products] ([ID], [ProductTypeName], [MakeName], [Mod], [Description], [Image], [Price], [DateCreated], [DateModified], [Quantity]) VALUES (39, N'Guitar', N'Gibson', N'Explorer', N'Featuring two humbucking pickups with a serious crunch.', N'/images/explorer.jpg', 499.0000, '20171114 15:01:47.350', '20171114 15:01:47.350', NULL)
INSERT INTO [dbo].[Products] ([ID], [ProductTypeName], [MakeName], [Mod], [Description], [Image], [Price], [DateCreated], [DateModified], [Quantity]) VALUES (40, N'Amplifier', N'Marshall', N'JCM2000', N'Trademark Marshall sounds.', N'/images/marshall.jpg', 199.0000, '20171114 15:02:00.220', '20171114 15:02:00.220', NULL)
INSERT INTO [dbo].[Products] ([ID], [ProductTypeName], [MakeName], [Mod], [Description], [Image], [Price], [DateCreated], [DateModified], [Quantity]) VALUES (41, N'Amplifier', N'Fender', N'Twinspeaker', N'Packs a double punch!', N'/images/fender.jpg', 349.0000, '20171114 15:02:15.333', '20171114 15:02:15.333', NULL)
INSERT INTO [dbo].[Products] ([ID], [ProductTypeName], [MakeName], [Mod], [Description], [Image], [Price], [DateCreated], [DateModified], [Quantity]) VALUES (42, N'Amplifier', N'Vox', N'Valvetronix', N'Tube emulation at an affordable price.', N'/images/vox.jpg', 499.0000, '20171114 15:02:23.690', '20171114 15:02:23.690', NULL)
INSERT INTO [dbo].[Products] ([ID], [ProductTypeName], [MakeName], [Mod], [Description], [Image], [Price], [DateCreated], [DateModified], [Quantity]) VALUES (44, N'Effect', N'Dunlop', N'CryBaby Wah', N'Add great wah effects to your guitar solos.', N'/images/wah.jpg', 79.0000, '20171114 15:04:05.527', '20171114 15:04:05.527', NULL)
INSERT INTO [dbo].[Products] ([ID], [ProductTypeName], [MakeName], [Mod], [Description], [Image], [Price], [DateCreated], [DateModified], [Quantity]) VALUES (45, N'Effect', N'Ibanez', N'Tube Screamer', N'Make your tone soar!', N'/images/tubescreamer.jpg', 129.0000, '20171114 15:04:20.293', '20171114 15:04:20.293', NULL)
INSERT INTO [dbo].[Products] ([ID], [ProductTypeName], [MakeName], [Mod], [Description], [Image], [Price], [DateCreated], [DateModified], [Quantity]) VALUES (54, N'Effect', N'Boss', N'DS-1', N'Classic distortion sound.', N'/images/distortionpedal.jpg', 39.0000, '20171114 15:11:47.163', '20171114 15:11:47.163', NULL)
SET IDENTITY_INSERT [dbo].[Products] OFF

INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Color', 1, N'Guitar', N'White')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Color', 38, N'Guitar', N'Sunburst')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Color', 39, N'Guitar', N'Black')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Effect Type', 44, N'Effect', N'Wah')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Effect Type', 45, N'Effect', N'Overdrive')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Effect Type', 54, N'Effect', N'Distortion')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'PickupStyle', 1, N'Guitar', N'Single Coil')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Size', 40, N'Amplifier', N'24 x 24')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Size', 41, N'Amplifier', N'10 x 10')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Size', 42, N'Amplifier', N'15 x 15')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Watts', 40, N'Amplifier', N'100')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Watts', 41, N'Amplifier', N'50')
INSERT INTO [dbo].[ProductTypePropertyValues] ([Property], [ProductID], [ProductTypeName], [Value]) VALUES (N'Watts', 42, N'Amplifier', N'75')
INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'ca28b9a5-b650-4ddd-8921-3685e396b036', N'mjsauro@gmail.com', 1, N'AIwNrhwJ3LUj4np7MUS6RO4cwdR2XJmYTFoJDs8ZAsNOA5ahjjGY98lLfsrNxp410w==', N'587d12a1-deb6-4a40-a5de-fb66a925594c', NULL, 0, 0, NULL, 0, 0, N'mjsauro@gmail.com')
