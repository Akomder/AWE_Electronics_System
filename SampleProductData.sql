
-- 1) Insert smartphone "categories" (brands) if they don't already exist
IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Apple')
    INSERT INTO Categories (CategoryName, Description, IsActive) VALUES ('Apple', 'Apple smartphones and devices', 1);

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Samsung')
    INSERT INTO Categories (CategoryName, Description, IsActive) VALUES ('Samsung', 'Samsung smartphones and devices', 1);

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Xiaomi')
    INSERT INTO Categories (CategoryName, Description, IsActive) VALUES ('Xiaomi', 'Xiaomi smartphones and devices', 1);

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Huawei')
    INSERT INTO Categories (CategoryName, Description, IsActive) VALUES ('Huawei', 'Huawei smartphones and devices', 1);

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Oppo')
    INSERT INTO Categories (CategoryName, Description, IsActive) VALUES ('Oppo', 'Oppo smartphones and devices', 1);

GO

-- 2) Insert suppliers used for smartphone stock (only if not already present)
-- You can change contact details and names as needed
IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE SupplierName = 'Apple Australia Distribution')
    INSERT INTO Suppliers (SupplierName, ContactPerson, Email, Phone, Address, City, State, PostalCode, Country, IsActive)
    VALUES ('Apple Australia Distribution', 'Lisa Brown', 'supply@appleau.com', '0399123456', '1 Apple Way', 'Melbourne', 'VIC', '3000', 'Australia', 1);

IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE SupplierName = 'Samsung Electronics AU')
    INSERT INTO Suppliers (SupplierName, ContactPerson, Email, Phone, Address, City, State, PostalCode, Country, IsActive)
    VALUES ('Samsung Electronics AU', 'Daniel Kim', 'supply@samsungau.com', '0399234567', '50 Galaxy Rd', 'Sydney', 'NSW', '2000', 'Australia', 1);

IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE SupplierName = 'Global Mobile Imports')
    INSERT INTO Suppliers (SupplierName, ContactPerson, Email, Phone, Address, City, State, PostalCode, Country, IsActive)
    VALUES ('Global Mobile Imports', 'Maya Nguyen', 'orders@globalmob.com', '0399345678', '200 Import St', 'Brisbane', 'QLD', '4000', 'Australia', 1);

GO

-- 3) Insert smartphone products (real models) using subqueries to get CategoryID and SupplierID
-- APPLE
INSERT INTO Products (ProductName, CategoryID, SupplierID, Description, Price, StockQuantity, ReorderLevel, ImageURL, SKU, Weight, Dimensions, Warranty, IsActive)
VALUES
('Apple iPhone 15 Pro', 
 (SELECT CategoryID FROM Categories WHERE CategoryName='Apple'),
 (SELECT SupplierID FROM Suppliers WHERE SupplierName='Apple Australia Distribution'),
 'Apple iPhone 15 Pro — A17 Pro chip, 128GB storage, 6.3-inch display', 
 2099.00, 25, 5, 'https://example.com/iphone15pro.jpg', 'PHN-APPLE-15PRO', 0.197, '147.6 x 71.6 x 7.8 mm', '1 year Apple warranty', 1),

('Apple iPhone 14 Pro', 
 (SELECT CategoryID FROM Categories WHERE CategoryName='Apple'),
 (SELECT SupplierID FROM Suppliers WHERE SupplierName='Apple Australia Distribution'),
 'Apple iPhone 14 Pro — 128GB storage, 6.1-inch display, Pro camera system', 
 1699.00, 30, 8, 'https://example.com/iphone14pro.jpg', 'PHN-APPLE-14PRO', 0.206, '147.5 x 71.5 x 7.85 mm', '1 year Apple warranty', 1);

-- SAMSUNG
INSERT INTO Products (ProductName, CategoryID, SupplierID, Description, Price, StockQuantity, ReorderLevel, ImageURL, SKU, Weight, Dimensions, Warranty, IsActive)
VALUES
('Samsung Galaxy S24 Ultra', 
 (SELECT CategoryID FROM Categories WHERE CategoryName='Samsung'),
 (SELECT SupplierID FROM Suppliers WHERE SupplierName='Samsung Electronics AU'),
 'Samsung Galaxy S24 Ultra — 200MP camera, 256GB storage, 6.8-inch Dynamic AMOLED', 
 2299.00, 20, 6, 'https://example.com/galaxy-s24-ultra.jpg', 'PHN-SAM-S24U', 0.233, '163.3 x 78.1 x 8.9 mm', '2 years Samsung warranty', 1),

('Samsung Galaxy S23', 
 (SELECT CategoryID FROM Categories WHERE CategoryName='Samsung'),
 (SELECT SupplierID FROM Suppliers WHERE SupplierName='Samsung Electronics AU'),
 'Samsung Galaxy S23 — 256GB, 6.1-inch AMOLED display', 
 1399.00, 35, 10, 'https://example.com/galaxy-s23.jpg', 'PHN-SAM-S23', 0.168, '146.3 x 70.9 x 7.6 mm', '2 years Samsung warranty', 1);

