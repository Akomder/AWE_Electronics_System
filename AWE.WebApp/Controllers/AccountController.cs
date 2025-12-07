#nullable disable
using Microsoft.AspNetCore.Mvc;
using AWE.BLL;
using AWE.Models;
using AWE.WebApp.Helpers;

namespace AWE.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager _userManager;

        public AccountController(UserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            try
            {
                User? user = _userManager.Login(username, password);

                if (user != null)
                {
                    SessionHelper.SetCurrentUser(HttpContext.Session, user);
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Login failed: {ex.Message}";
                return View();
            }
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            // If already logged in, redirect to dashboard
            if (SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string username, string email, string firstName, string lastName, 
                                     string password, string confirmPassword)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(username))
                {
                    ViewBag.Error = "Username is required.";
                    return View();
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    ViewBag.Error = "Email is required.";
                    return View();
                }

                if (string.IsNullOrWhiteSpace(firstName))
                {
                    ViewBag.Error = "First name is required.";
                    return View();
                }

                if (string.IsNullOrWhiteSpace(lastName))
                {
                    ViewBag.Error = "Last name is required.";
                    return View();
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Password is required.";
                    return View();
                }

                if (password != confirmPassword)
                {
                    ViewBag.Error = "Passwords do not match.";
                    return View();
                }

                // Create new user
                User newUser = new User
                {
                    Username = username.Trim(),
                    Email = email.Trim(),
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    Role = "Staff", // Default role for new users
                    IsActive = true
                };

                int userId = _userManager.RegisterUser(newUser, password, confirmPassword);

                if (userId > 0)
                {
                    ViewBag.Success = "Registration successful! You can now login.";
                    // Clear form
                    return RedirectToAction(nameof(Login));
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();
        }

        // GET: Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    ViewBag.Error = "Please enter your username.";
                    return View();
                }

                // Request password reset
                string token = _userManager.RequestPasswordReset(username.Trim());

                if (!string.IsNullOrEmpty(token))
                {
                    // Store token in session temporarily
                    HttpContext.Session.SetString("ResetToken", token);
                    
                    ViewBag.Success = "A password reset link has been generated. Please check your email or use the token below.";
                    ViewBag.Token = token;
                    ViewBag.ShowResetForm = true;
                    ViewBag.Username = username;
                }
                else
                {
                    // Don't reveal if username exists (security best practice)
                    ViewBag.Success = "If this username exists, a password reset token has been sent.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
            }

            return View();
        }

        // GET: Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token = "")
        {
            if (SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // If no token provided, check session
            if (string.IsNullOrEmpty(token))
            {
                token = HttpContext.Session.GetString("ResetToken") ?? "";
            }

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Invalid or missing reset token.";
                return View();
            }

            ViewBag.Token = token;
            return View();
        }

        // POST: Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string token, string newPassword, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    ViewBag.Error = "Reset token is missing.";
                    ViewBag.Token = token;
                    return View();
                }

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    ViewBag.Error = "New password is required.";
                    ViewBag.Token = token;
                    return View();
                }

                if (newPassword != confirmPassword)
                {
                    ViewBag.Error = "Passwords do not match.";
                    ViewBag.Token = token;
                    return View();
                }

                // Reset password with token
                bool success = _userManager.ResetPasswordWithToken(token, newPassword, confirmPassword);

                if (success)
                {
                    HttpContext.Session.Remove("ResetToken");
                    ViewBag.SuccessMessage = "Password reset successfully! You can now login with your new password.";
                    return View("ResetPasswordSuccess");
                }
                else
                {
                    ViewBag.Error = "Failed to reset password. Token may be invalid or expired.";
                    ViewBag.Token = token;
                }
            }
            catch (ArgumentException ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Token = token;
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error resetting password: {ex.Message}";
                ViewBag.Token = token;
            }

            return View();
        }

        public IActionResult Logout()
        {
            SessionHelper.ClearSession(HttpContext.Session);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
