#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class CategoryDAL
    {
        // --- CREATE ---
        public int Insert(Category entity)
        {
            string sql = @"INSERT INTO Categories (CategoryName, Description, IsActive) 
                          VALUES (@CategoryName, @Description, @IsActive);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = GetSqlParameters(entity);
            object result = DbHelper.ExecuteScalar(sql, parameters.ToArray());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // --- READ ---
        public Category GetById(int id)
        {
            string sql = "SELECT * FROM Categories WHERE CategoryID = @CategoryID";
            var parameters = new SqlParameter[] { new SqlParameter("@CategoryID", id) };
            
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            if (dt.Rows.Count > 0)
            {
                return MapRowToCategory(dt.Rows[0]);
            }
            return null;
        }

        public List<Category> GetAll()
        {
            string sql = "SELECT * FROM Categories ORDER BY CategoryID";
            DataTable dt = DbHelper.ExecuteReader(sql);
            
            List<Category> list = new List<Category>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToCategory(row));
            }
            return list;
        }

        public List<Category> GetActive()
        {
            string sql = "SELECT * FROM Categories WHERE IsActive = 1 ORDER BY CategoryID";
            DataTable dt = DbHelper.ExecuteReader(sql);
            
            List<Category> list = new List<Category>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToCategory(row));
            }
            return list;
        }

        // --- UPDATE ---
        public int Update(Category entity)
        {
            string sql = @"UPDATE Categories 
                          SET CategoryName = @CategoryName, Description = @Description, IsActive = @IsActive
                          WHERE CategoryID = @CategoryID";
            
            var parameters = GetSqlParameters(entity);
            parameters.Add(new SqlParameter("@CategoryID", entity.CategoryID));
            
            return DbHelper.ExecuteNonQuery(sql, parameters.ToArray());
        }

        // --- DELETE (Soft Delete) ---
        public int Delete(int id)
        {
            string sql = "UPDATE Categories SET IsActive = 0 WHERE CategoryID = @CategoryID";
            var parameters = new SqlParameter[] { new SqlParameter("@CategoryID", id) };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- HELPER METHODS ---
        private Category MapRowToCategory(DataRow row)
        {
            return new Category
            {
                CategoryID = Convert.ToInt32(row["CategoryID"]),
                CategoryName = row["CategoryName"] != DBNull.Value ? row["CategoryName"].ToString() : null, Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null, IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }

        private List<SqlParameter> GetSqlParameters(Category entity)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@CategoryName", entity.CategoryName ?? (object)DBNull.Value),
                new SqlParameter("@Description", entity.Description ?? (object)DBNull.Value),
                new SqlParameter("@IsActive", entity.IsActive)
            };
        }
    }
}