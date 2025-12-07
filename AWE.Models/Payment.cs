#nullable disable
using System;

namespace AWE.Models
{
    /// <summary>
    /// Represents a payment transaction
    /// </summary>
    public class Payment
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // PayPal, BankTransfer, Momo, etc.
        public string PaymentStatus { get; set; } // Pending, Completed, Failed, Refunded
        public string TransactionID { get; set; } // External transaction ID from payment provider
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Represents digital wallet (for customer balance)
    /// </summary>
    public class DigitalWallet
    {
        public int WalletID { get; set; }
        public int CustomerID { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } // VND, USD, etc.
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    /// <summary>
    /// Represents a wallet transaction (deposit, withdrawal, purchase)
    /// </summary>
    public class WalletTransaction
    {
        public int WalletTransactionID { get; set; }
        public int WalletID { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } // Deposit, Withdrawal, Purchase, Refund
        public string Description { get; set; }
        public string PaymentMethod { get; set; }
        public string ExternalTransactionID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
