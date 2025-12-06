#nullable disable
using System;
using System.Collections.Generic;
using AWE.DAL;
using AWE.Models;

namespace AWE.BLL
{
    public class CategoryManager
    {
        private readonly CategoryDAL _dal = new CategoryDAL();

        // --- Validation ---
        private bool IsValidCategory(Category category, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                errorMessage = "Category name is required.";
                return false;
            }

            if (category.CategoryName.Length > 100)
            {
                errorMessage = "Category name cannot exceed 100 characters.";
                return false;
            }

            return true;
        }

        // --- CREATE ---
        public int CreateCategory(Category category)
        {
            if (!IsValidCategory(category, out string errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            // Check for duplicate category name
            var existingCategories = _dal.GetAll();
            foreach (var existing in existingCategories)
            {
                if (existing.CategoryName.Equals(category.CategoryName, StringComparison.OrdinalIgnoreCase) 
                    && existing.CategoryID != category.CategoryID)
                {
                    throw new ArgumentException($"Category '{category.CategoryName}' already exists.");
                }
            }

            category.IsActive = true;
            return _dal.Insert(category);
        }

        // --- READ ---
        public Category GetCategoryById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid category ID.");
            }

            return _dal.GetById(id);
        }

        public List<Category> GetAllCategories()
        {
            return _dal.GetAll();
        }

        public List<Category> GetActiveCategories()
        {
            return _dal.GetActive();
        }

        // --- UPDATE ---
        public bool UpdateCategory(Category category)
        {
            if (!IsValidCategory(category, out string errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            // Check for duplicate category name
            var existingCategories = _dal.GetAll();
            foreach (var existing in existingCategories)
            {
                if (existing.CategoryName.Equals(category.CategoryName, StringComparison.OrdinalIgnoreCase) 
                    && existing.CategoryID != category.CategoryID)
                {
                    throw new ArgumentException($"Category '{category.CategoryName}' already exists.");
                }
            }

            return _dal.Update(category) > 0;
        }

        // --- DELETE (Soft Delete) ---
        public bool DeleteCategory(int id)
        {
            // Business Rule: Check if category has products before deleting
            // For now, we'll allow soft delete
            return _dal.Delete(id) > 0;
        }

        // --- ACTIVATE ---
        public bool ActivateCategory(int id)
        {
            Category category = _dal.GetById(id);
            if (category == null)
            {
                throw new Exception("Category not found.");
            }

            category.IsActive = true;
            return _dal.Update(category) > 0;
        }
    }
}
