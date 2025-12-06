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
