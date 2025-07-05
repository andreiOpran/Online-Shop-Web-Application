package com.onlineshop.service;

import com.onlineshop.entity.User;
import com.onlineshop.entity.Role;
import com.onlineshop.repository.UserRepository;
import com.onlineshop.repository.RoleRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import java.util.HashSet;
import java.util.List;
import java.util.Optional;
import java.util.Set;

@Service
@Transactional
public class UserService implements UserDetailsService {
    
    private static final Logger logger = LoggerFactory.getLogger(UserService.class);
    
    @Autowired
    private UserRepository userRepository;
    
    @Autowired
    private RoleRepository roleRepository;
    
    @Autowired
    private PasswordEncoder passwordEncoder;
    
    @Override
    public UserDetails loadUserByUsername(String username) throws UsernameNotFoundException {
        logger.debug("Loading user by username: {}", username);
        User user = userRepository.findByUsernameWithRoles(username)
                .orElseThrow(() -> new UsernameNotFoundException("User not found: " + username));
        return user;
    }
    
    public List<User> getAllUsers() {
        logger.debug("Fetching all users");
        return userRepository.findAllOrderByUsername();
    }
    
    public Optional<User> getUserById(Long userId) {
        logger.debug("Fetching user with ID: {}", userId);
        return userRepository.findById(userId);
    }
    
    public Optional<User> getUserByIdWithRoles(Long userId) {
        logger.debug("Fetching user with roles for ID: {}", userId);
        return userRepository.findByIdWithRoles(userId);
    }
    
    public Optional<User> getUserByUsername(String username) {
        logger.debug("Fetching user with username: {}", username);
        return userRepository.findByUsernameWithRoles(username);
    }
    
    public Optional<User> getUserByEmail(String email) {
        logger.debug("Fetching user with email: {}", email);
        return userRepository.findByEmailWithRoles(email);
    }
    
    public User createUser(User user) {
        logger.debug("Creating new user: {}", user.getUsername());
        
        // Check if username already exists
        if (userRepository.existsByUsername(user.getUsername())) {
            throw new IllegalArgumentException("Username already exists: " + user.getUsername());
        }
        
        // Check if email already exists
        if (userRepository.existsByEmail(user.getEmail())) {
            throw new IllegalArgumentException("Email already exists: " + user.getEmail());
        }
        
        // Encode password
        user.setPassword(passwordEncoder.encode(user.getPassword()));
        
        // Assign default role (User) if no roles are specified
        if (user.getRoles() == null || user.getRoles().isEmpty()) {
            Role userRole = roleRepository.findByName("User")
                    .orElseThrow(() -> new IllegalStateException("Default role 'User' not found"));
            Set<Role> roles = new HashSet<>();
            roles.add(userRole);
            user.setRoles(roles);
        }
        
        return userRepository.save(user);
    }
    
    public User updateUser(Long userId, User userDetails) {
        logger.debug("Updating user with ID: {}", userId);
        
        User user = userRepository.findById(userId)
                .orElseThrow(() -> new IllegalArgumentException("User not found with ID: " + userId));
        
        // Update fields
        user.setFirstName(userDetails.getFirstName());
        user.setLastName(userDetails.getLastName());
        user.setEnabled(userDetails.getEnabled());
        
        // Update email if changed
        if (!user.getEmail().equals(userDetails.getEmail())) {
            if (userRepository.existsByEmail(userDetails.getEmail())) {
                throw new IllegalArgumentException("Email already exists: " + userDetails.getEmail());
            }
            user.setEmail(userDetails.getEmail());
        }
        
        // Update username if changed
        if (!user.getUsername().equals(userDetails.getUsername())) {
            if (userRepository.existsByUsername(userDetails.getUsername())) {
                throw new IllegalArgumentException("Username already exists: " + userDetails.getUsername());
            }
            user.setUsername(userDetails.getUsername());
        }
        
        return userRepository.save(user);
    }
    
    public User updateUserPassword(Long userId, String newPassword) {
        logger.debug("Updating password for user ID: {}", userId);
        
        User user = userRepository.findById(userId)
                .orElseThrow(() -> new IllegalArgumentException("User not found with ID: " + userId));
        
        user.setPassword(passwordEncoder.encode(newPassword));
        return userRepository.save(user);
    }
    
    public User updateUserRoles(Long userId, Set<Role> roles) {
        logger.debug("Updating roles for user ID: {}", userId);
        
        User user = userRepository.findById(userId)
                .orElseThrow(() -> new IllegalArgumentException("User not found with ID: " + userId));
        
        user.setRoles(roles);
        return userRepository.save(user);
    }
    
    public void deleteUser(Long userId) {
        logger.debug("Deleting user with ID: {}", userId);
        User user = userRepository.findById(userId)
                .orElseThrow(() -> new IllegalArgumentException("User not found with ID: " + userId));
        
        userRepository.delete(user);
    }
    
    public List<User> getUsersByRole(String roleName) {
        logger.debug("Fetching users with role: {}", roleName);
        return userRepository.findByRoleName(roleName);
    }
    
    public boolean userExists(Long userId) {
        return userRepository.existsById(userId);
    }
    
    public boolean usernameExists(String username) {
        return userRepository.existsByUsername(username);
    }
    
    public boolean emailExists(String email) {
        return userRepository.existsByEmail(email);
    }
    
    public User registerUser(String username, String email, String password, String firstName, String lastName) {
        logger.debug("Registering new user: {}", username);
        
        User user = new User();
        user.setUsername(username);
        user.setEmail(email);
        user.setPassword(password);
        user.setFirstName(firstName);
        user.setLastName(lastName);
        user.setEnabled(true);
        
        return createUser(user);
    }
}