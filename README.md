# AWE Electronics Management System

##  Project Overview

AWE Electronics is an enterprise-level management system designed to handle:
- **Inventory Management**: Track products, categories, and suppliers
- **Goods Management**: Handle Goods Received Notes (GRN) and Goods Delivery Notes (GDN)
- **Order Processing**: Manage customer orders and order fulfillment
- **User Management**: Role-based access control (Admin, Manager, Accountant, Staff)
- **Reports & Analytics**: Generate business insights and reports

### Target Framework
- **.NET 8** (LTS)
- **C# 13.0**
- **SQL Server Express** (or SQL Server 2019+)

## System Architecture
The project follows a **3-layer architecture**:

```
┌─────────────────────────────────────────────┐
│         Presentation Layer                   │
│  ├─ AWE.DesktopApp (Windows Forms)          │
│  └─ AWE.WebApp (ASP.NET Core MVC)           │
├─────────────────────────────────────────────┤
│         Business Logic Layer (BLL)           │
│  └─ AWE.BLL (Managers & Services)           │
├─────────────────────────────────────────────┤
│         Data Access Layer (DAL)              │
│  ├─ AWE.DAL (Data Access Objects)           │
│  └─ Database (SQL Server)                   │
├─────────────────────────────────────────────┤
│         Shared Models                        │
│  └─ AWE.Models (Domain Objects)             │
└─────────────────────────────────────────────┘
```

### Projects in Solution

| Project | Type | Purpose |
|---------|------|---------|
| **AWE.Models** | Class Library | Domain models and entities |
| **AWE.DAL** | Class Library | Data Access Layer (SQL Server) |
| **AWE.BLL** | Class Library | Business Logic & Managers |
| **AWE.WebApp** | ASP.NET Core MVC | Web-based interface (Razor Pages) |
| **AWE.DesktopApp** | Windows Forms | Desktop application |

## rerequisites

Before you begin, ensure you have the following installed:

### Required Software
- **Visual Studio 2022** (or later) with the following workloads:
  - .NET desktop development
  - ASP.NET and web development
  - Data storage and processing
  
- **.NET 8 SDK** - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)

- **SQL Server Express 2019** (or higher) - Download from [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

- **SQL Server Management Studio (SSMS)** - Download from [Microsoft SSMS](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

## Installation & Setup

### Step 1: Clone the Repository

```bash
git clone https://github.com/Akomder/AWE_Electronics_System.git
cd AWE_Electronics_System
```

### Step 2: Verify .NET Installation

```bash
dotnet --version
# Should output: 8.x.x or higher
```

### Step 3: Open in Visual Studio

1. Open **Visual Studio 2022**
2. Click **File** → **Open** → **Project/Solution**
3. Navigate to the project folder and select **AWE_Electronics_System.sln**

### Step 4: Restore NuGet Packages

**Option A: Using Visual Studio**
- Go to **Tools** → **NuGet Package Manager** → **Manage NuGet Packages for Solution**
- Click **Restore**

**Option B: Using Command Line**

```bash
dotnet restore
```

### Step 5: Install Required NuGet Packages

The following packages are automatically installed via `dotnet restore`:

```
Microsoft.Data.SqlClient - SQL Server connectivity
System.Windows.Forms - Windows Forms UI
Bootstrap - Web UI framework
```

To manually verify installed packages:

```bash
dotnet list package --outdated
```

## Database Setup

### Step 1: Create Database

**Using SQL Server Management Studio (SSMS):**

1. Open **SSMS**
2. Connect to `.\SQLEXPRESS` (or your server instance)
3. Right-click on **Databases** → **New Database**
4. Name: `Awe_Electronics`
5. Click **OK**

**Using SQL Command:**

```sql
CREATE DATABASE Awe_Electronics;
GO
USE Awe_Electronics;
GO
```

### Step 2: Create Database Schema

Run the database initialization script in SSMS:

-- =============================================
-- USERS TABLE
-- =============================================
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Role NVARCHAR(50) NOT NULL, -- Admin, Manager, Accountant, Staff
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastLogin DATETIME NULL
);

-- =============================================
-- SUPPLIERS TABLE
-- =============================================
CREATE TABLE Suppliers (
    SupplierID INT PRIMARY KEY IDENTITY(1,1),
    SupplierName NVARCHAR(200) NOT NULL,
    ContactPerson NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    Country NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- =============================================
-- CATEGORIES TABLE
-- =============================================
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- =============================================
-- PRODUCTS TABLE
-- =============================================
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductCode NVARCHAR(50) NOT NULL UNIQUE,
    ProductName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    CategoryID INT NOT NULL,
    SupplierID INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    StockQuantity INT DEFAULT 0,
    ReorderLevel INT DEFAULT 10,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
);

-- =============================================
-- CUSTOMERS TABLE
-- =============================================
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    CustomerName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    Country NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- =============================================
-- ORDERS TABLE
-- =============================================
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
    CustomerID INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    DeliveryDate DATETIME,
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Completed, Cancelled
    TotalAmount DECIMAL(10,2) DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
);

