-- All sample users have the password: "password123"
-- SHA256 hash of "password123" = ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f

-- Insert Admin User
INSERT INTO Users (Username, PasswordHash, Role, FirstName, LastName, Email, Phone, IsActive)
VALUES ('admin', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Admin', 'John', 'Administrator', 'admin@aweelectronics.com', '0412345678', 1);

-- Insert Staff User
INSERT INTO Users (Username, PasswordHash, Role, FirstName, LastName, Email, Phone, IsActive)
VALUES ('staff1', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Staff', 'Sarah', 'Johnson', 'sarah.johnson@aweelectronics.com', '0423456789', 1);

-- Insert Accountant User
INSERT INTO Users (Username, PasswordHash, Role, FirstName, LastName, Email, Phone, IsActive)
VALUES ('accountant', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Accountant', 'Dang Doan', 'Minh Tan', '523V0017@Student.tdtu.edu.vn', '0434567890', 1);

-- Insert Manager User
INSERT INTO Users (Username, PasswordHash, Role, FirstName, LastName, Email, Phone, IsActive)
VALUES ('manager', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Manager', 'MOUNTHADY', 'Souk Akhom', '523K0076@student.tdtu.edu.vn', '0445678901', 1);

-- Verification Query
SELECT UserID, Username, Role, FirstName, LastName, Email, IsActive, CreatedDate
FROM Users
WHERE IsActive = 1
ORDER BY Role, Username;

