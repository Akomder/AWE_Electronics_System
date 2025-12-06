#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class PasswordResetTokenDAL
    {
        // --- CREATE ---
        public int Insert(PasswordResetToken token)
        {
            string sql = @"INSERT INTO PasswordResetTokens (UserID, Token, ExpirationDate, IsUsed) 
                          VALUES (@UserID, @Token, @ExpirationDate, @IsUsed);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", token.UserID),
                new SqlParameter("@Token", token.Token),
                new SqlParameter("@ExpirationDate", token.ExpirationDate),
                new SqlParameter("@IsUsed", token.IsUsed)
            };

            object result = DbHelper.ExecuteScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // --- READ ---
        public PasswordResetToken GetByToken(string token)
        {
            string sql = "SELECT * FROM PasswordResetTokens WHERE Token = @Token";
            var parameters = new SqlParameter[] { new SqlParameter("@Token", token) };
            
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            if (dt.Rows.Count > 0)
            {
                return MapRowToToken(dt.Rows[0]);
            }
            return null;
        }

        public PasswordResetToken GetById(int tokenId)
        {
            string sql = "SELECT * FROM PasswordResetTokens WHERE TokenID = @TokenID";
            var parameters = new SqlParameter[] { new SqlParameter("@TokenID", tokenId) };
            
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            
            if (dt.Rows.Count > 0)
            {
                return MapRowToToken(dt.Rows[0]);
            }
            return null;
        }

        public List<PasswordResetToken> GetByUserId(int userId)
        {
            string sql = "SELECT * FROM PasswordResetTokens WHERE UserID = @UserID ORDER BY CreatedDate DESC";
            var parameters = new SqlParameter[] { new SqlParameter("@UserID", userId) };
            
            DataTable dt = DbHelper.ExecuteReader(sql, parameters);
            List<PasswordResetToken> tokens = new List<PasswordResetToken>();
            
            foreach (DataRow row in dt.Rows)
            {
                tokens.Add(MapRowToToken(row));
            }
            return tokens;
        }

        // --- UPDATE ---
        public int MarkAsUsed(string token)
        {
            string sql = @"UPDATE PasswordResetTokens 
                          SET IsUsed = 1, UsedDate = GETDATE() 
                          WHERE Token = @Token";
            
            var parameters = new SqlParameter[] { new SqlParameter("@Token", token) };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- DELETE ---
        public int DeleteExpiredTokens()
        {
            string sql = "DELETE FROM PasswordResetTokens WHERE ExpirationDate < GETDATE()";
            return DbHelper.ExecuteNonQuery(sql);
        }

        public int DeleteByUserId(int userId)
        {
            string sql = "DELETE FROM PasswordResetTokens WHERE UserID = @UserID";
            var parameters = new SqlParameter[] { new SqlParameter("@UserID", userId) };
            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- HELPER METHOD ---
        private PasswordResetToken MapRowToToken(DataRow row)
        {
            return new PasswordResetToken
            {
                TokenID = Convert.ToInt32(row["TokenID"]),
                UserID = Convert.ToInt32(row["UserID"]),
                Token = row["Token"].ToString(),
                ExpirationDate = Convert.ToDateTime(row["ExpirationDate"]),
                IsUsed = Convert.ToBoolean(row["IsUsed"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                UsedDate = row["UsedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["UsedDate"]) : null
            };
        }
    }
}
