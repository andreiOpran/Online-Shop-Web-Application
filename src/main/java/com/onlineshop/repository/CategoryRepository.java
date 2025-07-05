package com.onlineshop.repository;

import com.onlineshop.entity.Category;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;
import java.util.List;
import java.util.Optional;

@Repository
public interface CategoryRepository extends JpaRepository<Category, Long> {
    
    Optional<Category> findByCategoryName(String categoryName);
    
    boolean existsByCategoryName(String categoryName);
    
    @Query("SELECT c FROM Category c LEFT JOIN FETCH c.products WHERE c.categoryId = :categoryId")
    Optional<Category> findByIdWithProducts(Long categoryId);
    
    @Query("SELECT c FROM Category c ORDER BY c.categoryName")
    List<Category> findAllOrderByCategoryName();
}