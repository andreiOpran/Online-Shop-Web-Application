package com.onlineshop.repository;

import com.onlineshop.entity.Product;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;
import java.util.List;
import java.util.Optional;

@Repository
public interface ProductRepository extends JpaRepository<Product, Long> {
    
    List<Product> findByStatus(String status);
    
    List<Product> findByCategoryId(Long categoryId);
    
    List<Product> findByUserId(Long userId);
    
    List<Product> findByTitleContainingIgnoreCase(String title);
    
    List<Product> findByDescriptionContainingIgnoreCase(String description);
    
    @Query("SELECT p FROM Product p WHERE p.title LIKE %:keyword% OR p.description LIKE %:keyword%")
    List<Product> searchByKeyword(@Param("keyword") String keyword);
    
    @Query("SELECT p FROM Product p LEFT JOIN FETCH p.category LEFT JOIN FETCH p.user LEFT JOIN FETCH p.reviews WHERE p.productId = :productId")
    Optional<Product> findByIdWithDetails(Long productId);
    
    @Query("SELECT p FROM Product p LEFT JOIN FETCH p.category LEFT JOIN FETCH p.user LEFT JOIN FETCH p.reviews WHERE p.status = :status")
    List<Product> findByStatusWithDetails(String status);
    
    @Query("SELECT p FROM Product p WHERE p.category.categoryId = :categoryId AND p.status = :status")
    List<Product> findByCategoryIdAndStatus(Long categoryId, String status);
    
    @Query("SELECT p FROM Product p WHERE p.status = 'Approved' ORDER BY p.createdDate DESC")
    Page<Product> findApprovedProductsOrderByCreatedDate(Pageable pageable);
    
    @Query("SELECT p FROM Product p WHERE p.status = 'Approved' AND p.category.categoryId = :categoryId ORDER BY p.createdDate DESC")
    Page<Product> findApprovedProductsByCategoryOrderByCreatedDate(Long categoryId, Pageable pageable);
    
    @Query("SELECT p FROM Product p WHERE p.pendingEdit = true")
    List<Product> findProductsWithPendingEdits();
    
    @Query("SELECT p FROM Product p WHERE p.pendingDelete = true")
    List<Product> findProductsWithPendingDeletes();
    
    @Query("SELECT COUNT(p) FROM Product p WHERE p.status = :status")
    long countByStatus(String status);
}