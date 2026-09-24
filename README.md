# E-Commerce API

🔗 **Live:** [Swagger API](https://ecomerccedeploy.runasp.net/swagger/index.html)

A production-ready E-Commerce backend built with **ASP.NET Core 8 Web API**, following a layered architecture with separate **Domain, Services, Persistence, Presentation, and Shared** projects.

The API provides functionality for product management, shopping baskets, orders, authentication, refresh tokens, payments, caching with Redis, email confirmation, and background cleanup jobs.
---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Key Features](#key-features)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)

---

## Overview

This project is the backend API for an E-Commerce application.

It provides the main functionality required for an online store, including:

- User registration and authentication
- JWT access tokens
- Refresh token authentication
- Email confirmation using OTP
- Password reset
- Product management
- Product filtering and pagination
- Shopping basket management
- Order creation and management
- Payment processing using Stripe
- Redis-based basket and caching support
- Email notifications
- Background refresh-token cleanup
- Global exception handling
- Consistent API response handling

The project is structured into separate layers to keep business logic, data access, and API concerns isolated.

---

## Architecture

The solution is organized into several projects:

```text

┌─────────────────────────────────────────────┐
│                Presentation                 │
│       Controllers / Middleware / API        │
├─────────────────────────────────────────────┤
│                Infrastructure               │
│   EF Core / Repositories / Email / DataSeed │
├─────────────────────────────────────────────┤
│                    Core                     │
│ Domain Entities / Services / Abstraction    │
├─────────────────────────────────────────────┤
│                   Shared                    │
│          DTOs / Result / Errors             │
└─────────────────────────────────────────────┘
```
---

## Key Features

### 🔐 Authentication & Identity
- User registration
- User login
- JWT-based authentication
- Access token and refresh token support
- Logout
- Email confirmation using OTP
- Resend email confirmation OTP
- Forgot password
- Reset password
- User management
- Role-based authorization

### 🛍️ Product Management
- Create products
- Update products
- Get product details
- Product listing
- Product search and filtering
- Product sorting
- Pagination
- Product specifications
- Product categories and brands
- Product image URL resolution

### 🛒 Shopping Basket
- Create and retrieve customer baskets
- Add items to basket
- Update basket items
- Remove basket items
- Clear basket
- Redis-based basket storage
- Basket persistence using Redis

### 📦 Order Management
- Create orders from customer baskets
- Retrieve customer orders
- Get order details
- Order items management
- Delivery method support
- Shipping address support
- Order status management
- Order specifications
- Payment status integration

### 💳 Payment
- Stripe payment integration
- PaymentIntent creation
- PaymentIntent update
- Payment processing
- Stripe webhook handling
- Payment status updates
- Payment failure handling

### ⚡ Redis Caching
- Redis integration using StackExchange.Redis
- Distributed caching
- Redis-based shopping basket storage
- API response caching
- Custom `RedisCacheAttribute`
- Cache repository abstraction

### 📧 Email & OTP
- SMTP email sending
- Email confirmation
- OTP generation
- OTP verification
- OTP expiration
- Resend OTP
- Password reset email support

### 🗄️ Data Access
- Entity Framework Core
- SQL Server
- Generic Repository Pattern
- Unit of Work Pattern
- Specification Pattern
- Repository abstractions
- EF Core migrations
- Database seeding
- Identity data initialization

### 🛡️ API & Error Handling
- Global exception handling middleware
- Centralized API error handling
- Result pattern
- Standardized error responses
- Custom error types
- API response factory

### 🏗️ Architecture & Design
- Layered architecture
- Separation of concerns
- Dependency Injection
- Service abstraction
- DTO-based API contracts
- AutoMapper
- Modular domain entities
- Clean separation between Domain, Services, Persistence, Presentation, and Shared layers

---

## Project Structure

```text
E-Commerce.Web/
│
├── Core/
│   │
│   ├── E-Commerce.Domain/
│   │   │
│   │   ├── Contracts/
│   │   │   ├── IBasketRepository.cs
│   │   │   ├── ICacheRepository.cs
│   │   │   ├── IDataInitializer.cs
│   │   │   ├── IGenericRepository.cs
│   │   │   ├── IRefreshTokenRepository.cs
│   │   │   ├── ISpecification.cs
│   │   │   └── IUnitOfWork.cs
│   │   │
│   │   ├── Entities/
│   │   │   ├── CartModule/
│   │   │   ├── IdentityModule/
│   │   │   ├── OrdersModule/
│   │   │   └── ProductModule/
│   │   │
│   │   ├── Exceptions/
│   │   │   └── NotFoundException.cs
│   │   │
│   │   ├── MappingProfile/
│   │   │   ├── CartProfile.cs
│   │   │   ├── OrderProfile.cs
│   │   │   ├── PictureURLResolver.cs
│   │   │   └── ProductProfile.cs
│   │   │
│   │   ├── Specifications/
│   │   │   ├── BaseSpecifications.cs
│   │   │   ├── OrderSpecification.cs
│   │   │   ├── OrderWithPaymentIntentSpecification.cs
│   │   │   ├── ProductCountSpecification.cs
│   │   │   ├── ProductSpecification.cs
│   │   │   ├── ProductSpecificationHelper.cs
│   │   │   └── ProductSpecificationWithFilter.cs
│   │   │
│   │   ├── AuthenticationService.cs
│   │   ├── BasketService.cs
│   │   ├── CacheService.cs
│   │   ├── OrderService.cs
│   │   ├── PaymentService.cs
│   │   ├── ProductService.cs
│   │   └── UserService.cs
│   │
│   └── E-Commerce.Services.Abstraction/
│       ├── IAuthenticationService.cs
│       ├── IBasketService.cs
│       ├── ICacheService.cs
│       ├── IOrderService.cs
│       ├── IPaymentService.cs
│       ├── IProductService.cs
│       └── IUserService.cs
│
├── Infrastructure/
│   │
│   └── E-Commerce.Persistence/
│       │
│       ├── Data/
│       │   ├── DataExtensions/
│       │   │   └── IdentityExtensions.cs
│       │   │
│       │   ├── DataSeed/
│       │   │   ├── JSONFiles/
│       │   │   ├── DataInitializer.cs
│       │   │   └── IdentityDataInitializer.cs
│       │   │
│       │   ├── DbContext/
│       │   │   └── EcommerceDbContext.cs
│       │   │
│       │   └── Migrations/
│       │
│       ├── Email/
│       │   └── EmailSender.cs
│       │
│       ├── Repository/
│       │   ├── BasketRepository.cs
│       │   ├── CacheRepository.cs
│       │   ├── GenericRepository.cs
│       │   ├── RefreshTokenRepository.cs
│       │   └── UnitOfWork.cs
│       │
│       └── SpecificationEvaluator.cs
│
├── Presentation/
│   │
│   └── E-Commerce.Presentation/
│       │
│       ├── Attributes/
│       │   └── RedisCacheAttribute.cs
│       │
│       ├── Controllers/
│       │   ├── ApiBaseController.cs
│       │   ├── AuthenticationController.cs
│       │   ├── BasketsController.cs
│       │   ├── OrdersController.cs
│       │   ├── PaymentController.cs
│       │   ├── ProductsController.cs
│       │   └── UserController.cs
│       │
│       ├── CustomMiddleware/
│       │   └── ExceptionHandlerMiddleware.cs
│       │
│       ├── Extensions/
│       │   ├── AuthenticationExtensions.cs
│       │   ├── InfrastructureExtensions.cs
│       │   ├── ServiceCollectionExtensions.cs
│       │   ├── SwaggerExtensions.cs
│       │   └── WebApplicationRegistration.cs
│       │
│       ├── Factory/
│       │   └── ApiResponseFactory.cs
│       │
│       ├── Jobs/
│       │   └── RefreshTokenCleanupJob.cs
│       │
│       └── E-Commerce.Web/
│           ├── Properties/
│           ├── wwwroot/
│           ├── CustomMiddleware/
│           └── Program.cs
│
└── E-Commerce.Shared/
    │
    ├── CommonResult/
    │   ├── Error.cs
    │   ├── ErrorType.cs
    │   └── Result.cs
    │
    ├── DTOs/
    │   ├── CartItemDTOs/
    │   │   ├── CartDto.cs
    │   │   └── CartItemDto.cs
    │   │
    │   ├── IdentityDTOs/
    │   │   ├── ConfirmEmailDto.cs
    │   │   ├── ForgetPasswordDto.cs
    │   │   ├── LoginDto.cs
    │   │   ├── RefreshTokenRequestDto.cs
    │   │   ├── RegisterDto.cs
    │   │   ├── ResetPasswordDto.cs
    │   │   └── UserDto.cs
    │   │
    │   ├── OrderDTOs/
    │   │   ├── AddressDto.cs
    │   │   ├── DeliveryMethodDto.cs
    │   │   ├── OrderDto.cs
    │   │   ├── OrderItemDto.cs
    │   │   ├── OrderStatus.cs
    │   │   └── OrderToReturnDto.cs
    │   │
    │   └── ProductDTOs/
    │       ├── CreateOrUpdateProductDto.cs
    │       └── ProductDto.cs
    │
    ├── PaginatedResult.cs
    ├── ProductQueryParams.cs
    └── ProductSortingOptions.cs

```
## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server
- Redis
- A [Stripe](https://dashboard.stripe.com/) account for payment features
- An SMTP-capable email account for email and OTP functionality

### 1. Clone & Restore

```bash
git clone <https://github.com/youssief-ibrahim/E-Commerce.git>
cd E-Commerce.Web
dotnet restore
```

### 2. Configure Secrets

Use .NET User Secrets (recommended) or your local configuration file:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
dotnet user-secrets set "Redis:ConnectionString" "<your-redis-connection-string>"
dotnet user-secrets set "JWT:SecretKey" "<your-secret-key>"
dotnet user-secrets set "Stripe:SecretKey" "<your-stripe-secret-key>"
dotnet user-secrets set "Stripe:WebhookSecret" "<your-webhook-secret>"
dotnet user-secrets set "Email:Username" "<your-email>"
dotnet user-secrets set "Email:Password" "<your-email-password>"
```
### 3. Apply Migrations

```bash
Update-database
```
