#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class GDNDAL
    {
        // --- CREATE ---
        public int Insert(GoodsDeliveryNote entity)
        {
            string sql = @"INSERT INTO GoodsDeliveryNotes (GDNNumber, OrderID, CustomerID, DeliveryDate, 
                          DeliveredByUserID, Status, DeliveryAddress, Notes) 
                          VALUES (@GDNNumber, @OrderID, @CustomerID, @DeliveryDate, @DeliveredByUserID, 
                          @Status, @DeliveryAddress, @Notes);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = GetSqlParameters(entity);
            object result = DbHelper.ExecuteScalar(sql, parameters.ToArray());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // --- READ ---
        public GoodsDeliveryNote GetById(int id)
        {
            string sql = "SELECT * FROM GoodsDeliveryNotes WHERE GDNID = @GDNID";
            var parameters = new SqlParameter[] { new SqlParameter("@GDNID", id) };
            
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            if (dt.Rows.Count > 0)
            {
                return MapRowToGDN(dt.Rows[0]);
            }
            return null;
        }

        public GoodsDeliveryNote GetByNumber(string gdnNumber)
        {
            string sql = "SELECT * FROM GoodsDeliveryNotes WHERE GDNNumber = @GDNNumber";
            var parameters = new SqlParameter[] { new SqlParameter("@GDNNumber", gdnNumber) };
            
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            if (dt.Rows.Count > 0)
            {
                return MapRowToGDN(dt.Rows[0]);
            }
            return null;
        }

        public List<GoodsDeliveryNote> GetAll()
        {
            string sql = "SELECT * FROM GoodsDeliveryNotes ORDER BY DeliveryDate DESC";
            DataTable dt = DbHelper.ExecuteReader(sql);
            
            List<GoodsDeliveryNote> list = new List<GoodsDeliveryNote>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToGDN(row));
            }
            return list;
        }

        public List<GoodsDeliveryNote> GetByStatus(string status)
        {
            string sql = "SELECT * FROM GoodsDeliveryNotes WHERE Status = @Status ORDER BY DeliveryDate DESC";
            var parameters = new SqlParameter[] { new SqlParameter("@Status", status) };
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            List<GoodsDeliveryNote> list = new List<GoodsDeliveryNote>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToGDN(row));
            }
            return list;
        }

        // --- UPDATE ---
        public int Update(GoodsDeliveryNote entity)
        {
            string sql = @"UPDATE GoodsDeliveryNotes 
                          SET GDNNumber = @GDNNumber, OrderID = @OrderID, CustomerID = @CustomerID,
                              DeliveryDate = @DeliveryDate, DeliveredByUserID = @DeliveredByUserID,
                              Status = @Status, DeliveryAddress = @DeliveryAddress, Notes = @Notes
                          WHERE GDNID = @GDNID";
            
            var parameters = GetSqlParameters(entity);
            parameters.Add(new SqlParameter("@GDNID", entity.GDNID));
            
            return DbHelper.ExecuteNonQuery(sql, parameters.ToArray());
        }

        public int UpdateStatus(int gdnId, string status)
        {
            string sql = "UPDATE GoodsDeliveryNotes SET Status = @Status WHERE GDNID = @GDNID";
            var parameters = new SqlParameter[] 
            { 
                new SqlParameter("@Status", status),
                new SqlParameter("@GDNID", gdnId)
            };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- DELETE ---
        public int Delete(int id)
        {
            string sql = "DELETE FROM GoodsDeliveryNotes WHERE GDNID = @GDNID";
            var parameters = new SqlParameter[] { new SqlParameter("@GDNID", id) };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- HELPER METHODS ---
        private GoodsDeliveryNote MapRowToGDN(DataRow row)
        {
            return new GoodsDeliveryNote
            {
                GDNID = Convert.ToInt32(row["GDNID"]),
                GDNNumber = row["GDNNumber"].ToString(),
                OrderID = row["OrderID"] != DBNull.Value ? (int?)Convert.ToInt32(row["OrderID"]) : null,
                CustomerID = row["CustomerID"] != DBNull.Value ? (int?)Convert.ToInt32(row["CustomerID"]) : null,
                DeliveryDate = Convert.ToDateTime(row["DeliveryDate"]),
                DeliveredByUserID = Convert.ToInt32(row["DeliveredByUserID"]),
                Status = row["Status"].ToString(),
                DeliveryAddress = row["DeliveryAddress"] != DBNull.Value ? row["DeliveryAddress"].ToString() : null,
                Notes = row["Notes"] != DBNull.Value ? row["Notes"].ToString() : null,
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }

        private List<SqlParameter> GetSqlParameters(GoodsDeliveryNote entity)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@GDNNumber", entity.GDNNumber),
                new SqlParameter("@OrderID", (object)entity.OrderID ?? DBNull.Value),
                new SqlParameter("@CustomerID", (object)entity.CustomerID ?? DBNull.Value),
                new SqlParameter("@DeliveryDate", entity.DeliveryDate),
                new SqlParameter("@DeliveredByUserID", entity.DeliveredByUserID),
                new SqlParameter("@Status", entity.Status),
                new SqlParameter("@DeliveryAddress", (object)entity.DeliveryAddress ?? DBNull.Value),
                new SqlParameter("@Notes", (object)entity.Notes ?? DBNull.Value)
            };
        }

        // --- Generate GDN Number ---
        public string GenerateGDNNumber()
        {
            string sql = "SELECT TOP 1 GDNNumber FROM GoodsDeliveryNotes ORDER BY GDNID DESC";
            DataTable dt = DbHelper.ExecuteReader(sql);
            
            if (dt.Rows.Count > 0)
            {
                string lastNumber = dt.Rows[0]["GDNNumber"].ToString();
                // Extract number part (e.g., GDN-2024-0001 -> 0001)
                string[] parts = lastNumber.Split('-');
                if (parts.Length == 3)
                {
                    int number = int.Parse(parts[2]) + 1;
                    return $"GDN-{DateTime.Now.Year}-{number:D4}";
                }
            }
            
            return $"GDN-{DateTime.Now.Year}-0001";
        }
    }

    public class GDNItemDAL
    {
        // --- CREATE ---
        public int Insert(GDNItem entity)
        {
            string sql = @"INSERT INTO GDNItems (GDNID, ProductID, QuantityDelivered, Notes) 
                          VALUES (@GDNID, @ProductID, @QuantityDelivered, @Notes);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@GDNID", entity.GDNID),
                new SqlParameter("@ProductID", entity.ProductID),
                new SqlParameter("@QuantityDelivered", entity.QuantityDelivered),
                new SqlParameter("@Notes", (object)entity.Notes ?? DBNull.Value)
            };

            object result = DbHelper.ExecuteScalar(sql, parameters.ToArray());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // --- READ ---
        public List<GDNItem> GetByGDNID(int gdnId)
        {
            string sql = "SELECT * FROM GDNItems WHERE GDNID = @GDNID";
            var parameters = new SqlParameter[] { new SqlParameter("@GDNID", gdnId) };
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            List<GDNItem> list = new List<GDNItem>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new GDNItem
                {
                    GDNItemID = Convert.ToInt32(row["GDNItemID"]),
                    GDNID = Convert.ToInt32(row["GDNID"]),
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    QuantityDelivered = Convert.ToInt32(row["QuantityDelivered"]),
                    Notes = row["Notes"] != DBNull.Value ? row["Notes"].ToString() : null
                });
            }
            return list;
        }

        // --- DELETE ---
        public int DeleteByGDNID(int gdnId)
        {
            string sql = "DELETE FROM GDNItems WHERE GDNID = @GDNID";
            var parameters = new SqlParameter[] { new SqlParameter("@GDNID", gdnId) };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }
    }
}
