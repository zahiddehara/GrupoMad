-- Script para popular ProductTypes en SQLite
-- Instrucciones de uso:
-- sqlite3 grupomad.db < seed-producttypes.sql
-- O en producción: sqlite3 /var/www/GrupoMad/grupomad.db < seed-producttypes.sql

-- Eliminar ProductTypes existentes (y sus productos relacionados si es necesario)
-- Descomenta la siguiente línea si quieres eliminar todos los Products primero
-- DELETE FROM Products;

-- Eliminar ProductTypes existentes
-- Descomenta la siguiente línea si quieres eliminar los ProductTypes existentes
-- DELETE FROM ProductTypes;

-- Insertar los 54 ProductTypes (reemplaza los existentes si tienen el mismo Id)
INSERT OR REPLACE INTO ProductTypes (Id, Name, Description, PricingType, IsActive, CreatedAt, UpdatedAt) VALUES
(1, 'Enrollable Essential', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(2, 'Sheer Elegance Neolux', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(3, 'Panel Deslizante Sencillo Essential', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(4, 'Romana Essential', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(5, 'Panel Deslizante Romanizado Essential', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(6, 'Sheer Elegance Linea Treeshades', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(7, 'Sheer Elegance Linea Mad', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(8, 'Sheer Elegance Linea Bling', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(9, 'Shangri-La Linea Treeshades', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(10, 'Shangri-La Linea Bling', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(11, 'Waves Linea Mad', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(12, 'Persianas Dunes 5 Manual Linea Vertilux', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(13, 'Persianas Dunes 5 Motorizada Linea Vertilux', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(14, 'Enrollable Linea Treeshades', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(15, 'Enrollable Linea Mad', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(16, 'Enrollable Linea Bling', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(17, 'Enrollable Screen Treeshades', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(18, 'Romana Linea Treeshades', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(19, 'Romana Linea Bling', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(20, 'Romana Linea Mad', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(21, 'Panel Deslizante Sencillo Linea Treeshades', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(22, 'Panel Deslizante Sencillo Linea Bling', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(23, 'Panel Deslizante Sencillo Linea Mad', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(24, 'Panel Deslizante Romanizado Linea Treeshades', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(25, 'Panel Deslizante Romanizado Linea Bling', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(26, 'Panel Deslizante Romanizado Linea Mad', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(27, 'Vertical de Tela Linea Vertilux', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(28, 'Vertical de Tela Linea Bling', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(29, 'Galeria para Persiana Vertical de Tela Linea Vertilux', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(30, 'Galeria para Persiana Vertical de Tela Linea Bling', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(31, 'Vertical de PVC Linea Vertilux', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(32, 'Vertical de Tela con Portatela Linea Vertilux', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(33, 'Accesorios para Vertical de PVC', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(34, 'Toldo Vertical Linea Mad Screen Basic Screen Soft', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(35, 'Toldo Vertical Linea Mad Screen One', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(36, 'Toldo Vertical Linea Mad Screen Milan', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(37, 'Toldo Retractil Sunset', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(38, 'Accesorios para Panel Vertical y Retractil', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(39, 'Persiana Horizontal de Madera 2 pulgadas', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(40, 'Persiana Horizontal de Aluminio 1 pulgada', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(41, 'Horizontal Aluminio 2 pulgadas', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(42, 'Horizontal Aluminio Microperforado 2 pulgadas', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(43, 'Sistema de Cordon Bolero Traslucida 25 mm', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(44, 'Sistema de Cordon Bolero Privee 25 mm', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(45, 'Sistema de Muelle Bolero Traslucida 25 mm', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(46, 'Sistema de Muelle Bolero Privee 25 mm', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(47, 'Sistema de Cordon Romance Traslucida 38 mm', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(48, 'Sistema de Cordon Romance Noite 38 mm', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(49, 'Sistema de Muelle Romance Traslucida 38 mm', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(50, 'Sistema de Muelle Romance Noite 38 mm', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(51, 'Bolero Dia y Noche Sistema de Cordon', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(52, 'Bolero Dia y Noche Sistema de Muelle', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(53, 'Romance Dia y Noche Sistema de Cordon', NULL, 0, 1, '2025-11-19 00:00:00', NULL),
(54, 'Romance Dia y Noche Sistema de Muelle', NULL, 0, 1, '2025-11-19 00:00:00', NULL);

-- Verificar que se insertaron correctamente
SELECT COUNT(*) as 'Total ProductTypes' FROM ProductTypes;
