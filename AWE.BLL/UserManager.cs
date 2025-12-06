#nullable disable
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using AWE.DAL;
using AWE.Models;

namespace AWE.BLL
{
    public class UserManager
    {
        private readonly UserDAL _dal = new UserDAL();
        private readonly PasswordResetTokenDAL _tokenDal = new PasswordResetTokenDAL();

        // --- Password Hashing Utilities ---

        /// <summary>
        /// Hashes a plain text password using SHA256.
        /// In a production environment, consider using BCrypt or PBKDF2 for better security.
        /// </summary>
        public string HashPassword(string plainPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verifies if the provided plain password matches the stored hash.
        /// </summary>
        public bool VerifyPassword(string plainPassword, string storedHash)
        {
            string hashOfInput = HashPassword(plainPassword);
            return string.Equals(hashOfInput, storedHash, StringComparison.OrdinalIgnoreCase);
        }

        // --- Authentication ---

        /// <summary>
        /// Authenticates a user by username and password.
        /// Returns the User object if successful, null otherwise.
        /// </summary>
        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null; // Invalid input
            }

            // Retrieve user by username
            User user = _dal.GetByUsername(username);

            if (user == null)
            {
                return null; // User not found or inactive
            }

            // Verify password
            if (VerifyPassword(password, user.PasswordHash))
            {
                // Update last login timestamp
                _dal.UpdateLastLogin(user.UserID);
                
                // Refresh user object to get updated LastLogin
                user = _dal.GetById(user.UserID);
                
                return user;
            }

            return null; // Password mismatch
        }

        // --- Validation Methods ---

        /// <summary>
        /// Validates email format using basic regex pattern.
        /// </summary>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates password strength.
        /// Requirements: At least 8 characters, contains letter and number.
        /// </summary>
        private bool IsValidPassword(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Password cannot be empty.";
                return false;
            }

            if (password.Length < 8)
            {
                errorMessage = "Password must be at least 8 characters long.";
                return false;
            }

