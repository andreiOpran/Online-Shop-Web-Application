package com.onlineshop.controller;

import com.onlineshop.entity.Product;
import com.onlineshop.service.ProductService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;
import jakarta.validation.Valid;
import java.util.List;
import java.util.Map;
import java.util.Optional;

@RestController
@RequestMapping("/api/products")
public class ProductController {
    
    private static final Logger logger = LoggerFactory.getLogger(ProductController.class);
    
    @Autowired
    private ProductService productService;
    
    // Public endpoints
    @GetMapping("/public")
    public ResponseEntity<Page<Product>> getApprovedProducts(
            @RequestParam(defaultValue = "0") int page,
            @RequestParam(defaultValue = "10") int size,
            @RequestParam(required = false) Long categoryId) {
        try {
            Pageable pageable = PageRequest.of(page, size);
            Page<Product> products;
            
            if (categoryId != null) {
                products = productService.getApprovedProductsByCategory(categoryId, pageable);
            } else {
                products = productService.getApprovedProducts(pageable);
            }
            
            return ResponseEntity.ok(products);
        } catch (Exception e) {
            logger.error("Error fetching approved products", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }
    
    @GetMapping("/public/{id}")
    public ResponseEntity<Product> getProductById(@PathVariable Long id) {
        try {
            Optional<Product> product = productService.getProductByIdWithDetails(id);
            return product.map(ResponseEntity::ok)
                         .orElse(ResponseEntity.notFound().build());
        } catch (Exception e) {
            logger.error("Error fetching product with ID: {}", id, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }
    
    @GetMapping("/public/search")
    public ResponseEntity<List<Product>> searchProducts(@RequestParam String keyword) {
        try {
            List<Product> products = productService.searchProducts(keyword);
            return ResponseEntity.ok(products);
        } catch (Exception e) {
            logger.error("Error searching products with keyword: {}", keyword, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }
    
    // Admin endpoints
    @GetMapping("/admin")
    @PreAuthorize("hasRole('Admin')")
    public ResponseEntity<List<Product>> getAllProducts() {
        try {
            List<Product> products = productService.getAllProducts();
            return ResponseEntity.ok(products);
        } catch (Exception e) {
            logger.error("Error fetching all products", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }
    
    @GetMapping("/admin/pending")
    @PreAuthorize("hasRole('Admin')")
    public ResponseEntity<List<Product>> getPendingProducts() {
        try {
            List<Product> products = productService.getProductsByStatus("Pending");
            return ResponseEntity.ok(products);
        } catch (Exception e) {
            logger.error("Error fetching pending products", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }
    
    @PostMapping("/admin/{id}/approve")
    @PreAuthorize("hasRole('Admin')")
    public ResponseEntity<?> approveProduct(@PathVariable Long id) {
        try {
            Product product = productService.approveProduct(id);
            return ResponseEntity.ok(Map.of("message", "Product approved successfully", "product", product));
        } catch (IllegalArgumentException e) {
            return ResponseEntity.badRequest().body(Map.of("error", e.getMessage()));
        } catch (Exception e) {
            logger.error("Error approving product with ID: {}", id, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of("error", "An error occurred while approving the product"));
        }
    }
    
    @PostMapping("/admin/{id}/reject")
    @PreAuthorize("hasRole('Admin')")
    public ResponseEntity<?> rejectProduct(@PathVariable Long id) {
        try {
            Product product = productService.rejectProduct(id);
            return ResponseEntity.ok(Map.of("message", "Product rejected successfully", "product", product));
        } catch (IllegalArgumentException e) {
            return ResponseEntity.badRequest().body(Map.of("error", e.getMessage()));
        } catch (Exception e) {
            logger.error("Error rejecting product with ID: {}", id, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of("error", "An error occurred while rejecting the product"));
        }
    }
    
    @DeleteMapping("/admin/{id}")
    @PreAuthorize("hasRole('Admin')")
    public ResponseEntity<?> deleteProduct(@PathVariable Long id) {
        try {
            productService.deleteProduct(id);
            return ResponseEntity.ok(Map.of("message", "Product deleted successfully"));
        } catch (IllegalArgumentException e) {
            return ResponseEntity.badRequest().body(Map.of("error", e.getMessage()));
        } catch (Exception e) {
            logger.error("Error deleting product with ID: {}", id, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of("error", "An error occurred while deleting the product"));
        }
    }
    
    // Editor and Admin endpoints
    @PostMapping("/editor")
    @PreAuthorize("hasRole('Admin') or hasRole('Editor')")
    public ResponseEntity<?> createProduct(@Valid @RequestBody Product product) {
        try {
            Product createdProduct = productService.createProduct(product);
            return ResponseEntity.status(HttpStatus.CREATED).body(createdProduct);
        } catch (IllegalArgumentException e) {
            return ResponseEntity.badRequest().body(Map.of("error", e.getMessage()));
        } catch (Exception e) {
            logger.error("Error creating product", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of("error", "An error occurred while creating the product"));
        }
    }
    
    @PutMapping("/editor/{id}")
    @PreAuthorize("hasRole('Admin') or hasRole('Editor')")
    public ResponseEntity<?> updateProduct(@PathVariable Long id, @Valid @RequestBody Product product) {
        try {
            Product updatedProduct = productService.updateProduct(id, product);
            return ResponseEntity.ok(updatedProduct);
        } catch (IllegalArgumentException e) {
            return ResponseEntity.badRequest().body(Map.of("error", e.getMessage()));
        } catch (Exception e) {
            logger.error("Error updating product with ID: {}", id, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of("error", "An error occurred while updating the product"));
        }
    }
    
    // User endpoints
    @GetMapping("/user/my-products")
    @PreAuthorize("hasRole('Admin') or hasRole('Editor') or hasRole('User')")
    public ResponseEntity<List<Product>> getMyProducts(@RequestParam Long userId) {
        try {
            List<Product> products = productService.getProductsByUser(userId);
            return ResponseEntity.ok(products);
        } catch (Exception e) {
            logger.error("Error fetching products for user ID: {}", userId, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }
}