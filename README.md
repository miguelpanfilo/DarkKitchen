# 255968_307265_329136

# DarkKitchen - Entrega 1

API desarrollada en ASP.NET Core para la gestión de una Dark Kitchen.

## Contenido del release

El archivo `Release.zip` incluye:

- API publicada en modo **Release**
- Archivo `docker-compose.yml` para levantar la base de datos
- Archivo de ejecucion (`.exe`)
- Configuración lista para conexión a SQL Server en Docker

## Requisitos

- Docker instalado
- .NET 8 

## Ejecución

### 1. Descomprimir el archivo

Descomprimir Entrega 1

### 2. Levantar la base de datos

Host=localhost
Database=DarkKitchenDb
User Id=sa
Password=Passw1rd
TrustServerCertificate=true

1) Abrir una terminal parado en la carpeta donde está el docker-compose.yml y ejecutar: docker compose up -d
2) En DBeaver apretar Crear nueva conexion a base de datos
3) Seleccionar Azure SQL Server
4) Host = localhost, Nombre de usuario = sa, Constraseña = Passw1rd,
5) Seleccionar Show All Database, Show All Schemas y Trust Server Certificate
6) Probar conexion
7) Finalizar
8) En master hacer click derecho -> crear -> Database
9) El nombre de la base de datos es DarkKitchenDb
10) Click derecho en master refrescar
11) En DarkKitchenDb hacer click derecho - > Editor SQL -> Nuevo Script SQL
12) Copiar los scripts que estan a continuacion y ejecutarlo ( 1: usuarios 2: productos: 3: promociones 4: ordenes) 
13) Click derecho en DarkKitchenDb -> refrescar
14) En la carpeta publish del Release abir el ejecutable llamado DarkKitchenWebApi.exe
17) Ingresar a Postman y a divertirse realizando requests :)

## SCRIPT DE TABLAS 

##  INICIO TABLAS  ##

USE DarkKitchenDb;

-- =========================
-- Products
-- =========================
CREATE TABLE dbo.Products (
Id int IDENTITY(1,1) NOT NULL,
Code nvarchar(40) NOT NULL,
Name nvarchar(100) NOT NULL,
Description nvarchar(1000) NOT NULL,
CommercialLine nvarchar(MAX) NOT NULL,
Category nvarchar(MAX) NOT NULL,
IsActive bit NOT NULL,
IsDeleted bit NOT NULL,
Price decimal(18,2) NOT NULL,
CONSTRAINT PK_Products PRIMARY KEY (Id)
);

-- =========================
-- Promotions
-- =========================
CREATE TABLE dbo.Promotions (
Id int IDENTITY(1,1) NOT NULL,
Name nvarchar(MAX) NOT NULL,
DiscountPercentage decimal(5,2) NOT NULL,
StartDate date NOT NULL,
EndDate date NOT NULL,
IsDeleted bit NOT NULL,
CONSTRAINT PK_Promotions PRIMARY KEY (Id)
);

-- =========================
-- Users
-- =========================
CREATE TABLE dbo.Users (
Id int IDENTITY(1,1) NOT NULL,
Name nvarchar(50) NOT NULL,
LastName nvarchar(50) NOT NULL,
Email nvarchar(900) NOT NULL,
Phone nvarchar(MAX) NOT NULL,
PasswordHash nvarchar(MAX) NOT NULL,
[Role] nvarchar(MAX) NOT NULL,
Status nvarchar(MAX) NOT NULL,
CONSTRAINT PK_Users PRIMARY KEY (Id)
);

CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email
ON dbo.Users (Email ASC);

-- =========================
-- Sessions
-- =========================
CREATE TABLE dbo.Sessions (
SessionId uniqueidentifier NOT NULL,
UserId int NOT NULL,
ExpireDate datetime2 NOT NULL,
CONSTRAINT PK_Sessions PRIMARY KEY (SessionId)
);

-- =========================
-- EF Migrations History
-- =========================
CREATE TABLE dbo.__EFMigrationsHistory (
MigrationId nvarchar(150) NOT NULL,
ProductVersion nvarchar(32) NOT NULL,
CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
);

