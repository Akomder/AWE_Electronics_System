#nullable disable
using System;
using System.Collections.Generic;
using AWE.DAL;
using AWE.Models;

namespace AWE.BLL
{
    public class PaymentManager
    {
        private readonly PaymentDAL _dal = new PaymentDAL();

        /// <summary>
        /// Create payment record
        /// </summary>
        public int CreatePayment(Payment payment)
        {
            if (payment.Amount <= 0)
                throw new ArgumentException("Payment amount must be greater than 0");

            if (string.IsNullOrWhiteSpace(payment.PaymentMethod))
                throw new ArgumentException("Payment method is required");

            payment.CreatedDate = DateTime.Now;
            payment.PaymentStatus = "Pending";

            return _dal.Insert(payment);
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        public Payment GetPaymentById(int paymentId)
        {
            return _dal.GetById(paymentId);
        }

        /// <summary>
        /// Get payment by transaction ID
        /// </summary>
        public Payment GetPaymentByTransactionId(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Transaction ID is required");

            return _dal.GetByTransactionId(transactionId);
        }

        /// <summary>
        /// Get all payments for an order
        /// </summary>
        public List<Payment> GetPaymentsByOrderId(int orderId)
        {
            return _dal.GetByOrderId(orderId);
        }

        /// <summary>
        /// Mark payment as completed
        /// </summary>
        public bool MarkPaymentCompleted(int paymentId)
        {
            int result = _dal.UpdatePaymentStatus(paymentId, "Completed", DateTime.Now);
            return result > 0;
        }

        /// <summary>
        /// Mark payment as failed
        /// </summary>
        public bool MarkPaymentFailed(int paymentId, string errorMessage)
        {
            // Note: UpdatePaymentStatus doesn't have error message, consider extending
            int result = _dal.UpdatePaymentStatus(paymentId, "Failed");
            return result > 0;
        }

        /// <summary>
        /// Verify payment (in production, check with payment provider)
        /// </summary>
        public bool VerifyPayment(string transactionId)
        {
            var payment = _dal.GetByTransactionId(transactionId);
            return payment != null && payment.PaymentStatus == "Pending";
        }
    }
}
