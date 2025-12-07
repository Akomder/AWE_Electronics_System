#nullable disable
using System;
using System.Collections.Generic;
using AWE.DAL;
using AWE.Models;

namespace AWE.BLL
{
    public class CartManager
    {
        private readonly CartDAL _dal = new CartDAL();

        /// <summary>
        /// Get or create cart for customer
        /// </summary>
        public ShoppingCart GetOrCreateCart(int customerId)
        {
            return _dal.GetOrCreateCart(customerId);
        }

        /// <summary>
        /// Add product to cart
        /// </summary>
        public int AddToCart(int cartId, int productId, int quantity, decimal price)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            if (price < 0)
                throw new ArgumentException("Price cannot be negative");

            return _dal.AddToCart(cartId, productId, quantity, price);
        }

        /// <summary>
        /// Get all items in cart
        /// </summary>
        public List<CartItem> GetCartItems(int cartId)
        {
            return _dal.GetCartItems(cartId);
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        public bool RemoveFromCart(int cartItemId)
        {
            return _dal.RemoveFromCart(cartItemId);
        }

        /// <summary>
        /// Clear entire cart
        /// </summary>
        public bool ClearCart(int cartId)
        {
            return _dal.ClearCart(cartId);
        }

        /// <summary>
        /// Update quantity of cart item
        /// </summary>
        public bool UpdateCartItemQuantity(int cartItemId, int quantity)
        {
            return _dal.UpdateCartItemQuantity(cartItemId, quantity);
        }

        /// <summary>
        /// Calculate cart total
        /// </summary>
        public decimal CalculateCartTotal(int cartId)
        {
            var items = _dal.GetCartItems(cartId);
            decimal total = 0;

            foreach (var item in items)
            {
                total += item.Quantity * item.PriceAtTime;
            }

            return total;
        }
    }
}
