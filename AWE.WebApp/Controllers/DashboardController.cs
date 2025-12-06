#nullable disable
using Microsoft.AspNetCore.Mvc;
using AWE.BLL;
using AWE.WebApp.Filters;
using AWE.WebApp.Helpers;

namespace AWE.WebApp.Controllers
{
    [AuthorizeUser]
    public class DashboardController : Controller
    {
        private readonly ProductManager _productManager;
        private readonly OrderManager _orderManager;
        private readonly InventoryManager _inventoryManager;

        public DashboardController(ProductManager productManager, OrderManager orderManager, InventoryManager inventoryManager)
        {
            _productManager = productManager;
            _orderManager = orderManager;
            _inventoryManager = inventoryManager;
        }

        public IActionResult Index()
        {
            var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
            ViewBag.CurrentUser = currentUser;

            try
            {
                // Get dashboard statistics
                var allProducts = _productManager.GetAllProducts();
                ViewBag.TotalProducts = allProducts.Count;
                ViewBag.ActiveProducts = allProducts.Count(p => p.IsActive);
                
                ViewBag.LowStockCount = _inventoryManager.GetLowStockProducts().Count;
                ViewBag.OutOfStockCount = _inventoryManager.GetOutOfStockProducts().Count;
                
                var allOrders = _orderManager.GetAllOrders();
                ViewBag.TotalOrders = allOrders.Count;
                ViewBag.PendingOrders = _orderManager.GetOrdersByStatus("Pending").Count;
                
                // Calculate total inventory value
                decimal totalValue = allProducts.Where(p => p.IsActive).Sum(p => p.Price * p.StockQuantity);
                ViewBag.TotalInventoryValue = totalValue;
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading dashboard data: {ex.Message}";
            }

            return View();
        }
    }
}
