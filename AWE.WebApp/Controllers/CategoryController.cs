#nullable disable
using Microsoft.AspNetCore.Mvc;
using AWE.BLL;
using AWE.Models;
using AWE.WebApp.Filters;

namespace AWE.WebApp.Controllers
{
    [AuthorizeUser(AllowedRoles = new[] { "Admin", "Manager", "Staff" })]
    public class CategoryController : Controller
    {
        private readonly CategoryManager _categoryManager;

        public CategoryController(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        // GET: Category
        public IActionResult Index()
        {
            try
            {
                var categories = _categoryManager.GetAllCategories();
                return View(categories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading categories: {ex.Message}";
                return View(new List<Category>());
            }
        }

        // GET: Category/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var category = _categoryManager.GetCategoryById(id);
                if (category == null)
                {
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading category: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            try
            {
                category.IsActive = true; // Default to active
                int categoryId = _categoryManager.CreateCategory(category);
                
                TempData["Success"] = $"Category '{category.CategoryName}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(category);
            }
        }

        // GET: Category/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var category = _categoryManager.GetCategoryById(id);
                if (category == null)
                {
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading category: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.CategoryID)
            {
                TempData["Error"] = "Invalid category ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                bool success = _categoryManager.UpdateCategory(category);
                
                if (success)
                {
                    TempData["Success"] = $"Category '{category.CategoryName}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Failed to update category.";
                    return View(category);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(category);
            }
        }

        // POST: Category/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                bool success = _categoryManager.DeleteCategory(id);
                
                if (success)
                {
                    TempData["Success"] = "Category deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete category.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
