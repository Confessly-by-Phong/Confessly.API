# Confessly API

[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-blue.svg)](https://www.postgresql.org/)
[![JWT](https://img.shields.io/badge/JWT-Authentication-green.svg)](https://jwt.io/)

A robust .NET 10 Web API built with Clean Architecture principles, featuring comprehensive logging, authentication, and validation systems for the Confessly platform.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [API Documentation](#api-documentation)
- [Configuration](#configuration)
- [Development Guidelines](#development-guidelines)
- [Contributing](#contributing)

## Features

- **Clean Architecture**: Strict layered architecture with no circular dependencies
- **JWT Authentication**: Secure authentication with bcrypt password hashing
- **Comprehensive Logging**: Serilog-based structured logging with correlation IDs
- **Repository Pattern**: Generic repository with Unit of Work pattern
- **Soft Delete Support**: Built-in soft delete functionality for all entities
- **Domain-Driven Design**: Domain entities with business rules and validation
- **Performance Tracking**: Built-in operation timing and performance monitoring
- **Global Exception Handling**: Centralized error handling and validation
- **PostgreSQL Integration**: Full EF Core integration with PostgreSQL
- **Request Correlation**: Automatic correlation ID tracking across requests

## Architecture

The solution follows Clean Architecture principles with 10 projects organized in layers:

### Core Projects
- **Confessly.Domain** - Entities and domain interfaces
- **Confessly.Infrastructure** - EF Core, authentication, and external concerns  
- **Confessly.Repository** - Repository pattern with Unit of Work
- **Confessly.Services** - Business logic and orchestration
- **Confessly.API** - Controllers and application entry point

### Supporting Projects
- **Confessly.Contracts** - DTOs and shared interfaces
- **Confessly.Configuration** - Configuration management
- **Confessly.Validators** - Validation infrastructure
- **Confessly.Messages** - Exception and validation messages
- **Confessly.Logging** - Comprehensive logging framework

## Technology Stack

- **Framework**: .NET 10.0
- **Database**: PostgreSQL with Entity Framework Core 10.0
- **Authentication**: JWT with Microsoft.AspNetCore.Identity (bcrypt)
- **Logging**: Serilog 4.3 with structured logging
- **ORM**: Entity Framework Core with Npgsql provider
- **Patterns**: Repository, Unit of Work, Domain-Driven Design
- **Architecture**: Clean Architecture with dependency injection

## Prerequisites

Before running this project, ensure you have the following installed:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## Getting Started

1. **Clone the repository**
   ```bash
   git clone <your-repository-url>
   cd Confessly.API
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure the database connection**
   
   Update the connection string in `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "Default": "Host=localhost;Database=ConfesslyDB;Username=your_username;Password=your_password"
     }
   }
   ```

4. **Set up JWT configuration**
   
   Update JWT settings in `appsettings.Development.json`:
   ```json
   {
     "Jwt": {
       "Secret": "your-secret-key-here-make-it-long-enough",
       "Issuer": "ConfesslyAPI",
       "Audience": "ConfesslyUsers",
       "ExpirationMinutes": 60
     }
   }
   ```

## Database Setup

### Creating and Running Migrations

The project uses Entity Framework Core for database operations. Use the following commands in the **Visual Studio Developer Command Prompt** or **Package Manager Console**:

#### Add a new migration:
```bash
dotnet ef migrations add User-UpdateName --project Confessly.Infrastructure --startup-project Confessly.API
```

#### Update the database:
```bash
dotnet ef database update --project Confessly.Infrastructure --startup-project Confessly.API
```

#### Other useful EF commands:
```bash
# List all migrations
dotnet ef migrations list --project Confessly.Infrastructure --startup-project Confessly.API

# Remove last migration (if not applied to database)
dotnet ef migrations remove --project Confessly.Infrastructure --startup-project Confessly.API

# Generate SQL script
dotnet ef migrations script --project Confessly.Infrastructure --startup-project Confessly.API
```

### Database Features
- **Time-ordered GUIDs**: All entities use `Guid.CreateVersion7()` for optimal database performance
- **Soft Deletes**: Entities are marked as deleted rather than physically removed
- **Audit Fields**: Automatic tracking of created/updated timestamps and users
- **PostgreSQL Optimized**: Configured specifically for PostgreSQL features

## Running the Application

### Development Environment

1. **Start the API**
   ```bash
   dotnet run --project Confessly.API
   ```

   Or in Visual Studio: Set `Confessly.API` as the startup project and press F5.

2. **Access the application**
   - API: `https://localhost:7166` or `http://localhost:5166`
   - Swagger UI: `https://localhost:7166/swagger` (Development only)

### Production Environment

```bash
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
cd publish
dotnet Confessly.API.dll
```

## Project Structure

```
Confessly.API/
├── Confessly.API/                 # Web API controllers and middleware
│   ├── Controllers/               # API controllers
│   ├── Middlewares/              # Custom middleware
│   └── Program.cs                # Application entry point
├── Confessly.Domain/             # Core entities and domain logic
├── Confessly.Infrastructure/     # EF Core, authentication, external concerns
│   └── Database/                 # DbContext and configurations
├── Confessly.Repository/         # Repository pattern implementation
├── Confessly.Services/           # Business logic and validation
├── Confessly.Contracts/          # DTOs and shared interfaces
├── Confessly.Configuration/      # Configuration management
├── Confessly.Validators/         # Validation framework
├── Confessly.Messages/           # Exception and validation messages
└── Confessly.Logging/            # Comprehensive logging framework
```

## API Documentation

### Authentication Endpoints
- `POST /api/auth/login` - User login with username/password
- `POST /api/auth/register` - User registration

### User Management Endpoints
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user information
- `DELETE /api/users/{id}` - Soft delete user

### Response Format
All API responses follow a consistent envelope pattern:
```json
{
  "data": {},
  "message": "Success",
  "success": true,
  "traceId": "correlation-id-here",
  "timestamp": "2026-03-30T12:00:00Z"
}
```

### Error Handling
- **400 Bad Request**: Validation errors with detailed messages
- **401 Unauthorized**: Authentication required or failed
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Unexpected server errors

## Configuration

### Environment-Specific Settings

The application uses a layered configuration approach:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- Environment variables - Runtime overrides

### Key Configuration Sections

```json
{
  "ConnectionStrings": {
    "Default": "PostgreSQL connection string"
  },
  "Jwt": {
    "Secret": "JWT signing secret (256-bit minimum)",
    "Issuer": "Token issuer",
    "Audience": "Token audience",
    "ExpirationMinutes": 60
  },
  "Serilog": {
    // Structured logging configuration
  }
}
```

### Static Configuration Access
The project uses a static configuration singleton:
```csharp
var connectionString = ConfesslyConfiguration.ConnectionString("Default");
var jwtSecret = ConfesslyConfiguration.JWTSecret;
```

## Development Guidelines

### Code Style & Conventions
- **Entities**: Inherit from `BaseEntity` with time-ordered GUIDs
- **Services**: Inherit from `BaseServices` and follow repository pattern
- **Controllers**: Inherit from `ConfesslyBaseController`
- **Validation**: Use exception-based validation with domain validators
- **Logging**: Always use `ILoggingService` abstraction
- **Async Operations**: Include `CancellationToken` parameters

### Key Patterns
- **Repository Pattern**: `IRepository<TEntity>` with `UnitOfWork`
- **Domain Validation**: Extension methods on entities
- **Soft Deletes**: All delete operations support `includeDeleted` parameter
- **Correlation IDs**: Automatic request correlation tracking
- **Performance Tracking**: Built-in operation timing

### Testing
- Mock `IUnitOfWork` and service interfaces
- Test both success and validation scenarios
- Verify logging and correlation ID behavior
- Test soft delete functionality
- Integration tests for complete request pipeline

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow the established code conventions and patterns
4. Write tests for new functionality
5. Ensure all tests pass (`dotnet test`)
6. Update documentation as needed
7. Commit changes (`git commit -m 'Add amazing feature'`)
8. Push to branch (`git push origin feature/amazing-feature`)
9. Open a Pull Request

### Development Workflow
- Follow Clean Architecture principles
- Use established base classes and interfaces
- Include comprehensive logging and error handling
- Maintain soft delete support
- Add appropriate validation and tests

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.