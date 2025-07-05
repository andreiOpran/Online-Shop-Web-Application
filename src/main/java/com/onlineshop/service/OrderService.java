package com.onlineshop.service;

import com.onlineshop.entity.Order;
import com.onlineshop.entity.Cart;
import com.onlineshop.repository.OrderRepository;
import com.onlineshop.repository.CartRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

@Service
@Transactional
public class OrderService {
    
    private static final Logger logger = LoggerFactory.getLogger(OrderService.class);
    
    @Autowired
    private OrderRepository orderRepository;
    
    @Autowired
    private CartRepository cartRepository;
    
    @Autowired
    private CartService cartService;
    
    public List<Order> getAllOrders() {
        logger.debug("Fetching all orders");
        return orderRepository.findAllOrderByOrderDateDesc();
    }
    
    public Optional<Order> getOrderById(Long orderId) {
        logger.debug("Fetching order with ID: {}", orderId);
        return orderRepository.findById(orderId);
    }
    
    public Optional<Order> getOrderByIdWithDetails(Long orderId) {
        logger.debug("Fetching order with details for ID: {}", orderId);
        return orderRepository.findByIdWithDetails(orderId);
    }
    
    public Optional<Order> getOrderByIdWithFullDetails(Long orderId) {
        logger.debug("Fetching order with full details for ID: {}", orderId);
        return orderRepository.findByIdWithFullDetails(orderId);
    }
    
    public List<Order> getOrdersByUserId(Long userId) {
        logger.debug("Fetching orders for user ID: {}", userId);
        return orderRepository.findByUserIdOrderByOrderDateDesc(userId);
    }
    
    public List<Order> getOrdersByStatus(String status) {
        logger.debug("Fetching orders with status: {}", status);
        return orderRepository.findByStatus(status);
    }
    
    public Order createOrder(Long cartId, String paymentMethod, String shippingAddress) {
        logger.debug("Creating new order for cart ID: {}", cartId);
        
        Cart cart = cartRepository.findByIdWithProducts(cartId)
                .orElseThrow(() -> new IllegalArgumentException("Cart not found with ID: " + cartId));
        
        // Validate cart is active and has products
        if (!cart.getIsActive()) {
            throw new IllegalArgumentException("Cart is not active");
        }
        
        if (cart.getCartProducts() == null || cart.getCartProducts().isEmpty()) {
            throw new IllegalArgumentException("Cart is empty");
        }
        
        // Validate stock availability for all products
        cart.getCartProducts().forEach(cartProduct -> {
            if (cartProduct.getProduct().getStock() != null && 
                cartProduct.getProduct().getStock() < cartProduct.getQuantity()) {
                throw new IllegalArgumentException("Insufficient stock for product: " + 
                    cartProduct.getProduct().getTitle());
            }
        });
        
        // Create order
        Order order = new Order(cart, paymentMethod, shippingAddress);
        order.setOrderDate(LocalDateTime.now());
        order.setStatus("Pending");
        
        Order savedOrder = orderRepository.save(order);
        
        // Deactivate cart after order creation
        cartService.deactivateCart(cartId);
        
        // Update stock for all products
        cart.getCartProducts().forEach(cartProduct -> {
            if (cartProduct.getProduct().getStock() != null) {
                cartProduct.getProduct().setStock(
                    cartProduct.getProduct().getStock() - cartProduct.getQuantity());
            }
        });
        
        logger.info("Order created successfully with ID: {}", savedOrder.getOrderId());
        return savedOrder;
    }
    
    public Order updateOrder(Long orderId, Order orderDetails) {
        logger.debug("Updating order with ID: {}", orderId);
        
        Order order = orderRepository.findById(orderId)
                .orElseThrow(() -> new IllegalArgumentException("Order not found with ID: " + orderId));
        
        // Update fields
        order.setStatus(orderDetails.getStatus());
        order.setPaymentMethod(orderDetails.getPaymentMethod());
        order.setShippingAddress(orderDetails.getShippingAddress());
        
        return orderRepository.save(order);
    }
    
    public Order updateOrderStatus(Long orderId, String status) {
        logger.debug("Updating order status for ID: {} to {}", orderId, status);
        
        Order order = orderRepository.findById(orderId)
                .orElseThrow(() -> new IllegalArgumentException("Order not found with ID: " + orderId));
        
        order.setStatus(status);
        return orderRepository.save(order);
    }
    
    public Order confirmOrder(Long orderId) {
        logger.debug("Confirming order with ID: {}", orderId);
        return updateOrderStatus(orderId, "Confirmed");
    }
    
    public Order shipOrder(Long orderId) {
        logger.debug("Shipping order with ID: {}", orderId);
        return updateOrderStatus(orderId, "Shipped");
    }
    
    public Order deliverOrder(Long orderId) {
        logger.debug("Delivering order with ID: {}", orderId);
        return updateOrderStatus(orderId, "Delivered");
    }
    
    public Order cancelOrder(Long orderId) {
        logger.debug("Cancelling order with ID: {}", orderId);
        
        Order order = orderRepository.findByIdWithFullDetails(orderId)
                .orElseThrow(() -> new IllegalArgumentException("Order not found with ID: " + orderId));
        
        // Only allow cancellation if order is not yet shipped
        if ("Shipped".equals(order.getStatus()) || "Delivered".equals(order.getStatus())) {
            throw new IllegalArgumentException("Cannot cancel order that has been shipped or delivered");
        }
        
        // Restore stock for all products
        if (order.getCart() != null && order.getCart().getCartProducts() != null) {
            order.getCart().getCartProducts().forEach(cartProduct -> {
                if (cartProduct.getProduct().getStock() != null) {
                    cartProduct.getProduct().setStock(
                        cartProduct.getProduct().getStock() + cartProduct.getQuantity());
                }
            });
        }
        
        return updateOrderStatus(orderId, "Cancelled");
    }
    
    public void deleteOrder(Long orderId) {
        logger.debug("Deleting order with ID: {}", orderId);
        
        Order order = orderRepository.findById(orderId)
                .orElseThrow(() -> new IllegalArgumentException("Order not found with ID: " + orderId));
        
        // Only allow deletion if order is cancelled or very old
        if (!"Cancelled".equals(order.getStatus())) {
            throw new IllegalArgumentException("Only cancelled orders can be deleted");
        }
        
        orderRepository.delete(order);
    }
    
    public long countOrdersByStatus(String status) {
        return orderRepository.countByStatus(status);
    }
    
    public boolean orderExists(Long orderId) {
        return orderRepository.existsById(orderId);
    }
}