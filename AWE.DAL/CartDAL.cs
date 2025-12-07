#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class CartDAL
    {
        /// <summary>
        /// Get or create shopping cart for customer
        /// </summary>
        public ShoppingCart GetOrCreateCart(int customerId)
        {
            string query = @"
                SELECT TOP 1 CartID, CustomerID, CreatedDate, LastModified, IsActive
                FROM ShoppingCart
                WHERE CustomerID = @CustomerID AND IsActive = 1
                ORDER BY LastModified DESC
            ";

            var parameters = new[] { new SqlParameter("@CustomerID", customerId) };
            DataTable dt = DbHelper.ExecuteReader(query, parameters);

            if (dt.Rows.Count > 0)
            {
                return MapRowToCart(dt.Rows[0]);
            }

            // Create new cart
            string insertQuery = @"
                INSERT INTO ShoppingCart (CustomerID, CreatedDate, LastModified, IsActive)
                VALUES (@CustomerID, @CreatedDate, @LastModified, 1)
                SELECT SCOPE_IDENTITY()
            ";

            var insertParams = new[]
            {
                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@CreatedDate", DateTime.Now),
                new SqlParameter("@LastModified", DateTime.Now)
            };

            var result = DbHelper.ExecuteScalar(insertQuery, insertParams);
            if (result != null && int.TryParse(result.ToString(), out int cartId))
            {
                return new ShoppingCart
                {
                    CartID = cartId,
                    CustomerID = customerId,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now,
                    IsActive = true
                };
            }

            return null;
        }

        /// <summary>
        /// Add item to cart
        /// </summary>
        public int AddToCart(int cartId, int productId, int quantity, decimal price)
        {
            string query = @"
                INSERT INTO CartItem (CartID, ProductID, Quantity, PriceAtTime, AddedDate)
                VALUES (@CartID, @ProductID, @Quantity, @PriceAtTime, @AddedDate)
                SELECT SCOPE_IDENTITY()
            ";

            var parameters = new[]
            {
                new SqlParameter("@CartID", cartId),
                new SqlParameter("@ProductID", productId),
                new SqlParameter("@Quantity", quantity),
                new SqlParameter("@PriceAtTime", price),
                new SqlParameter("@AddedDate", DateTime.Now)
            };

            var result = DbHelper.ExecuteScalar(query, parameters);
            return result != null ? int.Parse(result.ToString()) : -1;
        }

        /// <summary>
        /// Get cart items
        /// </summary>
        public List<CartItem> GetCartItems(int cartId)
        {
            string query = @"
                SELECT CartItemID, CartID, ProductID, Quantity, PriceAtTime, AddedDate
                FROM CartItem
                WHERE CartID = @CartID
                ORDER BY AddedDate DESC
            ";

            var parameters = new[] { new SqlParameter("@CartID", cartId) };
            DataTable dt = DbHelper.ExecuteReader(query, parameters);

            List<CartItem> items = new List<CartItem>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add(MapRowToCartItem(row));
            }

            return items;
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        public bool RemoveFromCart(int cartItemId)
        {
            string query = "DELETE FROM CartItem WHERE CartItemID = @CartItemID";
            var parameters = new[] { new SqlParameter("@CartItemID", cartItemId) };
            return DbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Clear entire cart
        /// </summary>
        public bool ClearCart(int cartId)
        {
            string query = "DELETE FROM CartItem WHERE CartID = @CartID";
            var parameters = new[] { new SqlParameter("@CartID", cartId) };
            return DbHelper.ExecuteNonQuery(query, parameters) >= 0;
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        public bool UpdateCartItemQuantity(int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                return RemoveFromCart(cartItemId);
            }

            string query = "UPDATE CartItem SET Quantity = @Quantity WHERE CartItemID = @CartItemID";
            var parameters = new[]
            {
                new SqlParameter("@CartItemID", cartItemId),
                new SqlParameter("@Quantity", quantity)
            };

            return DbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        private ShoppingCart MapRowToCart(DataRow row)
        {
            return new ShoppingCart
            {
                CartID = (int)row["CartID"],
                CustomerID = (int)row["CustomerID"],
                CreatedDate = (DateTime)row["CreatedDate"],
                LastModified = row["LastModified"] != DBNull.Value ? (DateTime)row["LastModified"] : null,
                IsActive = (bool)row["IsActive"]
            };
        }

        private CartItem MapRowToCartItem(DataRow row)
        {
            return new CartItem
            {
                CartItemID = (int)row["CartItemID"],
                CartID = (int)row["CartID"],
                ProductID = (int)row["ProductID"],
                Quantity = (int)row["Quantity"],
                PriceAtTime = (decimal)row["PriceAtTime"],
                AddedDate = (DateTime)row["AddedDate"]
            };
        }
    }
}
