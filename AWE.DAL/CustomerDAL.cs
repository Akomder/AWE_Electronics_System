#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using AWE.Models;

namespace AWE.DAL
{
    public class CustomerDAL
    {
        /// <summary>
        /// Get customer by email
        /// </summary>
        public Customer GetByEmail(string email)
        {
            string query = @"
                SELECT CustomerID, FirstName, LastName, Email, Phone, PasswordHash, 
                       ShippingAddress, ShippingCity, ShippingState, ShippingPostalCode,
                       BillingAddress, BillingCity, BillingState, BillingPostalCode,
                       IsActive, CreatedDate, LastLogin
                FROM Customers
                WHERE Email = @Email
            ";

            var parameters = new[] { new SqlParameter("@Email", email) };
            DataTable dt = DbHelper.ExecuteReader(query, parameters);

            return dt.Rows.Count > 0 ? MapRowToEntity(dt.Rows[0]) : null;
        }

        /// <summary>
        /// Get customer by ID
        /// </summary>
        public Customer GetById(int id)
        {
            string query = @"
                SELECT CustomerID, FirstName, LastName, Email, Phone, PasswordHash, 
                       ShippingAddress, ShippingCity, ShippingState, ShippingPostalCode,
                       BillingAddress, BillingCity, BillingState, BillingPostalCode,
                       IsActive, CreatedDate, LastLogin
                FROM Customers
                WHERE CustomerID = @CustomerID
            ";

            var parameters = new[] { new SqlParameter("@CustomerID", id) };
            DataTable dt = DbHelper.ExecuteReader(query, parameters);

            return dt.Rows.Count > 0 ? MapRowToEntity(dt.Rows[0]) : null;
        }

        /// <summary>
        /// Create new customer
        /// </summary>
        public int Insert(Customer customer)
        {
            string query = @"
                INSERT INTO Customers (FirstName, LastName, Email, Phone, PasswordHash, 
                                    ShippingAddress, ShippingCity, ShippingState, ShippingPostalCode,
                                    BillingAddress, BillingCity, BillingState, BillingPostalCode,
                                    IsActive, RegistrationDate)
                VALUES (@FirstName, @LastName, @Email, @Phone, @PasswordHash, 
                       @ShippingAddress, @ShippingCity, @ShippingState, @ShippingPostalCode,
                       @BillingAddress, @BillingCity, @BillingState, @BillingPostalCode,
                       @IsActive, @RegistrationDate)
                SELECT SCOPE_IDENTITY()
            ";

            var parameters = new[]
            {
                new SqlParameter("@FirstName", customer.FirstName ?? ""),
                new SqlParameter("@LastName", customer.LastName ?? ""),
                new SqlParameter("@Email", customer.Email ?? ""),
                new SqlParameter("@Phone", customer.Phone ?? ""),
                new SqlParameter("@PasswordHash", customer.PasswordHash ?? ""),
                new SqlParameter("@ShippingAddress", customer.ShippingAddress ?? ""),
                new SqlParameter("@ShippingCity", customer.ShippingCity ?? ""),
                new SqlParameter("@ShippingState", customer.ShippingState ?? ""),
                new SqlParameter("@ShippingPostalCode", customer.ShippingPostalCode ?? ""),
                new SqlParameter("@BillingAddress", customer.BillingAddress ?? ""),
                new SqlParameter("@BillingCity", customer.BillingCity ?? ""),
                new SqlParameter("@BillingState", customer.BillingState ?? ""),
                new SqlParameter("@BillingPostalCode", customer.BillingPostalCode ?? ""),
                new SqlParameter("@IsActive", customer.IsActive),
                new SqlParameter("@RegistrationDate", customer.CreatedDate)
            };

            var result = DbHelper.ExecuteScalar(query, parameters);
            return result != null ? int.Parse(result.ToString()) : -1;
        }

        /// <summary>
        /// Update customer
        /// </summary>
        public int Update(Customer customer)
        {
            string query = @"
                UPDATE Customers
                SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Phone = @Phone,
                    ShippingAddress = @ShippingAddress, ShippingCity = @ShippingCity, 
                    ShippingState = @ShippingState, ShippingPostalCode = @ShippingPostalCode,
                    BillingAddress = @BillingAddress, BillingCity = @BillingCity, 
                    BillingState = @BillingState, BillingPostalCode = @BillingPostalCode,
                    IsActive = @IsActive
                WHERE CustomerID = @CustomerID
            ";

            var parameters = new[]
            {
                new SqlParameter("@CustomerID", customer.CustomerID),
                new SqlParameter("@FirstName", customer.FirstName ?? ""),
                new SqlParameter("@LastName", customer.LastName ?? ""),
                new SqlParameter("@Email", customer.Email ?? ""),
                new SqlParameter("@Phone", customer.Phone ?? ""),
                new SqlParameter("@ShippingAddress", customer.ShippingAddress ?? ""),
                new SqlParameter("@ShippingCity", customer.ShippingCity ?? ""),
                new SqlParameter("@ShippingState", customer.ShippingState ?? ""),
                new SqlParameter("@ShippingPostalCode", customer.ShippingPostalCode ?? ""),
                new SqlParameter("@BillingAddress", customer.BillingAddress ?? ""),
                new SqlParameter("@BillingCity", customer.BillingCity ?? ""),
                new SqlParameter("@BillingState", customer.BillingState ?? ""),
                new SqlParameter("@BillingPostalCode", customer.BillingPostalCode ?? ""),
                new SqlParameter("@IsActive", customer.IsActive)
            };

            return DbHelper.ExecuteNonQuery(query, parameters);
        }

        private Customer MapRowToEntity(DataRow row)
        {
            return new Customer
            {
                CustomerID = (int)row["CustomerID"],
                FirstName = row["FirstName"] != DBNull.Value ? row["FirstName"].ToString() : "",
                LastName = row["LastName"] != DBNull.Value ? row["LastName"].ToString() : "",
                Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : "",
                Phone = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : "",
                PasswordHash = row["PasswordHash"] != DBNull.Value ? row["PasswordHash"].ToString() : "",
                ShippingAddress = row["ShippingAddress"] != DBNull.Value ? row["ShippingAddress"].ToString() : "",
                ShippingCity = row["ShippingCity"] != DBNull.Value ? row["ShippingCity"].ToString() : "",
                ShippingState = row["ShippingState"] != DBNull.Value ? row["ShippingState"].ToString() : "",
                ShippingPostalCode = row["ShippingPostalCode"] != DBNull.Value ? row["ShippingPostalCode"].ToString() : "",
                BillingAddress = row["BillingAddress"] != DBNull.Value ? row["BillingAddress"].ToString() : "",
                BillingCity = row["BillingCity"] != DBNull.Value ? row["BillingCity"].ToString() : "",
                BillingState = row["BillingState"] != DBNull.Value ? row["BillingState"].ToString() : "",
                BillingPostalCode = row["BillingPostalCode"] != DBNull.Value ? row["BillingPostalCode"].ToString() : "",
                IsActive = (bool)row["IsActive"],
                CreatedDate = (DateTime)row["CreatedDate"],
                LastLogin = row["LastLogin"] != DBNull.Value ? (DateTime)row["LastLogin"] : null
            };
        }
    }
}