-- XIAOMI
INSERT INTO Products (ProductName, CategoryID, SupplierID, Description, Price, StockQuantity, ReorderLevel, ImageURL, SKU, Weight, Dimensions, Warranty, IsActive)
VALUES
('Xiaomi 13 Pro', 
 (SELECT CategoryID FROM Categories WHERE CategoryName='Xiaomi'),
 (SELECT SupplierID FROM Suppliers WHERE SupplierName='Global Mobile Imports'),
 'Xiaomi 13 Pro — Snapdragon 8 Gen 2, 256GB, 6.73-inch AMOLED', 
 1199.00, 18, 6, 'https://example.com/xiaomi-13-pro.jpg', 'PHN-XIA-13PRO', 0.210, '163.6 x 74.6 x 8.4 mm', '2 years manufacturer warranty', 1),

('Xiaomi Redmi Note 12 Pro', 
 (SELECT CategoryID FROM Categories WHERE CategoryName='Xiaomi'),
 (SELECT SupplierID FROM Suppliers WHERE SupplierName='Global Mobile Imports'),
 'Xiaomi Redmi Note 12 Pro — 128GB, 6.67-inch AMOLED, value flagship', 
 449.00, 40, 12, 'https://example.com/xiaomi-note12pro.jpg', 'PHN-XIA-N12P', 0.205, '163.6 x 74.5 x 8.3 mm', '1 year warranty', 1);

-- HUAWEI
INSERT INTO Products (ProductName, CategoryID, SupplierID, Description, Price, StockQuantity, ReorderLevel, ImageURL, SKU, Weight, Dimensions, Warranty, IsActive)
VALUES
('Huawei P60 Pro', 
 (SELECT CategoryID FROM Categories WHERE CategoryName='Huawei'),
 (SELECT SupplierID FROM Suppliers WHERE SupplierName='Global Mobile Imports'),
 'Huawei P60 Pro — 256GB, 6.6-inch OLED, advanced camera system', 
 1499.00, 12, 4, 'https://example.com/huawei-p60-pro.jpg', 'PHN-HUA-P60P', 0.198, '162.0 x 75.0 x 7.9 mm', '1 year manufacturer warranty', 1),

('Huawei Mate 50 Pro', 
 (SELECT CategoryID FROM Categories WHERE CategoryName='Huawei'),
 (SELECT SupplierID FROM Suppliers WHERE SupplierName='Global Mobile Imports'),
 'Huawei Mate 50 Pro — premium camera and build, 256GB', 
 1399.00, 10, 4, 'https://example.com/huawei-mate50pro.jpg', 'PHN-HUA-M50P', 0.205, '162.0 x 75.5 x 8.6 mm', '1 year manufacturer warranty', 1);

-- OPPO
INSERT INTO Products (ProductName, CategoryID, SupplierID, Description, Price, StockQuantity, ReorderLevel, ImageURL, SKU, Weight, Dimensions, Warranty, IsActive)
VALUES
('Oppo Find X6 Pro', 
 (SELECT CategoryID FROM Categories WHERE CategoryName='Oppo'),
 (SELECT SupplierID FROM Suppliers WHERE SupplierName='Global Mobile Imports'),
 'Oppo Find X6 Pro — flagship camera tech, 512GB, 6.8-inch display', 
 1799.00, 14, 5, 'https://example.com/oppo-findx6pro.jpg', 'PHN-OPP-FX6P', 0.225, '164.6 x 74.4 x 9.1 mm', '2 years manufacturer warranty', 1),

('Oppo Reno 11 Pro', 
 (SELECT CategoryID FROM Categories WHERE CategoryName='Oppo'),
 (SELECT SupplierID FROM Suppliers WHERE SupplierName='Global Mobile Imports'),
 'Oppo Reno 11 Pro — stylish design, 256GB, 6.7-inch AMOLED', 
 799.00, 30, 8, 'https://example.com/oppo-reno11pro.jpg', 'PHN-OPP-R11P', 0.210, '161.8 x 73.3 x 7.9 mm', '1 year warranty', 1);

GO

-- 4) Verification: show only smartphone products (join categories and suppliers)
SELECT 
    p.ProductID, 
    p.ProductName, 
    c.CategoryName AS Brand, 
    s.SupplierName, 
    p.Price, 
    p.StockQuantity,
    p.IsActive
FROM Products p
INNER JOIN Categories c ON p.CategoryID = c.CategoryID
INNER JOIN Suppliers s ON p.SupplierID = s.SupplierID
WHERE c.CategoryName IN ('Apple','Samsung','Xiaomi','Huawei','Oppo')
ORDER BY c.CategoryName, p.ProductName;
GO
