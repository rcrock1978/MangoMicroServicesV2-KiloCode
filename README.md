# Mango Microservices E-Commerce Platform

A full-stack e-commerce platform built with .NET 10 microservices architecture, enabling scalable online shopping with modular services.

## Architecture Overview

```
                    ┌─────────────┐
                    │   Client    │
                    └──────┬──────┘
                           │
                    ┌──────▼──────┐
                    │  Gateway    │ :5000
                    │   (Ocelot)  │
                    └──────┬──────┘
                           │
         ┌─────────────────┼─────────────────┐
         │                 │                 │
    ┌────▼────┐      ┌────▼────┐      ┌────▼────┐
    │  Auth   │      │Product  │      │  Cart   │
    │   API   │      │   API   │      │   API   │
    └────┬────┘      └────┬────┘      └────┬────┘
         │                 │                 │
         └─────────────────┼─────────────────┘
                           │
                    ┌──────▼──────┐
                    │  RabbitMQ   │
                    │ MessageBus  │
                    └──────┬──────┘
                           │
         ┌─────────────────┼─────────────────┐
         │                 │                 │
    ┌────▼────┐      ┌────▼────┐      ┌────▼────┐
    │  Order  │      │  Email  │      │ Coupon  │
    │   API   │      │   API   │      │   API   │
    └─────────┘      └─────────┘      └─────────┘
```

## Project Structure

```
Mango.slnx
├── src/
│   ├── Mango.Web/                     # ASP.NET Core MVC Frontend
│   ├── Mango.GatewaySolution/         # Ocelot API Gateway
│   ├── Mango.MessageBus/              # RabbitMQ Message Bus Library
│   ├── Mango.Common/                  # Shared Utilities
│   ├── Mango.Services.AuthAPI/        # Authentication Service
│   ├── Mango.Services.ProductAPI/     # Product Catalog Service
│   ├── Mango.Services.ShoppingCartAPI/ # Shopping Cart Service
│   ├── Mango.Services.OrderAPI/       # Order Processing Service
│   ├── Mango.Services.EmailAPI/        # Email Notification Service
│   ├── Mango.Services.CouponAPI/       # Discount Coupon Service
│   └── Mango.Services.RewardAPI/      # Loyalty Reward Service
└── deployment/
    └── docker-compose.yml             # Docker Orchestration
```

## Technology Stack

| Component | Technology |
|-----------|------------|
| Framework | ASP.NET Core 10.0 |
| Language | C# 13 |
| Database | SQL Server 2022 |
| ORM | Entity Framework Core 10 |
| API Gateway | Ocelot 24.0 |
| Message Broker | RabbitMQ 3.12 |
| Authentication | JWT Bearer |
| Container | Docker & Kubernetes |

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- Docker Desktop
- SQL Server (or use Docker container)
- RabbitMQ (or use Docker container)

### Running with Docker Compose

```bash
# Clone the repository
cd MangoMicroServiceV2

# Start all services
docker-compose -f deployment/docker-compose.yml up -d

# View logs
docker-compose -f deployment/docker-compose.yml logs -f

# Stop all services
docker-compose -f deployment/docker-compose.yml down
```

### Running Locally

```bash
# Restore packages
dotnet restore Mango.slnx

# Build solution
dotnet build Mango.slnx

# Run specific service
dotnet run --project src/Mango.Services.AuthAPI
```

## Services

### Auth API (Port 5001)
- User registration and login
- JWT token generation
- Role-based authorization

### Product API (Port 5002)
- Product catalog CRUD
- Category management
- Inventory tracking

### Shopping Cart API (Port 5003)
- Cart management
- Add/remove items
- Quantity updates

### Order API (Port 5004)
- Order processing
- Payment integration
- Order status tracking

### Email API (Port 5005)
- Transactional emails
- Order confirmations
- Password resets

### Coupon API (Port 5006)
- Discount coupon management
- Promotional codes

### Reward API (Port 5007)
- Loyalty points
- Reward redemption

### API Gateway (Port 5000)
- Request routing
- Rate limiting
- Authentication

## Configuration

### Environment Variables

```bash
# Database
ConnectionStrings__DefaultConnection=Server=localhost;Database=MangoAuthDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True

# JWT Authentication
Jwt__Secret=YourSuperSecretKey
Jwt__Issuer=MangoAuthAPI
Jwt__Audience=MangoClient

# RabbitMQ
RABBITMQ_HOST=localhost
RABBITMQ_PORT=5672
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
```

## API Endpoints

### Authentication
```
POST /api/Auth/register     - Register new user
POST /api/Auth/login       - Login user
POST /api/Auth/assign-role - Assign role to user
```

### Products
```
GET    /api/Product        - Get all products
GET    /api/Product/{id}   - Get product by ID
POST   /api/Product        - Create product
PUT    /api/Product/{id}   - Update product
DELETE /api/Product/{id}  - Delete product
```

### Shopping Cart
```
GET    /api/Cart           - Get user cart
POST   /api/Cart           - Add item to cart
PUT    /api/Cart/{id}      - Update cart item
DELETE /api/Cart/{id}      - Remove item from cart
```

### Orders
```
GET    /api/Order          - Get user orders
POST   /api/Order          - Create order
GET    /api/Order/{id}     - Get order details
PUT    /api/Order/{id}     - Update order status
```

## Best Practices

1. **Service Autonomy** - Each microservice is independently deployable
2. **Event-Driven** - Services communicate via RabbitMQ message bus
3. **API-First** - OpenAPI documentation for all endpoints
4. **Security** - JWT authentication with role-based access
5. **Observability** - Structured logging with correlation IDs

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.
