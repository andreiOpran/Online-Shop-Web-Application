package com.onlineshop.service;

import com.onlineshop.entity.Cart;
import com.onlineshop.entity.CartProduct;
import com.onlineshop.entity.Product;
import com.onlineshop.entity.User;
import com.onlineshop.repository.CartRepository;
import com.onlineshop.repository.CartProductRepository;
import com.onlineshop.repository.ProductRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import java.util.List;
import java.util.Optional;

@Service
@Transactional
public class CartService {
    
    private static final Logger logger = LoggerFactory.getLogger(CartService.class);
    
    @Autowired
    private CartRepository cartRepository;
    
    @Autowired
    private CartProductRepository cartProductRepository;
    
    @Autowired
    private ProductRepository productRepository;
    
    public List<Cart> getAllCarts() {
        logger.debug("Fetching all carts");
        return cartRepository.findAllWithDetails();
    }
    
    public Optional<Cart> getCartById(Long cartId) {
        logger.debug("Fetching cart with ID: {}", cartId);
        return cartRepository.findById(cartId);
    }
    
    public Optional<Cart> getCartByIdWithProducts(Long cartId) {
        logger.debug("Fetching cart with products for ID: {}", cartId);
        return cartRepository.findByIdWithProducts(cartId);
    }
    
    public List<Cart> getCartsByUserId(Long userId) {
        logger.debug("Fetching carts for user ID: {}", userId);
        return cartRepository.findByUserId(userId);
    }
    
    public Optional<Cart> getActiveCartByUserId(Long userId) {
        logger.debug("Fetching active cart for user ID: {}", userId);
        return cartRepository.findByUserIdAndIsActiveTrue(userId);
    }
    
    public Optional<Cart> getActiveCartByUserIdWithProducts(Long userId) {
        logger.debug("Fetching active cart with products for user ID: {}", userId);
        return cartRepository.findActiveCartByUserIdWithProducts(userId);
    }
    
    public Cart createCart(User user) {
        logger.debug("Creating new cart for user: {}", user.getUsername());
        
        // Deactivate any existing active cart for the user
        Optional<Cart> existingActiveCart = getActiveCartByUserId(user.getUserId());
        if (existingActiveCart.isPresent()) {
            Cart activeCart = existingActiveCart.get();
            activeCart.setIsActive(false);
            cartRepository.save(activeCart);
        }
        
        Cart cart = new Cart(user);
        return cartRepository.save(cart);
    }
    
    public Cart getOrCreateActiveCart(User user) {
        logger.debug("Getting or creating active cart for user: {}", user.getUsername());
        
        Optional<Cart> activeCart = getActiveCartByUserId(user.getUserId());
        if (activeCart.isPresent()) {
            return activeCart.get();
        }
        
        return createCart(user);
    }
    
    public CartProduct addProductToCart(Long cartId, Long productId, Integer quantity) {
        logger.debug("Adding product {} to cart {} with quantity {}", productId, cartId, quantity);
        
        Cart cart = cartRepository.findById(cartId)
                .orElseThrow(() -> new IllegalArgumentException("Cart not found with ID: " + cartId));
        
        Product product = productRepository.findById(productId)
                .orElseThrow(() -> new IllegalArgumentException("Product not found with ID: " + productId));
        
        // Check if product is available and approved
        if (!"Approved".equals(product.getStatus())) {
            throw new IllegalArgumentException("Product is not available for purchase");
        }
        
        // Check stock availability
        if (product.getStock() != null && product.getStock() < quantity) {
            throw new IllegalArgumentException("Insufficient stock. Available: " + product.getStock());
        }
        
        // Check if product already exists in cart
        Optional<CartProduct> existingCartProduct = cartProductRepository.findByCartIdAndProductId(cartId, productId);
        if (existingCartProduct.isPresent()) {
            // Update quantity
            CartProduct cartProduct = existingCartProduct.get();
            int newQuantity = cartProduct.getQuantity() + quantity;
            
            // Check stock again for the new quantity
            if (product.getStock() != null && product.getStock() < newQuantity) {
                throw new IllegalArgumentException("Insufficient stock. Available: " + product.getStock());
            }
            
            cartProduct.setQuantity(newQuantity);
            return cartProductRepository.save(cartProduct);
        } else {
            // Create new cart product
            CartProduct cartProduct = new CartProduct(cart, product, quantity);
            return cartProductRepository.save(cartProduct);
        }
    }
    
    public CartProduct updateCartProductQuantity(Long cartId, Long productId, Integer quantity) {
        logger.debug("Updating cart product quantity for cart {} and product {} to {}", cartId, productId, quantity);
        
        CartProduct cartProduct = cartProductRepository.findByCartIdAndProductId(cartId, productId)
                .orElseThrow(() -> new IllegalArgumentException("Product not found in cart"));
        
        // Check stock availability
        Product product = cartProduct.getProduct();
        if (product.getStock() != null && product.getStock() < quantity) {
            throw new IllegalArgumentException("Insufficient stock. Available: " + product.getStock());
        }
        
        cartProduct.setQuantity(quantity);
        return cartProductRepository.save(cartProduct);
    }
    
    public void removeProductFromCart(Long cartId, Long productId) {
        logger.debug("Removing product {} from cart {}", productId, cartId);
        
        CartProduct cartProduct = cartProductRepository.findByCartIdAndProductId(cartId, productId)
                .orElseThrow(() -> new IllegalArgumentException("Product not found in cart"));
        
        cartProductRepository.delete(cartProduct);
    }
    
    public void clearCart(Long cartId) {
        logger.debug("Clearing cart with ID: {}", cartId);
        
        Cart cart = cartRepository.findById(cartId)
                .orElseThrow(() -> new IllegalArgumentException("Cart not found with ID: " + cartId));
        
        cartProductRepository.deleteByCartId(cartId);
    }
    
    public void deactivateCart(Long cartId) {
        logger.debug("Deactivating cart with ID: {}", cartId);
        
        Cart cart = cartRepository.findById(cartId)
                .orElseThrow(() -> new IllegalArgumentException("Cart not found with ID: " + cartId));
        
        cart.setIsActive(false);
        cartRepository.save(cart);
    }
    
    public void deleteCart(Long cartId) {
        logger.debug("Deleting cart with ID: {}", cartId);
        
        Cart cart = cartRepository.findById(cartId)
                .orElseThrow(() -> new IllegalArgumentException("Cart not found with ID: " + cartId));
        
        cartRepository.delete(cart);
    }
    
    public List<CartProduct> getCartProducts(Long cartId) {
        logger.debug("Fetching products for cart ID: {}", cartId);
        return cartProductRepository.findByCartIdWithProduct(cartId);
    }
    
    public Integer getTotalQuantityInCart(Long cartId) {
        logger.debug("Calculating total quantity for cart ID: {}", cartId);
        Integer total = cartProductRepository.getTotalQuantityByCartId(cartId);
        return total != null ? total : 0;
    }
    
    public long countActiveCarts() {
        return cartRepository.countActiveCarts();
    }
    
    public boolean cartExists(Long cartId) {
        return cartRepository.existsById(cartId);
    }
}