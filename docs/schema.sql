CREATE TABLE [dbo].[Product]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
    [ProductName] NVARCHAR(255) NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [CreatedOn] DATETIME2 NOT NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedOn] DATETIME2 NULL
);
GO
CREATE INDEX IX_Product_ProductName ON [dbo].[Product]([ProductName]);
GO
CREATE TABLE [dbo].[Item]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
    [ProductId] INT NOT NULL,
    [Quantity] INT NOT NULL,
    CONSTRAINT FK_Item_Product FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product]([Id]) ON DELETE CASCADE
);
GO
CREATE INDEX IX_Item_ProductId ON [dbo].[Item]([ProductId]);
