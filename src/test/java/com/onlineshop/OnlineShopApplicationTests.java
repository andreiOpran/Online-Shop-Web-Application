package com.onlineshop;

import org.junit.jupiter.api.Test;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.ActiveProfiles;

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.NONE)
@ActiveProfiles("test")
class OnlineShopApplicationTests {

    @Test
    void contextLoads() {
        // This test will pass if the Spring context loads successfully
    }

}