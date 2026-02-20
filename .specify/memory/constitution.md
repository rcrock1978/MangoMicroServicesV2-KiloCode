# Mango Microservices Constitution

<!-- Sync Impact Report v1.0.0 -> v1.0.0 -->
<!-- 
  Version Change: Initial creation (1.0.0)
  Modified Principles: N/A (new)
  Added Sections: Core Principles, Technology Stack, Quality Standards, Development Workflow, Governance
  Removed Sections: N/A
  Templates Updated: ✅ All templates aligned
-->

## Core Principles

### I. Service Autonomy
Each microservice MUST be independently deployable, scalable, and maintainable. Services communicate via well-defined APIs and MUST NOT share databases. Each service owns its data and exposes operations through RESTful endpoints or asynchronous messages.

### II. Event-Driven Architecture
Services MUST communicate asynchronously via RabbitMQ message bus for eventual consistency. All state-changing operations MUST publish events (OrderPlaced, InventoryUpdated, etc.). Services MUST subscribe to relevant events to maintain data consistency without tight coupling.

### III. API-First Design
All service contracts MUST be designed before implementation. OpenAPI/Swagger documentation REQUIRED for every HTTP endpoint. Breaking changes to public APIs MUST increment version numbers following semantic versioning.

### IV. Security by Design
Authentication and authorization MUST be centralized in AuthAPI using JWT tokens. Every internal service MUST validate incoming tokens. Role-based access control (RBAC) with at minimum: Customer, Admin roles. All sensitive data MUST be encrypted at rest and in transit.

### V. Observability
Every service MUST implement structured logging with correlation IDs. Health check endpoints REQUIRED at /health for Kubernetes probes. Metrics MUST be exposed via Prometheus format. Distributed tracing via OpenTelemetry for request flow across services.

## Technology Stack

**Framework**: ASP.NET Core 10.0 (Web API and MVC)  
**Language**: C# 13  
**Database**: SQL Server with Entity Framework Core 10  
**API Gateway**: Ocelot  
**Message Broker**: RabbitMQ  
**Authentication**: JWT tokens with cookie-based frontend auth  
**Object Mapping**: AutoMapper  
**Configuration**: appsettings.json with service URLs  
**Deployment**: Docker and Kubernetes  
**Package Management**: NuGet  

**Required Patterns**:
- Repository Pattern for data access
- Unit of Work for transaction management
- Mediator Pattern for cross-cutting concerns
- Circuit Breaker Pattern for fault tolerance (Polly)

## Quality Standards

### Performance Requirements
- API response time: p95 < 200ms for non-complex queries
- Maximum 3 round trips per user action
- Connection pooling required for database access
- Response caching for read-heavy operations

### Reliability Requirements
- Circuit breaker on all external service calls
- Retry policies with exponential backoff
- Dead letter queues for failed message processing
- Idempotency keys for all state-changing operations

### Code Quality
- Minimum 80% code coverage for business logic
- All public APIs MUST have XML documentation
- Strict nullable reference types enforcement
- FxCop analyzers for coding standards

## Development Workflow

### Project Structure
```
Mango.sln
├── src/
│   ├── Mango.Web/                 # ASP.NET Core MVC Frontend
│   ├── Mango.GatewaySolution/     # Ocelot API Gateway
│   ├── Mango.Services.AuthAPI/    # Authentication Service
│   ├── Mango.Services.ProductAPI/ # Product Catalog Service
│   ├── Mango.Services.ShoppingCartAPI/
│   ├── Mango.Services.OrderAPI/
│   ├── Mango.Services.EmailAPI/
│   ├── Mango.Services.CouponAPI/
│   ├── Mango.Services.RewardAPI/
│   └── Mango.MessageBus/          # Shared RabbitMQ Library
├── tests/
│   ├── Mango.Tests.Unit/
│   ├── Mango.Tests.Integration/
│   └── Mango.Tests.E2E/
└── deployment/
    ├── docker-compose.yml
    └── k8s/
```

### Service Implementation Standards
1. Each service MUST have its own DbContext and migration folder
2. Configuration via IOptions pattern with strongly-typed classes
3. Dependency injection for all service dependencies
4. AutoMapper profiles in each service's Infrastructure project

## Governance

### Amendment Procedure
1. Changes to core principles require MAJOR version bump
2. New services or significant features require MINOR version bump
3. Documentation fixes, typo corrections require PATCH version bump
4. All amendments MUST be documented in Sync Impact Report

### Compliance Review
- Every PR MUST verify principle compliance
- Architecture review for new service introduction
- Security review for authentication/authorization changes
- Performance review for database schema changes

**Version**: 1.0.0 | **Ratified**: 2026-02-20 | **Last Amended**: 2026-02-20
