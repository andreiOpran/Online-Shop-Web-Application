package com.onlineshop.service;

import com.onlineshop.entity.Product;
import com.onlineshop.entity.Category;
import com.onlineshop.entity.User;
import com.onlineshop.repository.ProductRepository;
import com.onlineshop.repository.CategoryRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

@Service
@Transactional
public class ProductService {
    
    private static final Logger logger = LoggerFactory.getLogger(ProductService.class);
    
    @Autowired
    private ProductRepository productRepository;
    
    @Autowired
    private CategoryRepository categoryRepository;
    
    public List<Product> getAllProducts() {
        logger.debug("Fetching all products");
        return productRepository.findAll();
    }
    
    public Optional<Product> getProductById(Long productId) {
        logger.debug("Fetching product with ID: {}", productId);
        return productRepository.findById(productId);
    }
    
    public Optional<Product> getProductByIdWithDetails(Long productId) {
        logger.debug("Fetching product with details for ID: {}", productId);
        return productRepository.findByIdWithDetails(productId);
    }
    
    public List<Product> getProductsByStatus(String status) {
        logger.debug("Fetching products with status: {}", status);
        return productRepository.findByStatus(status);
    }
    
    public List<Product> getProductsByCategory(Long categoryId) {
        logger.debug("Fetching products for category ID: {}", categoryId);
        return productRepository.findByCategoryId(categoryId);
    }
    
    public List<Product> getProductsByUser(Long userId) {
        logger.debug("Fetching products for user ID: {}", userId);
        return productRepository.findByUserId(userId);
    }
    
    public List<Product> searchProducts(String keyword) {
        logger.debug("Searching products with keyword: {}", keyword);
        return productRepository.searchByKeyword(keyword);
    }
    
    public Page<Product> getApprovedProducts(Pageable pageable) {
        logger.debug("Fetching approved products with pagination");
        return productRepository.findApprovedProductsOrderByCreatedDate(pageable);
    }
    
    public Page<Product> getApprovedProductsByCategory(Long categoryId, Pageable pageable) {
        logger.debug("Fetching approved products for category ID: {} with pagination", categoryId);
        return productRepository.findApprovedProductsByCategoryOrderByCreatedDate(categoryId, pageable);
    }
    
    public Product createProduct(Product product) {
        logger.debug("Creating new product: {}", product.getTitle());
        
        // Validate category exists
        if (product.getCategory() == null || product.getCategory().getCategoryId() == null) {
            throw new IllegalArgumentException("Product must have a valid category");
        }
        
        Category category = categoryRepository.findById(product.getCategory().getCategoryId())
                .orElseThrow(() -> new IllegalArgumentException("Category not found"));
        
        product.setCategory(category);
        product.setCreatedDate(LocalDateTime.now());
        product.setStatus("Pending");
        
        return productRepository.save(product);
    }
    
    public Product updateProduct(Long productId, Product productDetails) {
        logger.debug("Updating product with ID: {}", productId);
        
        Product product = productRepository.findById(productId)
                .orElseThrow(() -> new IllegalArgumentException("Product not found with ID: " + productId));
        
        // Update fields
        product.setTitle(productDetails.getTitle());
        product.setDescription(productDetails.getDescription());
        product.setPrice(productDetails.getPrice());
        product.setStock(productDetails.getStock());
        product.setImagePath(productDetails.getImagePath());
        product.setSalePercentage(productDetails.getSalePercentage());
        
        // Update category if provided
        if (productDetails.getCategory() != null && productDetails.getCategory().getCategoryId() != null) {
            Category category = categoryRepository.findById(productDetails.getCategory().getCategoryId())
                    .orElseThrow(() -> new IllegalArgumentException("Category not found"));
            product.setCategory(category);
        }
        
        return productRepository.save(product);
    }
    
    public void deleteProduct(Long productId) {
        logger.debug("Deleting product with ID: {}", productId);
        Product product = productRepository.findById(productId)
                .orElseThrow(() -> new IllegalArgumentException("Product not found with ID: " + productId));
        
        productRepository.delete(product);
    }
    
    public Product approveProduct(Long productId) {
        logger.debug("Approving product with ID: {}", productId);
        Product product = productRepository.findById(productId)
                .orElseThrow(() -> new IllegalArgumentException("Product not found with ID: " + productId));
        
        product.setStatus("Approved");
        return productRepository.save(product);
    }
    
    public Product rejectProduct(Long productId) {
        logger.debug("Rejecting product with ID: {}", productId);
        Product product = productRepository.findById(productId)
                .orElseThrow(() -> new IllegalArgumentException("Product not found with ID: " + productId));
        
        product.setStatus("Denied");
        return productRepository.save(product);
    }
    
    public void updateProductRating(Long productId, BigDecimal rating) {
        logger.debug("Updating rating for product ID: {} to {}", productId, rating);
        Product product = productRepository.findById(productId)
                .orElseThrow(() -> new IllegalArgumentException("Product not found with ID: " + productId));
        
        product.setRating(rating);
        productRepository.save(product);
    }
    
    public List<Product> getProductsWithPendingEdits() {
        logger.debug("Fetching products with pending edits");
        return productRepository.findProductsWithPendingEdits();
    }
    
    public List<Product> getProductsWithPendingDeletes() {
        logger.debug("Fetching products with pending deletes");
        return productRepository.findProductsWithPendingDeletes();
    }
    
    public long countProductsByStatus(String status) {
        return productRepository.countByStatus(status);
    }
    
    public boolean productExists(Long productId) {
        return productRepository.existsById(productId);
    }
}