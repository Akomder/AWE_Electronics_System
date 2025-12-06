#nullable disable
using Microsoft.AspNetCore.Mvc;
using AWE.BLL;
using AWE.WebApp.Filters;

namespace AWE.WebApp.Controllers
{
    [AuthorizeUser]
    public class ReportController : Controller
    {
        private readonly ProductManager _productManager;
        private readonly OrderManager _orderManager;

        public ReportController(ProductManager productManager, OrderManager orderManager)
        {
            _productManager = productManager;
            _orderManager = orderManager;
        }

        // GET: Report
        public IActionResult Index(string reportType = "LowStock")
        {
            try
            {
                ViewBag.ReportType = reportType;
                
                switch (reportType)
                {
                    case "LowStock":
                        var lowStockProducts = _productManager.GetLowStockProducts();
                        ViewBag.ReportData = lowStockProducts;
                        ViewBag.ReportTitle = "Low Stock Products";
                        break;
                        
                    case "OutOfStock":
                        var outOfStockProducts = _productManager.GetOutOfStockProducts();
                        ViewBag.ReportData = outOfStockProducts;
                        ViewBag.ReportTitle = "Out of Stock Products";
                        break;
                        
                    case "AllProducts":
                        var allProducts = _productManager.GetAllProducts();
                        ViewBag.ReportData = allProducts;
                        ViewBag.ReportTitle = "All Products Inventory";
                        break;
                        
                    case "Orders":
                        var allOrders = _orderManager.GetAllOrders();
                        ViewBag.ReportData = allOrders;
                        ViewBag.ReportTitle = "All Orders";
                        break;
                        
                    default:
                        ViewBag.ReportData = new List<object>();
                        ViewBag.ReportTitle = "Select a Report";
                        break;
                }
                
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error generating report: {ex.Message}";
                ViewBag.ReportType = reportType;
                ViewBag.ReportData = new List<object>();
                return View();
            }
        }
    }
}
