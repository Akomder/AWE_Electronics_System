#nullable disable
using System;
using System.Windows.Forms;
using AWE.BLL;
using AWE.Models;

namespace AWE.DesktopApp
{
    public partial class LoginForm : Form
    {
        private readonly UserManager _userManager = new UserManager();
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private LinkLabel lnkForgotPassword;
        private LinkLabel lnkResetPassword;
        private Label lblStatus;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            btnLogin = new Button();
            lblStatus = new Label();
            lnkForgotPassword = new LinkLabel();
            lnkResetPassword = new LinkLabel();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Location = new System.Drawing.Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(100, 23);
            lblTitle.TabIndex = 0;
            // 
            // lblUsername
            // 
            lblUsername.Location = new System.Drawing.Point(0, 0);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new System.Drawing.Size(100, 23);
            lblUsername.TabIndex = 1;
            // 
            // txtUsername
            // 
            txtUsername.Location = new System.Drawing.Point(0, 0);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new System.Drawing.Size(100, 27);
            txtUsername.TabIndex = 2;
            // 
            // lblPassword
            // 
            lblPassword.Location = new System.Drawing.Point(0, 0);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(100, 23);
            lblPassword.TabIndex = 3;
            // 
            // txtPassword
            // 
            txtPassword.Location = new System.Drawing.Point(0, 0);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new System.Drawing.Size(100, 27);
            txtPassword.TabIndex = 4;
            // 
            // btnLogin
            // 
            btnLogin.Location = new System.Drawing.Point(0, 0);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new System.Drawing.Size(75, 23);
            btnLogin.TabIndex = 5;
            btnLogin.Click += BtnLogin_Click;
            // 
            // lblStatus
            // 
            lblStatus.Location = new System.Drawing.Point(0, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(100, 23);
            lblStatus.TabIndex = 6;
            // 
            // lnkForgotPassword
            // 
            lnkForgotPassword.Location = new System.Drawing.Point(0, 0);
            lnkForgotPassword.Name = "lnkForgotPassword";
            lnkForgotPassword.Size = new System.Drawing.Size(100, 23);
            lnkForgotPassword.TabIndex = 7;
            lnkForgotPassword.LinkClicked += LnkForgotPassword_LinkClicked;
            // 
            // lnkResetPassword
            // 
            lnkResetPassword.Location = new System.Drawing.Point(0, 0);
            lnkResetPassword.Name = "lnkResetPassword";
            lnkResetPassword.Size = new System.Drawing.Size(100, 23);
            lnkResetPassword.TabIndex = 8;
            lnkResetPassword.LinkClicked += LnkResetPassword_LinkClicked;
            // 
            // LoginForm
            // 
            AcceptButton = btnLogin;
            ClientSize = new System.Drawing.Size(1153, 593);
            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(lblStatus);
            Controls.Add(lnkForgotPassword);
            Controls.Add(lnkResetPassword);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AWE Electronics - Staff Login";
            ResumeLayout(false);
            PerformLayout();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Clear previous status
            lblStatus.Text = "";

            // Validate input
            if (string.IsNullOrWhiteSpace(username))
            {
                lblStatus.Text = "Please enter your username.";
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                lblStatus.Text = "Please enter your password.";
                txtPassword.Focus();
                return;
            }

            try
            {
                // Disable login button to prevent multiple clicks
                btnLogin.Enabled = false;
                lblStatus.Text = "Authenticating...";
                lblStatus.ForeColor = System.Drawing.Color.Blue;
                Application.DoEvents();

                // Authenticate user
                User user = _userManager.Login(username, password);

                if (user != null)
                {
                    // Login successful
                    MessageBox.Show(
                        $"Welcome, {user.FirstName} {user.LastName}!\n\nRole: {user.Role}",
                        "Login Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Hide login form and open main application
                    this.Hide();
                    MainForm mainForm = new MainForm(user);
                    mainForm.ShowDialog();

                    // Close the application when main form is closed
                    this.Close();
                }
                else
                {
                    // Login failed
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    lblStatus.Text = "Invalid username or password.";
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Login error. Please try again.";
                MessageBox.Show(
                    $"An error occurred during login:\n{ex.Message}",
                    "Login Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void LnkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open password reset request form
            PasswordResetRequestForm resetRequestForm = new PasswordResetRequestForm();
            resetRequestForm.ShowDialog();
        }

        private void LnkResetPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open password reset form
            PasswordResetForm resetForm = new PasswordResetForm();
            resetForm.ShowDialog();
        }
    }
}
