#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class GRNDAL
    {
        // --- CREATE ---
        public int Insert(GoodsReceivedNote entity)
        {
            string sql = @"INSERT INTO GoodsReceivedNotes (GRNNumber, SupplierID, ReceivedDate, 
                          ReceivedByUserID, Status, TotalAmount, Notes) 
                          VALUES (@GRNNumber, @SupplierID, @ReceivedDate, @ReceivedByUserID, 
                          @Status, @TotalAmount, @Notes);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = GetSqlParameters(entity);
            object result = DbHelper.ExecuteScalar(sql, parameters.ToArray());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // --- READ ---
        public GoodsReceivedNote GetById(int id)
        {
            string sql = "SELECT * FROM GoodsReceivedNotes WHERE GRNID = @GRNID";
            var parameters = new SqlParameter[] { new SqlParameter("@GRNID", id) };
            
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            if (dt.Rows.Count > 0)
            {
                return MapRowToGRN(dt.Rows[0]);
            }
            return null;
        }

        public GoodsReceivedNote GetByNumber(string grnNumber)
        {
            string sql = "SELECT * FROM GoodsReceivedNotes WHERE GRNNumber = @GRNNumber";
            var parameters = new SqlParameter[] { new SqlParameter("@GRNNumber", grnNumber) };
            
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            if (dt.Rows.Count > 0)
            {
                return MapRowToGRN(dt.Rows[0]);
            }
            return null;
        }

        public List<GoodsReceivedNote> GetAll()
        {
            string sql = "SELECT * FROM GoodsReceivedNotes ORDER BY ReceivedDate DESC";
            DataTable dt = DbHelper.ExecuteReader(sql);
            
            List<GoodsReceivedNote> list = new List<GoodsReceivedNote>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToGRN(row));
            }
            return list;
        }

        public List<GoodsReceivedNote> GetByStatus(string status)
        {
            string sql = "SELECT * FROM GoodsReceivedNotes WHERE Status = @Status ORDER BY ReceivedDate DESC";
            var parameters = new SqlParameter[] { new SqlParameter("@Status", status) };
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            List<GoodsReceivedNote> list = new List<GoodsReceivedNote>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToGRN(row));
            }
            return list;
        }

        // --- UPDATE ---
        public int Update(GoodsReceivedNote entity)
        {
            string sql = @"UPDATE GoodsReceivedNotes 
                          SET GRNNumber = @GRNNumber, SupplierID = @SupplierID, 
                              ReceivedDate = @ReceivedDate, ReceivedByUserID = @ReceivedByUserID,
                              Status = @Status, TotalAmount = @TotalAmount, Notes = @Notes
                          WHERE GRNID = @GRNID";
            
            var parameters = GetSqlParameters(entity);
            parameters.Add(new SqlParameter("@GRNID", entity.GRNID));
            
            return DbHelper.ExecuteNonQuery(sql, parameters.ToArray());
        }

        public int UpdateStatus(int grnId, string status)
        {
            string sql = "UPDATE GoodsReceivedNotes SET Status = @Status WHERE GRNID = @GRNID";
            var parameters = new SqlParameter[] 
            { 
                new SqlParameter("@Status", status),
                new SqlParameter("@GRNID", grnId)
            };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- DELETE ---
        public int Delete(int id)
        {
            string sql = "DELETE FROM GoodsReceivedNotes WHERE GRNID = @GRNID";
            var parameters = new SqlParameter[] { new SqlParameter("@GRNID", id) };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- HELPER METHODS ---
        private GoodsReceivedNote MapRowToGRN(DataRow row)
        {
            return new GoodsReceivedNote
            {
                GRNID = Convert.ToInt32(row["GRNID"]),
                GRNNumber = row["GRNNumber"].ToString(),
                SupplierID = Convert.ToInt32(row["SupplierID"]),
                ReceivedDate = Convert.ToDateTime(row["ReceivedDate"]),
                ReceivedByUserID = Convert.ToInt32(row["ReceivedByUserID"]),
                Status = row["Status"].ToString(),
                TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                Notes = row["Notes"] != DBNull.Value ? row["Notes"].ToString() : null,
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }

        private List<SqlParameter> GetSqlParameters(GoodsReceivedNote entity)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@GRNNumber", entity.GRNNumber),
                new SqlParameter("@SupplierID", entity.SupplierID),
                new SqlParameter("@ReceivedDate", entity.ReceivedDate),
                new SqlParameter("@ReceivedByUserID", entity.ReceivedByUserID),
                new SqlParameter("@Status", entity.Status),
                new SqlParameter("@TotalAmount", entity.TotalAmount),
                new SqlParameter("@Notes", (object)entity.Notes ?? DBNull.Value)
            };
        }

        // --- Generate GRN Number ---
        public string GenerateGRNNumber()
        {
            string sql = "SELECT TOP 1 GRNNumber FROM GoodsReceivedNotes ORDER BY GRNID DESC";
            DataTable dt = DbHelper.ExecuteReader(sql);
            
            if (dt.Rows.Count > 0)
            {
                string lastNumber = dt.Rows[0]["GRNNumber"].ToString();
                // Extract number part (e.g., GRN-2024-0001 -> 0001)
                string[] parts = lastNumber.Split('-');
                if (parts.Length == 3)
                {
                    int number = int.Parse(parts[2]) + 1;
                    return $"GRN-{DateTime.Now.Year}-{number:D4}";
                }
            }
            
            return $"GRN-{DateTime.Now.Year}-0001";
        }
    }

    public class GRNItemDAL
    {
        // --- CREATE ---
        public int Insert(GRNItem entity)
        {
            string sql = @"INSERT INTO GRNItems (GRNID, ProductID, QuantityReceived, UnitCost, TotalCost, Notes) 
                          VALUES (@GRNID, @ProductID, @QuantityReceived, @UnitCost, @TotalCost, @Notes);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@GRNID", entity.GRNID),
                new SqlParameter("@ProductID", entity.ProductID),
                new SqlParameter("@QuantityReceived", entity.QuantityReceived),
                new SqlParameter("@UnitCost", entity.UnitCost),
                new SqlParameter("@TotalCost", entity.TotalCost),
                new SqlParameter("@Notes", (object)entity.Notes ?? DBNull.Value)
            };

            object result = DbHelper.ExecuteScalar(sql, parameters.ToArray());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // --- READ ---
        public List<GRNItem> GetByGRNID(int grnId)
        {
            string sql = "SELECT * FROM GRNItems WHERE GRNID = @GRNID";
            var parameters = new SqlParameter[] { new SqlParameter("@GRNID", grnId) };
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            List<GRNItem> list = new List<GRNItem>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new GRNItem
                {
                    GRNItemID = Convert.ToInt32(row["GRNItemID"]),
                    GRNID = Convert.ToInt32(row["GRNID"]),
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    QuantityReceived = Convert.ToInt32(row["QuantityReceived"]),
                    UnitCost = Convert.ToDecimal(row["UnitCost"]),
                    TotalCost = Convert.ToDecimal(row["TotalCost"]),
                    Notes = row["Notes"] != DBNull.Value ? row["Notes"].ToString() : null
                });
            }
            return list;
        }

        // --- DELETE ---
        public int DeleteByGRNID(int grnId)
        {
            string sql = "DELETE FROM GRNItems WHERE GRNID = @GRNID";
            var parameters = new SqlParameter[] { new SqlParameter("@GRNID", grnId) };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }
    }
}
