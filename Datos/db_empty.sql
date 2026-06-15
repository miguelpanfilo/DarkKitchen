-- =============================================
-- Dark Kitchen - Base de datos vacía (solo datos iniciales)
-- Ejecutar DESPUÉS de levantar la app con:
--   docker compose up --build
-- =============================================

USE DarkKitchenDb;

-- Limpieza previa (orden inverso a las FK)
DELETE FROM [OrderItems];
DELETE FROM [Orders];
DELETE FROM [PromotionProducts];
DELETE FROM [Promotions];
DELETE FROM [ProductPhotos];
DELETE FROM [Products];
DELETE FROM [Sessions];
DELETE FROM [AuditLogs];
DELETE FROM [Users];
DELETE FROM [ShippingTypes];

-- Resetear contadores IDENTITY
DBCC CHECKIDENT ('Users',         RESEED, 0);
DBCC CHECKIDENT ('Products',      RESEED, 0);
DBCC CHECKIDENT ('Promotions',    RESEED, 0);
DBCC CHECKIDENT ('Orders',        RESEED, 0);
DBCC CHECKIDENT ('OrderItems',    RESEED, 0);
DBCC CHECKIDENT ('ShippingTypes', RESEED, 0);

-- Admin
INSERT INTO [Users] (Name, LastName, Email, Phone, PasswordHash, Role, Status)
VALUES (N'Admin', N'System', N'admin@gmail.com', N'099123456', N'5A5C0BC1EE170F4A62529B37C551B81F', N'Administrative', N'Active');

-- Tipos de envío
INSERT INTO [ShippingTypes] (Name, Cost)
VALUES
(N'Express',                  100.00),
(N'Envio dentro de 24 horas',  50.00);
