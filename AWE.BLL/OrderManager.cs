#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using AWE.DAL;
using AWE.Models;

namespace AWE.BLL
{
    public class OrderManager
    {
        private readonly OrderDAL _orderDAL = new OrderDAL();

        // Get all orders
        public List<Order> GetAllOrders()
        {
            return _orderDAL.GetAll();
        }

        // Get order by ID
        public Order GetOrderById(int id)
        {
            return _orderDAL.GetById(id);
        }

        // Get orders by customer ID
        public List<Order> GetOrdersByCustomer(int customerId)
        {
            return _orderDAL.GetByCustomer(customerId);
        }

        // Get orders by agent ID
        public List<Order> GetOrdersByAgent(int agentId)
        {
            return new List<Order>(); 
        }

        // Get orders by status
        public List<Order> GetOrdersByStatus(string status)
        {
            return _orderDAL.GetByStatus(status);
        }

        // Create new order
        public int CreateOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            order.OrderDate = DateTime.Now;
            order.Status = "Pending";
            return _orderDAL.Insert(order);
        }

        // Update order
        public bool UpdateOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return _orderDAL.Update(order) > 0;
        }

        // Update order status
        public bool UpdateOrderStatus(int orderId, string newStatus)
        {
            Order order = _orderDAL.GetById(orderId);
            if (order == null)
                return false;

            order.Status = newStatus;

            // Update shipped/delivered dates
            if (newStatus == "Shipped" && !order.ShippedDate.HasValue)
                order.ShippedDate = DateTime.Now;
            else if (newStatus == "Delivered" && !order.DeliveredDate.HasValue)
                order.DeliveredDate = DateTime.Now;

            return _orderDAL.Update(order) > 0;
        }

        // Cancel order
        public bool CancelOrder(int orderId, string reason)
        {
            Order order = _orderDAL.GetById(orderId);
            if (order == null)
                return false;

            order.Status = "Cancelled";
            order.Notes = reason;
            return _orderDAL.Update(order) > 0;
        }

        // Delete order
        public bool DeleteOrder(int id)
        {
            return _orderDAL.Delete(id) > 0;
        }

        // Get order items for a specific order
        public List<OrderItem> GetOrderItems(int orderId)
        {
            return new List<OrderItem>();
        }

        public static readonly string[] ValidStatuses = new string[]
        {
            "Pending",
            "Processing",
            "Shipped",
            "Delivered",
            "Cancelled"
        };
    }
}
