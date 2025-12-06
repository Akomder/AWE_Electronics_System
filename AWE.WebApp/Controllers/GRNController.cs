#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AWE.BLL;
using AWE.Models;
using AWE.WebApp.Filters;

namespace AWE.WebApp.Controllers
{
    [AuthorizeUser(AllowedRoles = new[] { "Admin", "Manager", "Staff" })]
    public class GRNController : Controller
    {
        private readonly InventoryManager _inventoryManager;
        private readonly SupplierManager _supplierManager;
        private readonly ProductManager _productManager;

        public GRNController(InventoryManager inventoryManager, SupplierManager supplierManager, ProductManager productManager)
        {
            _inventoryManager = inventoryManager;
            _supplierManager = supplierManager;
            _productManager = productManager;
        }

        // GET: GRN
        public IActionResult Index()
        {
            try
            {
                var grns = _inventoryManager.GetAllGRNs();
                return View(grns);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading GRNs: {ex.Message}";
                return View(new List<GoodsReceivedNote>());
            }
        }

        // GET: GRN/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var grn = _inventoryManager.GetGRNById(id);
                if (grn == null)
                {
                    TempData["Error"] = "GRN not found.";
                    return RedirectToAction(nameof(Index));
                }
                
                var items = _inventoryManager.GetGRNItems(id);
                ViewBag.Items = items;
                
                return View(grn);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading GRN: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: GRN/Create
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                ViewBag.Suppliers = new SelectList(_supplierManager.GetActiveSuppliers(), "SupplierID", "SupplierName");
                ViewBag.Products = new SelectList(_productManager.GetAllProducts(), "ProductID", "ProductName");
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading form: {ex.Message}";
                return View();
            }
        }

        // POST: GRN/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GoodsReceivedNote grn, List<GRNItem> items)
        {
            try
            {
                grn.Status = "Draft";
                grn.ReceivedDate = DateTime.Now;
                
                int grnId = _inventoryManager.CreateGRN(grn, items);
                
                TempData["Success"] = $"GRN #{grn.GRNNumber} created successfully!";
                return RedirectToAction(nameof(Details), new { id = grnId });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Suppliers = new SelectList(_supplierManager.GetActiveSuppliers(), "SupplierID", "SupplierName");
                ViewBag.Products = new SelectList(_productManager.GetAllProducts(), "ProductID", "ProductName");
                return View(grn);
            }
        }

        // POST: GRN/Post/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedRoles = new[] { "Admin", "Manager" })]
        public IActionResult Post(int id)
        {
            try
            {
                bool success = _inventoryManager.PostGRN(id);
                
                if (success)
                {
                    TempData["Success"] = "GRN posted successfully! Stock quantities have been updated.";
                }
                else
                {
                    TempData["Error"] = "Failed to post GRN.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
