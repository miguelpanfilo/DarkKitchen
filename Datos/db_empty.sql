-- =============================================
-- Dark Kitchen - Base de datos vacía (solo datos iniciales)
-- Ejecutar DESPUÉS de levantar la app con:
--   docker compose up --build
-- =============================================

USE DarkKitchenDb;

-- =============================================
-- DROP tablas (orden inverso a FK)
-- =============================================
DROP TABLE IF EXISTS [OrderItems];
DROP TABLE IF EXISTS [PromotionProducts];
DROP TABLE IF EXISTS [ProductPhotos];
DROP TABLE IF EXISTS [Orders];
DROP TABLE IF EXISTS [Sessions];
DROP TABLE IF EXISTS [AuditLogs];
DROP TABLE IF EXISTS [ShippingTypes];
DROP TABLE IF EXISTS [Products];
DROP TABLE IF EXISTS [Promotions];
DROP TABLE IF EXISTS [Users];
DROP TABLE IF EXISTS [__EFMigrationsHistory];

-- =============================================
-- CREATE tablas
-- =============================================
CREATE TABLE [AuditLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DateTime] datetime2 NOT NULL,
    [EntityName] nvarchar(50) NOT NULL,
    [EntityId] int NOT NULL,
    [ChangeDescription] nvarchar(500) NOT NULL,
    [ResponsibleUser] nvarchar(120) NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);

CREATE TABLE [Products] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [CommercialLine] nvarchar(MAX) NOT NULL,
    [Category] nvarchar(MAX) NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);

CREATE TABLE [Promotions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(MAX) NOT NULL,
    [DiscountPercentage] decimal(5,2) NOT NULL,
    [StartDate] date NOT NULL,
    [EndDate] date NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Promotions] PRIMARY KEY ([Id])
);

CREATE TABLE [Sessions] (
    [SessionId] uniqueidentifier NOT NULL,
    [UserId] int NOT NULL,
    [ExpireDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY ([SessionId])
);

CREATE TABLE [ShippingTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(MAX) NOT NULL,
    [Cost] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ShippingTypes] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(25) NOT NULL,
    [LastName] nvarchar(25) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [Phone] nvarchar(MAX) NOT NULL,
    [PasswordHash] nvarchar(MAX) NOT NULL,
    [Role] nvarchar(MAX) NOT NULL,
    [Status] nvarchar(MAX) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);

CREATE TABLE [Orders] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OrderNumber] nvarchar(50) NOT NULL,
    [ClientId] int NOT NULL,
    [DeliveryType] nvarchar(100) NOT NULL,
    [Street] nvarchar(MAX) NOT NULL,
    [DoorNumber] nvarchar(MAX) NOT NULL,
    [Apartment] nvarchar(MAX) NULL,
    [Status] nvarchar(MAX) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [StatusUpdatedAt] datetime2 NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [ShippingCost] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Users_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Users]([Id])
);

CREATE TABLE [ProductPhotos] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Path] nvarchar(MAX) NOT NULL,
    [ProductId] int NOT NULL,
    CONSTRAINT [PK_ProductPhotos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductPhotos_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]) ON DELETE CASCADE
);

CREATE TABLE [PromotionProducts] (
    [PromotionId] int NOT NULL,
    [ProductId] int NOT NULL,
    CONSTRAINT [PK_PromotionProducts] PRIMARY KEY ([PromotionId], [ProductId]),
    CONSTRAINT [FK_PromotionProducts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PromotionProducts_Promotions_PromotionId] FOREIGN KEY ([PromotionId]) REFERENCES [Promotions]([Id]) ON DELETE CASCADE
);

CREATE TABLE [OrderItems] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [DiscountApplied] decimal(5,4) NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id])
);

-- Índices
CREATE UNIQUE INDEX [IX_Users_Email]          ON [Users]             ([Email]);
CREATE INDEX        [IX_Orders_ClientId]      ON [Orders]            ([ClientId]);
CREATE UNIQUE INDEX [IX_Orders_OrderNumber]   ON [Orders]            ([OrderNumber]);
CREATE INDEX        [IX_ProductPhotos_ProductId]    ON [ProductPhotos]   ([ProductId]);
CREATE INDEX        [IX_PromotionProducts_ProductId] ON [PromotionProducts] ([ProductId]);
CREATE INDEX        [IX_OrderItems_OrderId]   ON [OrderItems]        ([OrderId]);
CREATE INDEX        [IX_OrderItems_ProductId] ON [OrderItems]        ([ProductId]);

-- Marcar migración EF como aplicada
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530002050_InitialCreate', N'8.0.0');

-- Admin
INSERT INTO [Users] (Name, LastName, Email, Phone, PasswordHash, Role, Status)
VALUES (N'Admin', N'System', N'admin@gmail.com', N'099123456', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Administrative', N'Active');

-- Tipos de envío
INSERT INTO [ShippingTypes] (Name, Cost)
VALUES
(N'Express',                  100.00),
(N'Envio dentro de 24 horas',  50.00);
