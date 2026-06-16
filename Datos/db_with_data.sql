-- =============================================
-- Dark Kitchen - Datos de prueba
-- Ejecutar DESPUÉS de levantar la app con:
--   docker compose up --build
-- Todas las cuentas usan contraseña: PasswordSeguraee3!
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
CREATE UNIQUE INDEX [IX_Users_Email]               ON [Users]             ([Email]);
CREATE INDEX        [IX_Orders_ClientId]           ON [Orders]            ([ClientId]);
CREATE UNIQUE INDEX [IX_Orders_OrderNumber]        ON [Orders]            ([OrderNumber]);
CREATE INDEX        [IX_ProductPhotos_ProductId]   ON [ProductPhotos]     ([ProductId]);
CREATE INDEX        [IX_PromotionProducts_ProductId] ON [PromotionProducts] ([ProductId]);
CREATE INDEX        [IX_OrderItems_OrderId]        ON [OrderItems]        ([OrderId]);
CREATE INDEX        [IX_OrderItems_ProductId]      ON [OrderItems]        ([ProductId]);

-- Marcar migración EF como aplicada
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530002050_InitialCreate', N'8.0.0');

-- Usuarios
INSERT INTO [Users] (Name, LastName, Email, Phone, PasswordHash, Role, Status)
VALUES
(N'Admin',  N'System',    N'admin@gmail.com',          N'099123456', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Administrative', N'Active'),
(N'Lucas',  N'Gomez',     N'lucas.gomez@test.com',     N'099111222', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Client',         N'Active'),
(N'Sofia',  N'Rodriguez', N'sofia.rodriguez@test.com', N'097555666', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Client',         N'Active'),
(N'Juan',   N'Perez',     N'juan.perez@test.com',      N'098123456', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Client',         N'Active'),
(N'Marcos', N'Silva',     N'marcos.silva@test.com',    N'098333444', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Client',         N'Active'),
(N'Carlos', N'Cocina',    N'preparer1@test.com',       N'095111222', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Preparer',       N'Active'),
(N'Ana',    N'Chef',      N'preparer2@test.com',       N'095333444', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Preparer',       N'Active'),
(N'Miguel', N'Admin',     N'miguel.admin@test.com',    N'094999888', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Administrative', N'Active');

-- Tipos de envío
INSERT INTO [ShippingTypes] (Name, Cost)
VALUES
(N'Express',                  100.00),
(N'Envio dentro de 24 horas',  50.00);

-- Productos (Id 1-10)
INSERT INTO [Products] (Code, Name, Description, CommercialLine, Category, IsActive, IsDeleted, Price)
VALUES
('PIZZ02', 'Pizza Pepperoni',      'Pizza con pepperoni y muzzarella',        'Pizzas',        'Comida', 1, 0, 580.00),
('PIZZ03', 'Pizza Cuatro Quesos',  'Pizza con mezcla de quesos',              'Pizzas',        'Comida', 1, 0, 600.00),
('PAN02',  'Pancho con Queso',     'Pancho con queso fundido',                'Comida rápida', 'Comida', 1, 0, 270.00),
('PAN03',  'Pancho Doble',         'Pancho doble con salsas',                 'Comida rápida', 'Comida', 1, 0, 320.00),
('BURG03', 'Hamburguesa BBQ',      'Hamburguesa con salsa barbacoa',          'Comida rápida', 'Comida', 1, 0, 480.00),
('BURG04', 'Hamburguesa Completa', 'Hamburguesa con lechuga, tomate y huevo', 'Comida rápida', 'Comida', 1, 0, 520.00),
('BEB03',  'Refresco Naranja',     'Gaseosa sabor naranja 500ml',             'Bebidas',       'Bebida', 1, 0, 120.00),
('BEB04',  'Cerveza Lata',         'Cerveza lager 473ml',                     'Bebidas',       'Bebida', 1, 0, 180.00),
('POST03', 'Helado 1 Bocha',       'Helado artesanal un sabor',               'Postres',       'Comida', 1, 0, 150.00),
('POST04', 'Torta de Chocolate',   'Porción de torta de chocolate',           'Postres',       'Comida', 1, 0, 280.00);

-- Fotos de productos
INSERT INTO [ProductPhotos] (Path, ProductId) SELECT '/photos/pizza1.jpg',    Id FROM [Products] WHERE Code = 'PIZZ02';
INSERT INTO [ProductPhotos] (Path, ProductId) SELECT '/photos/pizza2.jpg',    Id FROM [Products] WHERE Code = 'PIZZ03';
INSERT INTO [ProductPhotos] (Path, ProductId) SELECT '/photos/panqueque.jpg', Id FROM [Products] WHERE Code = 'PAN02';
INSERT INTO [ProductPhotos] (Path, ProductId) SELECT '/photos/panqueue2.jpg', Id FROM [Products] WHERE Code = 'PAN03';
INSERT INTO [ProductPhotos] (Path, ProductId) SELECT '/photos/burga1.jpg',    Id FROM [Products] WHERE Code = 'BURG03';
INSERT INTO [ProductPhotos] (Path, ProductId) SELECT '/photos/burga2.jpg',    Id FROM [Products] WHERE Code = 'BURG04';
INSERT INTO [ProductPhotos] (Path, ProductId) SELECT '/photos/cafe.jpg',      Id FROM [Products] WHERE Code = 'BEB03';
INSERT INTO [ProductPhotos] (Path, ProductId) SELECT '/photos/cafe2.jpg',     Id FROM [Products] WHERE Code = 'BEB04';
INSERT INTO [ProductPhotos] (Path, ProductId) SELECT '/photos/helado.jpg',    Id FROM [Products] WHERE Code = 'POST03';
INSERT INTO [ProductPhotos] (Path, ProductId) SELECT '/photos/postre.jpg',    Id FROM [Products] WHERE Code = 'POST04';

-- Promociones (Id 1-6)
INSERT INTO [Promotions] (Name, DiscountPercentage, StartDate, EndDate, IsDeleted)
VALUES
('Promo Pizzas',            15.00, '2026-04-01', '2026-06-30', 0),
('Promo Comida Rapida',     10.00, '2026-04-01', '2026-06-30', 0),
('Promo Bebidas',           20.00, '2026-04-01', '2026-06-30', 0),
('Promo Postres',           12.00, '2026-04-01', '2026-06-30', 0),
('Promo Expirada Test',     10.00, '2026-01-01', '2026-01-10', 0),
('Promo Test Asociaciones', 18.00, '2026-04-01', '2026-12-31', 0);

-- Productos por promoción
INSERT INTO [PromotionProducts] (PromotionId, ProductId)
SELECT p.Id, pr.Id FROM [Promotions] p JOIN [Products] pr ON pr.Code IN ('PIZZ02','PIZZ03') WHERE p.Name = 'Promo Pizzas';

INSERT INTO [PromotionProducts] (PromotionId, ProductId)
SELECT p.Id, pr.Id FROM [Promotions] p JOIN [Products] pr ON pr.Code IN ('PAN02','PAN03','BURG03','BURG04') WHERE p.Name = 'Promo Comida Rapida';

INSERT INTO [PromotionProducts] (PromotionId, ProductId)
SELECT p.Id, pr.Id FROM [Promotions] p JOIN [Products] pr ON pr.Code IN ('BEB03','BEB04') WHERE p.Name = 'Promo Bebidas';

INSERT INTO [PromotionProducts] (PromotionId, ProductId)
SELECT p.Id, pr.Id FROM [Promotions] p JOIN [Products] pr ON pr.Code IN ('POST03','POST04') WHERE p.Name = 'Promo Postres';

-- Órdenes
INSERT INTO [Orders] (OrderNumber, ClientId, DeliveryType, Street, DoorNumber, Apartment, Status, CreatedAt, StatusUpdatedAt, Subtotal, ShippingCost, Total)
VALUES
('ORD-001', 2, 'Express',                  'Rivera',            '1234', '101', 'Pending',      '2026-04-20', '2026-04-20',  493.00, 100.00,  593.00),
('ORD-002', 2, 'Express',                  'Rivera',            '9999', '202', 'Pending',      '2026-04-25', '2026-04-25', 1261.00, 100.00, 1361.00),
('ORD-003', 4, 'Express',                  'Bvar Artigas',      '3333', '202', 'Prepared',     '2026-04-18', '2026-04-18',  432.00, 100.00,  532.00),
('ORD-004', 5, 'Envio dentro de 24 horas', 'Av Italia',         '4444', '102', 'OnTheWay',     '2026-04-17', '2026-04-17',  468.00,  50.00,  518.00),
('ORD-005', 2, 'Express',                  'Garibaldi',         '5555', '303', 'Delivered',    '2026-04-15', '2026-04-15',  288.00, 100.00,  388.00),
('ORD-006', 3, 'Envio dentro de 24 horas', 'Propios',           '6666', '303', 'NotDelivered', '2026-04-14', '2026-04-14',  243.00,  50.00,  293.00),
('ORD-007', 4, 'Express',                  '8 de Octubre',      '7777', '401', 'Cancelled',    '2026-04-13', '2026-04-13',  132.00, 100.00,  232.00),
('ORD-008', 3, 'Express',                  'Luis A de Herrera', '8888', '201', 'Pending',      '2026-04-26', '2026-04-26',  640.00, 100.00,  740.00),
('ORD-009', 5, 'Envio dentro de 24 horas', 'Av Brasil',         '9999', '402', 'Pending',      '2026-04-27', '2026-04-27',  520.00,  50.00,  570.00),
('ORD-010', 2, 'Express',                  'Colonia',           '1111', '105', 'Pending',      '2026-04-28', '2026-04-28',  320.00, 100.00,  420.00),
('ORD-011', 2, 'Express',                  'Av. Brasil',        '2222', '301', 'OnTheWay',     '2026-04-28', '2026-04-28',  512.00, 100.00,  612.00),
('ORD-012', 3, 'Envio dentro de 24 horas', 'Bulevar España',    '1234', '202', 'OnTheWay',     '2026-04-28', '2026-04-28',  520.00,  50.00,  570.00);

-- Items de órdenes
INSERT INTO [OrderItems] (OrderId, ProductId, Quantity, UnitPrice, DiscountApplied)
VALUES
(1,  1, 1, 580.00, 0.1500),
(2,  1, 1, 580.00, 0.1500),
(2,  7, 2, 120.00, 0.2000),
(2,  4, 2, 320.00, 0.1000),
(3,  5, 1, 480.00, 0.1000),
(4,  6, 1, 520.00, 0.1000),
(5,  4, 1, 320.00, 0.1000),
(6,  3, 1, 270.00, 0.1000),
(7,  9, 1, 150.00, 0.1200),
(8,  1, 1, 580.00, 0.0000),
(8,  4, 2, 320.00, 0.2000),
(9,  6, 1, 520.00, 0.0000),
(10, 4, 1, 320.00, 0.2000),
(11, 4, 2, 320.00, 0.2000),
(12, 6, 1, 520.00, 0.0000);
