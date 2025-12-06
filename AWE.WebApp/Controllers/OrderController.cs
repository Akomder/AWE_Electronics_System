#nullable disable
using Microsoft.AspNetCore.Mvc;
using AWE.BLL;
using AWE.Models;
using AWE.WebApp.Filters;

namespace AWE.WebApp.Controllers
{
    [AuthorizeUser]
    public class OrderController : Controller
    {
        private readonly OrderManager _orderManager;

        public OrderController(OrderManager orderManager)
        {
            _orderManager = orderManager;
        }

        // GET: Order
        public IActionResult Index(string status = "All")
        {
            try
            {
                List<Order> orders;
                
                if (status == "All")
                {
                    orders = _orderManager.GetAllOrders();
                }
                else
                {
                    orders = _orderManager.GetOrdersByStatus(status);
                }
                
                ViewBag.CurrentStatus = status;
                return View(orders);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading orders: {ex.Message}";
                ViewBag.CurrentStatus = status;
                return View(new List<Order>());
            }
        }

        // GET: Order/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var order = _orderManager.GetOrderById(id);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction(nameof(Index));
                }
                
                // Get order items
                var orderItems = _orderManager.GetOrderItems(id);
                ViewBag.OrderItems = orderItems;
                
                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading order: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Order/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedRoles = new[] { "Admin", "Manager", "Staff" })]
        public IActionResult UpdateStatus(int orderId, string newStatus)
        {
            try
            {
                bool success = _orderManager.UpdateOrderStatus(orderId, newStatus);
                
                if (success)
                {
                    TempData["Success"] = $"Order #{orderId} status updated to '{newStatus}'!";
                }
                else
                {
                    TempData["Error"] = "Failed to update order status.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        // POST: Order/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(AllowedRoles = new[] { "Admin", "Manager" })]
        public IActionResult Cancel(int orderId, string cancelReason)
        {
            try
            {
                bool success = _orderManager.CancelOrder(orderId, cancelReason);
                
                if (success)
                {
                    TempData["Success"] = $"Order #{orderId} has been cancelled.";
                }
                else
                {
                    TempData["Error"] = "Failed to cancel order.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            
            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}
