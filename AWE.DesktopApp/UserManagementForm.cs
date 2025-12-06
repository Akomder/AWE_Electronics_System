#nullable disable
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class UserManagementForm : Form
    {
        private readonly UserManager _userManager = new UserManager();
        private User _selectedUser = null;

        private DataGridView dataGridViewUsers;
        private TextBox txtUsername, txtFirstName, txtLastName, txtEmail, txtPhone;
        private ComboBox cmbRole;
        private CheckBox chkIsActive;
        private Button btnAdd, btnUpdate, btnDelete, btnClear, btnRefresh, btnResetPassword;
        private Label lblStatus;

        public UserManagementForm()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.Text = "User Management - AWE Electronics";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // DataGridView
            this.dataGridViewUsers = new DataGridView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(760, 600),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.dataGridViewUsers.SelectionChanged += DataGridViewUsers_SelectionChanged;

            // Input Panel
            int leftMargin = 790;
            int topStart = 20;
            int spacing = 40;

            Label lblTitle = new Label
            {
                Text = "User Details",
                Location = new System.Drawing.Point(leftMargin, topStart),
                Size = new System.Drawing.Size(350, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            // Username
            Label lblUsername = new Label { Text = "Username:", Location = new System.Drawing.Point(leftMargin, topStart + spacing), AutoSize = true };
            this.txtUsername = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing + 20), Width = 350 };

            // First Name
            Label lblFirstName = new Label { Text = "First Name:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 2), AutoSize = true };
            this.txtFirstName = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 2 + 20), Width = 350 };

            // Last Name
            Label lblLastName = new Label { Text = "Last Name:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 3), AutoSize = true };
            this.txtLastName = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 3 + 20), Width = 350 };

            // Email
            Label lblEmail = new Label { Text = "Email:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 4), AutoSize = true };
            this.txtEmail = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 4 + 20), Width = 350 };

            // Phone
            Label lblPhone = new Label { Text = "Phone:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 5), AutoSize = true };
            this.txtPhone = new TextBox { Location = new System.Drawing.Point(leftMargin, topStart + spacing * 5 + 20), Width = 350 };

            // Role
            Label lblRole = new Label { Text = "Role:", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 6), AutoSize = true };
            this.cmbRole = new ComboBox 
            { 
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 6 + 20), 
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.cmbRole.Items.AddRange(new string[] { "Admin", "Staff", "Accountant", "Manager" });

            // Is Active
            this.chkIsActive = new CheckBox 
            { 
                Text = "Active", 
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 7 + 20), 
                AutoSize = true,
                Checked = true
            };

            // Buttons
            this.btnAdd = new Button { Text = "Add New", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 8 + 20), Width = 100 };
            this.btnAdd.Click += BtnAdd_Click;

            this.btnUpdate = new Button { Text = "Update", Location = new System.Drawing.Point(leftMargin + 110, topStart + spacing * 8 + 20), Width = 100 };
            this.btnUpdate.Click += BtnUpdate_Click;

            this.btnDelete = new Button { Text = "Deactivate", Location = new System.Drawing.Point(leftMargin + 220, topStart + spacing * 8 + 20), Width = 100 };
            this.btnDelete.Click += BtnDelete_Click;

            this.btnResetPassword = new Button { Text = "Reset Password", Location = new System.Drawing.Point(leftMargin, topStart + spacing * 9 + 20), Width = 150 };
            this.btnResetPassword.Click += BtnResetPassword_Click;

            this.btnClear = new Button { Text = "Clear", Location = new System.Drawing.Point(leftMargin + 160, topStart + spacing * 9 + 20), Width = 80 };
            this.btnClear.Click += BtnClear_Click;

            this.btnRefresh = new Button { Text = "Refresh", Location = new System.Drawing.Point(leftMargin + 250, topStart + spacing * 9 + 20), Width = 80 };
            this.btnRefresh.Click += (s, e) => LoadUsers();

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(leftMargin, topStart + spacing * 10 + 20),
                Size = new System.Drawing.Size(350, 40),
                ForeColor = System.Drawing.Color.Red,
                Text = ""
            };

            // Add controls
            this.Controls.Add(dataGridViewUsers);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblFirstName);
            this.Controls.Add(txtFirstName);
            this.Controls.Add(lblLastName);
            this.Controls.Add(txtLastName);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPhone);
            this.Controls.Add(txtPhone);
            this.Controls.Add(lblRole);
            this.Controls.Add(cmbRole);
            this.Controls.Add(chkIsActive);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnUpdate);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnResetPassword);
            this.Controls.Add(btnClear);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(lblStatus);
        }

        private void LoadUsers()
        {
            try
            {
                List<User> users = _userManager.GetAllUsers();
                dataGridViewUsers.DataSource = users;

                // Hide password hash column
                if (dataGridViewUsers.Columns["PasswordHash"] != null)
                {
                    dataGridViewUsers.Columns["PasswordHash"].Visible = false;
                }

                lblStatus.Text = $"Loaded {users.Count} users.";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error loading users: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void DataGridViewUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                _selectedUser = (User)dataGridViewUsers.SelectedRows[0].DataBoundItem;
                PopulateFields(_selectedUser);
            }
        }

        private void PopulateFields(User user)
        {
            txtUsername.Text = user.Username;
            txtFirstName.Text = user.FirstName;
            txtLastName.Text = user.LastName;
            txtEmail.Text = user.Email;
            txtPhone.Text = user.Phone;
            cmbRole.SelectedItem = user.Role;
            chkIsActive.Checked = user.IsActive;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Open registration form
            UserRegistrationForm registrationForm = new UserRegistrationForm();
            if (registrationForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null)
            {
                lblStatus.Text = "Please select a user to update.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                _selectedUser.FirstName = txtFirstName.Text.Trim();
                _selectedUser.LastName = txtLastName.Text.Trim();
                _selectedUser.Email = txtEmail.Text.Trim();
                _selectedUser.Phone = txtPhone.Text.Trim();
                _selectedUser.Role = cmbRole.SelectedItem?.ToString();
                _selectedUser.IsActive = chkIsActive.Checked;

                bool success = _userManager.UpdateUser(_selectedUser);

                if (success)
                {
                    lblStatus.Text = "User updated successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    LoadUsers();
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
            if (_selectedUser == null)
            {
                lblStatus.Text = "Please select a user to deactivate.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to deactivate user '{_selectedUser.Username}'?",
                "Confirm Deactivation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = _userManager.DeactivateUser(_selectedUser.UserID);
                    if (success)
                    {
                        lblStatus.Text = "User deactivated successfully!";
                        lblStatus.ForeColor = System.Drawing.Color.Green;
                        LoadUsers();
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

        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null)
            {
                lblStatus.Text = "Please select a user to reset password.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string newPassword = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter new password for user '" + _selectedUser.Username + "':",
                "Reset Password",
                ""
            );

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                try
                {
                    bool success = _userManager.AdminResetPassword(_selectedUser.UserID, newPassword);
                    if (success)
                    {
                        lblStatus.Text = "Password reset successfully!";
                        lblStatus.ForeColor = System.Drawing.Color.Green;
                        MessageBox.Show($"Password for '{_selectedUser.Username}' has been reset.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.Text = $"Error: {ex.Message}";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtUsername.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            cmbRole.SelectedIndex = -1;
            chkIsActive.Checked = true;
            _selectedUser = null;
            dataGridViewUsers.ClearSelection();
        }
    }
}
