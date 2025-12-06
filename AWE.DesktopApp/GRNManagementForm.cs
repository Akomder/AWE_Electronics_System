#nullable disable
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class GRNManagementForm : Form
    {
        private readonly InventoryManager _inventoryManager = new InventoryManager();
        private readonly SupplierManager _supplierManager = new SupplierManager();
        private readonly ProductManager _productManager = new ProductManager();
        private readonly UserManager _userManager = new UserManager();
        
        private GoodsReceivedNote _selectedGRN = null;
        private User _currentUser;

        private DataGridView dataGridViewGRNs, dataGridViewGRNItems;
        private TextBox txtGRNNumber, txtTotalAmount, txtNotes;
        private ComboBox cmbSupplier, cmbStatus;
        private DateTimePicker dtpReceivedDate;
        private Button btnCreateNew, btnPost, btnRefresh, btnViewItems;
        private Label lblStatus;

        public GRNManagementForm(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            LoadSuppliers();
            LoadGRNs();
        }

        private void InitializeComponent()
        {
            this.Text = "Goods Received Notes (GRN) - AWE Electronics";
            this.Size = new System.Drawing.Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // GRN List Panel
            Label lblGRNList = new Label
            {
                Text = "GRN List",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(200, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            this.dataGridViewGRNs = new DataGridView
            {
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(700, 300),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.dataGridViewGRNs.SelectionChanged += DataGridViewGRNs_SelectionChanged;

            // GRN Items Panel
            Label lblGRNItems = new Label
            {
                Text = "GRN Items",
                Location = new System.Drawing.Point(10, 350),
                Size = new System.Drawing.Size(200, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            this.dataGridViewGRNItems = new DataGridView
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
                Text = "GRN Details",
                Location = new System.Drawing.Point(leftMargin, topStart),
                Size = new System.Drawing.Size(350, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            // GRN Number
            Label lblGRNNumber = new Label { Text = "GRN Number:", Location = new System.Drawing.Point(leftMargin, topStart + spacing), AutoSize = true };
            this.txtGRNNumber = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing + 20), Width = 350, ReadOnly = true };

            // Supplier
            Label lblSupplier = new Label { Text = "Supplier:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 2 + 20), AutoSize = true };
            this.cmbSupplier = new ComboBox
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 2 + 40),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            // Received Date
            Label lblReceivedDate = new Label { Text = "Received Date:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 3 + 40), AutoSize = true };
            this.dtpReceivedDate = new DateTimePicker { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 3 + 60), Width = 350, Enabled = false };

            // Status
            Label lblStatus = new Label { Text = "Status:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 4 + 60), AutoSize = true };
            this.cmbStatus = new ComboBox
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 4 + 80),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            this.cmbStatus.Items.AddRange(InventoryManager.ValidStatuses);

            // Total Amount
            Label lblTotalAmount = new Label { Text = "Total Amount:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 5 + 80), AutoSize = true };
            this.txtTotalAmount = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 5 + 100), Width = 350, ReadOnly = true };

            // Notes
            Label lblNotes = new Label { Text = "Notes:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 6 + 100), AutoSize = true };
            this.txtNotes = new TextBox
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 6 + 120),
                Width = 350,
                Height = 80,
                Multiline = true,
                ReadOnly = true
            };

            // Buttons
            this.btnCreateNew = new Button
            {
                Text = "Create New GRN",
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 7 + 200),
                Width = 150
            };
            this.btnCreateNew.Click += BtnCreateNew_Click;

            this.btnPost = new Button
            {
                Text = "Post GRN",
                Location = new System.Drawing.Point(leftMargin + 160, topStart + spacing * 7 + 200),
                Width = 100
            };
            this.btnPost.Click += BtnPost_Click;

            this.btnViewItems = new Button
            {
                Text = "View Items",
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 8 + 200),
                Width = 120
            };
            this.btnViewItems.Click += BtnViewItems_Click;

            this.btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new System.Drawing.Point(leftMargin + 130, topStart + spacing * 8 + 200),
                Width = 100
            };
            this.btnRefresh.Click += (s, e) => LoadGRNs();

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 9 + 200),
                Size = new System.Drawing.Size(350, 60),
                ForeColor = System.Drawing.Color.Red,
                Text = ""
            };

            // Add controls
            this.Controls.Add(lblGRNList);
            this.Controls.Add(dataGridViewGRNs);
            this.Controls.Add(lblGRNItems);
            this.Controls.Add(dataGridViewGRNItems);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblGRNNumber);
            this.Controls.Add(txtGRNNumber);
            this.Controls.Add(lblSupplier);
            this.Controls.Add(cmbSupplier);
            this.Controls.Add(lblReceivedDate);
            this.Controls.Add(dtpReceivedDate);
            this.Controls.Add(lblStatus);
            this.Controls.Add(cmbStatus);
            this.Controls.Add(lblTotalAmount);
            this.Controls.Add(txtTotalAmount);
            this.Controls.Add(lblNotes);
            this.Controls.Add(txtNotes);
            this.Controls.Add(btnCreateNew);
            this.Controls.Add(btnPost);
            this.Controls.Add(btnViewItems);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(this.lblStatus);
        }

        private void LoadSuppliers()
        {
            try
            {
                List<Supplier> suppliers = _supplierManager.GetActiveSuppliers();
                cmbSupplier.DataSource = suppliers;
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierID";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error loading suppliers: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void LoadGRNs()
        {
            try
            {
                List<GoodsReceivedNote> grns = _inventoryManager.GetAllGRNs();
                dataGridViewGRNs.DataSource = grns;

                lblStatus.Text = $"Loaded {grns.Count} GRNs.";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void DataGridViewGRNs_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewGRNs.SelectedRows.Count > 0)
            {
                _selectedGRN = (GoodsReceivedNote)dataGridViewGRNs.SelectedRows[0].DataBoundItem;
                PopulateFields(_selectedGRN);
                LoadGRNItems(_selectedGRN.GRNID);
            }
        }

        private void PopulateFields(GoodsReceivedNote grn)
        {
            txtGRNNumber.Text = grn.GRNNumber;
            cmbSupplier.SelectedValue = grn.SupplierID;
            dtpReceivedDate.Value = grn.ReceivedDate;
            cmbStatus.SelectedItem = grn.Status;
            txtTotalAmount.Text = grn.TotalAmount.ToString("C");
            txtNotes.Text = grn.Notes;
        }

        private void LoadGRNItems(int grnId)
        {
            try
            {
                List<GRNItem> items = _inventoryManager.GetGRNItems(grnId);
                dataGridViewGRNItems.DataSource = items;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error loading items: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void BtnCreateNew_Click(object sender, EventArgs e)
        {
            // Open a dialog to create new GRN
            MessageBox.Show("Create New GRN feature would open a detailed form to add products and quantities.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPost_Click(object sender, EventArgs e)
        {
            if (_selectedGRN == null)
            {
                lblStatus.Text = "Please select a GRN to post.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (_selectedGRN.Status == "Posted")
            {
                lblStatus.Text = "This GRN is already posted.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to post GRN '{_selectedGRN.GRNNumber}'? This will update stock quantities.",
                "Confirm Post",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = _inventoryManager.PostGRN(_selectedGRN.GRNID);
                    if (success)
                    {
                        lblStatus.Text = "GRN posted successfully! Stock quantities updated.";
                        lblStatus.ForeColor = System.Drawing.Color.Green;
                        LoadGRNs();
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
            if (_selectedGRN == null)
            {
                lblStatus.Text = "Please select a GRN to view items.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            LoadGRNItems(_selectedGRN.GRNID);
        }
    }
}
