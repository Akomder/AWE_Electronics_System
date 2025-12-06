#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class SupplierDAL
    {
        // --- CREATE ---
        public int Insert(Supplier entity)
        {
            string sql = @"INSERT INTO Suppliers (SupplierName, ContactPerson, Email, Phone, Address, City, State, PostalCode, Country, IsActive) 
                          VALUES (@SupplierName, @ContactPerson, @Email, @Phone, @Address, @City, @State, @PostalCode, @Country, @IsActive);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = GetSqlParameters(entity);
            object result = DbHelper.ExecuteScalar(sql, parameters.ToArray());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // --- READ ---
        public Supplier GetById(int id)
        {
            string sql = "SELECT * FROM Suppliers WHERE SupplierID = @SupplierID";
            var parameters = new SqlParameter[] { new SqlParameter("@SupplierID", id) };
            
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            if (dt.Rows.Count > 0)
            {
                return MapRowToSupplier(dt.Rows[0]);
            }
            return null;
        }

        public List<Supplier> GetAll()
        {
            string sql = "SELECT * FROM Suppliers ORDER BY SupplierID";
            DataTable dt = DbHelper.ExecuteReader(sql);
            
            List<Supplier> list = new List<Supplier>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToSupplier(row));
            }
            return list;
        }

        public List<Supplier> GetActive()
        {
            string sql = "SELECT * FROM Suppliers WHERE IsActive = 1 ORDER BY SupplierID";
            DataTable dt = DbHelper.ExecuteReader(sql);
            
            List<Supplier> list = new List<Supplier>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToSupplier(row));
            }
            return list;
        }

        // --- UPDATE ---
        public int Update(Supplier entity)
        {
            string sql = @"UPDATE Suppliers 
                          SET SupplierName = @SupplierName, ContactPerson = @ContactPerson, Email = @Email, Phone = @Phone, Address = @Address, City = @City, State = @State, PostalCode = @PostalCode, Country = @Country, IsActive = @IsActive
                          WHERE SupplierID = @SupplierID";
            
            var parameters = GetSqlParameters(entity);
            parameters.Add(new SqlParameter("@SupplierID", entity.SupplierID));
            
            return DbHelper.ExecuteNonQuery(sql, parameters.ToArray());
        }

        // --- DELETE (Soft Delete) ---
        public int Delete(int id)
        {
            string sql = "UPDATE Suppliers SET IsActive = 0 WHERE SupplierID = @SupplierID";
            var parameters = new SqlParameter[] { new SqlParameter("@SupplierID", id) };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- HELPER METHODS ---
        private Supplier MapRowToSupplier(DataRow row)
        {
            return new Supplier
            {
                SupplierID = Convert.ToInt32(row["SupplierID"]),
                SupplierName = row["SupplierName"] != DBNull.Value ? row["SupplierName"].ToString() : null, ContactPerson = row["ContactPerson"] != DBNull.Value ? row["ContactPerson"].ToString() : null, Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : null, Phone = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : null, Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : null, City = row["City"] != DBNull.Value ? row["City"].ToString() : null, State = row["State"] != DBNull.Value ? row["State"].ToString() : null, PostalCode = row["PostalCode"] != DBNull.Value ? row["PostalCode"].ToString() : null, Country = row["Country"] != DBNull.Value ? row["Country"].ToString() : null, IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }

        private List<SqlParameter> GetSqlParameters(Supplier entity)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@SupplierName", entity.SupplierName ?? (object)DBNull.Value),
                new SqlParameter("@ContactPerson", entity.ContactPerson ?? (object)DBNull.Value),
                new SqlParameter("@Email", entity.Email ?? (object)DBNull.Value),
                new SqlParameter("@Phone", entity.Phone ?? (object)DBNull.Value),
                new SqlParameter("@Address", entity.Address ?? (object)DBNull.Value),
                new SqlParameter("@City", entity.City ?? (object)DBNull.Value),
                new SqlParameter("@State", entity.State ?? (object)DBNull.Value),
                new SqlParameter("@PostalCode", entity.PostalCode ?? (object)DBNull.Value),
                new SqlParameter("@Country", entity.Country ?? (object)DBNull.Value),
                new SqlParameter("@IsActive", entity.IsActive)
            };
        }
    }
}