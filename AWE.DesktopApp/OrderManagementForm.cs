#nullable disable
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class OrderManagementForm : Form
    {
        private readonly OrderManager _orderManager = new OrderManager();
        private Order _selectedOrder = null;

        private DataGridView dataGridViewOrders;
        private TextBox txtOrderID, txtCustomerID, txtTotalAmount, txtShippingAddress, txtNotes;
        private ComboBox cmbStatus, cmbFilterStatus;
        private DateTimePicker dtpOrderDate;
        private Button btnUpdateStatus, btnCancelOrder, btnRefresh, btnViewAll;
        private Label lblStatus;

        public OrderManagementForm()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void InitializeComponent()
        {
            this.Text = "Order Management - AWE Electronics";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Filter Panel
            Label lblFilter = new Label
            {
                Text = "Filter by Status:",
                Location = new System.Drawing.Point(10, 15),
                AutoSize = true
            };

            this.cmbFilterStatus = new ComboBox
            {
                Location = new System.Drawing.Point(120, 12),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.cmbFilterStatus.Items.AddRange(new string[] { "All", "Pending", "Processing", "Shipped", "Delivered", "Cancelled" });
            this.cmbFilterStatus.SelectedIndex = 0;
            this.cmbFilterStatus.SelectedIndexChanged += (s, e) => FilterOrders();

            this.btnViewAll = new Button
            {
                Text = "View All",
                Location = new System.Drawing.Point(280, 10),
                Width = 80
            };
            this.btnViewAll.Click += (s, e) => { cmbFilterStatus.SelectedIndex = 0; LoadOrders(); };

            this.btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(370, 10),
                Width = 80
            };
            this.btnRefresh.Click += (s, e) => LoadOrders();

            // DataGridView
            this.dataGridViewOrders = new DataGridView
            {
                Location = new System.Drawing.Point(10, 50),
                Size = new System.Drawing.Size(750, 580),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.dataGridViewOrders.SelectionChanged += DataGridViewOrders_SelectionChanged;

            // Details Panel
            int leftMargin = 780;
            int topStart = 50;
            int spacing = 35;

            Label lblTitle = new Label
            {
                Text = "Order Details",
                Location = new System.Drawing.Point(leftMargin, topStart),
                Size = new System.Drawing.Size(350, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            // Order ID
            Label lblOrderID = new Label { Text = "Order ID:", Location = new System.Drawing.Point(leftMargin, topStart + spacing), AutoSize = true };
            this.txtOrderID = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing + 20), Width = 350, ReadOnly = true };

            // Customer ID
            Label lblCustomerID = new Label { Text = "Customer ID:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 2 + 20), AutoSize = true };
            this.txtCustomerID = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 2 + 40), Width = 350, ReadOnly = true };

            // Order Date
            Label lblOrderDate = new Label { Text = "Order Date:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 3 + 40), AutoSize = true };
            this.dtpOrderDate = new DateTimePicker { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 3 + 60), Width = 350, Enabled = false };

            // Status
            Label lblStatus = new Label { Text = "Status:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 4 + 60), AutoSize = true };
            this.cmbStatus = new ComboBox
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 4 + 80),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.cmbStatus.Items.AddRange(OrderManager.ValidStatuses);

            // Total Amount
            Label lblTotalAmount = new Label { Text = "Total Amount:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 5 + 80), AutoSize = true };
            this.txtTotalAmount = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 5 + 100), Width = 350, ReadOnly = true };

            // Shipping Address
            Label lblShippingAddress = new Label { Text = "Shipping Address:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 6 + 100), AutoSize = true };
            this.txtShippingAddress = new TextBox
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 6 + 120),
                Width = 350,
                Height = 60,
                Multiline = true,
                ReadOnly = true
            };

            // Notes
            Label lblNotes = new Label { Text = "Notes:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 7 + 160), AutoSize = true };
            this.txtNotes = new TextBox
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 7 + 180),
                Width = 350,
                Height = 60,
                Multiline = true,
                ReadOnly = true
            };

            // Buttons
            this.btnUpdateStatus = new Button
            {
                Text = "Update Status",
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 8 + 240),
                Width = 120
            };
            this.btnUpdateStatus.Click += BtnUpdateStatus_Click;

            this.btnCancelOrder = new Button
            {
                Text = "Cancel Order",
                Location = new System.Drawing.Point(leftMargin + 130, topStart + spacing * 8 + 240),
                Width = 120
            };
            this.btnCancelOrder.Click += BtnCancelOrder_Click;

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 9 + 240),
                Size = new System.Drawing.Size(350, 60),
                ForeColor = System.Drawing.Color.Red,
                Text = ""
            };

            // Add controls
            this.Controls.Add(lblFilter);
            this.Controls.Add(cmbFilterStatus);
            this.Controls.Add(btnViewAll);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(dataGridViewOrders);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblOrderID);
            this.Controls.Add(txtOrderID);
            this.Controls.Add(lblCustomerID);
            this.Controls.Add(txtCustomerID);
            this.Controls.Add(lblOrderDate);
            this.Controls.Add(dtpOrderDate);
            this.Controls.Add(lblStatus);
            this.Controls.Add(cmbStatus);
            this.Controls.Add(lblTotalAmount);
            this.Controls.Add(txtTotalAmount);
            this.Controls.Add(lblShippingAddress);
            this.Controls.Add(txtShippingAddress);
            this.Controls.Add(lblNotes);
            this.Controls.Add(txtNotes);
            this.Controls.Add(btnUpdateStatus);
            this.Controls.Add(btnCancelOrder);
            this.Controls.Add(this.lblStatus);
        }

        private void LoadOrders()
        {
            try
            {
                List<Order> orders = _orderManager.GetAllOrders();
                dataGridViewOrders.DataSource = orders;

                lblStatus.Text = $"Loaded {orders.Count} orders.";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void FilterOrders()
        {
            try
            {
                string selectedStatus = cmbFilterStatus.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(selectedStatus) || selectedStatus == "All")
                {
                    LoadOrders();
                }
                else
                {
                    List<Order> orders = _orderManager.GetOrdersByStatus(selectedStatus);
                    dataGridViewOrders.DataSource = orders;

                    lblStatus.Text = $"Loaded {orders.Count} orders with status '{selectedStatus}'.";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void DataGridViewOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewOrders.SelectedRows.Count > 0)
            {
                _selectedOrder = (Order)dataGridViewOrders.SelectedRows[0].DataBoundItem;
                PopulateFields(_selectedOrder);
            }
        }

        private void PopulateFields(Order order)
        {
            txtOrderID.Text = order.OrderID.ToString();
            txtCustomerID.Text = order.CustomerID.ToString();
            dtpOrderDate.Value = order.OrderDate;
            cmbStatus.SelectedItem = order.Status;
            txtTotalAmount.Text = order.TotalAmount.ToString("C");
            txtShippingAddress.Text = $"{order.ShippingAddress}\n{order.ShippingCity}, {order.ShippingState} {order.ShippingPostalCode}";
            txtNotes.Text = order.Notes;
        }

        private void BtnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                lblStatus.Text = "Please select an order to update.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (cmbStatus.SelectedItem == null)
            {
                lblStatus.Text = "Please select a status.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                string newStatus = cmbStatus.SelectedItem.ToString();
                bool success = _orderManager.UpdateOrderStatus(_selectedOrder.OrderID, newStatus);

                if (success)
                {
                    lblStatus.Text = $"Order status updated to '{newStatus}' successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    LoadOrders();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void BtnCancelOrder_Click(object sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                lblStatus.Text = "Please select an order to cancel.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string reason = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter cancellation reason:",
                "Cancel Order",
                ""
            );

            if (string.IsNullOrWhiteSpace(reason))
            {
                return;
            }

            try
            {
                bool success = _orderManager.CancelOrder(_selectedOrder.OrderID, reason);

                if (success)
                {
                    lblStatus.Text = "Order cancelled successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    LoadOrders();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
