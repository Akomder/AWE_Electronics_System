#nullable disable
using System;

namespace AWE.Models
{
    public class GoodsReceivedNote
    {
        public int GRNID { get; set; }
        public string GRNNumber { get; set; }
        public int SupplierID { get; set; }
        public DateTime ReceivedDate { get; set; }
        public int ReceivedByUserID { get; set; }
        public string Status { get; set; } // Draft, Approved, Posted
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class GRNItem
    {
        public int GRNItemID { get; set; }
        public int GRNID { get; set; }
        public int ProductID { get; set; }
        public int QuantityReceived { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string Notes { get; set; }
    }

    public class GoodsDeliveryNote
    {
        public int GDNID { get; set; }
        public string GDNNumber { get; set; }
        public int? OrderID { get; set; }
        public int? CustomerID { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int DeliveredByUserID { get; set; }
        public string Status { get; set; } // Draft, Approved, Posted
        public string DeliveryAddress { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class GDNItem
    {
        public int GDNItemID { get; set; }
        public int GDNID { get; set; }
        public int ProductID { get; set; }
        public int QuantityDelivered { get; set; }
        public string Notes { get; set; }
    }
}
