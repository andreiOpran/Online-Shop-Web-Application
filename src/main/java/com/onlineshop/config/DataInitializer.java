package com.onlineshop.config;

import com.onlineshop.entity.Role;
import com.onlineshop.repository.RoleRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.CommandLineRunner;
import org.springframework.stereotype.Component;

@Component
public class DataInitializer implements CommandLineRunner {
    
    private static final Logger logger = LoggerFactory.getLogger(DataInitializer.class);
    
    @Autowired
    private RoleRepository roleRepository;
    
    @Override
    public void run(String... args) throws Exception {
        initializeRoles();
    }
    
    private void initializeRoles() {
        logger.info("Initializing roles...");
        
        // Create Admin role if it doesn't exist
        if (!roleRepository.existsByName("Admin")) {
            Role adminRole = new Role("Admin");
            roleRepository.save(adminRole);
            logger.info("Created Admin role");
        }
        
        // Create Editor role if it doesn't exist
        if (!roleRepository.existsByName("Editor")) {
            Role editorRole = new Role("Editor");
            roleRepository.save(editorRole);
            logger.info("Created Editor role");
        }
        
        // Create User role if it doesn't exist
        if (!roleRepository.existsByName("User")) {
            Role userRole = new Role("User");
            roleRepository.save(userRole);
            logger.info("Created User role");
        }
        
        logger.info("Role initialization completed");
    }
}