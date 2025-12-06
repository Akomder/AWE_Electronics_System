#nullable disable
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class CategoryManagementForm : Form
    {
        private readonly CategoryManager _categoryManager = new CategoryManager();
        private Category _selectedCategory = null;

        private DataGridView dataGridViewCategories;
        private TextBox txtCategoryName, txtDescription;
        private CheckBox chkIsActive;
        private Button btnAdd, btnUpdate, btnDelete, btnClear, btnRefresh;
        private Label lblStatus;

        public CategoryManagementForm()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void InitializeComponent()
        {
            this.Text = "Category Management - AWE Electronics";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // DataGridView
            this.dataGridViewCategories = new DataGridView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(550, 500),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.dataGridViewCategories.SelectionChanged += DataGridViewCategories_SelectionChanged;

            // Input Panel
            int leftMargin = 580;
            int topStart = 20;

            Label lblTitle = new Label
            {
                Text = "Category Details",
                Location = new System.Drawing.Point(leftMargin, topStart),
                Size = new System.Drawing.Size(280, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            // Category Name
            Label lblCategoryName = new Label { Text = "Category Name:", Location = new System.Drawing.Point(leftMargin, topStart + 40), AutoSize = true };
            this.txtCategoryName = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + 60), Width = 280 };

            // Description
            Label lblDescription = new Label { Text = "Description:", Location = new System.Drawing.Point(leftMargin, topStart + 100), AutoSize = true };
            this.txtDescription = new TextBox 
            { 
                Location = new System.Drawing.Point(leftMargin, topStart + 120), 
                Width = 280,
                Height = 80,
                Multiline = true
            };

            // Is Active
            this.chkIsActive = new CheckBox 
            { 
                Text = "Active", 
                Location = new System.Drawing.Point(leftMargin, topStart + 220), 
                AutoSize = true,
                Checked = true
            };

            // Buttons
            this.btnAdd = new Button { Text = "Add", Location = new System.Drawing.Point(leftMargin, topStart + 260), Width = 80 };
            this.btnAdd.Click += BtnAdd_Click;

            this.btnUpdate = new Button { Text = "Update", Location = new System.Drawing.Point(leftMargin + 90, topStart + 260), Width = 80 };
            this.btnUpdate.Click += BtnUpdate_Click;

            this.btnDelete = new Button { Text = "Deactivate", Location = new System.Drawing.Point(leftMargin, topStart + 300), Width = 80 };
            this.btnDelete.Click += BtnDelete_Click;

            this.btnClear = new Button { Text = "Clear", Location = new System.Drawing.Point(leftMargin + 90, topStart + 300), Width = 80 };
            this.btnClear.Click += (s, e) => ClearFields();

            this.btnRefresh = new Button { Text = "Refresh", Location = new System.Drawing.Point(leftMargin + 180, topStart + 300), Width = 80 };
            this.btnRefresh.Click += (s, e) => LoadCategories();

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(leftMargin, topStart + 350),
                Size = new System.Drawing.Size(280, 60),
                ForeColor = System.Drawing.Color.Red,
                Text = ""
            };

            // Add controls
            this.Controls.Add(dataGridViewCategories);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblCategoryName);
            this.Controls.Add(txtCategoryName);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(chkIsActive);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnUpdate);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnClear);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(lblStatus);
        }

        private void LoadCategories()
        {
            try
            {
                List<Category> categories = _categoryManager.GetAllCategories();
                dataGridViewCategories.DataSource = categories;

                lblStatus.Text = $"Loaded {categories.Count} categories.";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void DataGridViewCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewCategories.SelectedRows.Count > 0)
            {
                _selectedCategory = (Category)dataGridViewCategories.SelectedRows[0].DataBoundItem;
                PopulateFields(_selectedCategory);
            }
        }

        private void PopulateFields(Category category)
        {
            txtCategoryName.Text = category.CategoryName;
            txtDescription.Text = category.Description;
            chkIsActive.Checked = category.IsActive;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Category category = new Category
                {
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    IsActive = chkIsActive.Checked
                };

                int categoryId = _categoryManager.CreateCategory(category);

                if (categoryId > 0)
                {
                    lblStatus.Text = "Category added successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    LoadCategories();
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
            if (_selectedCategory == null)
            {
                lblStatus.Text = "Please select a category to update.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                _selectedCategory.CategoryName = txtCategoryName.Text.Trim();
                _selectedCategory.Description = txtDescription.Text.Trim();
                _selectedCategory.IsActive = chkIsActive.Checked;

                bool success = _categoryManager.UpdateCategory(_selectedCategory);

                if (success)
                {
                    lblStatus.Text = "Category updated successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    LoadCategories();
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
            if (_selectedCategory == null)
            {
                lblStatus.Text = "Please select a category to deactivate.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to deactivate category '{_selectedCategory.CategoryName}'?",
                "Confirm Deactivation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = _categoryManager.DeleteCategory(_selectedCategory.CategoryID);
                    if (success)
                    {
                        lblStatus.Text = "Category deactivated successfully!";
                        lblStatus.ForeColor = System.Drawing.Color.Green;
                        LoadCategories();
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
            txtCategoryName.Clear();
            txtDescription.Clear();
            chkIsActive.Checked = true;
            _selectedCategory = null;
            dataGridViewCategories.ClearSelection();
        }
    }
}
