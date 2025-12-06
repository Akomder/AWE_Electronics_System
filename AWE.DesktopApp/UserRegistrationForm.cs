#nullable disable
using System;
using System.Windows.Forms;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class UserRegistrationForm : Form
    {
        private readonly UserManager _userManager = new UserManager();

        // UI Controls
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private ComboBox cmbRole;
        private Button btnRegister;
        private Button btnCancel;
        private Label lblStatus;
        private CheckBox chkShowPassword;

        public UserRegistrationForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "User Registration - AWE Electronics";
            this.Size = new System.Drawing.Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Title Label
            Label lblTitle = new Label
            {
                Text = "Register New User",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(450, 30),
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Username
            int yPos = 70;
            this.txtUsername = CreateLabeledTextBox(this, "Username:", 20, yPos, 400);
            AddHelpLabel(this, "3-50 characters, letters, numbers, and underscores only", 20, yPos + 25);

            // Password
            yPos += 60;
            this.txtPassword = CreateLabeledTextBox(this, "Password:", 20, yPos, 400);
            this.txtPassword.PasswordChar = '●';
            AddHelpLabel(this, "At least 8 characters, must contain letters and numbers", 20, yPos + 25);

            // Confirm Password
            yPos += 60;
            this.txtConfirmPassword = CreateLabeledTextBox(this, "Confirm Password:", 20, yPos, 400);
            this.txtConfirmPassword.PasswordChar = '●';

            // Show Password Checkbox
            yPos += 35;
            this.chkShowPassword = new CheckBox
            {
                Text = "Show Passwords",
                Location = new System.Drawing.Point(140, yPos),
                AutoSize = true
            };
            this.chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;
            this.Controls.Add(this.chkShowPassword);

            // First Name
            yPos += 35;
            this.txtFirstName = CreateLabeledTextBox(this, "First Name:", 20, yPos, 400);

            // Last Name
            yPos += 40;
            this.txtLastName = CreateLabeledTextBox(this, "Last Name:", 20, yPos, 400);

            // Email
            yPos += 40;
            this.txtEmail = CreateLabeledTextBox(this, "Email:", 20, yPos, 400);
            AddHelpLabel(this, "Valid email format required", 20, yPos + 25);

            // Phone
            yPos += 60;
            this.txtPhone = CreateLabeledTextBox(this, "Phone:", 20, yPos, 400);

            // Role
            yPos += 40;
            Label lblRole = new Label
            {
                Text = "Role:",
                Location = new System.Drawing.Point(20, yPos),
                Size = new System.Drawing.Size(110, 20)
            };
            this.cmbRole = new ComboBox
            {
                Location = new System.Drawing.Point(140, yPos),
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.cmbRole.Items.AddRange(new object[] { "Staff", "Accountant", "Manager", "Admin" });
            this.cmbRole.SelectedIndex = 0; // Default to Staff
            this.Controls.Add(lblRole);
            this.Controls.Add(this.cmbRole);

            // Buttons
            yPos += 50;
            this.btnRegister = new Button
            {
                Text = "Register User",
                Location = new System.Drawing.Point(140, yPos),
                Size = new System.Drawing.Size(120, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.btnRegister.Click += BtnRegister_Click;

            this.btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(270, yPos),
                Size = new System.Drawing.Size(100, 35)
            };
            this.btnCancel.Click += (s, e) => this.Close();

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(20, yPos + 45),
                Size = new System.Drawing.Size(450, 40),
                ForeColor = System.Drawing.Color.Red,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Text = ""
            };

            // Add all controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblStatus);

            // Set Accept button
            this.AcceptButton = this.btnRegister;
        }

        private TextBox CreateLabeledTextBox(Form form, string labelText, int x, int y, int width)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(110, 20)
            };
            TextBox txt = new TextBox
            {
                Location = new System.Drawing.Point(x + 120, y),
                Width = width - 120
            };
            form.Controls.Add(lbl);
            form.Controls.Add(txt);
            return txt;
        }

        private void AddHelpLabel(Form form, string text, int x, int y)
        {
            Label lbl = new Label
            {
                Text = text,
                Location = new System.Drawing.Point(x + 120, y),
                Size = new System.Drawing.Size(350, 20),
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Arial", 8)
            };
            form.Controls.Add(lbl);
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                txtPassword.PasswordChar = '\0';
                txtConfirmPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '●';
                txtConfirmPassword.PasswordChar = '●';
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // Clear previous status
            lblStatus.Text = "";
            lblStatus.ForeColor = System.Drawing.Color.Red;

            // Validate all fields are filled
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblStatus.Text = "Please enter a username.";
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.Text = "Please enter a password.";
                txtPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                lblStatus.Text = "Please confirm your password.";
                txtConfirmPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                lblStatus.Text = "Please enter your first name.";
                txtFirstName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                lblStatus.Text = "Please enter your last name.";
                txtLastName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblStatus.Text = "Please enter your email address.";
                txtEmail.Focus();
                return;
            }

            try
            {
                // Disable button to prevent double-click
                btnRegister.Enabled = false;
                lblStatus.Text = "Registering user...";
                lblStatus.ForeColor = System.Drawing.Color.Blue;
                Application.DoEvents();

                // Create user object
                User newUser = new User
                {
                    Username = txtUsername.Text.Trim(),
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Role = cmbRole.SelectedItem.ToString()
                };

                // Register user with validation
                int newUserId = _userManager.RegisterUser(
                    newUser,
                    txtPassword.Text,
                    txtConfirmPassword.Text
                );

                if (newUserId > 0)
                {
                    lblStatus.Text = "User registered successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;

                    MessageBox.Show(
                        $"User '{newUser.Username}' has been registered successfully!\n\n" +
                        $"User ID: {newUserId}\n" +
                        $"Name: {newUser.FirstName} {newUser.LastName}\n" +
                        $"Role: {newUser.Role}\n" +
                        $"Email: {newUser.Email}",
                        "Registration Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Close the form
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (ArgumentException ex)
            {
                // Validation errors
                lblStatus.Text = ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                // Other errors (e.g., username already exists)
                lblStatus.Text = ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show(
                    $"Registration failed:\n{ex.Message}",
                    "Registration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnRegister.Enabled = true;
            }
        }
    }
}
