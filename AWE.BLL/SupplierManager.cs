#nullable disable
using System;
using System.Collections.Generic;
using System.Net.Mail;
using AWE.DAL;
using AWE.Models;

namespace AWE.BLL
{
    public class SupplierManager
    {
        private readonly SupplierDAL _dal = new SupplierDAL();

        // --- Validation ---
        private bool IsValidSupplier(Supplier supplier, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(supplier.SupplierName))
            {
                errorMessage = "Supplier name is required.";
                return false;
            }

            if (supplier.SupplierName.Length > 200)
            {
                errorMessage = "Supplier name cannot exceed 200 characters.";
                return false;
            }

            // Validate email if provided
            if (!string.IsNullOrWhiteSpace(supplier.Email))
            {
                try
                {
                    MailAddress mailAddress = new MailAddress(supplier.Email);
                }
                catch
                {
                    errorMessage = "Invalid email format.";
                    return false;
                }
            }

            return true;
        }

        // --- CREATE ---
        public int CreateSupplier(Supplier supplier)
        {
            if (!IsValidSupplier(supplier, out string errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            // Check for duplicate supplier name
            var existingSuppliers = _dal.GetAll();
            foreach (var existing in existingSuppliers)
            {
                if (existing.SupplierName.Equals(supplier.SupplierName, StringComparison.OrdinalIgnoreCase) 
                    && existing.SupplierID != supplier.SupplierID)
                {
                    throw new ArgumentException($"Supplier '{supplier.SupplierName}' already exists.");
                }
            }

            supplier.IsActive = true;
            return _dal.Insert(supplier);
        }

        // --- READ ---
        public Supplier GetSupplierById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid supplier ID.");
            }

            return _dal.GetById(id);
        }

        public List<Supplier> GetAllSuppliers()
        {
            return _dal.GetAll();
        }

        public List<Supplier> GetActiveSuppliers()
        {
            return _dal.GetActive();
        }

        // --- UPDATE ---
        public bool UpdateSupplier(Supplier supplier)
        {
            if (!IsValidSupplier(supplier, out string errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            // Check for duplicate supplier name
            var existingSuppliers = _dal.GetAll();
            foreach (var existing in existingSuppliers)
            {
                if (existing.SupplierName.Equals(supplier.SupplierName, StringComparison.OrdinalIgnoreCase) 
                    && existing.SupplierID != supplier.SupplierID)
                {
                    throw new ArgumentException($"Supplier '{supplier.SupplierName}' already exists.");
                }
            }

            return _dal.Update(supplier) > 0;
        }

        // --- DELETE (Soft Delete) ---
        public bool DeleteSupplier(int id)
        {
            // Business Rule: Check if supplier has products or GRNs before deleting
            // For now, we'll allow soft delete
            return _dal.Delete(id) > 0;
        }

        // --- ACTIVATE ---
        public bool ActivateSupplier(int id)
        {
            Supplier supplier = _dal.GetById(id);
            if (supplier == null)
            {
                throw new Exception("Supplier not found.");
            }

            supplier.IsActive = true;
            return _dal.Update(supplier) > 0;
        }
    }
}
