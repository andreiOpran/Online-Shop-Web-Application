package com.onlineshop.service;

import com.onlineshop.entity.Review;
import com.onlineshop.entity.Product;
import com.onlineshop.entity.User;
import com.onlineshop.repository.ReviewRepository;
import com.onlineshop.repository.ProductRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import java.math.BigDecimal;
import java.math.RoundingMode;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

@Service
@Transactional
public class ReviewService {
    
    private static final Logger logger = LoggerFactory.getLogger(ReviewService.class);
    
    @Autowired
    private ReviewRepository reviewRepository;
    
    @Autowired
    private ProductRepository productRepository;
    
    @Autowired
    private ProductService productService;
    
    public List<Review> getAllReviews() {
        logger.debug("Fetching all reviews");
        return reviewRepository.findAll();
    }
    
    public Optional<Review> getReviewById(Long reviewId) {
        logger.debug("Fetching review with ID: {}", reviewId);
        return reviewRepository.findById(reviewId);
    }
    
    public List<Review> getReviewsByProductId(Long productId) {
        logger.debug("Fetching reviews for product ID: {}", productId);
        return reviewRepository.findByProductIdWithUserOrderByCreatedDate(productId);
    }
    
    public List<Review> getReviewsByUserId(Long userId) {
        logger.debug("Fetching reviews for user ID: {}", userId);
        return reviewRepository.findByUserIdWithProductOrderByCreatedDate(userId);
    }
    
    public Review createReview(Long productId, Long userId, Integer rating, String content) {
        logger.debug("Creating new review for product ID: {} by user ID: {}", productId, userId);
        
        Product product = productRepository.findById(productId)
                .orElseThrow(() -> new IllegalArgumentException("Product not found with ID: " + productId));
        
        // Validate rating if provided
        if (rating != null && (rating < 1 || rating > 5)) {
            throw new IllegalArgumentException("Rating must be between 1 and 5");
        }
        
        Review review = new Review();
        review.setProduct(product);
        review.setRating(rating);
        review.setContent(content);
        review.setCreatedDate(LocalDateTime.now());
        
        // Set user if provided (for authenticated reviews)
        if (userId != null) {
            User user = new User();
            user.setUserId(userId);
            review.setUser(user);
        }
        
        Review savedReview = reviewRepository.save(review);
        
        // Update product rating
        updateProductRating(productId);
        
        logger.info("Review created successfully with ID: {}", savedReview.getReviewId());
        return savedReview;
    }
    
    public Review updateReview(Long reviewId, Review reviewDetails) {
        logger.debug("Updating review with ID: {}", reviewId);
        
        Review review = reviewRepository.findById(reviewId)
                .orElseThrow(() -> new IllegalArgumentException("Review not found with ID: " + reviewId));
        
        // Validate rating if provided
        if (reviewDetails.getRating() != null && 
            (reviewDetails.getRating() < 1 || reviewDetails.getRating() > 5)) {
            throw new IllegalArgumentException("Rating must be between 1 and 5");
        }
        
        Long productId = review.getProduct().getProductId();
        
        // Update fields
        review.setRating(reviewDetails.getRating());
        review.setContent(reviewDetails.getContent());
        
        Review updatedReview = reviewRepository.save(review);
        
        // Update product rating
        updateProductRating(productId);
        
        return updatedReview;
    }
    
    public void deleteReview(Long reviewId) {
        logger.debug("Deleting review with ID: {}", reviewId);
        
        Review review = reviewRepository.findById(reviewId)
                .orElseThrow(() -> new IllegalArgumentException("Review not found with ID: " + reviewId));
        
        Long productId = review.getProduct().getProductId();
        
        reviewRepository.delete(review);
        
        // Update product rating after deletion
        updateProductRating(productId);
    }
    
    public void updateProductRating(Long productId) {
        logger.debug("Updating rating for product ID: {}", productId);
        
        Double averageRating = reviewRepository.getAverageRatingByProductId(productId);
        
        BigDecimal rating = BigDecimal.ZERO;
        if (averageRating != null && averageRating > 0) {
            rating = BigDecimal.valueOf(averageRating).setScale(2, RoundingMode.HALF_UP);
        }
        
        productService.updateProductRating(productId, rating);
    }
    
    public Double getAverageRatingByProductId(Long productId) {
        logger.debug("Calculating average rating for product ID: {}", productId);
        return reviewRepository.getAverageRatingByProductId(productId);
    }
    
    public long countReviewsByProductId(Long productId) {
        return reviewRepository.countByProductId(productId);
    }
    
    public long countRatingsByProductId(Long productId) {
        return reviewRepository.countRatingsByProductId(productId);
    }
    
    public boolean reviewExists(Long reviewId) {
        return reviewRepository.existsById(reviewId);
    }
    
    public boolean userCanReviewProduct(Long userId, Long productId) {
        // TODO: Implement logic to check if user has purchased the product
        // For now, allow all users to review any product
        return true;
    }
}