using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class ProductDAL
    {
        private Product MapRowToEntity(DataRow row)
        {
            return new Product
            {
                ProductID = row["ProductID"] is DBNull ? (int)default : (int)row["ProductID"],
                ProductName = row["ProductName"] is DBNull ? (string)default : (string)row["ProductName"],
                CategoryID = row["CategoryID"] is DBNull ? (int)default : (int)row["CategoryID"],
                SupplierID = row["SupplierID"] is DBNull ? (int)default : (int)row["SupplierID"],
                Description = row["Description"] is DBNull ? (string)default : (string)row["Description"],
                Price = row["Price"] is DBNull ? (decimal)default : (decimal)row["Price"],
                StockQuantity = row["StockQuantity"] is DBNull ? (int)default : (int)row["StockQuantity"],
                ReorderLevel = row["ReorderLevel"] is DBNull ? (int)default : (int)row["ReorderLevel"],
                ImageURL = row["ImageURL"] is DBNull ? (string)default : (string)row["ImageURL"],
                SKU = row["SKU"] is DBNull ? (string)default : (string)row["SKU"],
                Weight = row["Weight"] is DBNull ? (decimal)default : (decimal)row["Weight"],
                Dimensions = row["Dimensions"] is DBNull ? (string)default : (string)row["Dimensions"],
                Warranty = row["Warranty"] is DBNull ? (string)default : (string)row["Warranty"],
                IsActive = row["IsActive"] is DBNull ? (bool)default : (bool)row["IsActive"],
                CreatedDate = row["CreatedDate"] is DBNull ? (DateTime)default : (DateTime)row["CreatedDate"],
                LastUpdated = row["LastUpdated"] is DBNull ? (DateTime)default : (DateTime)row["LastUpdated"],
            };
        }

        public Product GetById(int id)
        {
            string sql = "SELECT * FROM Products WHERE ProductID = @id";
            var dt = DbHelper.ExecuteReader(sql, new SqlParameter("@id", id));
            if (dt.Rows.Count > 0)
            {
                return MapRowToEntity(dt.Rows[0]);
            }
            return null;
        }

        public List<Product> GetAll()
        {
            string sql = "SELECT * FROM Products";
            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<Product>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToEntity(row));
            }
            return list;
        }

        public int Insert(Product entity)
        {
            string sql = "INSERT INTO Products (ProductName, CategoryID, SupplierID, Description, Price, StockQuantity, ReorderLevel, ImageURL, SKU, Weight, Dimensions, Warranty, IsActive, CreatedDate, LastUpdated) VALUES (@ProductName, @CategoryID, @SupplierID, @Description, @Price, @StockQuantity, @ReorderLevel, @ImageURL, @SKU, @Weight, @Dimensions, @Warranty, @IsActive, @CreatedDate, @LastUpdated)";
            var parameters = new SqlParameter[]
            {
            new SqlParameter("@ProductName", entity.ProductName),
            new SqlParameter("@CategoryID", entity.CategoryID),
            new SqlParameter("@SupplierID", entity.SupplierID),
            new SqlParameter("@Description", entity.Description),
            new SqlParameter("@Price", entity.Price),
            new SqlParameter("@StockQuantity", entity.StockQuantity),
            new SqlParameter("@ReorderLevel", entity.ReorderLevel),
            new SqlParameter("@ImageURL", entity.ImageURL),
            new SqlParameter("@SKU", entity.SKU),
            new SqlParameter("@Weight", entity.Weight),
            new SqlParameter("@Dimensions", entity.Dimensions),
            new SqlParameter("@Warranty", entity.Warranty),
            new SqlParameter("@IsActive", entity.IsActive),
            new SqlParameter("@CreatedDate", entity.CreatedDate),
            new SqlParameter("@LastUpdated", entity.LastUpdated),
            };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        public int Update(Product entity)
        {
            string sql = "UPDATE Products SET ProductName = @ProductName, CategoryID = @CategoryID, SupplierID = @SupplierID, Description = @Description, Price = @Price, StockQuantity = @StockQuantity, ReorderLevel = @ReorderLevel, ImageURL = @ImageURL, SKU = @SKU, Weight = @Weight, Dimensions = @Dimensions, Warranty = @Warranty, IsActive = @IsActive, CreatedDate = @CreatedDate, LastUpdated = @LastUpdated WHERE ProductID = @ProductID WHERE ProductID = @ProductID";
            var parameters = new SqlParameter[]
            {
            new SqlParameter("@ProductName", entity.ProductName),
            new SqlParameter("@CategoryID", entity.CategoryID),
            new SqlParameter("@SupplierID", entity.SupplierID),
            new SqlParameter("@Description", entity.Description),
            new SqlParameter("@Price", entity.Price),
            new SqlParameter("@StockQuantity", entity.StockQuantity),
            new SqlParameter("@ReorderLevel", entity.ReorderLevel),
            new SqlParameter("@ImageURL", entity.ImageURL),
            new SqlParameter("@SKU", entity.SKU),
            new SqlParameter("@Weight", entity.Weight),
            new SqlParameter("@Dimensions", entity.Dimensions),
            new SqlParameter("@Warranty", entity.Warranty),
            new SqlParameter("@IsActive", entity.IsActive),
            new SqlParameter("@CreatedDate", entity.CreatedDate),
            new SqlParameter("@LastUpdated", entity.LastUpdated),
            };
            // Add PK parameter for WHERE clause
            var pk_param = new SqlParameter("@ProductID", entity.ProductID);
            var all_params = new List<SqlParameter>(parameters) { pk_param };
            
            return DbHelper.ExecuteNonQuery(sql, all_params.ToArray());
        }

        public int Delete(int id)
        {
            string sql = "DELETE FROM Products WHERE ProductID = @id";
            return DbHelper.ExecuteNonQuery(sql, new SqlParameter("@id", id));
        }
    }
}
