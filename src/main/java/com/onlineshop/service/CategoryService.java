package com.onlineshop.service;

import com.onlineshop.entity.Category;
import com.onlineshop.repository.CategoryRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import java.util.List;
import java.util.Optional;

@Service
@Transactional
public class CategoryService {
    
    private static final Logger logger = LoggerFactory.getLogger(CategoryService.class);
    
    @Autowired
    private CategoryRepository categoryRepository;
    
    public List<Category> getAllCategories() {
        logger.debug("Fetching all categories");
        return categoryRepository.findAllOrderByCategoryName();
    }
    
    public Optional<Category> getCategoryById(Long categoryId) {
        logger.debug("Fetching category with ID: {}", categoryId);
        return categoryRepository.findById(categoryId);
    }
    
    public Optional<Category> getCategoryByName(String categoryName) {
        logger.debug("Fetching category with name: {}", categoryName);
        return categoryRepository.findByCategoryName(categoryName);
    }
    
    public Category createCategory(Category category) {
        logger.debug("Creating new category: {}", category.getCategoryName());
        if (categoryRepository.existsByCategoryName(category.getCategoryName())) {
            throw new IllegalArgumentException("Category with name '" + category.getCategoryName() + "' already exists");
        }
        return categoryRepository.save(category);
    }
    
    public Category updateCategory(Long categoryId, Category categoryDetails) {
        logger.debug("Updating category with ID: {}", categoryId);
        Category category = categoryRepository.findById(categoryId)
                .orElseThrow(() -> new IllegalArgumentException("Category not found with ID: " + categoryId));
        
        // Check if the new name already exists (except for the current category)
        if (!category.getCategoryName().equals(categoryDetails.getCategoryName()) &&
            categoryRepository.existsByCategoryName(categoryDetails.getCategoryName())) {
            throw new IllegalArgumentException("Category with name '" + categoryDetails.getCategoryName() + "' already exists");
        }
        
        category.setCategoryName(categoryDetails.getCategoryName());
        return categoryRepository.save(category);
    }
    
    public void deleteCategory(Long categoryId) {
        logger.debug("Deleting category with ID: {}", categoryId);
        Category category = categoryRepository.findById(categoryId)
                .orElseThrow(() -> new IllegalArgumentException("Category not found with ID: " + categoryId));
        
        // Check if category has products
        if (category.getProducts() != null && !category.getProducts().isEmpty()) {
            throw new IllegalStateException("Cannot delete category with existing products");
        }
        
        categoryRepository.delete(category);
    }
    
    public boolean categoryExists(Long categoryId) {
        return categoryRepository.existsById(categoryId);
    }
    
    public boolean categoryExistsByName(String categoryName) {
        return categoryRepository.existsByCategoryName(categoryName);
    }
}