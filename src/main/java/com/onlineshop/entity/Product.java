package com.onlineshop.entity;

import jakarta.persistence.*;
import jakarta.validation.constraints.*;
import com.fasterxml.jackson.annotation.JsonIgnore;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;

@Entity
@Table(name = "products")
public class Product {
    
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "product_id")
    private Long productId;
    
    @NotBlank(message = "Product title is required.")
    @Size(max = 255, message = "Product title cannot exceed 255 characters.")
    @Column(name = "title", nullable = false)
    private String title;
    
    @Size(max = 1000, message = "Description cannot exceed 1000 characters.")
    @Column(name = "description", length = 1000)
    private String description;
    
    @Column(name = "image_path")
    private String imagePath;
    
    @NotNull(message = "Price is required.")
    @DecimalMin(value = "0.0", inclusive = false, message = "Price must be greater than 0.")
    @Column(name = "price", nullable = false, precision = 10, scale = 2)
    private BigDecimal price;
    
    @Min(value = 0, message = "Stock must be at least 0.")
    @Column(name = "stock")
    private Integer stock;
    
    @Column(name = "created_date")
    private LocalDateTime createdDate;
    
    @DecimalMin(value = "0.0", message = "Sale percentage must be at least 0.")
    @DecimalMax(value = "100.0", message = "Sale percentage cannot exceed 100.")
    @Column(name = "sale_percentage", precision = 5, scale = 2)
    private BigDecimal salePercentage = BigDecimal.ZERO;
    
    @NotNull(message = "Category is required. Please select a category.")
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "category_id", nullable = false)
    private Category category;
    
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id")
    private User user;
    
    @OneToMany(mappedBy = "product", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    @JsonIgnore
    private List<Review> reviews;
    
    @OneToMany(mappedBy = "product", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    @JsonIgnore
    private List<CartProduct> cartProducts;
    
    @Column(name = "rating", precision = 3, scale = 2)
    private BigDecimal rating = BigDecimal.ZERO;
    
    @Column(name = "status", length = 20)
    private String status = "Pending"; // Pending, Approved, Denied, PendingEdit, PendingDelete
    
    @Column(name = "pending_edit")
    private Boolean pendingEdit = false;
    
    @Column(name = "pending_delete")
    private Boolean pendingDelete = false;
    
    // Constructors
    public Product() {
        this.createdDate = LocalDateTime.now();
    }
    
    public Product(String title, String description, BigDecimal price, Category category) {
        this();
        this.title = title;
        this.description = description;
        this.price = price;
        this.category = category;
    }
    
    // Getters and Setters
    public Long getProductId() {
        return productId;
    }
    
    public void setProductId(Long productId) {
        this.productId = productId;
    }
    
    public String getTitle() {
        return title;
    }
    
    public void setTitle(String title) {
        this.title = title;
    }
    
    public String getDescription() {
        return description;
    }
    
    public void setDescription(String description) {
        this.description = description;
    }
    
    public String getImagePath() {
        return imagePath;
    }
    
    public void setImagePath(String imagePath) {
        this.imagePath = imagePath;
    }
    
    public BigDecimal getPrice() {
        return price;
    }
    
    public void setPrice(BigDecimal price) {
        this.price = price;
    }
    
    public Integer getStock() {
        return stock;
    }
    
    public void setStock(Integer stock) {
        this.stock = stock;
    }
    
    public LocalDateTime getCreatedDate() {
        return createdDate;
    }
    
    public void setCreatedDate(LocalDateTime createdDate) {
        this.createdDate = createdDate;
    }
    
    public BigDecimal getSalePercentage() {
        return salePercentage;
    }
    
    public void setSalePercentage(BigDecimal salePercentage) {
        this.salePercentage = salePercentage;
    }
    
    public Category getCategory() {
        return category;
    }
    
    public void setCategory(Category category) {
        this.category = category;
    }
    
    public User getUser() {
        return user;
    }
    
    public void setUser(User user) {
        this.user = user;
    }
    
    public List<Review> getReviews() {
        return reviews;
    }
    
    public void setReviews(List<Review> reviews) {
        this.reviews = reviews;
    }
    
    public List<CartProduct> getCartProducts() {
        return cartProducts;
    }
    
    public void setCartProducts(List<CartProduct> cartProducts) {
        this.cartProducts = cartProducts;
    }
    
    public BigDecimal getRating() {
        return rating;
    }
    
    public void setRating(BigDecimal rating) {
        this.rating = rating;
    }
    
    public String getStatus() {
        return status;
    }
    
    public void setStatus(String status) {
        this.status = status;
    }
    
    public Boolean getPendingEdit() {
        return pendingEdit;
    }
    
    public void setPendingEdit(Boolean pendingEdit) {
        this.pendingEdit = pendingEdit;
    }
    
    public Boolean getPendingDelete() {
        return pendingDelete;
    }
    
    public void setPendingDelete(Boolean pendingDelete) {
        this.pendingDelete = pendingDelete;
    }
}