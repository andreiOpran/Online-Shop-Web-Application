package com.onlineshop.entity;

import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "pending_edits")
public class PendingEdit {
    
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "pending_edit_id")
    private Long pendingEditId;
    
    @Column(name = "product_id")
    private Long productId;
    
    @Column(name = "original_product_id")
    private Long originalProductId;
    
    @Column(name = "edited_product_id")
    private Long editedProductId;
    
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "original_product_id", insertable = false, updatable = false)
    private Product originalProduct;
    
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "edited_product_id", insertable = false, updatable = false)
    private Product editedProduct;
    
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id")
    private User user;
    
    @Column(name = "created_date")
    private LocalDateTime createdDate;
    
    // Constructors
    public PendingEdit() {
        this.createdDate = LocalDateTime.now();
    }
    
    public PendingEdit(Long productId, Long originalProductId, Long editedProductId, User user) {
        this();
        this.productId = productId;
        this.originalProductId = originalProductId;
        this.editedProductId = editedProductId;
        this.user = user;
    }
    
    // Getters and Setters
    public Long getPendingEditId() {
        return pendingEditId;
    }
    
    public void setPendingEditId(Long pendingEditId) {
        this.pendingEditId = pendingEditId;
    }
    
    public Long getProductId() {
        return productId;
    }
    
    public void setProductId(Long productId) {
        this.productId = productId;
    }
    
    public Long getOriginalProductId() {
        return originalProductId;
    }
    
    public void setOriginalProductId(Long originalProductId) {
        this.originalProductId = originalProductId;
    }
    
    public Long getEditedProductId() {
        return editedProductId;
    }
    
    public void setEditedProductId(Long editedProductId) {
        this.editedProductId = editedProductId;
    }
    
    public Product getOriginalProduct() {
        return originalProduct;
    }
    
    public void setOriginalProduct(Product originalProduct) {
        this.originalProduct = originalProduct;
    }
    
    public Product getEditedProduct() {
        return editedProduct;
    }
    
    public void setEditedProduct(Product editedProduct) {
        this.editedProduct = editedProduct;
    }
    
    public User getUser() {
        return user;
    }
    
    public void setUser(User user) {
        this.user = user;
    }
    
    public LocalDateTime getCreatedDate() {
        return createdDate;
    }
    
    public void setCreatedDate(LocalDateTime createdDate) {
        this.createdDate = createdDate;
    }
}