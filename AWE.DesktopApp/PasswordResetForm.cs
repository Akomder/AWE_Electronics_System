#nullable disable
using System;
using System.Windows.Forms;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class PasswordResetForm : Form
    {
        private readonly UserManager _userManager = new UserManager();
        private TextBox txtToken;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkShowPassword;
        private Button btnResetPassword;
        private Button btnCancel;
        private Label lblStatus;
        private Label lblUsername;

        public PasswordResetForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Reset Password - AWE Electronics";
            this.Size = new System.Drawing.Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Title Label
            Label lblTitle = new Label
            {
                Text = "Reset Your Password",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(450, 30),
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Instructions Label
            Label lblInstructions = new Label
            {
                Text = "Enter your reset token and choose a new password.",
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(450, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = System.Drawing.Color.Gray
            };

            // Token Label and TextBox
            Label lblTokenLabel = new Label
            {
                Text = "Reset Token:",
                Location = new System.Drawing.Point(50, 100),
                Size = new System.Drawing.Size(110, 20)
            };

            this.txtToken = new TextBox
            {
                Location = new System.Drawing.Point(170, 100),
                Width = 270,
                Font = new System.Drawing.Font("Courier New", 9)
            };
            this.txtToken.TextChanged += TxtToken_TextChanged;

            // Username Display Label (shows after token validation)
            this.lblUsername = new Label
            {
                Location = new System.Drawing.Point(170, 125),
                Size = new System.Drawing.Size(270, 20),
                ForeColor = System.Drawing.Color.Green,
                Text = "",
                Visible = false
            };

            // New Password Label and TextBox
            Label lblNewPassword = new Label
            {
                Text = "New Password:",
                Location = new System.Drawing.Point(50, 160),
                Size = new System.Drawing.Size(110, 20)
            };

            this.txtNewPassword = new TextBox
            {
                Location = new System.Drawing.Point(170, 160),
                Width = 270,
                PasswordChar = '●'
            };

            // Help text for password
            Label lblPasswordHelp = new Label
            {
                Text = "At least 8 characters, must contain letters and numbers",
                Location = new System.Drawing.Point(170, 185),
                Size = new System.Drawing.Size(270, 20),
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Arial", 8)
            };

            // Confirm Password Label and TextBox
            Label lblConfirmPassword = new Label
            {
                Text = "Confirm Password:",
                Location = new System.Drawing.Point(50, 215),
                Size = new System.Drawing.Size(110, 20)
            };

            this.txtConfirmPassword = new TextBox
            {
                Location = new System.Drawing.Point(170, 215),
                Width = 270,
                PasswordChar = '●'
            };

            // Show Password Checkbox
            this.chkShowPassword = new CheckBox
            {
                Text = "Show Passwords",
                Location = new System.Drawing.Point(170, 245),
                AutoSize = true
            };
            this.chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;

            // Reset Password Button
            this.btnResetPassword = new Button
            {
                Text = "Reset Password",
                Location = new System.Drawing.Point(170, 285),
                Size = new System.Drawing.Size(130, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.btnResetPassword.Click += BtnResetPassword_Click;

            // Cancel Button
            this.btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(310, 285),
                Size = new System.Drawing.Size(100, 35)
            };
            this.btnCancel.Click += (s, e) => this.Close();

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(20, 335),
                Size = new System.Drawing.Size(450, 40),
                ForeColor = System.Drawing.Color.Red,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Text = ""
            };

            // Add all controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblInstructions);
            this.Controls.Add(lblTokenLabel);
            this.Controls.Add(this.txtToken);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(lblNewPassword);
            this.Controls.Add(this.txtNewPassword);
            this.Controls.Add(lblPasswordHelp);
            this.Controls.Add(lblConfirmPassword);
            this.Controls.Add(this.txtConfirmPassword);
            this.Controls.Add(this.chkShowPassword);
            this.Controls.Add(this.btnResetPassword);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblStatus);

            // Set Accept button
            this.AcceptButton = this.btnResetPassword;
        }

        private void TxtToken_TextChanged(object sender, EventArgs e)
        {
            // Validate token when user enters it
            string token = txtToken.Text.Trim();
            
            if (token.Length > 10) // Only validate if reasonable length
            {
                if (_userManager.ValidateResetToken(token, out string errorMessage))
                {
                    // Token is valid, show username
                    User user = _userManager.GetUserByResetToken(token);
                    if (user != null)
                    {
                        lblUsername.Text = $"✓ Resetting password for: {user.Username}";
                        lblUsername.ForeColor = System.Drawing.Color.Green;
                        lblUsername.Visible = true;
                    }
                }
                else
                {
                    lblUsername.Text = $"✗ {errorMessage}";
                    lblUsername.ForeColor = System.Drawing.Color.Red;
                    lblUsername.Visible = true;
                }
            }
            else
            {
                lblUsername.Visible = false;
            }
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                txtNewPassword.PasswordChar = '\0';
                txtConfirmPassword.PasswordChar = '\0';
            }
            else
            {
                txtNewPassword.PasswordChar = '●';
                txtConfirmPassword.PasswordChar = '●';
            }
        }

        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            // Clear previous status
            lblStatus.Text = "";
            lblStatus.ForeColor = System.Drawing.Color.Red;

            // Validate all fields
            if (string.IsNullOrWhiteSpace(txtToken.Text))
            {
                lblStatus.Text = "Please enter your reset token.";
                txtToken.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                lblStatus.Text = "Please enter a new password.";
                txtNewPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                lblStatus.Text = "Please confirm your new password.";
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                // Disable button to prevent double-click
                btnResetPassword.Enabled = false;
                lblStatus.Text = "Resetting password...";
                lblStatus.ForeColor = System.Drawing.Color.Blue;
                Application.DoEvents();

                // Reset password with token
                bool success = _userManager.ResetPasswordWithToken(
                    txtToken.Text.Trim(),
                    txtNewPassword.Text,
                    txtConfirmPassword.Text
                );

                if (success)
                {
                    lblStatus.Text = "Password reset successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;

                    MessageBox.Show(
                        "Your password has been reset successfully!\n\n" +
                        "You can now login with your new password.",
                        "Password Reset Successful",
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
                // Other errors
                lblStatus.Text = ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show(
                    $"Password reset failed:\n{ex.Message}",
                    "Reset Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnResetPassword.Enabled = true;
            }
        }
    }
}
