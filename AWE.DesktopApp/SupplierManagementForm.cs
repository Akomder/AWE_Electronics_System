#nullable disable
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class SupplierManagementForm : Form
    {
        private readonly SupplierManager _supplierManager = new SupplierManager();
        private Supplier _selectedSupplier = null;

        private DataGridView dataGridViewSuppliers;
        private TextBox txtSupplierName, txtContactPerson, txtEmail, txtPhone, txtAddress, txtCity, txtState, txtPostalCode, txtCountry;
        private CheckBox chkIsActive;
        private Button btnAdd, btnUpdate, btnDelete, btnClear, btnRefresh;
        private Label lblStatus;

        public SupplierManagementForm()
        {
            InitializeComponent();
            LoadSuppliers();
        }

        private void InitializeComponent()
        {
            this.Text = "Supplier Management - AWE Electronics";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // DataGridView
            this.dataGridViewSuppliers = new DataGridView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(750, 600),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.dataGridViewSuppliers.SelectionChanged += DataGridViewSuppliers_SelectionChanged;

            // Input Panel
            int leftMargin = 780;
            int topStart = 20;
            int spacing = 35;

            Label lblTitle = new Label
            {
                Text = "Supplier Details",
                Location = new System.Drawing.Point(leftMargin, topStart),
                Size = new System.Drawing.Size(350, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            // Supplier Name
            Label lblSupplierName = new Label { Text = "Supplier Name:", Location = new System.Drawing.Point(leftMargin, topStart + spacing), AutoSize = true };
            this.txtSupplierName = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing + 20), Width = 350 };

            // Contact Person
            Label lblContactPerson = new Label { Text = "Contact Person:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 2 + 20), AutoSize = true };
            this.txtContactPerson = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 2 + 40), Width = 350 };

            // Email
            Label lblEmail = new Label { Text = "Email:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 3 + 40), AutoSize = true };
            this.txtEmail = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 3 + 60), Width = 350 };

            // Phone
            Label lblPhone = new Label { Text = "Phone:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 4 + 60), AutoSize = true };
            this.txtPhone = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 4 + 80), Width = 350 };

            // Address
            Label lblAddress = new Label { Text = "Address:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 5 + 80), AutoSize = true };
            this.txtAddress = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 5 + 100), Width = 350 };

            // City
            Label lblCity = new Label { Text = "City:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 6 + 100), AutoSize = true };
            this.txtCity = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 6 + 120), Width = 170 };

            // State
            Label lblState = new Label { Text = "State:", Location = new System.Drawing.Point(leftMargin + 180, topStart + spacing * 6 + 100), AutoSize = true };
            this.txtState = new TextBox { Location = new System.Drawing.Point(leftMargin + 180, topStart + spacing * 6 + 120), Width = 170 };

            // Postal Code
            Label lblPostalCode = new Label { Text = "Postal Code:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 7 + 120), AutoSize = true };
            this.txtPostalCode = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 7 + 140), Width = 170 };

            // Country
            Label lblCountry = new Label { Text = "Country:", Location = new System.Drawing.Point(leftMargin + 180, topStart + spacing * 7 + 120), AutoSize = true };
            this.txtCountry = new TextBox { Location = new System.Drawing.Point(leftMargin + 180, topStart + spacing * 7 + 140), Width = 170 };

            // Is Active
            this.chkIsActive = new CheckBox 
            { 
                Text = "Active", 
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 8 + 140), 
                AutoSize = true,
                Checked = true
            };

            // Buttons
            this.btnAdd = new Button { Text = "Add", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 9 + 140), Width = 80 };
            this.btnAdd.Click += BtnAdd_Click;

            this.btnUpdate = new Button { Text = "Update", Location = new System.Drawing.Point(leftMargin + 90, topStart + spacing * 9 + 140), Width = 80 };
            this.btnUpdate.Click += BtnUpdate_Click;

            this.btnDelete = new Button { Text = "Deactivate", Location = new System.Drawing.Point(leftMargin + 180, topStart + spacing * 9 + 140), Width = 100 };
            this.btnDelete.Click += BtnDelete_Click;

            this.btnClear = new Button { Text = "Clear", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 10 + 140), Width = 80 };
            this.btnClear.Click += (s, e) => ClearFields();

            this.btnRefresh = new Button { Text = "Refresh", Location = new System.Drawing.Point(leftMargin + 90, topStart + spacing * 10 + 140), Width = 80 };
            this.btnRefresh.Click += (s, e) => LoadSuppliers();

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 11 + 140),
                Size = new System.Drawing.Size(350, 40),
                ForeColor = System.Drawing.Color.Red,
                Text = ""
            };

            // Add controls
            this.Controls.Add(dataGridViewSuppliers);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSupplierName);
            this.Controls.Add(txtSupplierName);
            this.Controls.Add(lblContactPerson);
            this.Controls.Add(txtContactPerson);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPhone);
            this.Controls.Add(txtPhone);
            this.Controls.Add(lblAddress);
            this.Controls.Add(txtAddress);
            this.Controls.Add(lblCity);
            this.Controls.Add(txtCity);
            this.Controls.Add(lblState);
            this.Controls.Add(txtState);
            this.Controls.Add(lblPostalCode);
            this.Controls.Add(txtPostalCode);
            this.Controls.Add(lblCountry);
            this.Controls.Add(txtCountry);
            this.Controls.Add(chkIsActive);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnUpdate);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnClear);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(lblStatus);
        }

        private void LoadSuppliers()
        {
            try
            {
                List<Supplier> suppliers = _supplierManager.GetAllSuppliers();
                dataGridViewSuppliers.DataSource = suppliers;

                lblStatus.Text = $"Loaded {suppliers.Count} suppliers.";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void DataGridViewSuppliers_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewSuppliers.SelectedRows.Count > 0)
            {
                _selectedSupplier = (Supplier)dataGridViewSuppliers.SelectedRows[0].DataBoundItem;
                PopulateFields(_selectedSupplier);
            }
        }

        private void PopulateFields(Supplier supplier)
        {
            txtSupplierName.Text = supplier.SupplierName;
            txtContactPerson.Text = supplier.ContactPerson;
            txtEmail.Text = supplier.Email;
            txtPhone.Text = supplier.Phone;
            txtAddress.Text = supplier.Address;
            txtCity.Text = supplier.City;
            txtState.Text = supplier.State;
            txtPostalCode.Text = supplier.PostalCode;
            txtCountry.Text = supplier.Country;
            chkIsActive.Checked = supplier.IsActive;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Supplier supplier = new Supplier
                {
                    SupplierName = txtSupplierName.Text.Trim(),
                    ContactPerson = txtContactPerson.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    City = txtCity.Text.Trim(),
                    State = txtState.Text.Trim(),
                    PostalCode = txtPostalCode.Text.Trim(),
                    Country = txtCountry.Text.Trim(),
                    IsActive = chkIsActive.Checked
                };

                int supplierId = _supplierManager.CreateSupplier(supplier);

                if (supplierId > 0)
                {
                    lblStatus.Text = "Supplier added successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    LoadSuppliers();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedSupplier == null)
            {
                lblStatus.Text = "Please select a supplier to update.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                _selectedSupplier.SupplierName = txtSupplierName.Text.Trim();
                _selectedSupplier.ContactPerson = txtContactPerson.Text.Trim();
                _selectedSupplier.Email = txtEmail.Text.Trim();
                _selectedSupplier.Phone = txtPhone.Text.Trim();
                _selectedSupplier.Address = txtAddress.Text.Trim();
                _selectedSupplier.City = txtCity.Text.Trim();
                _selectedSupplier.State = txtState.Text.Trim();
                _selectedSupplier.PostalCode = txtPostalCode.Text.Trim();
                _selectedSupplier.Country = txtCountry.Text.Trim();
                _selectedSupplier.IsActive = chkIsActive.Checked;

                bool success = _supplierManager.UpdateSupplier(_selectedSupplier);

                if (success)
                {
                    lblStatus.Text = "Supplier updated successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    LoadSuppliers();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedSupplier == null)
            {
                lblStatus.Text = "Please select a supplier to deactivate.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to deactivate supplier '{_selectedSupplier.SupplierName}'?",
                "Confirm Deactivation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = _supplierManager.DeleteSupplier(_selectedSupplier.SupplierID);
                    if (success)
                    {
                        lblStatus.Text = "Supplier deactivated successfully!";
                        lblStatus.ForeColor = System.Drawing.Color.Green;
                        LoadSuppliers();
                        ClearFields();
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.Text = $"Error: {ex.Message}";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private void ClearFields()
        {
            txtSupplierName.Clear();
            txtContactPerson.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtCity.Clear();
            txtState.Clear();
            txtPostalCode.Clear();
            txtCountry.Clear();
            chkIsActive.Checked = true;
            _selectedSupplier = null;
            dataGridViewSuppliers.ClearSelection();
        }
    }
}
