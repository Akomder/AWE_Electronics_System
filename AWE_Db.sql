CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Staff', 'Accountant', 'Manager')),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Phone NVARCHAR(20),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastLogin DATETIME NULL
);
GO

CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20),
    Address NVARCHAR(200),
    City NVARCHAR(50),
    State NVARCHAR(50),
    PostalCode NVARCHAR(10),
    Country NVARCHAR(50) DEFAULT 'Australia',
    RegistrationDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    LastLogin DATETIME NULL
);
GO

CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) UNIQUE NOT NULL,
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Suppliers (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(100) NOT NULL,
    ContactPerson NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Address NVARCHAR(200),
    City NVARCHAR(50),
    State NVARCHAR(50),
    PostalCode NVARCHAR(10),
    Country NVARCHAR(50) DEFAULT 'Australia',
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(200) NOT NULL,
    CategoryID INT FOREIGN KEY REFERENCES Categories(CategoryID),
    SupplierID INT FOREIGN KEY REFERENCES Suppliers(SupplierID),
    Description NVARCHAR(MAX),
    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    StockQuantity INT DEFAULT 0 CHECK (StockQuantity >= 0),
    ReorderLevel INT DEFAULT 10,
    ImageURL NVARCHAR(255),
    SKU NVARCHAR(50) UNIQUE,
    Weight DECIMAL(8,2),
    Dimensions NVARCHAR(50),
    Warranty NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastUpdated DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE ShoppingCart (
    CartID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    AddedDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(12,2) NOT NULL CHECK (TotalAmount >= 0),
    OrderStatus NVARCHAR(20) DEFAULT 'Pending' CHECK (OrderStatus IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')),
    ShippingAddress NVARCHAR(200) NOT NULL,
    ShippingCity NVARCHAR(50) NOT NULL,
    ShippingState NVARCHAR(50) NOT NULL,
    ShippingPostalCode NVARCHAR(10) NOT NULL,
    PaymentMethod NVARCHAR(50),
    PaymentStatus NVARCHAR(20) DEFAULT 'Pending' CHECK (PaymentStatus IN ('Pending', 'Paid', 'Failed', 'Refunded')),
    TrackingNumber NVARCHAR(100),
    Notes NVARCHAR(500),
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserID),
    UpdatedDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID) ON DELETE CASCADE,
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(10,2) NOT NULL CHECK (UnitPrice >= 0),
    Subtotal AS (Quantity * UnitPrice) PERSISTED,
    Discount DECIMAL(5,2) DEFAULT 0 CHECK (Discount >= 0 AND Discount <= 100)
);
GO

CREATE TABLE Invoices (
    InvoiceID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    InvoiceNumber NVARCHAR(50) UNIQUE NOT NULL,
    InvoiceDate DATETIME DEFAULT GETDATE(),
    DueDate DATETIME,
    SubTotal DECIMAL(12,2) NOT NULL,
    TaxAmount DECIMAL(10,2) DEFAULT 0,
    TotalAmount DECIMAL(12,2) NOT NULL,
    PaymentStatus NVARCHAR(20) DEFAULT 'Unpaid' CHECK (PaymentStatus IN ('Unpaid', 'Paid', 'Overdue', 'Cancelled')),
    Notes NVARCHAR(500),
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Payments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceID INT FOREIGN KEY REFERENCES Invoices(InvoiceID),
    PaymentDate DATETIME DEFAULT GETDATE(),
    PaymentMethod NVARCHAR(50) NOT NULL,
    Amount DECIMAL(12,2) NOT NULL CHECK (Amount > 0),
    TransactionID NVARCHAR(100),
    PaymentStatus NVARCHAR(20) DEFAULT 'Completed' CHECK (PaymentStatus IN ('Completed', 'Failed', 'Pending', 'Refunded')),
    Notes NVARCHAR(500),
    ProcessedBy INT FOREIGN KEY REFERENCES Users(UserID)
);
GO

CREATE TABLE GoodsReceivedNotes (
    GRNID INT IDENTITY(1,1) PRIMARY KEY,
    GRNNumber NVARCHAR(50) UNIQUE NOT NULL,
    SupplierID INT FOREIGN KEY REFERENCES Suppliers(SupplierID),
    ReceivedDate DATETIME DEFAULT GETDATE(),
    ReceivedBy INT FOREIGN KEY REFERENCES Users(UserID),
    TotalQuantity INT DEFAULT 0,
    Notes NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Received' CHECK (Status IN ('Received', 'Verified', 'Cancelled')),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE GoodsReceivedNoteDetails (
    GRNDetailID INT IDENTITY(1,1) PRIMARY KEY,
    GRNID INT FOREIGN KEY REFERENCES GoodsReceivedNotes(GRNID) ON DELETE CASCADE,
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    QuantityReceived INT NOT NULL CHECK (QuantityReceived > 0),
    UnitCost DECIMAL(10,2),
    Notes NVARCHAR(500)
);
GO

CREATE TABLE GoodsDeliveryNotes (
    GDNID INT IDENTITY(1,1) PRIMARY KEY,
    GDNNumber NVARCHAR(50) UNIQUE NOT NULL,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    DeliveryDate DATETIME DEFAULT GETDATE(),
    DeliveredBy NVARCHAR(100),
    TrackingNumber NVARCHAR(100),
    DeliveryStatus NVARCHAR(20) DEFAULT 'Pending' CHECK (DeliveryStatus IN ('Pending', 'In Transit', 'Delivered', 'Failed')),
    RecipientName NVARCHAR(100),
    RecipientSignature NVARCHAR(255),
    Notes NVARCHAR(500),
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE GoodsDeliveryNoteDetails (
    GDNDetailID INT IDENTITY(1,1) PRIMARY KEY,
    GDNID INT FOREIGN KEY REFERENCES GoodsDeliveryNotes(GDNID) ON DELETE CASCADE,
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    QuantityDelivered INT NOT NULL CHECK (QuantityDelivered > 0),
    Notes NVARCHAR(500)
);
GO