#nullable disable
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class GDNManagementForm : Form
    {
        private readonly InventoryManager _inventoryManager = new InventoryManager();
        private readonly ProductManager _productManager = new ProductManager();
        private readonly UserManager _userManager = new UserManager();
        
        private GoodsDeliveryNote _selectedGDN = null;
        private User _currentUser;

        private DataGridView dataGridViewGDNs, dataGridViewGDNItems;
        private TextBox txtGDNNumber, txtOrderID, txtCustomerID, txtDeliveryAddress, txtNotes;
        private ComboBox cmbStatus;
        private DateTimePicker dtpDeliveryDate;
        private Button btnCreateNew, btnPost, btnRefresh, btnViewItems;
        private Label lblStatus;

        public GDNManagementForm(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            LoadGDNs();
        }

        private void InitializeComponent()
        {
            this.Text = "Goods Delivery Notes (GDN) - AWE Electronics";
            this.Size = new System.Drawing.Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // GDN List Panel
            Label lblGDNList = new Label
            {
                Text = "GDN List",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(200, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            this.dataGridViewGDNs = new DataGridView
            {
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(700, 300),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.dataGridViewGDNs.SelectionChanged += DataGridViewGDNs_SelectionChanged;

            // GDN Items Panel
            Label lblGDNItems = new Label
            {
                Text = "GDN Items",
                Location = new System.Drawing.Point(10, 350),
                Size = new System.Drawing.Size(200, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            this.dataGridViewGDNItems = new DataGridView
            {
                Location = new System.Drawing.Point(10, 380),
                Size = new System.Drawing.Size(700, 260),
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            // Details Panel
            int leftMargin = 730;
            int topStart = 40;
            int spacing = 35;

            Label lblTitle = new Label
            {
                Text = "GDN Details",
                Location = new System.Drawing.Point(leftMargin, topStart),
                Size = new System.Drawing.Size(350, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            // GDN Number
            Label lblGDNNumber = new Label { Text = "GDN Number:", Location = new System.Drawing.Point(leftMargin, topStart + spacing), AutoSize = true };
            this.txtGDNNumber = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing + 20), Width = 350, ReadOnly = true };

            // Order ID
            Label lblOrderID = new Label { Text = "Order ID:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 2 + 20), AutoSize = true };
            this.txtOrderID = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 2 + 40), Width = 350, ReadOnly = true };

            // Customer ID
            Label lblCustomerID = new Label { Text = "Customer ID:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 3 + 40), AutoSize = true };
            this.txtCustomerID = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 3 + 60), Width = 350, ReadOnly = true };

            // Delivery Date
            Label lblDeliveryDate = new Label { Text = "Delivery Date:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 4 + 60), AutoSize = true };
            this.dtpDeliveryDate = new DateTimePicker { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 4 + 80), Width = 350, Enabled = false };

            // Status
            Label lblStatus = new Label { Text = "Status:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 5 + 80), AutoSize = true };
            this.cmbStatus = new ComboBox
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 5 + 100),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            this.cmbStatus.Items.AddRange(InventoryManager.ValidStatuses);

            // Delivery Address
            Label lblDeliveryAddress = new Label { Text = "Delivery Address:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 6 + 100), AutoSize = true };
            this.txtDeliveryAddress = new TextBox
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
            this.btnCreateNew = new Button
            {
                Text = "Create New GDN",
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 8 + 240),
                Width = 150
            };
            this.btnCreateNew.Click += BtnCreateNew_Click;

            this.btnPost = new Button
            {
                Text = "Post GDN",
                Location = new System.Drawing.Point(leftMargin + 160, topStart + spacing * 8 + 240),
                Width = 100
            };
            this.btnPost.Click += BtnPost_Click;

            this.btnViewItems = new Button
            {
                Text = "View Items",
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 9 + 240),
                Width = 120
            };
            this.btnViewItems.Click += BtnViewItems_Click;

            this.btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(leftMargin + 130, topStart + spacing * 9 + 240),
                Width = 100
            };
            this.btnRefresh.Click += (s, e) => LoadGDNs();

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 10 + 240),
                Size = new System.Drawing.Size(350, 60),
                ForeColor = System.Drawing.Color.Red,
                Text = ""
            };

            // Add controls
            this.Controls.Add(lblGDNList);
            this.Controls.Add(dataGridViewGDNs);
            this.Controls.Add(lblGDNItems);
            this.Controls.Add(dataGridViewGDNItems);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblGDNNumber);
            this.Controls.Add(txtGDNNumber);
            this.Controls.Add(lblOrderID);
            this.Controls.Add(txtOrderID);
            this.Controls.Add(lblCustomerID);
            this.Controls.Add(txtCustomerID);
            this.Controls.Add(lblDeliveryDate);
            this.Controls.Add(dtpDeliveryDate);
            this.Controls.Add(lblStatus);
            this.Controls.Add(cmbStatus);
            this.Controls.Add(lblDeliveryAddress);
            this.Controls.Add(txtDeliveryAddress);
            this.Controls.Add(lblNotes);
            this.Controls.Add(txtNotes);
            this.Controls.Add(btnCreateNew);
            this.Controls.Add(btnPost);
            this.Controls.Add(btnViewItems);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(this.lblStatus);
        }

        private void LoadGDNs()
        {
            try
            {
                List<GoodsDeliveryNote> gdns = _inventoryManager.GetAllGDNs();
                dataGridViewGDNs.DataSource = gdns;

                lblStatus.Text = $"Loaded {gdns.Count} GDNs.";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void DataGridViewGDNs_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewGDNs.SelectedRows.Count > 0)
            {
                _selectedGDN = (GoodsDeliveryNote)dataGridViewGDNs.SelectedRows[0].DataBoundItem;
                PopulateFields(_selectedGDN);
                LoadGDNItems(_selectedGDN.GDNID);
            }
        }

        private void PopulateFields(GoodsDeliveryNote gdn)
        {
            txtGDNNumber.Text = gdn.GDNNumber;
            txtOrderID.Text = gdn.OrderID?.ToString() ?? "N/A";
            txtCustomerID.Text = gdn.CustomerID?.ToString() ?? "N/A";
            dtpDeliveryDate.Value = gdn.DeliveryDate;
            cmbStatus.SelectedItem = gdn.Status;
            txtDeliveryAddress.Text = gdn.DeliveryAddress;
            txtNotes.Text = gdn.Notes;
        }

        private void LoadGDNItems(int gdnId)
        {
            try
            {
                List<GDNItem> items = _inventoryManager.GetGDNItems(gdnId);
                dataGridViewGDNItems.DataSource = items;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error loading items: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void BtnCreateNew_Click(object sender, EventArgs e)
        {
            // Open a dialog to create new GDN
            MessageBox.Show("Create New GDN feature would open a detailed form to select products and quantities for delivery.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPost_Click(object sender, EventArgs e)
        {
            if (_selectedGDN == null)
            {
                lblStatus.Text = "Please select a GDN to post.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (_selectedGDN.Status == "Posted")
            {
                lblStatus.Text = "This GDN is already posted.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to post GDN '{_selectedGDN.GDNNumber}'? This will reduce stock quantities.",
                "Confirm Post",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = _inventoryManager.PostGDN(_selectedGDN.GDNID);
                    if (success)
                    {
                        lblStatus.Text = "GDN posted successfully! Stock quantities updated.";
                        lblStatus.ForeColor = System.Drawing.Color.Green;
                        LoadGDNs();
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.Text = $"Error: {ex.Message}";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private void BtnViewItems_Click(object sender, EventArgs e)
        {
            if (_selectedGDN == null)
            {
                lblStatus.Text = "Please select a GDN to view items.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            LoadGDNItems(_selectedGDN.GDNID);
        }
    }
}
