-- =============================================
-- Dark Kitchen - Base de datos vacía (solo estructura)
-- =============================================

CREATE DATABASE DarkKitchenDb;
GO

USE DarkKitchenDb;
GO

CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(300) NOT NULL,
    [ProductVersion] nvarchar(64) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

CREATE TABLE [AuditLogs] (
    [Id] int NOT NULL IDENTITY,
    [DateTime] datetime2 NOT NULL,
    [EntityName] nvarchar(100) NOT NULL,
    [EntityId] int NOT NULL,
    [ChangeDescription] nvarchar(1000) NOT NULL,
    [ResponsibleUser] nvarchar(240) NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(40) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [CommercialLine] nvarchar(max) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Promotions] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [DiscountPercentage] decimal(5,2) NOT NULL,
    [StartDate] date NOT NULL,
    [EndDate] date NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Promotions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Sessions] (
    [SessionId] uniqueidentifier NOT NULL,
    [UserId] int NOT NULL,
    [ExpireDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY ([SessionId])
);
GO

CREATE TABLE [ShippingTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Cost] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ShippingTypes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [Email] nvarchar(900) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ProductPhotos] (
    [Id] int NOT NULL IDENTITY,
    [Path] nvarchar(max) NOT NULL,
    [ProductId] int NOT NULL,
    CONSTRAINT [PK_ProductPhotos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductPhotos_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [Products] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PromotionProducts] (
    [PromotionId] int NOT NULL,
    [ProductId] int NOT NULL,
    CONSTRAINT [PK_PromotionProducts] PRIMARY KEY ([PromotionId], [ProductId]),
    CONSTRAINT [FK_PromotionProducts_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [Products] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PromotionProducts_Promotions_PromotionId] FOREIGN KEY ([PromotionId])
        REFERENCES [Promotions] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [OrderNumber] nvarchar(100) NOT NULL,
    [ClientId] int NOT NULL,
    [DeliveryType] nvarchar(200) NOT NULL,
    [Street] nvarchar(max) NOT NULL,
    [DoorNumber] nvarchar(max) NOT NULL,
    [Apartment] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [StatusUpdatedAt] datetime2 NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [ShippingCost] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Users_ClientId] FOREIGN KEY ([ClientId])
        REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [OrderItems] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [DiscountApplied] decimal(5,4) NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId])
        REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [Products] ([Id]) ON DELETE NO ACTION
);
GO

-- Índices
CREATE UNIQUE INDEX [IX_Orders_OrderNumber] ON [Orders] ([OrderNumber]);
CREATE INDEX [IX_Orders_ClientId] ON [Orders] ([ClientId]);
CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);
CREATE INDEX [IX_ProductPhotos_ProductId] ON [ProductPhotos] ([ProductId]);
CREATE INDEX [IX_PromotionProducts_ProductId] ON [PromotionProducts] ([ProductId]);
CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530002050_InitialCreate', N'8.0.0');
GO
