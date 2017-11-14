﻿CREATE TABLE [dbo].[Cart]
(
	[ID] UNIQUEIDENTIFIER DEFAULT NewID(),
	[AspNetUserID] NVARCHAR (100) NULL,
	[DateCreated] DATETIME NOT NULL DEFAULT GetDate(),
	[DateModified] DATETIME NOT NULL DEFAULT GetDate(),

	CONSTRAINT pk_Cart PRIMARY KEY ([ID])

)