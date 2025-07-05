package com.onlineshop.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Embeddable;
import java.io.Serializable;
import java.util.Objects;

@Embeddable
public class CartProductId implements Serializable {
    
    @Column(name = "cart_id")
    private Long cartId;
    
    @Column(name = "product_id")
    private Long productId;
    
    // Constructors
    public CartProductId() {}
    
    public CartProductId(Long cartId, Long productId) {
        this.cartId = cartId;
        this.productId = productId;
    }
    
    // Getters and Setters
    public Long getCartId() {
        return cartId;
    }
    
    public void setCartId(Long cartId) {
        this.cartId = cartId;
    }
    
    public Long getProductId() {
        return productId;
    }
    
    public void setProductId(Long productId) {
        this.productId = productId;
    }
    
    // equals and hashCode
    @Override
    public boolean equals(Object obj) {
        if (this == obj) return true;
        if (obj == null || getClass() != obj.getClass()) return false;
        CartProductId that = (CartProductId) obj;
        return Objects.equals(cartId, that.cartId) && 
               Objects.equals(productId, that.productId);
    }
    
    @Override
    public int hashCode() {
        return Objects.hash(cartId, productId);
    }
}