package com.onlineshop.repository;

import com.onlineshop.entity.CartProduct;
import com.onlineshop.entity.CartProductId;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;
import java.util.List;
import java.util.Optional;

@Repository
public interface CartProductRepository extends JpaRepository<CartProduct, CartProductId> {
    
    List<CartProduct> findByCartId(Long cartId);
    
    List<CartProduct> findByProductId(Long productId);
    
    Optional<CartProduct> findByCartIdAndProductId(Long cartId, Long productId);
    
    @Query("SELECT cp FROM CartProduct cp LEFT JOIN FETCH cp.product WHERE cp.cart.cartId = :cartId")
    List<CartProduct> findByCartIdWithProduct(Long cartId);
    
    @Query("SELECT SUM(cp.quantity) FROM CartProduct cp WHERE cp.cart.cartId = :cartId")
    Integer getTotalQuantityByCartId(Long cartId);
    
    void deleteByCartId(Long cartId);
    
    void deleteByProductId(Long productId);
}