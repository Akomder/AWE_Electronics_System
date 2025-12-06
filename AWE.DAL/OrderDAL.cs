#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class OrderDAL
    {
        // --- CREATE ---
        public int Insert(Order entity)
        {
            string sql = @"INSERT INTO Orders (CustomerID, AgentID, OrderDate, Status, TotalAmount, 
                          ShippingAddress, ShippingCity, ShippingState, ShippingPostalCode, 
                          PaymentMethod, PaymentStatus, Notes) 
                          VALUES (@CustomerID, @AgentID, @OrderDate, @Status, @TotalAmount,
                          @ShippingAddress, @ShippingCity, @ShippingState, @ShippingPostalCode,
                          @PaymentMethod, @PaymentStatus, @Notes);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = GetSqlParameters(entity);
            object result = DbHelper.ExecuteScalar(sql, parameters.ToArray());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // --- READ ---
        public Order GetById(int id)
        {
            string sql = "SELECT * FROM Orders WHERE OrderID = @OrderID";
            var parameters = new SqlParameter[] { new SqlParameter("@OrderID", id) };
            
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            if (dt.Rows.Count > 0)
            {
                return MapRowToOrder(dt.Rows[0]);
            }
            return null;
        }

        public List<Order> GetAll()
        {
            string sql = "SELECT * FROM Orders ORDER BY OrderDate DESC";
            DataTable dt = DbHelper.ExecuteReader(sql);
            
            List<Order> list = new List<Order>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToOrder(row));
            }
            return list;
        }

        public List<Order> GetByStatus(string status)
        {
            string sql = "SELECT * FROM Orders WHERE Status = @Status ORDER BY OrderDate DESC";
            var parameters = new SqlParameter[] { new SqlParameter("@Status", status) };
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            List<Order> list = new List<Order>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToOrder(row));
            }
            return list;
        }

        public List<Order> GetByCustomer(int customerId)
        {
            string sql = "SELECT * FROM Orders WHERE CustomerID = @CustomerID ORDER BY OrderDate DESC";
            var parameters = new SqlParameter[] { new SqlParameter("@CustomerID", customerId) };
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            List<Order> list = new List<Order>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToOrder(row));
            }
            return list;
        }

        // --- UPDATE ---
        public int Update(Order entity)
        {
            string sql = @"UPDATE Orders 
                          SET CustomerID = @CustomerID, AgentID = @AgentID, Status = @Status,
                              TotalAmount = @TotalAmount, ShippingAddress = @ShippingAddress,
                              ShippingCity = @ShippingCity, ShippingState = @ShippingState,
                              ShippingPostalCode = @ShippingPostalCode, PaymentMethod = @PaymentMethod,
                              PaymentStatus = @PaymentStatus, ShippedDate = @ShippedDate,
                              DeliveredDate = @DeliveredDate, Notes = @Notes
                          WHERE OrderID = @OrderID";
            
            var parameters = GetSqlParameters(entity);
            parameters.Add(new SqlParameter("@OrderID", entity.OrderID));
            
            return DbHelper.ExecuteNonQuery(sql, parameters.ToArray());
        }

        public int UpdateStatus(int orderId, string status)
        {
            string sql = "UPDATE Orders SET Status = @Status WHERE OrderID = @OrderID";
            var parameters = new SqlParameter[] 
            { 
                new SqlParameter("@Status", status),
                new SqlParameter("@OrderID", orderId)
            };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- DELETE ---
        public int Delete(int id)
        {
            string sql = "DELETE FROM Orders WHERE OrderID = @OrderID";
            var parameters = new SqlParameter[] { new SqlParameter("@OrderID", id) };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- HELPER METHODS ---
        private Order MapRowToOrder(DataRow row)
        {
            return new Order
            {
                OrderID = Convert.ToInt32(row["OrderID"]),
                CustomerID = Convert.ToInt32(row["CustomerID"]),
                AgentID = row["AgentID"] != DBNull.Value ? (int?)Convert.ToInt32(row["AgentID"]) : null,
                OrderDate = Convert.ToDateTime(row["OrderDate"]),
                Status = row["Status"].ToString(),
                TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                ShippingAddress = row["ShippingAddress"] != DBNull.Value ? row["ShippingAddress"].ToString() : null,
                ShippingCity = row["ShippingCity"] != DBNull.Value ? row["ShippingCity"].ToString() : null,
                ShippingState = row["ShippingState"] != DBNull.Value ? row["ShippingState"].ToString() : null,
                ShippingPostalCode = row["ShippingPostalCode"] != DBNull.Value ? row["ShippingPostalCode"].ToString() : null,
                PaymentMethod = row["PaymentMethod"] != DBNull.Value ? row["PaymentMethod"].ToString() : null,
                PaymentStatus = row["PaymentStatus"] != DBNull.Value ? row["PaymentStatus"].ToString() : null,
                ShippedDate = row["ShippedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["ShippedDate"]) : null,
                DeliveredDate = row["DeliveredDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["DeliveredDate"]) : null,
                Notes = row["Notes"] != DBNull.Value ? row["Notes"].ToString() : null
            };
        }

        private List<SqlParameter> GetSqlParameters(Order entity)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@CustomerID", entity.CustomerID),
                new SqlParameter("@AgentID", (object)entity.AgentID ?? DBNull.Value),
                new SqlParameter("@OrderDate", entity.OrderDate),
                new SqlParameter("@Status", entity.Status),
                new SqlParameter("@TotalAmount", entity.TotalAmount),
                new SqlParameter("@ShippingAddress", (object)entity.ShippingAddress ?? DBNull.Value),
                new SqlParameter("@ShippingCity", (object)entity.ShippingCity ?? DBNull.Value),
                new SqlParameter("@ShippingState", (object)entity.ShippingState ?? DBNull.Value),
                new SqlParameter("@ShippingPostalCode", (object)entity.ShippingPostalCode ?? DBNull.Value),
                new SqlParameter("@PaymentMethod", (object)entity.PaymentMethod ?? DBNull.Value),
                new SqlParameter("@PaymentStatus", (object)entity.PaymentStatus ?? DBNull.Value),
                new SqlParameter("@ShippedDate", (object)entity.ShippedDate ?? DBNull.Value),
                new SqlParameter("@DeliveredDate", (object)entity.DeliveredDate ?? DBNull.Value),
                new SqlParameter("@Notes", (object)entity.Notes ?? DBNull.Value)
            };
        }
    }
}
