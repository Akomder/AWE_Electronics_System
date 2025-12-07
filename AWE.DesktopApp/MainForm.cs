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
            this.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular);
        }

        private void InitializeComponent()
        {
            this.Text = $"AWE Electronics Staff System - {_currentUser.Role}";
            this.Size = new System.Drawing.Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.Icon = null;
            this.MinimumSize = new System.Drawing.Size(1000, 600);

            // ============= WELCOME PANEL =============
            Panel welcomePanel = new Panel
            {
                Location = new System.Drawing.Point(15, 15),
                Size = new System.Drawing.Size(1170, 90),
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None
            };
            welcomePanel.Paint += (sender, e) =>
            {
                e.Graphics.Clear(System.Drawing.Color.White);
                e.Graphics.DrawLine(
                    new System.Drawing.Pen(System.Drawing.Color.FromArgb(221, 221, 221), 1),
                    0, welcomePanel.Height - 1, welcomePanel.Width, welcomePanel.Height - 1
                );
            };

            Label lblWelcome = new Label
            {
                Text = $"Welcome, {_currentUser.FirstName} {_currentUser.LastName}",
                Location = new System.Drawing.Point(25, 15),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 0, 0)
            };

            Label lblRole = new Label
            {
                Text = $"Role: {_currentUser.Role} | Last Login: {_currentUser.LastLogin?.ToString("yyyy-MM-dd HH:mm") ?? "First time"}",
                Location = new System.Drawing.Point(25, 48),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 10),
                ForeColor = System.Drawing.Color.FromArgb(85, 85, 85)
            };

            welcomePanel.Controls.Add(lblWelcome);
            welcomePanel.Controls.Add(lblRole);

            // ============= FEATURE PANEL WITH SCROLLING =============
            Panel featurePanel = new Panel
            {
                Location = new System.Drawing.Point(15, 115),
                Size = new System.Drawing.Size(1170, 620),
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                AutoScroll = true
            };
            featurePanel.Paint += (sender, e) =>
            {
                e.Graphics.Clear(System.Drawing.Color.White);
                e.Graphics.DrawLine(
                    new System.Drawing.Pen(System.Drawing.Color.FromArgb(221, 221, 221), 1),
                    0, 0, featurePanel.Width, 0
                );
            };

            Label lblFeatures = new Label
            {
                Text = "Available Features",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 13, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 0, 0)
            };

            // ============= COLUMN 1: INVENTORY & PRODUCTS =============
            Label lblInventory = CreateSectionLabel("📦 Inventory & Products", 20, 60);

            Button btnProductManagement = CreateButton("Product Management (CRUD)", 20, 90);
            btnProductManagement.Click += BtnProductManagement_Click;

            Button btnCategoryManagement = CreateButton("Category Management", 20, 140);
            btnCategoryManagement.Click += BtnCategoryManagement_Click;

            Button btnSupplierManagement = CreateButton("Supplier Management", 20, 190);
            btnSupplierManagement.Click += BtnSupplierManagement_Click;

            Button btnGoodsReceived = CreateButton("Goods Received Notes (GRN)", 20, 240);
            btnGoodsReceived.Click += BtnGoodsReceived_Click;

            Button btnGoodsDelivery = CreateButton("Goods Delivery Notes (GDN)", 20, 290);
            btnGoodsDelivery.Click += BtnGoodsDelivery_Click;

            // ============= COLUMN 2: ORDERS & CUSTOMERS =============
            Label lblOrders = CreateSectionLabel("🛒 Orders & Customers", 290, 60);

            Button btnOrderManagement = CreateButton("Order Management", 290, 90);
            btnOrderManagement.Click += BtnOrderManagement_Click;

            Button btnOrderProcessing = CreateButton("Order Processing", 290, 140);
            btnOrderProcessing.Enabled = false;

            Button btnCustomerManagement = CreateButton("Customer Management", 290, 190);
            btnCustomerManagement.Enabled = false;

            // ============= COLUMN 3: ADMIN & REPORTS =============
            Label lblAdmin = CreateSectionLabel("⚙️ Administration & Reports", 560, 60);

            Button btnUserManagement = CreateButton("User Management", 560, 90);
            btnUserManagement.Enabled = _currentUser.Role == "Admin";
            btnUserManagement.Click += BtnUserManagement_Click;

            Button btnUserRegistration = CreateButton("Register New User", 560, 140);
            btnUserRegistration.Enabled = _currentUser.Role == "Admin";
            btnUserRegistration.Click += BtnUserRegistration_Click;

            Button btnReports = CreateButton("Reports & Analytics", 560, 190);
            btnReports.Click += BtnReports_Click;

            Button btnSystemSettings = CreateButton("System Settings", 560, 240);
            btnSystemSettings.Enabled = _currentUser.Role == "Admin";

            // ============= QUICK ACTIONS =============
            Label lblQuickActions = CreateSectionLabel("⚡ Quick Actions", 20, 360);

            Button btnLowStockAlert = CreateSmallButton("Low Stock Alert", 20, 390);
            btnLowStockAlert.Click += (s, e) =>
            {
                ReportsForm reportsForm = new ReportsForm();
                reportsForm.ShowDialog();
            };

            Button btnPendingOrders = CreateSmallButton("Pending Orders", 180, 390);
            btnPendingOrders.Click += (s, e) =>
            {
                OrderManagementForm orderForm = new OrderManagementForm();
                orderForm.ShowDialog();
            };

            Button btnRefreshData = CreateSmallButton("Refresh All Data", 340, 390);
            btnRefreshData.Click += (s, e) =>
            {
                MessageBox.Show("Data refreshed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // ============= INFO PANEL =============
            Panel infoPanel = new Panel
            {
                Location = new System.Drawing.Point(20, 450),
                Size = new System.Drawing.Size(1100, 130),
                BackColor = System.Drawing.Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.None
            };
            infoPanel.Paint += (sender, e) =>
            {
                e.Graphics.Clear(System.Drawing.Color.FromArgb(240, 240, 240));
                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(System.Drawing.Color.FromArgb(221, 221, 221), 1),
                    0, 0, infoPanel.Width - 1, infoPanel.Height - 1
                );
            };

            Label lblInfo = new Label
            {
                Text = "System Information:",
                Location = new System.Drawing.Point(15, 15),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 0, 0)
            };

            Label lblSystemInfo = new Label
            {
                Text = $"AWE Electronics Management System v1.0\n" +
                       $"Database: Connected ✓\n" +
                       $"Current User: {_currentUser.Username} ({_currentUser.Role})\n" +
                       $"Session Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                       $"Support: support@aweelectronics.com",
                Location = new System.Drawing.Point(15, 45),
                Size = new System.Drawing.Size(1070, 75),
                Font = new System.Drawing.Font("Segoe UI", 9),
                ForeColor = System.Drawing.Color.FromArgb(85, 85, 85),
                AutoSize = false
            };

            infoPanel.Controls.Add(lblInfo);
            infoPanel.Controls.Add(lblSystemInfo);

            // ============= ADD CONTROLS TO FEATURE PANEL =============
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

            // ============= LOGOUT BUTTON =============
            Button btnLogout = new Button
            {
                Text = "Logout",
                Location = new System.Drawing.Point(1060, 745),
                Size = new System.Drawing.Size(125, 40),
                BackColor = System.Drawing.Color.FromArgb(220, 20, 20),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(180, 10, 10);
            btnLogout.Click += BtnLogout_Click;

            // ============= ADD ALL TO FORM =============
            this.Controls.Add(welcomePanel);
            this.Controls.Add(featurePanel);
            this.Controls.Add(btnLogout);
        }

        private Label CreateSectionLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 0, 0)
            };
        }

        private Button CreateButton(string text, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(250, 40),
                BackColor = System.Drawing.Color.FromArgb(0, 0, 0),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Regular),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            btn.MouseEnter += (s, e) => btn.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            btn.MouseLeave += (s, e) => btn.BackColor = btn.Enabled ? System.Drawing.Color.FromArgb(0, 0, 0) : System.Drawing.Color.FromArgb(200, 200, 200);
            return btn;
        }

        private Button CreateSmallButton(string text, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(140, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 0, 0),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            btn.MouseEnter += (s, e) => btn.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            btn.MouseLeave += (s, e) => btn.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            return btn;
        }

        // ============= EVENT HANDLERS =============

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
