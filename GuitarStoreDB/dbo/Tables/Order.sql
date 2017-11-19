CREATE TABLE [dbo].[Order] (
    [ID]                  INT             IDENTITY (1, 1) NOT NULL,
    [TrackingNumber]      CHAR (8)        DEFAULT (left(newid(),(8))) NOT NULL,
    [Email]               NVARCHAR (100)  NOT NULL,
    [PurchaserName]       NVARCHAR (512)  NOT NULL,
    [ShippingAddress1]    NVARCHAR (1000) NOT NULL,
    [ShippingAddress2]    NVARCHAR (1000) NULL,
    [ShippingCity]        NVARCHAR (1000) NOT NULL,
    [ShippingState]       NVARCHAR (100)  NOT NULL,
    [ShippingPostalCode]  NVARCHAR (10)   NOT NULL,
    [SubTotal]            MONEY           NOT NULL,
    [ShippingAndHandling] MONEY           NOT NULL,
    [Tax]                 MONEY           NOT NULL,
    [DateCreated]         DATETIME        DEFAULT (getdate()) NOT NULL,
    [DateModified]        DATETIME        DEFAULT (getdate()) NOT NULL,
    [ShipDate]            DATETIME        NULL,
    CONSTRAINT [pk_Order] PRIMARY KEY CLUSTERED ([ID] ASC)
);


