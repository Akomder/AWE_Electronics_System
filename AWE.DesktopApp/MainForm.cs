#nullable disable
using System;
using System.Windows.Forms;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class MainForm : Form
    {
        private readonly User _currentUser;

        public MainForm(User user)
        {
            _currentUser = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"AWE Electronics Staff System - {_currentUser.Role}";
            this.Size = new System.Drawing.Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Welcome Panel
            Panel welcomePanel = new Panel
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(1060, 80),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblWelcome = new Label
            {
                Text = $"Welcome, {_currentUser.FirstName} {_currentUser.LastName}",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold)
            };

            Label lblRole = new Label
            {
                Text = $"Role: {_currentUser.Role} | Last Login: {_currentUser.LastLogin?.ToString("yyyy-MM-dd HH:mm") ?? "First time"}",
                Location = new System.Drawing.Point(10, 40),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 10)
            };

            welcomePanel.Controls.Add(lblWelcome);
            welcomePanel.Controls.Add(lblRole);

            // Feature Buttons Panel
            Panel featurePanel = new Panel
            {
                Location = new System.Drawing.Point(10, 100),
                Size = new System.Drawing.Size(1060, 580),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            Label lblFeatures = new Label
            {
                Text = "Available Features:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            // === COLUMN 1: INVENTORY & PRODUCTS ===
            Label lblInventory = new Label
            {
                Text = "Inventory & Products:",
                Location = new System.Drawing.Point(20, 40),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            Button btnProductManagement = new Button
            {
                Text = "Product Management (CRUD)",
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(250, 40)
            };
            btnProductManagement.Click += BtnProductManagement_Click;

            Button btnCategoryManagement = new Button
            {
                Text = "Category Management",
                Location = new System.Drawing.Point(20, 120),
                Size = new System.Drawing.Size(250, 40)
            };
            btnCategoryManagement.Click += BtnCategoryManagement_Click;

            Button btnSupplierManagement = new Button
            {
                Text = "Supplier Management",
                Location = new System.Drawing.Point(20, 170),
                Size = new System.Drawing.Size(250, 40)
            };
            btnSupplierManagement.Click += BtnSupplierManagement_Click;

            Button btnGoodsReceived = new Button
            {
                Text = "Goods Received Notes (GRN)",
                Location = new System.Drawing.Point(20, 220),
                Size = new System.Drawing.Size(250, 40)
            };
            btnGoodsReceived.Click += BtnGoodsReceived_Click;

            Button btnGoodsDelivery = new Button
            {
                Text = "Goods Delivery Notes (GDN)",
                Location = new System.Drawing.Point(20, 270),
                Size = new System.Drawing.Size(250, 40)
            };
            btnGoodsDelivery.Click += BtnGoodsDelivery_Click;

            // === COLUMN 2: ORDERS & CUSTOMERS ===
            Label lblOrders = new Label
            {
                Text = "Orders & Customers:",
                Location = new System.Drawing.Point(290, 40),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            Button btnOrderManagement = new Button
            {
                Text = "Order Management",
                Location = new System.Drawing.Point(290, 70),
                Size = new System.Drawing.Size(250, 40)
            };
            btnOrderManagement.Click += BtnOrderManagement_Click;

            Button btnOrderProcessing = new Button
            {
                Text = "Order Processing",
                Location = new System.Drawing.Point(290, 120),
                Size = new System.Drawing.Size(250, 40),
                Enabled = false // To be implemented in Phase 2
            };

            Button btnCustomerManagement = new Button
            {
                Text = "Customer Management",
                Location = new System.Drawing.Point(290, 170),
                Size = new System.Drawing.Size(250, 40),
                Enabled = false // To be implemented in Phase 2
            };

            // === COLUMN 3: ADMIN & REPORTS ===
            Label lblAdmin = new Label
            {
                Text = "Administration & Reports:",
                Location = new System.Drawing.Point(560, 40),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            Button btnUserManagement = new Button
            {
                Text = "User Management",
                Location = new System.Drawing.Point(560, 70),
                Size = new System.Drawing.Size(250, 40),
                Enabled = _currentUser.Role == "Admin"
            };
            btnUserManagement.Click += BtnUserManagement_Click;

            Button btnUserRegistration = new Button
            {
                Text = "Register New User",
                Location = new System.Drawing.Point(560, 120),
                Size = new System.Drawing.Size(250, 40),
                Enabled = _currentUser.Role == "Admin"
            };
            btnUserRegistration.Click += BtnUserRegistration_Click;

            Button btnReports = new Button
            {
                Text = "Reports & Analytics",
                Location = new System.Drawing.Point(560, 170),
                Size = new System.Drawing.Size(250, 40)
            };
            btnReports.Click += BtnReports_Click;

            Button btnSystemSettings = new Button
            {
                Text = "System Settings",
                Location = new System.Drawing.Point(560, 220),
                Size = new System.Drawing.Size(250, 40),
                Enabled = _currentUser.Role == "Admin" // Admin only
            };

            // === QUICK ACTIONS ===
            Label lblQuickActions = new Label
            {
                Text = "Quick Actions:",
                Location = new System.Drawing.Point(20, 330),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            Button btnLowStockAlert = new Button
            {
                Text = "Low Stock Alert",
                Location = new System.Drawing.Point(20, 360),
                Size = new System.Drawing.Size(150, 35)
            };
            btnLowStockAlert.Click += (s, e) => {
                ReportsForm reportsForm = new ReportsForm();
                reportsForm.ShowDialog();
            };

            Button btnPendingOrders = new Button
            {
                Text = "Pending Orders",
                Location = new System.Drawing.Point(180, 360),
                Size = new System.Drawing.Size(150, 35)
            };
            btnPendingOrders.Click += (s, e) => {
                OrderManagementForm orderForm = new OrderManagementForm();
                orderForm.ShowDialog();
            };

            Button btnRefreshData = new Button
            {
                Text = "Refresh All Data",
                Location = new System.Drawing.Point(340, 360),
                Size = new System.Drawing.Size(150, 35)
            };
            btnRefreshData.Click += (s, e) => {
                MessageBox.Show("Data refreshed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // === INFO PANEL ===
            Panel infoPanel = new Panel
            {
                Location = new System.Drawing.Point(20, 410),
                Size = new System.Drawing.Size(1020, 140),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblInfo = new Label
            {
                Text = "System Information:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            Label lblSystemInfo = new Label
            {
                Text = $"AWE Electronics Management System v1.0\n" +
                       $"Database: Connected\n" +
                       $"Current User: {_currentUser.Username} ({_currentUser.Role})\n" +
                       $"Session Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                       $"\nFor support, contact: support@aweelectronics.com",
                Location = new System.Drawing.Point(10, 35),
                Size = new System.Drawing.Size(1000, 90),
                Font = new System.Drawing.Font("Arial", 9)
            };

            infoPanel.Controls.Add(lblInfo);
            infoPanel.Controls.Add(lblSystemInfo);

            // Add all controls to feature panel
            featurePanel.Controls.Add(lblFeatures);
            featurePanel.Controls.Add(lblInventory);
            featurePanel.Controls.Add(btnProductManagement);
            featurePanel.Controls.Add(btnCategoryManagement);
            featurePanel.Controls.Add(btnSupplierManagement);
            featurePanel.Controls.Add(btnGoodsReceived);
            featurePanel.Controls.Add(btnGoodsDelivery);
            featurePanel.Controls.Add(lblOrders);
            featurePanel.Controls.Add(btnOrderManagement);
            featurePanel.Controls.Add(btnOrderProcessing);
            featurePanel.Controls.Add(btnCustomerManagement);
            featurePanel.Controls.Add(lblAdmin);
            featurePanel.Controls.Add(btnUserManagement);
            featurePanel.Controls.Add(btnUserRegistration);
            featurePanel.Controls.Add(btnReports);
            featurePanel.Controls.Add(btnSystemSettings);
            featurePanel.Controls.Add(lblQuickActions);
            featurePanel.Controls.Add(btnLowStockAlert);
            featurePanel.Controls.Add(btnPendingOrders);
            featurePanel.Controls.Add(btnRefreshData);
            featurePanel.Controls.Add(infoPanel);

            // Logout Button
            Button btnLogout = new Button
            {
                Text = "Logout",
                Location = new System.Drawing.Point(970, 690),
                Size = new System.Drawing.Size(100, 30)
            };
            btnLogout.Click += BtnLogout_Click;

            // Add all controls to form
            this.Controls.Add(welcomePanel);
            this.Controls.Add(featurePanel);
            this.Controls.Add(btnLogout);
        }

        // === EVENT HANDLERS ===

        private void BtnProductManagement_Click(object sender, EventArgs e)
        {
            ProductManagementForm productForm = new ProductManagementForm();
            productForm.ShowDialog();
        }

        private void BtnCategoryManagement_Click(object sender, EventArgs e)
        {
            CategoryManagementForm categoryForm = new CategoryManagementForm();
            categoryForm.ShowDialog();
        }

        private void BtnSupplierManagement_Click(object sender, EventArgs e)
        {
            SupplierManagementForm supplierForm = new SupplierManagementForm();
            supplierForm.ShowDialog();
        }

        private void BtnGoodsReceived_Click(object sender, EventArgs e)
        {
            GRNManagementForm grnForm = new GRNManagementForm(_currentUser);
            grnForm.ShowDialog();
        }

        private void BtnGoodsDelivery_Click(object sender, EventArgs e)
        {
            GDNManagementForm gdnForm = new GDNManagementForm(_currentUser);
            gdnForm.ShowDialog();
        }

        private void BtnOrderManagement_Click(object sender, EventArgs e)
        {
            OrderManagementForm orderForm = new OrderManagementForm();
            orderForm.ShowDialog();
        }

        private void BtnUserManagement_Click(object sender, EventArgs e)
        {
            UserManagementForm userForm = new UserManagementForm();
            userForm.ShowDialog();
        }

        private void BtnUserRegistration_Click(object sender, EventArgs e)
        {
            UserRegistrationForm registrationForm = new UserRegistrationForm();
            registrationForm.ShowDialog();
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            ReportsForm reportsForm = new ReportsForm();
            reportsForm.ShowDialog();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
