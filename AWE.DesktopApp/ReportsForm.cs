#nullable disable
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class ReportsForm : Form
    {
        private readonly InventoryManager _inventoryManager = new InventoryManager();
        private readonly ProductManager _productManager = new ProductManager();
        private readonly OrderManager _orderManager = new OrderManager();

        private TabControl tabControl;
        private DataGridView dgvLowStock, dgvOutOfStock, dgvAllProducts, dgvOrders;
        private Label lblLowStockStatus, lblOutOfStockStatus, lblProductsStatus, lblOrdersStatus;
        private Button btnRefreshLowStock, btnRefreshOutOfStock, btnRefreshProducts, btnRefreshOrders;
        private ComboBox cmbOrderStatus;

        public ReportsForm()
        {
            InitializeComponent();
            LoadAllReports();
        }

        private void InitializeComponent()
        {
            this.Text = "Reports - AWE Electronics";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Tab Control
            this.tabControl = new TabControl
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(1160, 630)
            };

            // === LOW STOCK TAB ===
            TabPage tabLowStock = new TabPage("Low Stock Products");
            
            this.dgvLowStock = new DataGridView
            {
                Location = new System.Drawing.Point(10, 50),
                Size = new System.Drawing.Size(1120, 500),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.btnRefreshLowStock = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(10, 10),
                Width = 100
            };
            this.btnRefreshLowStock.Click += (s, e) => LoadLowStockReport();

            this.lblLowStockStatus = new Label
            {
                Location = new System.Drawing.Point(120, 15),
                Size = new System.Drawing.Size(900, 20),
                ForeColor = System.Drawing.Color.Green
            };

            tabLowStock.Controls.Add(btnRefreshLowStock);
            tabLowStock.Controls.Add(lblLowStockStatus);
            tabLowStock.Controls.Add(dgvLowStock);

            // === OUT OF STOCK TAB ===
            TabPage tabOutOfStock = new TabPage("Out of Stock Products");
            
            this.dgvOutOfStock = new DataGridView
            {
                Location = new System.Drawing.Point(10, 50),
                Size = new System.Drawing.Size(1120, 500),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.btnRefreshOutOfStock = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(10, 10),
                Width = 100
            };
            this.btnRefreshOutOfStock.Click += (s, e) => LoadOutOfStockReport();

            this.lblOutOfStockStatus = new Label
            {
                Location = new System.Drawing.Point(120, 15),
                Size = new System.Drawing.Size(900, 20),
                ForeColor = System.Drawing.Color.Green
            };

            tabOutOfStock.Controls.Add(btnRefreshOutOfStock);
            tabOutOfStock.Controls.Add(lblOutOfStockStatus);
            tabOutOfStock.Controls.Add(dgvOutOfStock);

            // === ALL PRODUCTS TAB ===
            TabPage tabAllProducts = new TabPage("All Products");
            
            this.dgvAllProducts = new DataGridView
            {
                Location = new System.Drawing.Point(10, 50),
                Size = new System.Drawing.Size(1120, 500),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.btnRefreshProducts = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(10, 10),
                Width = 100
            };
            this.btnRefreshProducts.Click += (s, e) => LoadAllProductsReport();

            this.lblProductsStatus = new Label
            {
                Location = new System.Drawing.Point(120, 15),
                Size = new System.Drawing.Size(900, 20),
                ForeColor = System.Drawing.Color.Green
            };

            tabAllProducts.Controls.Add(btnRefreshProducts);
            tabAllProducts.Controls.Add(lblProductsStatus);
            tabAllProducts.Controls.Add(dgvAllProducts);

            // === ORDERS TAB ===
            TabPage tabOrders = new TabPage("Orders Report");
            
            Label lblOrderFilter = new Label
            {
                Text = "Filter by Status:",
                Location = new System.Drawing.Point(10, 15),
                AutoSize = true
            };

            this.cmbOrderStatus = new ComboBox
            {
                Location = new System.Drawing.Point(120, 12),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.cmbOrderStatus.Items.AddRange(new string[] { "All", "Pending", "Processing", "Shipped", "Delivered", "Cancelled" });
            this.cmbOrderStatus.SelectedIndex = 0;
            this.cmbOrderStatus.SelectedIndexChanged += (s, e) => LoadOrdersReport();

            this.dgvOrders = new DataGridView
            {
                Location = new System.Drawing.Point(10, 50),
                Size = new System.Drawing.Size(1120, 500),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.btnRefreshOrders = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(280, 10),
                Width = 100
            };
            this.btnRefreshOrders.Click += (s, e) => LoadOrdersReport();

            this.lblOrdersStatus = new Label
            {
                Location = new System.Drawing.Point(10, 560),
                Size = new System.Drawing.Size(1100, 20),
                ForeColor = System.Drawing.Color.Green
            };

            tabOrders.Controls.Add(lblOrderFilter);
            tabOrders.Controls.Add(cmbOrderStatus);
            tabOrders.Controls.Add(btnRefreshOrders);
            tabOrders.Controls.Add(dgvOrders);
            tabOrders.Controls.Add(lblOrdersStatus);

            // Add tabs to control
            tabControl.TabPages.Add(tabLowStock);
            tabControl.TabPages.Add(tabOutOfStock);
            tabControl.TabPages.Add(tabAllProducts);
            tabControl.TabPages.Add(tabOrders);

            this.Controls.Add(tabControl);
        }

        private void LoadAllReports()
        {
            LoadLowStockReport();
            LoadOutOfStockReport();
            LoadAllProductsReport();
            LoadOrdersReport();
        }

        private void LoadLowStockReport()
        {
            try
            {
                List<Product> products = _inventoryManager.GetLowStockProducts();
                dgvLowStock.DataSource = products;

                lblLowStockStatus.Text = $"Found {products.Count} products with low stock (at or below reorder level).";
                lblLowStockStatus.ForeColor = products.Count > 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblLowStockStatus.Text = $"Error: {ex.Message}";
                lblLowStockStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void LoadOutOfStockReport()
        {
            try
            {
                List<Product> products = _inventoryManager.GetOutOfStockProducts();
                dgvOutOfStock.DataSource = products;

                lblOutOfStockStatus.Text = $"Found {products.Count} products out of stock.";
                lblOutOfStockStatus.ForeColor = products.Count > 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblOutOfStockStatus.Text = $"Error: {ex.Message}";
                lblOutOfStockStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void LoadAllProductsReport()
        {
            try
            {
                List<Product> products = _productManager.GetAllProducts();
                dgvAllProducts.DataSource = products;

                // Calculate summary statistics
                int activeCount = 0;
                int inactiveCount = 0;
                decimal totalValue = 0;

                foreach (var product in products)
                {
                    if (product.IsActive)
                    {
                        activeCount++;
                        totalValue += product.Price * product.StockQuantity;
                    }
                    else
                    {
                        inactiveCount++;
                    }
                }

                lblProductsStatus.Text = $"Total: {products.Count} products | Active: {activeCount} | Inactive: {inactiveCount} | Total Inventory Value: {totalValue:C}";
                lblProductsStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblProductsStatus.Text = $"Error: {ex.Message}";
                lblProductsStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void LoadOrdersReport()
        {
            try
            {
                string selectedStatus = cmbOrderStatus.SelectedItem?.ToString();
                List<Order> orders;

                if (string.IsNullOrEmpty(selectedStatus) || selectedStatus == "All")
                {
                    orders = _orderManager.GetAllOrders();
                }
                else
                {
                    orders = _orderManager.GetOrdersByStatus(selectedStatus);
                }

                dgvOrders.DataSource = orders;

                // Calculate summary statistics
                decimal totalRevenue = 0;
                foreach (var order in orders)
                {
                    totalRevenue += order.TotalAmount;
                }

                lblOrdersStatus.Text = $"Total Orders: {orders.Count} | Total Revenue: {totalRevenue:C}";
                lblOrdersStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblOrdersStatus.Text = $"Error: {ex.Message}";
                lblOrdersStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