            bool hasLetter = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c)) hasLetter = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            if (!hasLetter || !hasDigit)
            {
                errorMessage = "Password must contain at least one letter and one number.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates username format.
        /// Requirements: 3-50 characters, alphanumeric and underscore only.
        /// </summary>
        private bool IsValidUsername(string username, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Username cannot be empty.";
                return false;
            }

            if (username.Length < 3 || username.Length > 50)
            {
                errorMessage = "Username must be between 3 and 50 characters.";
                return false;
            }

            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    errorMessage = "Username can only contain letters, numbers, and underscores.";
                    return false;
                }
            }

            return true;
        }

        // --- User Management (CRUD) ---

        public User GetUserById(int id)
        {
            return _dal.GetById(id);
        }

        public User GetUserByUsername(string username)
        {
            return _dal.GetByUsername(username);
        }

        public List<User> GetAllUsers()
        {
            return _dal.GetAll();
        }

        /// <summary>
        /// Registers a new user with comprehensive validation and password hashing.
        /// </summary>
        public int RegisterUser(User user, string plainPassword, string confirmPassword)
        {
            // Validate username
            if (!IsValidUsername(user.Username, out string usernameError))
            {
                throw new ArgumentException(usernameError);
            }

            // Validate password
            if (!IsValidPassword(plainPassword, out string passwordError))
            {
                throw new ArgumentException(passwordError);
            }

            // Validate password confirmation
            if (plainPassword != confirmPassword)
            {
                throw new ArgumentException("Password and confirmation password do not match.");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                throw new ArgumentException("First name is required.");
            }
            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                throw new ArgumentException("Last name is required.");
            }

            // Validate email
            if (!IsValidEmail(user.Email))
            {
                throw new ArgumentException("Invalid email format.");
            }

            // Check if username already exists
            if (_dal.GetByUsername(user.Username) != null)
            {
                throw new Exception($"Username '{user.Username}' is already taken.");
            }

            // Validate role
            string[] validRoles = { "Admin", "Staff", "Accountant", "Manager" };
            if (Array.IndexOf(validRoles, user.Role) == -1)
            {
                throw new ArgumentException("Invalid role. Must be Admin, Staff, Accountant, or Manager.");
            }

            // Hash the password
            user.PasswordHash = HashPassword(plainPassword);
            user.IsActive = true;

            return _dal.Insert(user);
        }

        /// <summary>
        /// Creates a new user with a hashed password (legacy method for backward compatibility).
        /// </summary>
        public int CreateUser(User user, string plainPassword)
        {
            return RegisterUser(user, plainPassword, plainPassword);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        public bool UpdateUser(User user)
        {
            if (_dal.GetById(user.UserID) == null)
            {
                throw new Exception($"User with ID {user.UserID} not found.");
            }

            return _dal.Update(user) > 0;
        }

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            User user = _dal.GetById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Verify old password
            if (!VerifyPassword(oldPassword, user.PasswordHash))
            {
                throw new Exception("Old password is incorrect.");
            }

            // Hash and update new password
            user.PasswordHash = HashPassword(newPassword);
            return _dal.Update(user) > 0;
        }

        // --- Password Reset Functionality ---

        /// <summary>
        /// Generates a secure random token for password reset.
        /// </summary>
        private string GenerateSecureToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                
                // Convert to base64 and make URL-safe
                string token = Convert.ToBase64String(tokenData)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");
                
                return token;
            }
        }

        /// <summary>
        /// Initiates a password reset request by generating a secure token.
        /// Returns the generated token if successful, null if user not found.
        /// </summary>
        public string RequestPasswordReset(string username)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty.");
            }

            // Get user by username
            User user = _dal.GetByUsername(username);
            if (user == null)
            {
                // For security, don't reveal if user exists or not
                // Return null but don't throw exception
                return null;
            }

            // Check if user is active
            if (!user.IsActive)
            {
                throw new Exception("This account is inactive. Please contact an administrator.");
            }

            // Generate secure token
            string token = GenerateSecureToken();

            // Create token record with 24-hour expiration
            PasswordResetToken resetToken = new PasswordResetToken
            {
                UserID = user.UserID,
                Token = token,
                ExpirationDate = DateTime.Now.AddHours(24),
                IsUsed = false
            };

            // Save token to database
            int tokenId = _tokenDal.Insert(resetToken);

            if (tokenId > 0)
            {
                return token;
            }

            throw new Exception("Failed to generate password reset token.");
        }

        /// <summary>
        /// Validates a password reset token.
        /// Returns true if token is valid, false otherwise.
        /// </summary>
        public bool ValidateResetToken(string token, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(token))
            {
                errorMessage = "Token cannot be empty.";
                return false;
            }

            // Get token from database
            PasswordResetToken resetToken = _tokenDal.GetByToken(token);

            if (resetToken == null)
            {
                errorMessage = "Invalid token.";
                return false;
            }

            // Check if token has been used
            if (resetToken.IsUsed)
            {
                errorMessage = "This token has already been used.";
                return false;
            }

            // Check if token has expired
            if (DateTime.Now > resetToken.ExpirationDate)
            {
                errorMessage = "This token has expired. Please request a new password reset.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Resets a user's password using a valid reset token.
        /// </summary>
        public bool ResetPasswordWithToken(string token, string newPassword, string confirmPassword)
        {
            // Validate passwords
            if (!IsValidPassword(newPassword, out string passwordError))
            {
                throw new ArgumentException(passwordError);
            }

            if (newPassword != confirmPassword)
            {
                throw new ArgumentException("Password and confirmation password do not match.");
            }

            // Validate token
            if (!ValidateResetToken(token, out string tokenError))
            {
                throw new Exception(tokenError);
            }

            // Get token from database
            PasswordResetToken resetToken = _tokenDal.GetByToken(token);

            // Get user
            User user = _dal.GetById(resetToken.UserID);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Hash new password
            user.PasswordHash = HashPassword(newPassword);

            // Update user password
            bool updateSuccess = _dal.Update(user) > 0;

            if (updateSuccess)
            {
                // Mark token as used
                _tokenDal.MarkAsUsed(token);
                return true;
            }

            throw new Exception("Failed to update password.");
        }

        /// <summary>
        /// Gets the user associated with a reset token (for display purposes).
        /// </summary>
        public User GetUserByResetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            PasswordResetToken resetToken = _tokenDal.GetByToken(token);
            if (resetToken == null)
            {
                return null;
            }

            return _dal.GetById(resetToken.UserID);
        }

        /// <summary>
        /// Cleans up expired tokens from the database.
        /// Should be called periodically (e.g., daily maintenance task).
        /// </summary>
        public int CleanupExpiredTokens()
        {
            return _tokenDal.DeleteExpiredTokens();
        }

        /// <summary>
        /// Admin function to reset a user's password directly without token.
        /// </summary>
        public bool AdminResetPassword(int userId, string newPassword)
        {
            // Validate password
            if (!IsValidPassword(newPassword, out string passwordError))
            {
                throw new ArgumentException(passwordError);
            }

            // Get user
            User user = _dal.GetById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Hash new password
            user.PasswordHash = HashPassword(newPassword);

            // Update user password
            return _dal.Update(user) > 0;
        }

        // Deactivate user (soft delete)
        public bool DeactivateUser(int userId)
        {
            User user = _dal.GetById(userId);
            if (user == null) return false;
            
            user.IsActive = false;
            return _dal.Update(user) > 0;
        }

        // Activate user
        public bool ActivateUser(int userId)
        {
            User user = _dal.GetById(userId);
            if (user == null) return false;
            
            user.IsActive = true;
            return _dal.Update(user) > 0;
        }
    }
}
