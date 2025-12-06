#nullable disable
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class ProductManagementForm : Form
    {
        private readonly ProductManager _productManager = new ProductManager();
        private Product _selectedProduct = null;

        // UI Controls
        private DataGridView dataGridViewProducts;
        private TextBox txtProductName;
        private TextBox txtPrice;
        private TextBox txtStock;
        private TextBox txtReorderLevel;
        private TextBox txtDescription;
        private TextBox txtCategoryID;
        private TextBox txtSupplierID;
        private TextBox txtSKU;
        private TextBox txtImageURL;
        private TextBox txtWeight;
        private TextBox txtDimensions;
        private TextBox txtWarranty;
        private CheckBox chkIsActive;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private Button btnRefresh;
        private Label lblStatus;

        public ProductManagementForm()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void InitializeComponent()
        {
            this.Text = "Product Management - AWE Electronics";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title Label
            Label lblTitle = new Label
            {
                Text = "Product Inventory Management",
                Location = new System.Drawing.Point(20, 10),
                Size = new System.Drawing.Size(400, 30),
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold)
            };

            // DataGridView for displaying products
            this.dataGridViewProducts = new DataGridView
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(1140, 300),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.dataGridViewProducts.SelectionChanged += DataGridViewProducts_SelectionChanged;

            // Product Details Panel
            int yPos = 360;
            Label lblDetails = new Label
            {
                Text = "Product Details:",
                Location = new System.Drawing.Point(20, yPos),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            // Row 1: Product Name, Price, Stock
            yPos += 30;
            this.txtProductName = CreateLabeledTextBox(this, "Product Name:", 20, yPos, 300);
            this.txtPrice = CreateLabeledTextBox(this, "Price ($):", 350, yPos, 150);
            this.txtStock = CreateLabeledTextBox(this, "Stock Qty:", 530, yPos, 100);
            this.txtReorderLevel = CreateLabeledTextBox(this, "Reorder Level:", 660, yPos, 100);

            // Row 2: Category, Supplier, SKU
            yPos += 40;
            this.txtCategoryID = CreateLabeledTextBox(this, "Category ID:", 20, yPos, 150);
            this.txtSupplierID = CreateLabeledTextBox(this, "Supplier ID:", 200, yPos, 150);
            this.txtSKU = CreateLabeledTextBox(this, "SKU:", 380, yPos, 200);

            // Row 3: Weight, Dimensions, Warranty
            yPos += 40;
            this.txtWeight = CreateLabeledTextBox(this, "Weight (kg):", 20, yPos, 150);
            this.txtDimensions = CreateLabeledTextBox(this, "Dimensions:", 200, yPos, 200);
            this.txtWarranty = CreateLabeledTextBox(this, "Warranty:", 430, yPos, 200);

            // Row 4: Image URL
            yPos += 40;
            this.txtImageURL = CreateLabeledTextBox(this, "Image URL:", 20, yPos, 600);

            // Row 5: Description (larger)
            yPos += 40;
            Label lblDescription = new Label
            {
                Text = "Description:",
                Location = new System.Drawing.Point(20, yPos),
                AutoSize = true
            };
            this.txtDescription = new TextBox
            {
                Location = new System.Drawing.Point(120, yPos),
                Size = new System.Drawing.Size(500, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // IsActive Checkbox
            this.chkIsActive = new CheckBox
            {
                Text = "Active",
                Location = new System.Drawing.Point(650, yPos),
                Checked = true
            };

            // Action Buttons
            yPos += 70;
            this.btnAdd = CreateButton(this, "Add New Product", 20, yPos, 120, btnAdd_Click);
            this.btnUpdate = CreateButton(this, "Update Product", 150, yPos, 120, btnUpdate_Click);
            this.btnDelete = CreateButton(this, "Delete Product", 280, yPos, 120, btnDelete_Click);
            this.btnClear = CreateButton(this, "Clear Form", 410, yPos, 120, btnClear_Click);
            this.btnRefresh = CreateButton(this, "Refresh List", 540, yPos, 120, btnRefresh_Click);

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(20, yPos + 40),
                Size = new System.Drawing.Size(1140, 20),
                ForeColor = System.Drawing.Color.Blue,
                Text = "Ready"
            };

            // Add all controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(this.dataGridViewProducts);
            this.Controls.Add(lblDetails);
            this.Controls.Add(lblDescription);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.chkIsActive);
            this.Controls.Add(this.lblStatus);
        }

        // Helper method to create a labeled textbox
        private TextBox CreateLabeledTextBox(Form form, string labelText, int x, int y, int width)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(90, 20)
            };
            TextBox txt = new TextBox
            {
                Location = new System.Drawing.Point(x + 100, y),
                Width = width - 100
            };
            form.Controls.Add(lbl);
            form.Controls.Add(txt);
            return txt;
        }

        // Helper method to create a button
        private Button CreateButton(Form form, string text, int x, int y, int width, EventHandler clickHandler)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Width = width,
                Height = 30
            };
            btn.Click += clickHandler;
            form.Controls.Add(btn);
            return btn;
        }

        private void LoadProducts()
        {
            try
            {
                lblStatus.Text = "Loading products...";
                lblStatus.ForeColor = System.Drawing.Color.Blue;
                Application.DoEvents();

                List<Product> products = _productManager.GetAllProducts();
                this.dataGridViewProducts.DataSource = products;

                // Hide unnecessary columns
                if (this.dataGridViewProducts.Columns.Contains("CreatedDate"))
                    this.dataGridViewProducts.Columns["CreatedDate"].Visible = false;
                if (this.dataGridViewProducts.Columns.Contains("LastUpdated"))
                    this.dataGridViewProducts.Columns["LastUpdated"].Visible = false;
                if (this.dataGridViewProducts.Columns.Contains("ImageURL"))
                    this.dataGridViewProducts.Columns["ImageURL"].Visible = false;

                lblStatus.Text = $"Loaded {products.Count} product(s)";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading products";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show($"Error loading products:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridViewProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dataGridViewProducts.SelectedRows.Count > 0)
            {
                _selectedProduct = this.dataGridViewProducts.SelectedRows[0].DataBoundItem as Product;
                if (_selectedProduct != null)
                {
                    PopulateFormFields(_selectedProduct);
                }
            }
        }

        private void PopulateFormFields(Product product)
        {
            txtProductName.Text = product.ProductName;
            txtPrice.Text = product.Price.ToString("F2");
            txtStock.Text = product.StockQuantity.ToString();
            txtReorderLevel.Text = product.ReorderLevel.ToString();
            txtDescription.Text = product.Description ?? "";
            txtCategoryID.Text = product.CategoryID.ToString();
            txtSupplierID.Text = product.SupplierID.ToString();
            txtSKU.Text = product.SKU ?? "";
            txtImageURL.Text = product.ImageURL ?? "";
            txtWeight.Text = product.Weight?.ToString("F2") ?? "";
            txtDimensions.Text = product.Dimensions ?? "";
            txtWarranty.Text = product.Warranty ?? "";
            chkIsActive.Checked = product.IsActive;
        }

        private Product GetProductFromForm()
        {
            return new Product
            {
                ProductID = _selectedProduct?.ProductID ?? 0,
                ProductName = txtProductName.Text.Trim(),
                Price = decimal.TryParse(txtPrice.Text, out decimal price) ? price : 0,
                StockQuantity = int.TryParse(txtStock.Text, out int stock) ? stock : 0,
                ReorderLevel = int.TryParse(txtReorderLevel.Text, out int reorder) ? reorder : 10,
                Description = txtDescription.Text.Trim(),
                CategoryID = int.TryParse(txtCategoryID.Text, out int catId) ? catId : 1,
                SupplierID = int.TryParse(txtSupplierID.Text, out int suppId) ? suppId : 1,
                SKU = txtSKU.Text.Trim(),
                ImageURL = txtImageURL.Text.Trim(),
                Weight = decimal.TryParse(txtWeight.Text, out decimal weight) ? (decimal?)weight : null,
                Dimensions = txtDimensions.Text.Trim(),
                Warranty = txtWarranty.Text.Trim(),
                IsActive = chkIsActive.Checked
            };
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Product newProduct = GetProductFromForm();

                bool newId = _productManager.CreateProduct(newProduct);
                if (newId)
                {
                    lblStatus.Text = $"Product '{newProduct.ProductName}' added successfully (ID: {newId})";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    MessageBox.Show($"Product added successfully!\nProduct ID: {newId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts();
                    btnClear_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error adding product";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show($"Error adding product:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Please select a product to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Product updatedProduct = GetProductFromForm();
                updatedProduct.ProductID = _selectedProduct.ProductID;

                if (_productManager.UpdateProduct(updatedProduct))
                {
                    lblStatus.Text = $"Product '{updatedProduct.ProductName}' updated successfully";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error updating product";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show($"Error updating product:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Please select a product to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete (deactivate) the product:\n\n'{_selectedProduct.ProductName}'?\n\nThis will set the product as inactive.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (_productManager.DeleteProduct(_selectedProduct.ProductID))
                    {
                        lblStatus.Text = $"Product '{_selectedProduct.ProductName}' deactivated successfully";
                        lblStatus.ForeColor = System.Drawing.Color.Green;
                        MessageBox.Show("Product deactivated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProducts();
                        btnClear_Click(null, null);
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Error deleting product";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    MessageBox.Show($"Error deleting product:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _selectedProduct = null;
            txtProductName.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            txtReorderLevel.Text = "10";
            txtDescription.Clear();
            txtCategoryID.Clear();
            txtSupplierID.Clear();
            txtSKU.Clear();
            txtImageURL.Clear();
            txtWeight.Clear();
            txtDimensions.Clear();
            txtWarranty.Clear();
            chkIsActive.Checked = true;
            this.dataGridViewProducts.ClearSelection();
            lblStatus.Text = "Form cleared - Ready to add new product";
            lblStatus.ForeColor = System.Drawing.Color.Blue;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }
    }
}
