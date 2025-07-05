package com.onlineshop.entity;

import jakarta.persistence.*;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;
import java.time.LocalDateTime;

@Entity
@Table(name = "orders")
public class Order {
    
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "order_id")
    private Long orderId;
    
    @OneToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "cart_id")
    private Cart cart;
    
    @Column(name = "status", length = 50)
    private String status;
    
    @Column(name = "order_date")
    private LocalDateTime orderDate;
    
    @NotBlank(message = "Payment method is required.")
    @Size(max = 50, message = "Payment method cannot exceed 50 characters.")
    @Column(name = "payment_method", nullable = false, length = 50)
    private String paymentMethod;
    
    @NotBlank(message = "Shipping address is required.")
    @Size(min = 10, max = 200, message = "Shipping address must be between 10 and 200 characters.")
    @Column(name = "shipping_address", nullable = false, length = 200)
    private String shippingAddress;
    
    // Constructors
    public Order() {
        this.orderDate = LocalDateTime.now();
        this.status = "Pending";
    }
    
    public Order(Cart cart, String paymentMethod, String shippingAddress) {
        this();
        this.cart = cart;
        this.paymentMethod = paymentMethod;
        this.shippingAddress = shippingAddress;
    }
    
    // Getters and Setters
    public Long getOrderId() {
        return orderId;
    }
    
    public void setOrderId(Long orderId) {
        this.orderId = orderId;
    }
    
    public Cart getCart() {
        return cart;
    }
    
    public void setCart(Cart cart) {
        this.cart = cart;
    }
    
    public String getStatus() {
        return status;
    }
    
    public void setStatus(String status) {
        this.status = status;
    }
    
    public LocalDateTime getOrderDate() {
        return orderDate;
    }
    
    public void setOrderDate(LocalDateTime orderDate) {
        this.orderDate = orderDate;
    }
    
    public String getPaymentMethod() {
        return paymentMethod;
    }
    
    public void setPaymentMethod(String paymentMethod) {
        this.paymentMethod = paymentMethod;
    }
    
    public String getShippingAddress() {
        return shippingAddress;
    }
    
    public void setShippingAddress(String shippingAddress) {
        this.shippingAddress = shippingAddress;
    }
}