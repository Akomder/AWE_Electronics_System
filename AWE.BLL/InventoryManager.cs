#nullable disable
using System;
using System.Collections.Generic;
using AWE.DAL;
using AWE.Models;

namespace AWE.BLL
{
    public class InventoryManager
    {
        private readonly GRNDAL _grnDal = new GRNDAL();
        private readonly GRNItemDAL _grnItemDal = new GRNItemDAL();
        private readonly GDNDAL _gdnDal = new GDNDAL();
        private readonly GDNItemDAL _gdnItemDal = new GDNItemDAL();
        private readonly ProductDAL _productDal = new ProductDAL();

        // Valid statuses
        public static readonly string[] ValidStatuses = { "Draft", "Approved", "Posted" };

        // --- GRN Operations ---

        public int CreateGRN(GoodsReceivedNote grn, List<GRNItem> items)
        {
            // Validation
            if (grn.SupplierID <= 0)
            {
                throw new ArgumentException("Invalid supplier ID.");
            }

            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("GRN must have at least one item.");
            }

            // Generate GRN number if not provided
            if (string.IsNullOrWhiteSpace(grn.GRNNumber))
            {
                grn.GRNNumber = _grnDal.GenerateGRNNumber();
            }

            // Set default status
            if (string.IsNullOrWhiteSpace(grn.Status))
            {
                grn.Status = "Draft";
            }

            // Calculate total amount
            decimal totalAmount = 0;
            foreach (var item in items)
            {
                item.TotalCost = item.QuantityReceived * item.UnitCost;
                totalAmount += item.TotalCost;
            }
            grn.TotalAmount = totalAmount;

            // Insert GRN
            int grnId = _grnDal.Insert(grn);

            if (grnId > 0)
            {
                // Insert GRN items
                foreach (var item in items)
                {
                    item.GRNID = grnId;
                    _grnItemDal.Insert(item);
                }

                // If status is Posted, update stock quantities
                if (grn.Status.Equals("Posted", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateStockFromGRN(grnId, items);
                }
            }

            return grnId;
        }

        public bool PostGRN(int grnId)
        {
            GoodsReceivedNote grn = _grnDal.GetById(grnId);
            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }

            if (grn.Status.Equals("Posted", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("GRN is already posted.");
            }

            // Get GRN items
            List<GRNItem> items = _grnItemDal.GetByGRNID(grnId);

            // Update stock quantities
            UpdateStockFromGRN(grnId, items);

            // Update GRN status
            return _grnDal.UpdateStatus(grnId, "Posted") > 0;
        }

        private void UpdateStockFromGRN(int grnId, List<GRNItem> items)
        {
            foreach (var item in items)
            {
                Product product = _productDal.GetById(item.ProductID);
                if (product != null)
                {
                    product.StockQuantity += item.QuantityReceived;
                    _productDal.Update(product);
                }
            }
        }

        public List<GoodsReceivedNote> GetAllGRNs()
        {
            return _grnDal.GetAll();
        }

        public GoodsReceivedNote GetGRNById(int id)
        {
            return _grnDal.GetById(id);
        }

        public List<GRNItem> GetGRNItems(int grnId)
        {
            return _grnItemDal.GetByGRNID(grnId);
        }

        // --- GDN Operations ---

        public int CreateGDN(GoodsDeliveryNote gdn, List<GDNItem> items)
        {
            // Validation
            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("GDN must have at least one item.");
            }

            // Generate GDN number if not provided
            if (string.IsNullOrWhiteSpace(gdn.GDNNumber))
            {
                gdn.GDNNumber = _gdnDal.GenerateGDNNumber();
            }

            // Set default status
            if (string.IsNullOrWhiteSpace(gdn.Status))
            {
                gdn.Status = "Draft";
            }

            // Insert GDN
            int gdnId = _gdnDal.Insert(gdn);

            if (gdnId > 0)
            {
                // Insert GDN items
                foreach (var item in items)
                {
                    item.GDNID = gdnId;
                    _gdnItemDal.Insert(item);
                }

                // If status is Posted, update stock quantities
                if (gdn.Status.Equals("Posted", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateStockFromGDN(gdnId, items);
                }
            }

            return gdnId;
        }

        public bool PostGDN(int gdnId)
        {
            GoodsDeliveryNote gdn = _gdnDal.GetById(gdnId);
            if (gdn == null)
            {
                throw new Exception("GDN not found.");
            }

            if (gdn.Status.Equals("Posted", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("GDN is already posted.");
            }

            // Get GDN items
            List<GDNItem> items = _gdnItemDal.GetByGDNID(gdnId);

            // Check stock availability
            foreach (var item in items)
            {
                Product product = _productDal.GetById(item.ProductID);
                if (product == null)
                {
                    throw new Exception($"Product ID {item.ProductID} not found.");
                }

                if (product.StockQuantity < item.QuantityDelivered)
                {
                    throw new Exception($"Insufficient stock for product '{product.ProductName}'. Available: {product.StockQuantity}, Required: {item.QuantityDelivered}");
                }
            }

            // Update stock quantities
            UpdateStockFromGDN(gdnId, items);

            // Update GDN status
            return _gdnDal.UpdateStatus(gdnId, "Posted") > 0;
        }

        private void UpdateStockFromGDN(int gdnId, List<GDNItem> items)
        {
            foreach (var item in items)
            {
                Product product = _productDal.GetById(item.ProductID);
                if (product != null)
                {
                    product.StockQuantity -= item.QuantityDelivered;
                    _productDal.Update(product);
                }
            }
        }

        public List<GoodsDeliveryNote> GetAllGDNs()
        {
            return _gdnDal.GetAll();
        }

        public GoodsDeliveryNote GetGDNById(int id)
        {
            return _gdnDal.GetById(id);
        }

        public List<GDNItem> GetGDNItems(int gdnId)
        {
            return _gdnItemDal.GetByGDNID(gdnId);
        }

        // --- Stock Level Reports ---

        public List<Product> GetLowStockProducts()
        {
            List<Product> allProducts = _productDal.GetAll();
            List<Product> lowStockProducts = new List<Product>();

            foreach (var product in allProducts)
            {
                if (product.IsActive && product.StockQuantity <= product.ReorderLevel)
                {
                    lowStockProducts.Add(product);
                }
            }

            return lowStockProducts;
        }

        public List<Product> GetOutOfStockProducts()
        {
            List<Product> allProducts = _productDal.GetAll();
            List<Product> outOfStockProducts = new List<Product>();

            foreach (var product in allProducts)
            {
                if (product.IsActive && product.StockQuantity == 0)
                {
                    outOfStockProducts.Add(product);
                }
            }

            return outOfStockProducts;
        }
    }
}
