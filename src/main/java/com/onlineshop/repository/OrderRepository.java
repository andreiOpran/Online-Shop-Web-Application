package com.onlineshop.repository;

import com.onlineshop.entity.Order;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;
import java.util.List;
import java.util.Optional;

@Repository
public interface OrderRepository extends JpaRepository<Order, Long> {
    
    List<Order> findByStatus(String status);
    
    List<Order> findByCartUserId(Long userId);
    
    @Query("SELECT o FROM Order o LEFT JOIN FETCH o.cart c LEFT JOIN FETCH c.user WHERE o.orderId = :orderId")
    Optional<Order> findByIdWithDetails(Long orderId);
    
    @Query("SELECT o FROM Order o LEFT JOIN FETCH o.cart c LEFT JOIN FETCH c.user LEFT JOIN FETCH c.cartProducts cp LEFT JOIN FETCH cp.product WHERE o.orderId = :orderId")
    Optional<Order> findByIdWithFullDetails(Long orderId);
    
    @Query("SELECT o FROM Order o LEFT JOIN FETCH o.cart c LEFT JOIN FETCH c.user WHERE c.user.userId = :userId ORDER BY o.orderDate DESC")
    List<Order> findByUserIdOrderByOrderDateDesc(Long userId);
    
    @Query("SELECT o FROM Order o LEFT JOIN FETCH o.cart c LEFT JOIN FETCH c.user ORDER BY o.orderDate DESC")
    List<Order> findAllOrderByOrderDateDesc();
    
    @Query("SELECT COUNT(o) FROM Order o WHERE o.status = :status")
    long countByStatus(String status);
}