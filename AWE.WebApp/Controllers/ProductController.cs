#nullable disable
using Microsoft.AspNetCore.Mvc;
using AWE.BLL;
using AWE.Models;
using AWE.WebApp.Filters;

namespace AWE.WebApp.Controllers
{
    [AuthorizeUser]
    public class ProductController : Controller
    {
        private readonly ProductManager _productManager;
        private readonly CategoryManager _categoryManager;
        private readonly SupplierManager _supplierManager;

        public ProductController(ProductManager productManager, CategoryManager categoryManager, SupplierManager supplierManager)
        {
            _productManager = productManager;
            _categoryManager = categoryManager;
            _supplierManager = supplierManager;
        }

        // GET: Product
        public IActionResult Index()
        {
            var products = _productManager.GetAllProducts();
            return View(products);
        }

        // GET: Product/Details/5
        public IActionResult Details(int id)
        {
            var product = _productManager.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Product/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _categoryManager.GetActiveCategories();
            ViewBag.Suppliers = _supplierManager.GetActiveSuppliers();
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            try
            {
                // Set default values
                product.IsActive = true;
                product.CreatedDate = DateTime.Now;
                product.LastUpdated = DateTime.Now;

                int productId = _productManager.AddProduct(product);
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Categories = _categoryManager.GetActiveCategories();
                ViewBag.Suppliers = _supplierManager.GetActiveSuppliers();
                return View(product);
            }
        }

        // GET: Product/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _productManager.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _categoryManager.GetActiveCategories();
            ViewBag.Suppliers = _supplierManager.GetActiveSuppliers();
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            try
            {
                product.LastUpdated = DateTime.Now;
                bool success = _productManager.UpdateProduct(product);
                
                if (success)
                {
                    TempData["Success"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Failed to update product.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.Categories = _categoryManager.GetActiveCategories();
            ViewBag.Suppliers = _supplierManager.GetActiveSuppliers();
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                bool success = _productManager.DeleteProduct(id);
                if (success)
                {
                    TempData["Success"] = "Product deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete product.";
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