-- =========================
-- Orders
-- =========================
CREATE TABLE dbo.Orders (
Id int IDENTITY(1,1) NOT NULL,
OrderNumber nvarchar(100) NOT NULL,
ClientId int NOT NULL,
DeliveryType nvarchar(MAX) NOT NULL,
Street nvarchar(MAX) NOT NULL,
DoorNumber nvarchar(MAX) NOT NULL,
Apartment nvarchar(MAX) NULL,
Status nvarchar(MAX) NOT NULL,
CreatedAt datetime2 NOT NULL,
StatusUpdatedAt datetime2 NOT NULL,
Subtotal decimal(18,2) NOT NULL,
ShippingCost decimal(18,2) NOT NULL,
Total decimal(18,2) NOT NULL,
CONSTRAINT PK_Orders PRIMARY KEY (Id),
CONSTRAINT FK_Orders_Users_ClientId FOREIGN KEY (ClientId) REFERENCES dbo.Users(Id)
);

CREATE NONCLUSTERED INDEX IX_Orders_ClientId
ON dbo.Orders (ClientId ASC);

CREATE UNIQUE NONCLUSTERED INDEX IX_Orders_OrderNumber
ON dbo.Orders (OrderNumber ASC);

-- =========================
-- ProductPhotos
-- =========================
CREATE TABLE dbo.ProductPhotos (
Id int IDENTITY(1,1) NOT NULL,
[Path] nvarchar(MAX) NOT NULL,
ProductId int NOT NULL,
CONSTRAINT PK_ProductPhotos PRIMARY KEY (Id),
CONSTRAINT FK_ProductPhotos_Products_ProductId FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX IX_ProductPhotos_ProductId
ON dbo.ProductPhotos (ProductId ASC);

-- =========================
-- PromotionProducts
-- =========================
CREATE TABLE dbo.PromotionProducts (
PromotionId int NOT NULL,
ProductId int NOT NULL,
CONSTRAINT PK_PromotionProducts PRIMARY KEY (PromotionId, ProductId),
CONSTRAINT FK_PromotionProducts_Products_ProductId FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id) ON DELETE
CASCADE,
CONSTRAINT FK_PromotionProducts_Promotions_PromotionId FOREIGN KEY (PromotionId) REFERENCES dbo.Promotions(Id) ON DELETE
CASCADE
);

CREATE NONCLUSTERED INDEX IX_PromotionProducts_ProductId
ON dbo.PromotionProducts (ProductId ASC);

-- =========================
-- OrderItems
-- =========================
CREATE TABLE dbo.OrderItems (
Id int IDENTITY(1,1) NOT NULL,
OrderId int NOT NULL,
ProductId int NOT NULL,
Quantity int NOT NULL,
UnitPrice decimal(18,2) NOT NULL,
DiscountApplied decimal(5,4) NOT NULL,
CONSTRAINT PK_OrderItems PRIMARY KEY (Id),
CONSTRAINT FK_OrderItems_Orders_OrderId FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id) ON DELETE CASCADE,
CONSTRAINT FK_OrderItems_Products_ProductId FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
);

CREATE NONCLUSTERED INDEX IX_OrderItems_OrderId
ON dbo.OrderItems (OrderId ASC);

CREATE NONCLUSTERED INDEX IX_OrderItems_ProductId
ON dbo.OrderItems (ProductId ASC);

-- =========================
-- Mark EF migration as applied
-- =========================
INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20260426024753_InitialCreate', '8.0.0');


##   FIN TABLAS   ##

## DATOS PARA INRGESAR A LA BASE DE DATOS

## SCRIPT DE USUARIOS

INSERT INTO DarkKitchenDb.dbo.Users
(Name, LastName, Email, Phone, PasswordHash, Role, Status)
VALUES

-- ADMIN
(N'Admin', N'System', N'admin@gmail.com', N'099123456', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Administrative',
N'Active'),

