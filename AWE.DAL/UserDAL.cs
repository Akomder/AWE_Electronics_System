#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class UserDAL
    {
        private User MapRowToEntity(DataRow row)
        {
            return new User
            {
                UserID = (int)row["UserID"],
                Username = row["Username"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                Role = row["Role"].ToString(),
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                Email = row["Email"].ToString(),
                Phone = row["Phone"] is DBNull ? null : row["Phone"].ToString(),
                IsActive = (bool)row["IsActive"],
                CreatedDate = (DateTime)row["CreatedDate"],
                LastLogin = row["LastLogin"] is DBNull ? (DateTime?)null : (DateTime)row["LastLogin"]
            };
        }

        // --- READ Operations ---

        public User GetById(int id)
        {
            string sql = "SELECT * FROM Users WHERE UserID = @id";
            var dt = DbHelper.ExecuteReader(sql, new SqlParameter("@id", id));
            if (dt.Rows.Count > 0)
            {
                return MapRowToEntity(dt.Rows[0]);
            }
            return null;
        }

        public User GetByUsername(string username)
        {
            string sql = "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1";
            var dt = DbHelper.ExecuteReader(sql, new SqlParameter("@Username", username));
            if (dt.Rows.Count > 0)
            {
                return MapRowToEntity(dt.Rows[0]);
            }
            return null;
        }

        public List<User> GetAll()
        {
            string sql = "SELECT * FROM Users WHERE IsActive = 1 ORDER BY Username";
            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<User>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToEntity(row));
            }
            return list;
        }

        // --- CREATE Operation ---

        public int Insert(User entity)
        {
            string sql = @"
                INSERT INTO Users (Username, PasswordHash, Role, FirstName, LastName, Email, Phone, IsActive)
                VALUES (@Username, @PasswordHash, @Role, @FirstName, @LastName, @Email, @Phone, @IsActive);
                SELECT SCOPE_IDENTITY();";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", entity.Username),
                new SqlParameter("@PasswordHash", entity.PasswordHash),
                new SqlParameter("@Role", entity.Role),
                new SqlParameter("@FirstName", entity.FirstName),
                new SqlParameter("@LastName", entity.LastName),
                new SqlParameter("@Email", entity.Email),
                new SqlParameter("@Phone", (object)entity.Phone ?? DBNull.Value),
                new SqlParameter("@IsActive", entity.IsActive)
            };

            var dt = DbHelper.ExecuteReader(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            return 0;
        }

        // --- UPDATE Operation ---

        public int Update(User entity)
        {
            string sql = @"
                UPDATE Users SET 
                    Username = @Username, 
                    PasswordHash = @PasswordHash, 
                    Role = @Role, 
                    FirstName = @FirstName, 
                    LastName = @LastName, 
                    Email = @Email, 
                    Phone = @Phone, 
                    IsActive = @IsActive
                WHERE UserID = @UserID";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", entity.UserID),
                new SqlParameter("@Username", entity.Username),
                new SqlParameter("@PasswordHash", entity.PasswordHash),
                new SqlParameter("@Role", entity.Role),
                new SqlParameter("@FirstName", entity.FirstName),
                new SqlParameter("@LastName", entity.LastName),
                new SqlParameter("@Email", entity.Email),
                new SqlParameter("@Phone", (object)entity.Phone ?? DBNull.Value),
                new SqlParameter("@IsActive", entity.IsActive)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // --- UPDATE LastLogin ---

        public int UpdateLastLogin(int userId)
        {
            string sql = "UPDATE Users SET LastLogin = GETDATE() WHERE UserID = @UserID";
            return DbHelper.ExecuteNonQuery(sql, new SqlParameter("@UserID", userId));
        }

        // --- DELETE Operation (Soft Delete) ---

        public int Delete(int id)
        {
            string sql = "UPDATE Users SET IsActive = 0 WHERE UserID = @id";
            return DbHelper.ExecuteNonQuery(sql, new SqlParameter("@id", id));
        }
    }
}
