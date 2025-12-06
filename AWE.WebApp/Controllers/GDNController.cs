#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AWE.BLL;
using AWE.Models;
using AWE.WebApp.Filters;

namespace AWE.WebApp.Controllers
{
    [AuthorizeUser(AllowedRoles = new[] { "Admin", "Manager", "Staff" })]
    public class GDNController : Controller
    {
        private readonly InventoryManager _inventoryManager;
        private readonly ProductManager _productManager;

        public GDNController(InventoryManager inventoryManager, ProductManager productManager)
        {
            _inventoryManager = inventoryManager;
            _productManager = productManager;
        }

        // GET: GDN
        public IActionResult Index()
        {
            try
            {
                var gdns = _inventoryManager.GetAllGDNs();
                return View(gdns);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading GDNs: {ex.Message}";
                return View(new List<GoodsDeliveryNote>());
            }
        }

        // GET: GDN/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var gdn = _inventoryManager.GetGDNById(id);
                if (gdn == null)
                {
                    TempData["Error"] = "GDN not found.";
                    return RedirectToAction(nameof(Index));
                }
                
                var items = _inventoryManager.GetGDNItems(id);
                ViewBag.Items = items;
                
                return View(gdn);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading GDN: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: GDN/Create
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                ViewBag.Products = new SelectList(_productManager.GetAllProducts(), "ProductID", "ProductName");
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading form: {ex.Message}";
                return View();
            }
        }

        // POST: GDN/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GoodsDeliveryNote gdn, List<GDNItem> items)
        {
            try
            {
                gdn.Status = "Draft";
                gdn.DeliveryDate = DateTime.Now;
                
                int gdnId = _inventoryManager.CreateGDN(gdn, items);
                
                TempData["Success"] = $"GDN #{gdn.GDNNumber} created successfully!";
                return RedirectToAction(nameof(Details), new { id = gdnId });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Products = new SelectList(_productManager.GetAllProducts(), "ProductID", "ProductName");
                return View(gdn);
            }
        }

        // POST: GDN/Post/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedRoles = new[] { "Admin", "Manager" })]
        public IActionResult Post(int id)
        {
            try
            {
                bool success = _inventoryManager.PostGDN(id);
                
                if (success)
                {
                    TempData["Success"] = "GDN posted successfully! Stock quantities have been reduced.";
                }
                else
                {
                    TempData["Error"] = "Failed to post GDN.";
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