-- CLIENTES
(N'Lucas', N'Gomez', N'lucas.gomez@test.com', N'099111222', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Client', N'Active'),
(N'Sofia', N'Rodriguez', N'sofia.rodriguez@test.com', N'097555666', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Client',
N'Active'),
(N'Juan', N'Perez', N'juan.perez@test.com', N'098123456', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Client', N'Active'),
(N'Marcos', N'Silva', N'marcos.silva@test.com', N'098333444', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Client',
N'Active'),

-- PREPARERS
(N'Carlos', N'Cocina', N'preparer1@test.com', N'095111222', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Preparer',
N'Active'),
(N'Ana', N'Chef', N'preparer2@test.com', N'095333444', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Preparer', N'Active'),

-- ADMIN EXTRA
(N'Miguel', N'Admin', N'miguel.admin@test.com', N'094999888', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Administrative',
N'Active');

-- Todas las cuentas usan la misma contraseña:
-- Password: PasswordSeguraee3!

-- ADMIN
-- {
--   "email": "admin@gmail.com",
--   "password": "PasswordSeguraee3!"
-- }

-- CLIENTES
-- {
--   "email": "lucas.gomez@test.com",
--   "password": "PasswordSeguraee3!"
-- }

-- {
--   "email": "sofia.rodriguez@test.com",
--   "password": "PasswordSeguraee3!"
-- }

-- {
--   "email": "juan.perez@test.com",
--   "password": "PasswordSeguraee3!"
-- }

-- {
--   "email": "marcos.silva@test.com",
--   "password": "PasswordSeguraee3!"
-- }

-- PREPARERS
-- {
--   "email": "preparer1@test.com",
--   "password": "PasswordSeguraee3!"
-- }

-- {
--   "email": "preparer2@test.com",
--   "password": "PasswordSeguraee3!"
-- }

-- ADMIN EXTRA
-- {
--   "email": "miguel.admin@test.com",
--   "password": "PasswordSeguraee3!"
-- }

## FIN SCRIPT TABLAS
## SCRIPT DE PRODUCTOS
## INICIO SCRIPT PRODUCTOS

DELETE FROM DarkKitchenDb.dbo.PromotionProducts
WHERE ProductId IN (
SELECT Id FROM DarkKitchenDb.dbo.Products
WHERE Code IN ('PIZZ02','PIZZ03','PAN02','PAN03','BURG03','BURG04','BEB03','BEB04','POST03','POST04')
);

DELETE FROM DarkKitchenDb.dbo.ProductPhotos
WHERE ProductId IN (
SELECT Id FROM DarkKitchenDb.dbo.Products
WHERE Code IN ('PIZZ02','PIZZ03','PAN02','PAN03','BURG03','BURG04','BEB03','BEB04','POST03','POST04')
);

DELETE FROM DarkKitchenDb.dbo.Products
WHERE Code IN ('PIZZ02','PIZZ03','PAN02','PAN03','BURG03','BURG04','BEB03','BEB04','POST03','POST04');
INSERT INTO DarkKitchenDb.dbo.Products
(Code, Name, Description, CommercialLine, Category, IsActive, IsDeleted, Price)
VALUES

-- PIZZAS
('PIZZ02', 'Pizza Pepperoni', 'Pizza con pepperoni y muzzarella', 'Pizzas', 'Comida', 1, 0, 580.00),
('PIZZ03', 'Pizza Cuatro Quesos', 'Pizza con mezcla de quesos', 'Pizzas', 'Comida', 1, 0, 600.00),

-- COMIDA RAPIDA
('PAN02', 'Pancho con Queso', 'Pancho con queso fundido', 'Comida rápida', 'Comida', 1, 0, 270.00),
('PAN03', 'Pancho Doble', 'Pancho doble con salsas', 'Comida rápida', 'Comida', 1, 0, 320.00),

-- HAMBURGUESAS
('BURG03', 'Hamburguesa BBQ', 'Hamburguesa con salsa barbacoa', 'Comida rápida', 'Comida', 1, 0, 480.00),
('BURG04', 'Hamburguesa Completa', 'Hamburguesa con lechuga, tomate y huevo', 'Comida rápida', 'Comida', 1, 0, 520.00),

-- BEBIDAS
('BEB03', 'Refresco Naranja', 'Gaseosa sabor naranja 500ml', 'Bebidas', 'Bebida', 1, 0, 120.00),
('BEB04', 'Cerveza Lata', 'Cerveza lager 473ml', 'Bebidas', 'Bebida', 1, 0, 180.00),

-- POSTRES
('POST03', 'Helado 1 Bocha', 'Helado artesanal un sabor', 'Postres', 'Comida', 1, 0, 150.00),
('POST04', 'Torta de Chocolate', 'Porción de torta de chocolate', 'Postres', 'Comida', 1, 0, 280.00);

INSERT INTO DarkKitchenDb.dbo.ProductPhotos (Path, ProductId)
SELECT 'images/pizza_pepperoni.jpg', Id FROM DarkKitchenDb.dbo.Products WHERE Code = 'PIZZ02';

INSERT INTO DarkKitchenDb.dbo.ProductPhotos (Path, ProductId)
SELECT 'images/pizza_4quesos.jpg', Id FROM DarkKitchenDb.dbo.Products WHERE Code = 'PIZZ03';

INSERT INTO DarkKitchenDb.dbo.ProductPhotos (Path, ProductId)
SELECT 'images/pancho_queso.jpg', Id FROM DarkKitchenDb.dbo.Products WHERE Code = 'PAN02';

INSERT INTO DarkKitchenDb.dbo.ProductPhotos (Path, ProductId)
SELECT 'images/pancho_doble.jpg', Id FROM DarkKitchenDb.dbo.Products WHERE Code = 'PAN03';

INSERT INTO DarkKitchenDb.dbo.ProductPhotos (Path, ProductId)
SELECT 'images/burger_bbq.jpg', Id FROM DarkKitchenDb.dbo.Products WHERE Code = 'BURG03';

INSERT INTO DarkKitchenDb.dbo.ProductPhotos (Path, ProductId)
SELECT 'images/burger_completa.jpg', Id FROM DarkKitchenDb.dbo.Products WHERE Code = 'BURG04';

INSERT INTO DarkKitchenDb.dbo.ProductPhotos (Path, ProductId)
SELECT 'images/refresco_naranja.jpg', Id FROM DarkKitchenDb.dbo.Products WHERE Code = 'BEB03';

INSERT INTO DarkKitchenDb.dbo.ProductPhotos (Path, ProductId)
SELECT 'images/cerveza.jpg', Id FROM DarkKitchenDb.dbo.Products WHERE Code = 'BEB04';

INSERT INTO DarkKitchenDb.dbo.ProductPhotos (Path, ProductId)
SELECT 'images/helado.jpg', Id FROM DarkKitchenDb.dbo.Products WHERE Code = 'POST03';

INSERT INTO DarkKitchenDb.dbo.ProductPhotos (Path, ProductId)
SELECT 'images/torta_chocolate.jpg', Id FROM DarkKitchenDb.dbo.Products WHERE Code = 'POST04';

## FIN SCRIPT PRODUCTOS
## SCRIPT DE PROMOCIONES
## INICIO SCRIPT PROMOCIONES

INSERT INTO DarkKitchenDb.dbo.Promotions
(Name, DiscountPercentage, StartDate, EndDate, IsDeleted)
VALUES
('Promo Pizzas', 15.00, '2026-04-01', '2026-06-30', 0),
('Promo Comida Rapida', 10.00, '2026-04-01', '2026-06-30', 0),
('Promo Bebidas', 20.00, '2026-04-01', '2026-06-30', 0),
('Promo Postres', 12.00, '2026-04-01', '2026-06-30', 0);

INSERT INTO DarkKitchenDb.dbo.Promotions
(Name, DiscountPercentage, StartDate, EndDate, IsDeleted)
VALUES
('Promo Expirada Test', 10.00, '2026-01-01', '2026-01-10', 0);

INSERT INTO DarkKitchenDb.dbo.Promotions
(Name, DiscountPercentage, StartDate, EndDate, IsDeleted)
VALUES
('Promo Test Asociaciones', 18.00, '2026-04-01', '2026-12-31', 0);

-- Promo Pizzas
INSERT INTO DarkKitchenDb.dbo.PromotionProducts (PromotionId, ProductId)
SELECT p.Id, pr.Id
FROM DarkKitchenDb.dbo.Promotions p
JOIN DarkKitchenDb.dbo.Products pr ON pr.Code IN ('PIZZ02', 'PIZZ03')
WHERE p.Name = 'Promo Pizzas';

-- Promo Comida Rapida
INSERT INTO DarkKitchenDb.dbo.PromotionProducts (PromotionId, ProductId)
SELECT p.Id, pr.Id
FROM DarkKitchenDb.dbo.Promotions p
JOIN DarkKitchenDb.dbo.Products pr ON pr.Code IN ('PAN02', 'PAN03', 'BURG03', 'BURG04')
WHERE p.Name = 'Promo Comida Rapida';

-- Promo Bebidas
INSERT INTO DarkKitchenDb.dbo.PromotionProducts (PromotionId, ProductId)
SELECT p.Id, pr.Id
FROM DarkKitchenDb.dbo.Promotions p
JOIN DarkKitchenDb.dbo.Products pr ON pr.Code IN ('BEB03', 'BEB04')
WHERE p.Name = 'Promo Bebidas';

-- Promo Postres
INSERT INTO DarkKitchenDb.dbo.PromotionProducts (PromotionId, ProductId)
SELECT p.Id, pr.Id
FROM DarkKitchenDb.dbo.Promotions p
JOIN DarkKitchenDb.dbo.Products pr ON pr.Code IN ('POST03', 'POST04')
WHERE p.Name = 'Promo Postres';

## FIN SCRIPT PROMOCIONES
## SCRIPT DE ORDENES
## INICIO SCRIPT ORDENES 


                     INSERT INTO DarkKitchenDb.dbo.Orders
            (OrderNumber, ClientId, DeliveryType, Street, DoorNumber, Apartment, Status, CreatedAt, StatusUpdatedAt, Subtotal, ShippingCost, Total)
            VALUES

            -- PENDING SIMPLE
            ('ORD-001', 2, 'Express', 'Rivera', '1234', '101', 'Pending', '2026-04-20', '2026-04-20', 493.00, 300.00, 967.46),

            -- PENDING MULTIPLE ITEMS
            ('ORD-002', 2, 'Express', 'Rivera', '9999', '202', 'Pending', '2026-04-25', '2026-04-25', 1261.00, 300.00, 1904.42),

            -- PREPARED
            ('ORD-003', 4, 'Express', 'Bvar Artigas', '3333', '202', 'Prepared', '2026-04-18', '2026-04-18', 432.00, 300.00, 893.04),

            -- ON THE WAY
            ('ORD-004', 5, 'TwentyFourHours', 'Av Italia', '4444', '102', 'OnTheWay', '2026-04-17', '2026-04-17', 468.00, 200.00, 814.96),

            -- DELIVERED
            ('ORD-005', 2, 'Express', 'Garibaldi', '5555', '303', 'Delivered', '2026-04-15', '2026-04-15', 288.00, 300.00, 717.36),

            -- NOT DELIVERED
            ('ORD-006', 3, 'TwentyFourHours', 'Propios', '6666', '303', 'NotDelivered', '2026-04-14', '2026-04-14', 243.00, 200.00, 540.46),

            -- CANCELLED
            ('ORD-007', 4, 'Express', '8 de Octubre', '7777', '401', 'Cancelled', '2026-04-13', '2026-04-13', 132.00, 300.00, 527.04),

            -- PENDING 3
            ('ORD-008', 3, 'Express', 'Luis A de Herrera', '8888', '201', 'Pending', '2026-04-26', '2026-04-26', 640.00, 300.00, 1140.80),

            -- PENDING 4
            ('ORD-009', 5, 'TwentyFourHours', 'Av Brasil', '9999', '402', 'Pending', '2026-04-27', '2026-04-27', 520.00, 200.00, 878.40),

            -- PENDING 5
            ('ORD-010', 2, 'Express', 'Colonia', '1111', '105', 'Pending', '2026-04-28', '2026-04-28', 320.00, 300.00, 756.40);

            INSERT INTO DarkKitchenDb.dbo.Orders
            (OrderNumber, ClientId, DeliveryType, Street, DoorNumber, Apartment, Status, CreatedAt, StatusUpdatedAt, Subtotal, ShippingCost, Total)
            VALUES

            -- ON THE WAY 1
            ('ORD-011', 2, 'Express', 'Av. Brasil', '2222', '301', 'OnTheWay', '2026-04-28', '2026-04-28', 512.00, 300.00, 990.64),

            -- ON THE WAY 2
            ('ORD-012', 3, 'TwentyFourHours', 'Bulevar España', '1234', '202', 'OnTheWay', '2026-04-28', '2026-04-28', 520.00, 200.00, 878.40);



            INSERT INTO DarkKitchenDb.dbo.OrderItems
            (OrderId, ProductId, Quantity, UnitPrice, DiscountApplied)
            VALUES

            -- ORD-001 (simple)
            (1, 1, 1, 580.00, 0.1500),

            -- ORD-002 (multiple items)
            (2, 1, 1, 580.00, 0.1500),  -- pizza
            (2, 7, 2, 120.00, 0.2000),  -- bebidas
            (2, 4, 2, 320.00, 0.1000),  -- panchos


            (3, 5, 1, 480.00, 0.1000),
            (4, 6, 1, 520.00, 0.1000),
            (5, 4, 1, 320.00, 0.1000),
            (6, 3, 1, 270.00, 0.1000),
            (7, 9, 1, 150.00, 0.1200),

            -- ORD-008 (multiple items)
            (8, 1, 1, 580.00, 0.0000),  -- pizza
            (8, 4, 2, 320.00, 0.2000),  -- pancho con promo

            -- ORD-009 (simple)
            (9, 6, 1, 520.00, 0.0000),  -- hamburguesa completa

            -- ORD-010 (simple con promo)
            (10, 4, 1, 320.00, 0.2000); -- pancho con promo

            INSERT INTO DarkKitchenDb.dbo.OrderItems
            (OrderId, ProductId, Quantity, UnitPrice, DiscountApplied)
            VALUES

            -- ORD-011 → usa producto con promo (id 4)
            (11, 4, 2, 320.00, 0.2000),

            -- ORD-012 → sin promo
            (12, 6, 1, 520.00, 0.0000);


## FIN SCRIPT ORDENES

