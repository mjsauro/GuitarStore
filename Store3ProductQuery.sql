DROP TABLE Products

CREATE TABLE Products
(
	[ProductID] INT IDENTITY (1,1),
	[ProductType] NVARCHAR (100),
	[Make] NVARCHAR (100),
	[Mod] NVARCHAR (100),
	[Description] NVARCHAR(1000),
	[Image] NVARCHAR(1000),
	[Price] MONEY,
	[Quantity] INT,

	--Guitar
	[Color] NVARCHAR(100),

	--Amps
	[Watts] INT,
	[Size] NVARCHAR(100),

	--Effects
	[EffectType] NVARCHAR(100),

	CONSTRAINT PK_Products PRIMARY KEY ([ProductID])

)

--Guitars
INSERT INTO Products ([ProductType], [Make], [Mod], [Description], [Image], [Price], [Quantity], [Color])
VALUES 
	('Guitar', 'Fender', 'Stratocaster', 'A true classic from the 1950s.', '/images/strat.jpg', 599, 1, 'White'),
	('Guitar', 'Gibson', 'Les Paul', 'A perfect balance of clean and distorted tones.', '/images/lespaul.jpg', 699, 1, 'Sunburst'),
	('Guitar', 'Gibson', 'Explorer', 'Featuring two humbucking pickups with a serious crunch.', '/images/explorer.jpg', 299, 1, 'Black')

INSERT INTO Products ([ProductType], [Make], [Mod], [Description], [Image], [Price], [Quantity], [Watts], [Size])
VALUES
	('Amplifier', 'Marshall', 'JCM2000', 'Trademark Marshall Sound', '/images/marshall.jpg', 199, 1, 100, '24 x 24'),
	('Amplifier', 'Fender', 'Twinspeaker', 'Packs a double punch!', '/images/fender.jpg', 299, 1, 50, '10 x 10'),
	('Amplifier', 'Vox', 'Tube', 'A classic tube amp.', '/images/vox.jpg', 399, 1, 75, '10 x 10')

INSERT INTO Products ([ProductType], [Make], [Mod], [Description], [Image], [Price], [Quantity], [EffectType])
VALUES
	('Effect', 'Boss', 'Metal Zone', 'An effects pedal for bone crunching metal enthusiasts!', '/images/distortionpedal.jpg', 34, 1, 'Distortion Pedal'),
	('Effect', 'Dunlop', 'CryBaby Wah', 'Add great wah effects to your guitar solos.', '/images/wah.jpg', 49, 1, 'Wah Pedal'),
	('Effect', 'Ibanez', 'Tube Screamer', 'Make your tone soar!', '/images/tubescreamer.jpg', 79, 1, 'Overdrive')

SELECT * FROM Products