-- =============================================
-- ORDER ITEMS TABLE
-- =============================================
CREATE TABLE OrderItems (
    OrderItemID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- =============================================
-- GOODS RECEIVED NOTES (GRN) TABLE
-- =============================================
CREATE TABLE GoodsReceivedNotes (
    GRNID INT PRIMARY KEY IDENTITY(1,1),
    GRNNumber NVARCHAR(50) NOT NULL UNIQUE,
    SupplierID INT NOT NULL,
    ReceivedDate DATETIME NOT NULL,
    ReceivedByUserID INT NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Draft', -- Draft, Approved, Posted
    TotalAmount DECIMAL(10,2) DEFAULT 0,
    Notes NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
    FOREIGN KEY (ReceivedByUserID) REFERENCES Users(UserID)
);

-- =============================================
-- GRN ITEMS TABLE
-- =============================================
CREATE TABLE GRNItems (
    GRNItemID INT PRIMARY KEY IDENTITY(1,1),
    GRNID INT NOT NULL,
    ProductID INT NOT NULL,
    QuantityReceived INT NOT NULL,
    UnitCost DECIMAL(10,2) NOT NULL,
    TotalCost DECIMAL(10,2) NOT NULL,
    Notes NVARCHAR(MAX),
    FOREIGN KEY (GRNID) REFERENCES GoodsReceivedNotes(GRNID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- =============================================
-- GOODS DELIVERY NOTES (GDN) TABLE
-- =============================================
CREATE TABLE GoodsDeliveryNotes (
    GDNID INT PRIMARY KEY IDENTITY(1,1),
    GDNNumber NVARCHAR(50) NOT NULL UNIQUE,
    OrderID INT,
    CustomerID INT,
    DeliveryDate DATETIME NOT NULL,
    DeliveredByUserID INT NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Draft', -- Draft, Approved, Posted
    DeliveryAddress NVARCHAR(500),
    Notes NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    FOREIGN KEY (DeliveredByUserID) REFERENCES Users(UserID)
);

-- =============================================
-- GDN ITEMS TABLE
-- =============================================
CREATE TABLE GDNItems (
    GDNItemID INT PRIMARY KEY IDENTITY(1,1),
    GDNID INT NOT NULL,
    ProductID INT NOT NULL,
    QuantityDelivered INT NOT NULL,
    Notes NVARCHAR(MAX),
    FOREIGN KEY (GDNID) REFERENCES GoodsDeliveryNotes(GDNID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- =============================================
-- CREATE INDEXES FOR PERFORMANCE
-- =============================================
CREATE INDEX IDX_Products_CategoryID ON Products(CategoryID);
CREATE INDEX IDX_Products_SupplierID ON Products(SupplierID);
CREATE INDEX IDX_Orders_CustomerID ON Orders(CustomerID);
CREATE INDEX IDX_GRN_SupplierID ON GoodsReceivedNotes(SupplierID);
CREATE INDEX IDX_GDN_OrderID ON GoodsDeliveryNotes(OrderID);
CREATE INDEX IDX_GDN_CustomerID ON GoodsDeliveryNotes(CustomerID);
```

### Step 3: Insert Sample Data (Optional)

```sql
-- Insert sample users
INSERT INTO Users (Username, PasswordHash, FirstName, LastName, Email, Role) 
VALUES 
('admin', 'admin123', 'Admin', 'User', 'admin@awe.com', 'Admin'),
('manager', 'manager123', 'Manager', 'User', 'manager@awe.com', 'Manager'),
('accountant', 'accountant123', 'Accountant', 'User', 'accountant@awe.com', 'Accountant'),
('staff', 'staff123', 'Staff', 'User', 'staff@awe.com', 'Staff');

-- Insert sample categories
INSERT INTO Categories (CategoryName, Description) 
VALUES 
('Resistors', 'Electronic resistors'),
('Capacitors', 'Capacitive components'),
('Semiconductors', 'IC and transistors');

-- Insert sample suppliers
INSERT INTO Suppliers (SupplierName, Email, Phone, City, Country) 
VALUES 
('Tech Supplies Co.', 'info@techsupplies.com', '+1-123-456-7890', 'New York', 'USA');

-- Insert sample customers
INSERT INTO Customers (CustomerName, Email, Phone, City, Country) 
VALUES 
('ABC Electronics Ltd.', 'contact@abcelectronics.com', '+1-987-654-3210', 'Los Angeles', 'USA');
```

### Step 4: Verify Database Connection

Update the connection string in `AWE.DAL/DbHelper.cs` if needed:

```csharp
// Default connection string
private static readonly string ConnectionString = 
    @"Server=.\SQLEXPRESS;Database=Awe_Electronics;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;";
```

**To modify connection string:**
1. Open `AWE.DAL/DbHelper.cs`
2. Change the `ConnectionString` property to match your SQL Server configuration
3. Common variations:
   ```csharp
   // For named instance
   @"Server=SERVERNAME\SQLEXPRESS;Database=Awe_Electronics;Integrated Security=True;"
   
   // For SQL Server Authentication
   @"Server=.\SQLEXPRESS;Database=Awe_Electronics;User Id=sa;Password=YourPassword;"
   ```

## Running the Application

### Option 1: Web Application (ASP.NET Core MVC)

**Using Visual Studio:**

1. Right-click on **AWE.WebApp** project → **Set as Startup Project**
2. Press **F5** or click **Debug** → **Start Debugging**
3. The application will open in your default browser at `https://localhost:5001`

**Using Command Line:**

```bash
cd AWE.WebApp
dotnet run
# Application will be available at https://localhost:5001
```

**Using IIS Express:**

1. Open **AWE.WebApp** properties
2. Set **Launch Profile** to **IIS Express**
3. Press **F5**

### Option 2: Desktop Application (Windows Forms)

**Using Visual Studio:**

1. Right-click on **AWE.DesktopApp** project → **Set as Startup Project**
2. Press **F5** or click **Debug** → **Start Debugging**
3. The login form will appear

**Using Command Line:**

```bash
cd AWE.DesktopApp
dotnet run
```

### Option 3: Running Both Applications Simultaneously

**Visual Studio Multi-Startup:**

1. Right-click on **Solution** → **Set Startup Projects**
2. Select **Multiple startup projects**
3. Set both **AWE.WebApp** and **AWE.DesktopApp** to **Start**
4. Press **F5**

**Separate Terminal Windows:**

```bash
# Terminal 1 - Web App
cd AWE.WebApp
dotnet run

# Terminal 2 - Desktop App
cd AWE.DesktopApp
dotnet run
```

## Project Structure

```
AWE_Electronics_System/
├── AWE.Models/                 # Shared domain models
│   ├── User.cs
│   ├── Product.cs
│   ├── Order.cs
│   ├── GoodsReceivedNote.cs
│   ├── GoodsDeliveryNote.cs
│   └── ... (other models)
│
├── AWE.DAL/                    # Data Access Layer
│   ├── DbHelper.cs            # Database connection & helpers
│   ├── UserDAL.cs             # User data access
│   ├── ProductDAL.cs
│   ├── OrderDAL.cs
│   ├── GRNDAL.cs              # Goods Received Notes
│   ├── GDNDAL.cs              # Goods Delivery Notes
│   └── ... (other DAL classes)
│
├── AWE.BLL/                    # Business Logic Layer
│   ├── UserManager.cs         # User business logic
│   ├── ProductManager.cs
│   ├── OrderManager.cs
│   ├── InventoryManager.cs
│   └── ... (other managers)
│
├── AWE.WebApp/                 # ASP.NET Core MVC Web Application
│   ├── Controllers/           # MVC Controllers
│   │   ├── AccountController.cs
│   │   ├── DashboardController.cs
│   │   ├── ProductController.cs
│   │   ├── UserController.cs
│   │   ├── GRNController.cs
│   │   ├── GDNController.cs
│   │   └── ... (other controllers)
│   ├── Views/                 # Razor views
│   │   ├── Account/          # Login, Register, Password Reset
│   │   ├── Dashboard/
│   │   ├── Product/          # Product CRUD views
│   │   ├── User/             # User management views
│   │   ├── GRN/              # Goods Received Notes views
│   │   ├── GDN/              # Goods Delivery Notes views
│   │   ├── Shared/           # Layout & shared components
│   │   └── ...
│   ├── Helpers/              # SessionHelper, utilities
│   ├── Filters/              # AuthorizeUser attribute
│   ├── wwwroot/              # Static files (CSS, JS, images)
│   ├── appsettings.json      # Configuration
│   ├── Program.cs            # Startup configuration
│   └── AWE.WebApp.csproj
│
├── AWE.DesktopApp/             # Windows Forms Application
│   ├── LoginForm.cs           # Login interface
│   ├── MainForm.cs            # Main dashboard
│   ├── ProductManagementForm.cs
│   ├── UserManagementForm.cs
│   ├── GRNManagementForm.cs
│   ├── GDNManagementForm.cs
│   ├── OrderManagementForm.cs
│   ├── ReportsForm.cs
│   ├── Program.cs
│   └── AWE.DesktopApp.csproj
│
└── README.md                    # This file
```

## Features

### User Management
- ✅ User registration and authentication
- ✅ Role-based access control (Admin, Manager, Accountant, Staff)
- ✅ Password management and reset
- ✅ User profile management
- ✅ Session management

### Inventory Management
- ✅ Product CRUD operations
- ✅ Category management
- ✅ Supplier management
- ✅ Stock tracking and reorder levels
- ✅ Product search and filtering

### Goods Management
- ✅ Goods Received Notes (GRN) creation and tracking
- ✅ Goods Delivery Notes (GDN) creation and tracking
- ✅ Status workflows (Draft → Approved → Posted)
- ✅ Line item management

### Order Processing
- ✅ Order creation and management
- ✅ Customer management
- ✅ Order status tracking
- ✅ Order items management

### Reporting & Analytics
- ✅ Low stock alerts
- ✅ Pending orders report
- ✅ Sales analytics
- ✅ Inventory status report
- ✅ Data export capabilities

## Configuration

### Web Application Configuration

**File:** `AWE.WebApp/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Session Configuration

**File:** `AWE.WebApp/Program.cs`

```csharp
// Session timeout: 30 minutes
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

### Dependency Injection Registration

All BLL managers are registered in `Program.cs`:

```csharp
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<ProductManager>();
builder.Services.AddScoped<OrderManager>();
builder.Services.AddScoped<InventoryManager>();
// ... etc
```

## Default Credentials

These are sample credentials for testing:

| Role | Username | Password |
|------|----------|----------|
| Admin | admin | admin123 |
| Manager | manager | manager123 |
| Accountant | accountant | accountant123 |
| Staff | staff | staff123 |

**IMPORTANT**: Change these credentials immediately in production!

### To Update Credentials:

1. **Via Web Application:**
   - Login as Admin
   - Go to **Administration** → **User Management**
   - Click on user → **Change Password**

2. **Via Database:**
   ```sql
   -- Note: Passwords should be hashed in production
   UPDATE Users SET PasswordHash = 'NewPassword' 
   WHERE Username = 'admin';
   ```

## Troubleshooting

### Issue 1: Database Connection Failed

**Error:** `Login failed for user 'NT AUTHORITY\SYSTEM'`

**Solutions:**
1. Verify SQL Server is running
   ```bash
   services.msc  # Check "SQL Server (SQLEXPRESS)" is Running
   ```
2. Check connection string in `DbHelper.cs`
3. Verify SQL Server instance name: `sqlcmd -L` (lists available instances)
4. Test connection in SSMS

### Issue 2: Build Fails - Missing Dependencies

**Error:** `The type or namespace name 'X' could not be found`

**Solutions:**
1. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
2. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet build
   ```
3. Check .NET version: `dotnet --version` (should be 8.x)

### Issue 3: Port Already in Use

**Error:** `Address already in use` or `Unable to listen on port 5001`

**Solutions:**
1. Change port in `launchSettings.json`:
   ```json
   {
     "profiles": {
       "https": {
         "commandName": "Project",
         "applicationUrl": "https://localhost:5002"
       }
     }
   }
   ```
2. Or kill the process using the port:
   ```bash
   # Find process on port 5001
   netstat -ano | findstr :5001
   
   # Kill the process (replace PID)
   taskkill /PID <PID> /F
   ```

### Issue 4: Authentication Issues

**Error:** `Login failed`, `User not found`, or `Session expired`

**Solutions:**
1. Verify user exists in database:
   ```sql
   SELECT * FROM Users WHERE Username = 'admin';
   ```
2. Check password matches (case-sensitive)
3. Verify user `IsActive` bit is 1
4. Check session timeout settings

### Issue 5: HTTPS Certificate Error

**Error:** `CERTIFICATE_VERIFY_FAILED` or SSL issues

**Solution:** For development, disable HTTPS verification:
```bash
dotnet user-secrets set "https_port" ""
```

Or modify `launchSettings.json`:
```json
{
  "https": {
    "commandName": "Project",
    "dotnetRunMessages": true,
    "applicationUrl": "http://localhost:5000"
  }
}
```

## API Documentation

### Authentication Flow

**Web Application:**
1. User navigates to `https://localhost:5001/Account/Login`
2. Enters credentials
3. System validates against Users table
4. On success: Session created, redirected to Dashboard
5. On failure: Error message displayed

**Desktop Application:**
1. LoginForm displayed on startup
2. User enters credentials
3. Validation against database
4. On success: MainForm opens
5. On failure: Error message displayed

### Controller Routes

**Web Application:**

| Controller | Routes |
|-----------|--------|
| AccountController | `/Account/Login`, `/Account/Register`, `/Account/Logout` |
| DashboardController | `/Dashboard` |
| ProductController | `/Product`, `/Product/Create`, `/Product/Edit/{id}`, `/Product/Delete/{id}` |
| UserController | `/User`, `/User/Create`, `/User/Edit/{id}` |
| GRNController | `/GRN`, `/GRN/Details/{id}` |
| GDNController | `/GDN`, `/GDN/Details/{id}` |
| OrderController | `/Order`, `/Order/Details/{id}` |

## Database Maintenance

### Regular Backups

```bash
# Backup database
sqlcmd -S .\SQLEXPRESS -Q "BACKUP DATABASE Awe_Electronics TO DISK = 'C:\Backups\awe_electronics_backup.bak'"

# Restore database
sqlcmd -S .\SQLEXPRESS -Q "RESTORE DATABASE Awe_Electronics FROM DISK = 'C:\Backups\awe_electronics_backup.bak'"
```

### Database Health Check

```sql
-- Check database size
sp_helpdb Awe_Electronics;

-- Check table record counts
SELECT 
    TABLE_NAME, 
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES) AS RecordCount
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_CATALOG = 'Awe_Electronics';

-- Check for orphaned records
SELECT * FROM Orders WHERE CustomerID NOT IN (SELECT CustomerID FROM Customers);
```

## Performance Optimization

### Applied Indexes

```sql
-- Already created for performance
CREATE INDEX IDX_Products_CategoryID ON Products(CategoryID);
CREATE INDEX IDX_Products_SupplierID ON Products(SupplierID);
CREATE INDEX IDX_Orders_CustomerID ON Orders(CustomerID);
CREATE INDEX IDX_GRN_SupplierID ON GoodsReceivedNotes(SupplierID);
CREATE INDEX IDX_GDN_OrderID ON GoodsDeliveryNotes(OrderID);
```

### Recommended Optimizations

1. **Enable Connection Pooling** in `DbHelper.cs`:
   ```csharp
   "Pooling=true;Max Pool Size=100;"
   ```

2. **Add Caching** in BLL for frequently accessed data

3. **Use Pagination** in list views

4. **Add Database Maintenance Plans** in SQL Server

## Deployment

### Web Application Deployment (IIS)

1. Publish the application:
   ```bash
   cd AWE.WebApp
   dotnet publish -c Release -o .\publish
   ```

2. Create IIS Website:
   - Physical Path: `C:\inetpub\wwwroot\awe-webapp`
   - Copy published files there
   - Set App Pool to ".NET CLR Version: No Managed Code"

3. Update connection string for production environment

### Desktop Application Deployment

1. Create installer:
   ```bash
   cd AWE.DesktopApp
   dotnet publish -c Release -r win-x64 --self-contained
   ```

2. Distribute the executable from `bin\Release\net8.0-windows\win-x64\publish\`

## Contributing

1. Create a feature branch: `git checkout -b feature/amazing-feature`
2. Commit changes: `git commit -m 'Add amazing feature'`
3. Push to branch: `git push origin feature/amazing-feature`
4. Open a Pull Request

## License

This project is proprietary software for AWE Electronics.

## Support

For issues or questions:
- Email: support@aweelectronics.com
- Repository: https://github.com/Akomder/AWE_Electronics_System
- Issues: https://github.com/Akomder/AWE_Electronics_System/issues

## Acknowledgments

- Built with .NET 8 and ASP.NET Core
- UI powered by Bootstrap and Bootstrap Icons
- Database: SQL Server Express

---
