package com.onlineshop.repository;

import com.onlineshop.entity.Cart;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;
import java.util.List;
import java.util.Optional;

@Repository
public interface CartRepository extends JpaRepository<Cart, Long> {
    
    List<Cart> findByUserId(Long userId);
    
    List<Cart> findByUserIdAndIsActive(Long userId, Boolean isActive);
    
    Optional<Cart> findByUserIdAndIsActiveTrue(Long userId);
    
    @Query("SELECT c FROM Cart c LEFT JOIN FETCH c.cartProducts cp LEFT JOIN FETCH cp.product WHERE c.cartId = :cartId")
    Optional<Cart> findByIdWithProducts(Long cartId);
    
    @Query("SELECT c FROM Cart c LEFT JOIN FETCH c.cartProducts cp LEFT JOIN FETCH cp.product WHERE c.user.userId = :userId AND c.isActive = true")
    Optional<Cart> findActiveCartByUserIdWithProducts(Long userId);
    
    @Query("SELECT c FROM Cart c LEFT JOIN FETCH c.user LEFT JOIN FETCH c.cartProducts")
    List<Cart> findAllWithDetails();
    
    @Query("SELECT COUNT(c) FROM Cart c WHERE c.isActive = true")
    long countActiveCarts();
}