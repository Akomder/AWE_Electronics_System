#nullable disable
using Microsoft.AspNetCore.Mvc;
using AWE.BLL;
using AWE.Models;
using AWE.WebApp.Filters;

namespace AWE.WebApp.Controllers
{
    [AuthorizeUser(AllowedRoles = new[] { "Admin", "Manager", "Staff" })]
    public class SupplierController : Controller
    {
        private readonly SupplierManager _supplierManager;

        public SupplierController(SupplierManager supplierManager)
        {
            _supplierManager = supplierManager;
        }

        // GET: Supplier
        public IActionResult Index()
        {
            try
            {
                var suppliers = _supplierManager.GetAllSuppliers();
                return View(suppliers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading suppliers: {ex.Message}";
                return View(new List<Supplier>());
            }
        }

        // GET: Supplier/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var supplier = _supplierManager.GetSupplierById(id);
                if (supplier == null)
                {
                    TempData["Error"] = "Supplier not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading supplier: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Supplier/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier supplier)
        {
            try
            {
                supplier.IsActive = true; // Default to active
                int supplierId = _supplierManager.CreateSupplier(supplier);
                
                TempData["Success"] = $"Supplier '{supplier.SupplierName}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(supplier);
            }
        }

        // GET: Supplier/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var supplier = _supplierManager.GetSupplierById(id);
                if (supplier == null)
                {
                    TempData["Error"] = "Supplier not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading supplier: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Supplier supplier)
        {
            if (id != supplier.SupplierID)
            {
                TempData["Error"] = "Invalid supplier ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                bool success = _supplierManager.UpdateSupplier(supplier);
                
                if (success)
                {
                    TempData["Success"] = $"Supplier '{supplier.SupplierName}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Failed to update supplier.";
                    return View(supplier);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(supplier);
            }
        }

        // POST: Supplier/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                bool success = _supplierManager.DeleteSupplier(id);
                
                if (success)
                {
                    TempData["Success"] = "Supplier deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete supplier.";
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
