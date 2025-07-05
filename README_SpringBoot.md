# Online Shop - Spring Boot Application

A comprehensive online shop application built with Spring Boot, converted from the original ASP.NET Core implementation.

## Features

- **Product Management**: CRUD operations for products with categories
- **User Authentication**: JWT-based authentication with role-based authorization
- **Shopping Cart**: Add, update, and remove products from cart
- **Order Management**: Place and track orders
- **Review System**: Product reviews and ratings
- **Product Approval Workflow**: Admin approval for new products
- **Role-based Access Control**: Admin, Editor, and User roles

## Technology Stack

- **Backend**: Spring Boot 3.2.0
- **Database**: MySQL 8.0+
- **Security**: Spring Security with JWT
- **ORM**: Spring Data JPA with Hibernate
- **API Documentation**: OpenAPI 3.0 (Swagger)
- **Build Tool**: Maven
- **Java Version**: 17+

## Prerequisites

- Java 17 or higher
- Maven 3.6+
- MySQL 8.0+ (or Docker)
- Git

## Database Setup

### Option 1: Using Docker (Recommended)

```bash
docker run --name OnlineShopDBContainer \
  -e MYSQL_ROOT_PASSWORD=rootPassword \
  -e MYSQL_DATABASE=OnlineShopDB \
  -e MYSQL_USER=OnlineShopUser \
  -e MYSQL_PASSWORD=Parola1! \
  -p 3306:3306 \
  -d mysql:8.0
```

### Option 2: Local MySQL Installation

Create a database and user:

```sql
CREATE DATABASE OnlineShopDB;
CREATE USER 'OnlineShopUser'@'localhost' IDENTIFIED BY 'Parola1!';
GRANT ALL PRIVILEGES ON OnlineShopDB.* TO 'OnlineShopUser'@'localhost';
FLUSH PRIVILEGES;
```

## Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/andreiOpran/Online-Shop-Web-Application.git
   cd Online-Shop-Web-Application
   ```

2. **Configure the database** (if using different credentials)
   
   Edit `src/main/resources/application.properties`:
   ```properties
   spring.datasource.url=jdbc:mysql://localhost:3306/OnlineShopDB
   spring.datasource.username=OnlineShopUser
   spring.datasource.password=Parola1!
   ```

3. **Build the application**
   ```bash
   mvn clean compile
   ```

4. **Run the application**
   ```bash
   mvn spring-boot:run
   ```

The application will start on `http://localhost:8080`

## API Documentation

Once the application is running, you can access:

- **Swagger UI**: http://localhost:8080/swagger-ui.html
- **OpenAPI JSON**: http://localhost:8080/v3/api-docs

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/validate` - Validate JWT token

### Products
- `GET /api/products/public` - Get approved products (public)
- `GET /api/products/public/{id}` - Get product details (public)
- `GET /api/products/public/search` - Search products (public)
- `POST /api/products/editor` - Create product (Editor/Admin)
- `PUT /api/products/editor/{id}` - Update product (Editor/Admin)
- `POST /api/products/admin/{id}/approve` - Approve product (Admin)
- `POST /api/products/admin/{id}/reject` - Reject product (Admin)
- `DELETE /api/products/admin/{id}` - Delete product (Admin)

### Categories
- `GET /api/categories/public` - Get all categories (public)
- `GET /api/categories/public/{id}` - Get category details (public)
- `POST /api/categories/admin` - Create category (Admin)
- `PUT /api/categories/admin/{id}` - Update category (Admin)
- `DELETE /api/categories/admin/{id}` - Delete category (Admin)

## User Roles

1. **User**: Can browse products, manage cart, place orders, write reviews
2. **Editor**: Can create and edit products (subject to admin approval)
3. **Admin**: Full access to all features including user management and product approval

## Testing

Run unit tests:
```bash
mvn test
```

Run integration tests:
```bash
mvn integration-test
```

## Configuration

Key configuration properties in `application.properties`:

```properties
# Database
spring.datasource.url=jdbc:mysql://localhost:3306/OnlineShopDB
spring.datasource.username=OnlineShopUser
spring.datasource.password=Parola1!

# JPA/Hibernate
spring.jpa.hibernate.ddl-auto=update
spring.jpa.show-sql=true

# JWT
jwt.secret=MySecretKey123456789012345678901234567890
jwt.expiration=86400000

# Server
server.port=8080
```

## Default Data

The application automatically creates default roles on startup:
- Admin
- Editor  
- User

## Troubleshooting

1. **Database Connection Issues**
   - Ensure MySQL is running
   - Check database credentials
   - Verify database exists

2. **Port Already in Use**
   - Change server port in `application.properties`
   - Kill process using port 8080: `lsof -ti:8080 | xargs kill -9`

3. **JWT Issues**
   - Ensure JWT secret is at least 256 bits
   - Check token expiration time

## Migration from ASP.NET Core

This Spring Boot application maintains feature parity with the original ASP.NET Core version:

- All entity models converted to JPA entities
- Controllers converted to REST endpoints
- ASP.NET Identity replaced with Spring Security + JWT
- Entity Framework replaced with Spring Data JPA
- Dependency injection patterns preserved

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.