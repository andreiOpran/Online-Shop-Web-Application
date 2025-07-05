package com.onlineshop.repository;

import com.onlineshop.entity.PendingEdit;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;
import java.util.List;

@Repository
public interface PendingEditRepository extends JpaRepository<PendingEdit, Long> {
    
    List<PendingEdit> findByProductId(Long productId);
    
    List<PendingEdit> findByUserId(Long userId);
    
    @Query("SELECT pe FROM PendingEdit pe LEFT JOIN FETCH pe.originalProduct LEFT JOIN FETCH pe.editedProduct LEFT JOIN FETCH pe.user ORDER BY pe.createdDate DESC")
    List<PendingEdit> findAllWithDetailsOrderByCreatedDate();
    
    @Query("SELECT pe FROM PendingEdit pe LEFT JOIN FETCH pe.originalProduct LEFT JOIN FETCH pe.editedProduct LEFT JOIN FETCH pe.user WHERE pe.user.userId = :userId ORDER BY pe.createdDate DESC")
    List<PendingEdit> findByUserIdWithDetailsOrderByCreatedDate(Long userId);
}