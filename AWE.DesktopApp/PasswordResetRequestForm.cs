#nullable disable
using System;
using System.Windows.Forms;
using AWE.BLL;

namespace AWE.DesktopApp
{
    public partial class PasswordResetRequestForm : Form
    {
        private readonly UserManager _userManager = new UserManager();
        private TextBox txtUsername;
        private Button btnRequestReset;
        private Button btnCancel;
        private Label lblStatus;
        private Label lblGeneratedToken;
        private TextBox txtGeneratedToken;

        public PasswordResetRequestForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Request Password Reset - AWE Electronics";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Title Label
            Label lblTitle = new Label
            {
                Text = "Request Password Reset",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(450, 30),
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Instructions Label
            Label lblInstructions = new Label
            {
                Text = "Enter your username to request a password reset token.\n" +
                       "The token will be valid for 24 hours.",
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(450, 40),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = System.Drawing.Color.Gray
            };

            // Username Label and TextBox
            Label lblUsername = new Label
            {
                Text = "Username:",
                Location = new System.Drawing.Point(50, 120),
                Size = new System.Drawing.Size(100, 20)
            };

            this.txtUsername = new TextBox
            {
                Location = new System.Drawing.Point(160, 120),
                Width = 280
            };

            // Request Reset Button
            this.btnRequestReset = new Button
            {
                Text = "Request Reset Token",
                Location = new System.Drawing.Point(160, 160),
                Size = new System.Drawing.Size(150, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.btnRequestReset.Click += BtnRequestReset_Click;

            // Cancel Button
            this.btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(320, 160),
                Size = new System.Drawing.Size(100, 35)
            };
            this.btnCancel.Click += (s, e) => this.Close();

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(20, 210),
                Size = new System.Drawing.Size(450, 20),
                ForeColor = System.Drawing.Color.Red,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Text = ""
            };

            // Generated Token Label (initially hidden)
            this.lblGeneratedToken = new Label
            {
                Text = "Reset Token (copy this):",
                Location = new System.Drawing.Point(50, 240),
                Size = new System.Drawing.Size(200, 20),
                Visible = false
            };

            // Generated Token TextBox (initially hidden)
            this.txtGeneratedToken = new TextBox
            {
                Location = new System.Drawing.Point(50, 265),
                Width = 390,
                ReadOnly = true,
                Visible = false,
                Font = new System.Drawing.Font("Courier New", 10),
                BackColor = System.Drawing.Color.LightYellow
            };

            // Add all controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblInstructions);
            this.Controls.Add(lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.btnRequestReset);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblGeneratedToken);
            this.Controls.Add(this.txtGeneratedToken);

            // Set Accept button
            this.AcceptButton = this.btnRequestReset;
        }

        private void BtnRequestReset_Click(object sender, EventArgs e)
        {
            // Clear previous status
            lblStatus.Text = "";
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblGeneratedToken.Visible = false;
            txtGeneratedToken.Visible = false;

            // Validate username
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblStatus.Text = "Please enter your username.";
                txtUsername.Focus();
                return;
            }

            try
            {
                // Disable button to prevent double-click
                btnRequestReset.Enabled = false;
                lblStatus.Text = "Generating reset token...";
                lblStatus.ForeColor = System.Drawing.Color.Blue;
                Application.DoEvents();

                // Request password reset
                string token = _userManager.RequestPasswordReset(txtUsername.Text.Trim());

                if (!string.IsNullOrEmpty(token))
                {
                    // Success - show token
                    lblStatus.Text = "Reset token generated successfully!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;

                    lblGeneratedToken.Visible = true;
                    txtGeneratedToken.Visible = true;
                    txtGeneratedToken.Text = token;

                    MessageBox.Show(
                        "Password reset token has been generated!\n\n" +
                        "IMPORTANT: Copy the token below and use it to reset your password.\n" +
                        "The token will expire in 24 hours.\n\n" +
                        "In a real application, this token would be sent via email.",
                        "Token Generated",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Select the token text for easy copying
                    txtGeneratedToken.SelectAll();
                    txtGeneratedToken.Focus();
                }
                else
                {
                    // User not found (don't reveal this for security)
                    lblStatus.Text = "If the username exists, a reset token has been generated.";
                    lblStatus.ForeColor = System.Drawing.Color.Blue;

                    MessageBox.Show(
                        "If your username is correct, a password reset token has been generated.\n\n" +
                        "In a real application, this would be sent to your registered email address.",
                        "Request Processed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error generating reset token.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show(
                    $"An error occurred:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnRequestReset.Enabled = true;
            }
        }
    }
}
