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
        private Button btnRegister;
        private Button btnForgotPassword;
        private Label lblStatus;
        private CheckBox chkShowPassword;

        public LoginForm()
        {
            InitializeComponent(); // Assume this is implemented by the designer
        }

        private void InitializeComponent()
        {
            this.Text = "AWE Electronics Staff Login";
            this.Size = new System.Drawing.Size(450, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Title Label
            Label lblTitle = new Label
            {
                Text = "AWE Electronics",
                Location = new System.Drawing.Point(50, 30),
                Size = new System.Drawing.Size(350, 40),
                Font = new System.Drawing.Font("Arial", 18, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Subtitle Label
            Label lblSubtitle = new Label
            {
                Text = "Staff Login Portal",
                Location = new System.Drawing.Point(50, 75),
                Size = new System.Drawing.Size(350, 20),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Italic),
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
                Width = 240,
                Name = "txtUsername"
            };

            // Password Label and TextBox
            Label lblPassword = new Label
            {
                Text = "Password:",
                Location = new System.Drawing.Point(50, 160),
                Size = new System.Drawing.Size(100, 20)
            };
            this.txtPassword = new TextBox
            {
                Location = new System.Drawing.Point(160, 160),
                Width = 240,
                PasswordChar = '●',
                Name = "txtPassword"
            };

            // Show Password Checkbox
            this.chkShowPassword = new CheckBox
            {
                Text = "Show Password",
                Location = new System.Drawing.Point(160, 190),
                AutoSize = true
            };
            this.chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;

            // Login Button
            this.btnLogin = new Button
            {
                Text = "Login",
                Location = new System.Drawing.Point(160, 230),
                Size = new System.Drawing.Size(100, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.btnLogin.Click += BtnLogin_Click;

            // Register Button
            this.btnRegister = new Button
            {
                Text = "Register New Account",
                Location = new System.Drawing.Point(160, 275),
                Size = new System.Drawing.Size(240, 30),
                BackColor = System.Drawing.Color.FromArgb(76, 175, 80),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.btnRegister.Click += BtnRegister_Click;

            // Forgot Password Button
            this.btnForgotPassword = new Button
            {
                Text = "Forgot Password?",
                Location = new System.Drawing.Point(160, 310),
                Size = new System.Drawing.Size(240, 30),
                BackColor = System.Drawing.Color.FromArgb(255, 152, 0),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.btnForgotPassword.Click += BtnForgotPassword_Click;

            // Status Label
            this.lblStatus = new Label
            {
                Location = new System.Drawing.Point(50, 350),
                Size = new System.Drawing.Size(350, 50),
                ForeColor = System.Drawing.Color.Red,
                TextAlign = System.Drawing.ContentAlignment.TopCenter,
                Text = ""
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.chkShowPassword);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.btnForgotPassword);
            this.Controls.Add(this.lblStatus);

            // Set Accept button
            this.AcceptButton = this.btnLogin;
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '●';
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            lblStatus.ForeColor = System.Drawing.Color.Red;

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblStatus.Text = "Please enter your username.";
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.Text = "Please enter your password.";
                txtPassword.Focus();
                return;
            }

            try
            {
                btnLogin.Enabled = false;
                lblStatus.Text = "Logging in...";
                lblStatus.ForeColor = System.Drawing.Color.Blue;
                Application.DoEvents();

                User user = _userManager.Login(txtUsername.Text, txtPassword.Text);

                if (user != null)
                {
                    lblStatus.Text = "Login successful!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;

                    MessageBox.Show(
                        $"Login Successful!\n\nWelcome, {user.FirstName} {user.LastName} ({user.Role}).",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Close login form and open main application form (e.g., MainForm)
                    this.Hide();
                    MainForm mainForm = new MainForm(user);
                    mainForm.ShowDialog();
                    this.Close();
                }
                else
                {
                    lblStatus.Text = "Invalid username or password.";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Login error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            using (UserRegistrationForm registerForm = new UserRegistrationForm())
            {
                registerForm.ShowDialog(this);
            }
        }

        private void BtnForgotPassword_Click(object sender, EventArgs e)
        {
            // First show the request form to get a token
            using (PasswordResetRequestForm requestForm = new PasswordResetRequestForm())
            {
                DialogResult result = requestForm.ShowDialog(this);
                
                // If user closed the form, offer to reset with token
                if (result == DialogResult.OK)
                {
                    using (PasswordResetForm resetForm = new PasswordResetForm())
                    {
                        resetForm.ShowDialog(this);
                    }
                }
            }
        }
    }
}
