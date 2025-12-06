#nullable disable
using Microsoft.AspNetCore.Mvc;
using AWE.BLL;
using AWE.Models;
using AWE.WebApp.Filters;
using AWE.WebApp.Helpers;

namespace AWE.WebApp.Controllers
{
    [AuthorizeUser(AllowedRoles = new[] { "Admin" })]
    public class UserController : Controller
    {
        private readonly UserManager _userManager;

        public UserController(UserManager userManager)
        {
            _userManager = userManager;
        }

        // GET: User
        public IActionResult Index()
        {
            try
            {
                var users = _userManager.GetAllUsers();
                return View(users);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading users: {ex.Message}";
                return View(new List<User>());
            }
        }

        // GET: User/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var user = _userManager.GetUserById(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading user details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: User/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user, string password, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Password is required.";
                    return View(user);
                }

                if (password != confirmPassword)
                {
                    ViewBag.Error = "Passwords do not match.";
                    return View(user);
                }

                // Register user using UserManager
                int userId = _userManager.RegisterUser(user, password, confirmPassword);
                
                TempData["Success"] = $"User '{user.Username}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(user);
            }
        }

        // GET: User/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var user = _userManager.GetUserById(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading user: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User user)
        {
            if (id != user.UserID)
            {
                TempData["Error"] = "Invalid user ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                bool success = _userManager.UpdateUser(user);
                
                if (success)
                {
                    TempData["Success"] = $"User '{user.Username}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Failed to update user.";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(user);
            }
        }

        // POST: User/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deactivate(int id)
        {
            try
            {
                var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
                if (currentUser != null && currentUser.UserID == id)
                {
                    TempData["Error"] = "You cannot deactivate your own account.";
                    return RedirectToAction(nameof(Index));
                }

                bool success = _userManager.DeactivateUser(id);
                
                if (success)
                {
                    TempData["Success"] = "User deactivated successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to deactivate user.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        // POST: User/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Activate(int id)
        {
            try
            {
                bool success = _userManager.ActivateUser(id);
                
                if (success)
                {
                    TempData["Success"] = "User activated successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to activate user.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: User/ChangePassword/5
        [HttpGet]
        public IActionResult ChangePassword(int id)
        {
            try
            {
                var user = _userManager.GetUserById(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction(nameof(Index));
                }
                
                ViewBag.User = user;
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: User/ChangePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(int id, string newPassword, string confirmPassword)
        {
            try
            {
                var user = _userManager.GetUserById(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    ViewBag.Error = "New password is required.";
                    ViewBag.User = user;
                    return View();
                }

                if (newPassword != confirmPassword)
                {
                    ViewBag.Error = "Passwords do not match.";
                    ViewBag.User = user;
                    return View();
                }

                bool success = _userManager.ChangePassword(id, newPassword, confirmPassword);
                
                if (success)
                {
                    TempData["Success"] = $"Password changed successfully for user '{user.Username}'!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Failed to change password.";
                    ViewBag.User = user;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var user = _userManager.GetUserById(id);
                ViewBag.User = user;
                return View();
            }
        }
    }
}
