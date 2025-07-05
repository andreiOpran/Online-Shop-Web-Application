package com.onlineshop.repository;

import com.onlineshop.entity.Review;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;
import java.util.List;

@Repository
public interface ReviewRepository extends JpaRepository<Review, Long> {
    
    List<Review> findByProductId(Long productId);
    
    List<Review> findByUserId(Long userId);
    
    @Query("SELECT r FROM Review r LEFT JOIN FETCH r.user WHERE r.product.productId = :productId ORDER BY r.createdDate DESC")
    List<Review> findByProductIdWithUserOrderByCreatedDate(Long productId);
    
    @Query("SELECT r FROM Review r LEFT JOIN FETCH r.product LEFT JOIN FETCH r.user WHERE r.user.userId = :userId ORDER BY r.createdDate DESC")
    List<Review> findByUserIdWithProductOrderByCreatedDate(Long userId);
    
    @Query("SELECT AVG(r.rating) FROM Review r WHERE r.product.productId = :productId AND r.rating IS NOT NULL")
    Double getAverageRatingByProductId(Long productId);
    
    @Query("SELECT COUNT(r) FROM Review r WHERE r.product.productId = :productId")
    long countByProductId(Long productId);
    
    @Query("SELECT COUNT(r) FROM Review r WHERE r.product.productId = :productId AND r.rating IS NOT NULL")
    long countRatingsByProductId(Long productId);
}