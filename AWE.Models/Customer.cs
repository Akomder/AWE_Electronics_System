#nullable disable
using System;

namespace AWE.Models
{
    /// <summary>
    /// Represents a B2C customer (different from internal staff users)
    /// </summary>
    public class Customer
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingPostalCode { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingPostalCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    /// <summary>
    /// Represents a shopping cart for a customer
    /// </summary>
    public class ShoppingCart
    {
        public int CartID { get; set; }
        public int CustomerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Represents items in a shopping cart
    /// </summary>
    public class CartItem
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTime { get; set; }
        public DateTime AddedDate { get; set; }
    }

    /// <summary>
    /// Represents a customer review
    /// </summary>
    public class Review
    {
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public int CustomerID { get; set; }
        public int Rating { get; set; } // 1-5
        public string Title { get; set; }
        public string Comment { get; set; }
        public int HelpfulCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
