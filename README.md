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
