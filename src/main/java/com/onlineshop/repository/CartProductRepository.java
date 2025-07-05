package com.onlineshop.repository;

import com.onlineshop.entity.CartProduct;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;
import java.util.List;
import java.util.Optional;

@Repository
public interface CartProductRepository extends JpaRepository<CartProduct, Long> {
    
    @Query("SELECT cp FROM CartProduct cp WHERE cp.cart.cartId = :cartId")
    List<CartProduct> findByCartId(Long cartId);
    
    @Query("SELECT cp FROM CartProduct cp WHERE cp.product.productId = :productId")
    List<CartProduct> findByProductId(Long productId);
    
    @Query("SELECT cp FROM CartProduct cp WHERE cp.cart.cartId = :cartId AND cp.product.productId = :productId")
    Optional<CartProduct> findByCartIdAndProductId(Long cartId, Long productId);
    
    @Query("SELECT cp FROM CartProduct cp LEFT JOIN FETCH cp.product WHERE cp.cart.cartId = :cartId")
    List<CartProduct> findByCartIdWithProduct(Long cartId);
    
    @Query("SELECT SUM(cp.quantity) FROM CartProduct cp WHERE cp.cart.cartId = :cartId")
    Integer getTotalQuantityByCartId(Long cartId);
    
    @Query("DELETE FROM CartProduct cp WHERE cp.cart.cartId = :cartId")
    void deleteByCartId(Long cartId);
    
    @Query("DELETE FROM CartProduct cp WHERE cp.product.productId = :productId")
    void deleteByProductId(Long productId);
}