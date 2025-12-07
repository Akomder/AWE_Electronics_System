#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class PaymentDAL
    {
        /// <summary>
        /// Create new payment record
        /// </summary>
        public int Insert(Payment payment)
        {
            string query = @"
                INSERT INTO Payments (OrderID, CustomerID, Amount, PaymentMethod, PaymentStatus, 
                                   TransactionID, Description, CreatedDate, ErrorMessage)
                VALUES (@OrderID, @CustomerID, @Amount, @PaymentMethod, @PaymentStatus, 
                       @TransactionID, @Description, @CreatedDate, @ErrorMessage)
                SELECT SCOPE_IDENTITY()
            ";

            var parameters = new[]
            {
                new SqlParameter("@OrderID", payment.OrderID),
                new SqlParameter("@CustomerID", payment.CustomerID),
                new SqlParameter("@Amount", payment.Amount),
                new SqlParameter("@PaymentMethod", payment.PaymentMethod ?? ""),
                new SqlParameter("@PaymentStatus", payment.PaymentStatus ?? "Pending"),
                new SqlParameter("@TransactionID", payment.TransactionID ?? ""),
                new SqlParameter("@Description", payment.Description ?? ""),
                new SqlParameter("@CreatedDate", payment.CreatedDate),
                new SqlParameter("@ErrorMessage", payment.ErrorMessage ?? "")
            };

            var result = DbHelper.ExecuteScalar(query, parameters);
            return result != null ? int.Parse(result.ToString()) : -1;
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        public Payment GetById(int id)
        {
            string query = @"
                SELECT PaymentID, OrderID, CustomerID, Amount, PaymentMethod, PaymentStatus, 
                       TransactionID, Description, CreatedDate, CompletedDate, ErrorMessage
                FROM Payments
                WHERE PaymentID = @PaymentID
            ";

            var parameters = new[] { new SqlParameter("@PaymentID", id) };
            DataTable dt = DbHelper.ExecuteReader(query, parameters);

            return dt.Rows.Count > 0 ? MapRowToEntity(dt.Rows[0]) : null;
        }

        /// <summary>
        /// Get payment by transaction ID
        /// </summary>
        public Payment GetByTransactionId(string transactionId)
        {
            string query = @"
                SELECT PaymentID, OrderID, CustomerID, Amount, PaymentMethod, PaymentStatus, 
                       TransactionID, Description, CreatedDate, CompletedDate, ErrorMessage
                FROM Payments
                WHERE TransactionID = @TransactionID
            ";

            var parameters = new[] { new SqlParameter("@TransactionID", transactionId) };
            DataTable dt = DbHelper.ExecuteReader(query, parameters);

            return dt.Rows.Count > 0 ? MapRowToEntity(dt.Rows[0]) : null;
        }

        /// <summary>
        /// Get payments for order
        /// </summary>
        public List<Payment> GetByOrderId(int orderId)
        {
            string query = @"
                SELECT PaymentID, OrderID, CustomerID, Amount, PaymentMethod, PaymentStatus, 
                       TransactionID, Description, CreatedDate, CompletedDate, ErrorMessage
                FROM Payments
                WHERE OrderID = @OrderID
                ORDER BY CreatedDate DESC
            ";

            var parameters = new[] { new SqlParameter("@OrderID", orderId) };
            DataTable dt = DbHelper.ExecuteReader(query, parameters);

            var payments = new List<Payment>();
            foreach (DataRow row in dt.Rows)
            {
                payments.Add(MapRowToEntity(row));
            }

            return payments;
        }

        /// <summary>
        /// Update payment status
        /// </summary>
        public int UpdatePaymentStatus(int paymentId, string status, DateTime? completedDate = null)
        {
            string query = @"
                UPDATE Payments
                SET PaymentStatus = @PaymentStatus, CompletedDate = @CompletedDate
                WHERE PaymentID = @PaymentID
            ";

            var parameters = new[]
            {
                new SqlParameter("@PaymentID", paymentId),
                new SqlParameter("@PaymentStatus", status),
                new SqlParameter("@CompletedDate", completedDate ?? (object)DBNull.Value)
            };

            return DbHelper.ExecuteNonQuery(query, parameters);
        }

        private Payment MapRowToEntity(DataRow row)
        {
            return new Payment
            {
                PaymentID = (int)row["PaymentID"],
                OrderID = (int)row["OrderID"],
                CustomerID = (int)row["CustomerID"],
                Amount = (decimal)row["Amount"],
                PaymentMethod = row["PaymentMethod"] != DBNull.Value ? row["PaymentMethod"].ToString() : "",
                PaymentStatus = row["PaymentStatus"] != DBNull.Value ? row["PaymentStatus"].ToString() : "",
                TransactionID = row["TransactionID"] != DBNull.Value ? row["TransactionID"].ToString() : "",
                Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : "",
                CreatedDate = (DateTime)row["CreatedDate"],
                CompletedDate = row["CompletedDate"] != DBNull.Value ? (DateTime)row["CompletedDate"] : null,
                ErrorMessage = row["ErrorMessage"] != DBNull.Value ? row["ErrorMessage"].ToString() : ""
            };
        }
    }
}
