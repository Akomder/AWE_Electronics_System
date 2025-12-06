#nullable disable
using System.Collections.Generic;
using System.Linq;
using AWE.DAL;
using AWE.Models;

namespace AWE.BLL
{
    public class ProductManager
    {
        private readonly ProductDAL _dal = new ProductDAL();

        public Product GetProductById(int id)
        {
            // Add business logic/validation here if needed
            return _dal.GetById(id);
        }

        public List<Product> GetAllProducts()
        {
            return _dal.GetAll();
        }

        public bool CreateProduct(Product entity)
        {
            // Example: Add validation logic before insertion
            if (string.IsNullOrEmpty(entity.ProductName)) // Simple validation for Product
            {
                return false;
            }
            return _dal.Insert(entity) > 0;
        }

        public bool UpdateProduct(Product entity)
        {
            // Example: Check if entity exists before updating
            if (_dal.GetById(entity.ProductID) == null)
            {
                return false;
            }
            return _dal.Update(entity) > 0;
        }

        public bool DeleteProduct(int id)
        {
            return _dal.Delete(id) > 0;
        }

        // Get low stock products (at or below reorder level)
        public List<Product> GetLowStockProducts()
        {
            return _dal.GetAll().Where(p => p.IsActive && p.StockQuantity <= p.ReorderLevel && p.StockQuantity > 0).ToList();
        }

        // Get out of stock products
        public List<Product> GetOutOfStockProducts()
        {
            return _dal.GetAll().Where(p => p.IsActive && p.StockQuantity == 0).ToList();
        }

        // Add product (alias for CreateProduct)
        public int AddProduct(Product product)
        {
            return CreateProduct(product) ? 1 : 0;
        }
    }
}
