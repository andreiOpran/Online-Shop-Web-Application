# Online Shop (TechSense) - ASP.NET Core

## Overview
An e-commerce web application built using ASP.NET Core MVC, featuring a modern shopping experience with robust user management and product handling capabilities.

## Technologies Used
- ASP.NET Core MVC
- Docker
- Entity Framework Core
- MySQL Database (v8.0.31)
- Identity Framework
- Bootstrap
- HTML/CSS

## Features

### User Management
- Role-based authentication (Admin, Editor, User)
- User registration and login
- Email confirmation
- Secure password policies

### Shopping Experience
- Product browsing and searching
- Shopping cart functionality
- Order management and history
- Product ratings and reviews
- Category-based product organization

### Admin Features
- Complete product management
- User administration
- Order tracking and management
- Category management
- Sales monitoring

### Editor Features
- Product creation and management
- Category management
- Content moderation

## System Requirements
- .NET Core Runtime
- MySQL Server 8.0.31
- Docker (optional)

## Installation

Clone the repository:
```bash
git clone https://github.com/andreiOpran/Online-Shop-Web-Application.git
```

Navigate to the project directory:
```bash
cd Online-Shop-Web-Application
```

Update the connection string in `appsettings.json`

Run database migrations:
```bash
dotnet ef database update
```

Run the application:
```bash
dotnet run
```

## Docker Support
The application can be containerized using Docker:
```bash
docker build -t online-shop .
docker run -p 80:80 online-shop
```

## Project Structure
- `/Areas` - Identity and authentication
- `/Controllers` - MVC Controllers
- `/Models` - Data models
- `/Views` - UI templates
- `/Data` - Database context and migrations

## Contributing
1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Agile Development
We used agile development practices and managed our tasks via a Trello board. You can view our Trello board [here](./OnlineShop%20-%20TechSense%20-%20Trello%20Board.png).

## Documentation
We have a PDF documentation where we detailed the tasks assigned to each person. You can find it [here](./OnlineShop%20-%20TechSense%20-%20Tasks%20management%20documentation.pdf).


## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Authors
[ForceOfNature13](https://github.com/ForceOfNature13)  
[andreiOpran](https://github.com/andreiOpran)

_Last Updated: 13-01-2025_
