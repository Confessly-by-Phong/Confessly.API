# Copilot Instructions for Confessly API Solution

## Project Overview

This is the **Confessly API** solution, a .NET 10 web API project following Clean Architecture principles with comprehensive logging, authentication, and validation systems. The solution consists of 10 projects organized in a strict layered architecture with no circular dependencies.

## Architecture & Project Structure

### Core Projects
- **Confessly.Domain**: Core entities (`User`, `BaseEntity`) and domain interfaces (`IUser`) - no external dependencies
- **Confessly.Infrastructure**: EF Core `DbContext`, entity configurations, JWT/password authentication implementation (PostgreSQL)
- **Confessly.Repository**: Generic repository pattern with `UnitOfWork`, soft delete support, and comprehensive logging
- **Confessly.Services**: Business logic layer orchestrating repository + infrastructure with domain validators
- **Confessly.API**: Web API controllers, middleware, dependency injection setup, and application entry point

### Supporting Projects
- **Confessly.Contracts**: DTOs, request/response models, and shared interfaces across layers
- **Confessly.Configuration**: Static singleton pattern for accessing configuration values (JWT, connection strings)
- **Confessly.Validators**: Validation infrastructure with `ConfesslyValidationException` and extension methods
- **Confessly.Messages**: String constants for exception and validation messages (no logic)
- **Confessly.Logging**: Comprehensive Serilog-based logging with correlation IDs, performance tracking, and middleware

### Key Architecture Patterns
1. **Clean Layered Architecture**: Strict dependency direction with no circular references
2. **Repository Pattern**: Generic `IRepository<TEntity>` with soft delete support and comprehensive logging
3. **Unit of Work Pattern**: Transaction management with typed repository access (`IUnitOfWork.Users`)
4. **Domain-Driven Design**: Domain entities with business rules, domain validators as extension methods
5. **Static Configuration**: Global configuration access via singleton pattern
6. **Exception-Based Validation**: Custom `ConfesslyValidationException` with structured messages
7. **Comprehensive Logging**: Serilog with correlation IDs, performance tracking, and request logging
8. **JWT Authentication**: Custom authentication service with bcrypt password hashing

### Project Dependencies & Architecture Flow
```
API → Services → Repository → Infrastructure → Domain
                                             ↓
                              Contracts (shared across layers)
                              Configuration (accessed by Infrastructure & API)
                              Logging (accessed by Repository, Services, API)
                              Validators / Messages (accessed by Services, Validators)
```

## Code Style & Conventions

### Naming Conventions
- **Classes**: PascalCase (e.g., `BaseEntity`, `UserRepository`, `ConfesslyDbContext`)
- **Interfaces**: Start with 'I' (e.g., `IRepository<T>`, `ILoggingService`, `IUserContext`)
- **Methods**: PascalCase with descriptive names
- **Private Fields**: `_camelCase` (e.g., `_work`, `_logger`, `_authenticator`)
- **Namespaces**: Match project names exactly (`Confessly.Domain`, `Confessly.Services`)
- **Extension Methods**: Heavily used for cross-cutting concerns (logging, validation, utilities)
- **Partial Classes**: Used for splitting implementations (`EntityRepository<T>`, `ConfesslyValidationMessages`)

### Key Base Classes & Interfaces

**Domain Layer:**
- `BaseEntity` (abstract): `Id` (Guid.CreateVersion7()), `IsDeleted`, audit fields (`CreatedBy`, `CreatedTime`, `UpdatedBy`, `UpdatedTime`)
- `IUser`: Domain interface with `Username`, `Password`, `Name` properties

**Repository Layer:**
- `IRepository<TEntity>`: Generic interface with overloaded `Get()` methods, `Insert`, `Delete` (soft/hard), `Update`
- `BaseRepository<TEntity>`: Abstract base with `Context`, `Table`, `Logger`, `EntityName` properties
- `EntityRepository<TEntity>`: Concrete partial class implementing all operations with logging
- `IUnitOfWork`/`UnitOfWork`: Exposes typed repositories (`IRepository<User> Users`) and `SaveChanges()`

**Service Layer:**
- `BaseServices`: Abstract base with `_work` (`IUnitOfWork`) and `_logger` (`ILoggingService`)
- Domain validators as static extension methods on entities (e.g., `user.Validate(repository, ct)`)

**Infrastructure:**
- `IConfesslyAuthentication`: `HashPassword`, `VerifyPassword`, `GenerateJwtToken`
- `BaseEntityTypeConfiguration<TEntity>`: Abstract EF configuration base
- `ConfesslyDbContext`: DbContext with automatic audit field population

**Logging:**
- `ILoggingService`: Application logging abstraction (wraps Serilog)
- `IPerformanceLogger`: Operation timing and performance tracking
- `ICorrelationService`: Request correlation ID management

**API:**
- `ConfesslyBaseController`: Base controller with `ApiOk<T>()` helper returning `ConfesslyResponse<T>`

### Entity & Database Guidelines
- All entities inherit from `BaseEntity` with time-ordered GUIDs (`Guid.CreateVersion7()`)
- **PostgreSQL** with Npgsql provider, EF Core 10.0
- Soft deletes by default (`IsDeleted = true`), with `includeDeleted` parameter support
- Audit fields automatically set in `DbContext.SaveChanges()` using `IUserContext`
- Entity configurations via `IEntityTypeConfiguration<T>` and fluent API
- Migrations stored in `Confessly.Infrastructure/Database/Migrations/`

### Authentication & Security Guidelines
- Password hashing via `Microsoft.AspNetCore.Identity.PasswordHasher<IUser>` (bcrypt-based)
- JWT tokens with `HmacSha256Signature`, claims include user ID, name, jti, iat
- Configuration accessed via `ConfesslyConfiguration` static singleton
- Current user context via `IUserContext`/`UserContext` from HTTP context
- Claims extraction via `ClaimsPrincipalExtensions.GetUserId()`

### Validation Guidelines
- Exception-based validation: throw `ConfesslyValidationException` (no FluentValidation/DataAnnotations)
- Domain validators as static extension methods in `Confessly.Services/Validation/`
- `StringValidator.StringValidate()` extension for common string validation
- `ExceptionHandlingMiddleware` catches validation exceptions → HTTP 400
- Messages from `ConfesslyValidationMessages` (partial static class split by domain)

### Logging Guidelines
- **Always** use `ILoggingService` abstraction, never `ILogger<T>` directly
- Correlation IDs automatically included via `ICorrelationService`
- Performance tracking with `IPerformanceLogger.TrackOperation()` (disposable timers)
- Request logging via `RequestLoggingMiddleware` (start/completion with status codes)
- Repository exception logging with `LogRepositoryException` extension
- Logging services registered as Singleton lifetime

## Development Guidelines

### When Creating New Features

1. **Domain Entities**
   - Inherit from `BaseEntity` with time-ordered GUID (`Guid.CreateVersion7()`)
   - Implement domain interface if needed (like `IUser`)
   - Add entity configuration class inheriting from `BaseEntityTypeConfiguration<T>`
   - Add to `ConfesslyDbContext` as `DbSet<TEntity>`
   - Create and run EF Core migration

2. **Repository Layer**
   - Repository interface inherits from `IRepository<TEntity>`
   - Concrete repository inherits from `EntityRepository<TEntity>`
   - Add typed repository property to `IUnitOfWork`/`UnitOfWork`
   - All operations use structured logging and soft delete filtering

3. **Service Layer**
   - Service interface and implementation with constructor injection
   - Inherit from `BaseServices` for `_work` and `_logger` access
   - Create domain validator extension methods in `Validation/` folder
   - Register services in DI container with appropriate lifetime

4. **API Controllers**
   - Inherit from `ConfesslyBaseController` for `ApiOk<T>()` helper
   - Use constructor injection (prefer interfaces over concrete classes)
   - Include `CancellationToken` parameters for async operations
   - Handle validation exceptions via global exception middleware
   - Use structured logging with meaningful context

5. **Messages & Validation**
   - Add message constants to `ConfesslyValidationMessages` (use partial classes)
   - Throw `ConfesslyValidationException` for validation errors
   - Use `StringValidator.StringValidate()` for common validations
   - Exception messages automatically formatted with `ConfesslyExceptionMessages.ValidationFailed`

### Configuration Management
- **Static Singleton**: `ConfesslyConfiguration.Initialize(configuration)` in `Program.cs`
- **Global Access**: Use static properties (`ConfesslyConfiguration.JWTSecret`, `ConnectionString(key)`)
- **Environment Specific**: `appsettings.json` base + `appsettings.Development.json` overrides
- **JWT Config**: Secret, Issuer, Audience, ExpirationMinutes from `Jwt` section
- **Database**: PostgreSQL connection strings by environment

### Code Quality Standards
- **Always** use async/await for database operations with `CancellationToken`
- **Always** inject interfaces rather than concrete classes (except where noted)
- **Always** use `ILoggingService` abstraction instead of `ILogger<T>`
- **Always** handle soft deletes with `includeDeleted` parameters
- **Always** use time-ordered GUIDs (`Guid.CreateVersion7()`) for entity IDs
- **Never** expose internal implementation details in public APIs
- **Never** bypass the repository pattern for data access
- **Never** access configuration directly - use `ConfesslyConfiguration` static singleton
- **Prefer** extension methods for cross-cutting concerns (validation, logging, utilities)
- **Prefer** partial classes when splitting functionality across files
- **Prefer** exception-based validation over attribute-based validation

### Testing Considerations
- Mock `IUnitOfWork` and service interfaces in unit tests
- Test both success and validation failure scenarios
- Verify logging behavior and correlation ID propagation
- Test soft delete functionality explicitly with `includeDeleted` parameter
- Test JWT authentication and claims extraction
- Integration tests should verify complete request/response pipeline

## Common Patterns & Examples

### Repository Usage
```csharp
// Getting entities with filtering and soft delete handling
var users = await _work.Users.Get(q => 
    q.Where(u => u.Name.Contains("John"))
     .OrderBy(u => u.CreatedTime), 
    includeDeleted: false, // Default soft delete filtering
    cancellationToken);

// Getting single entity by ID
var user = await _work.Users.Get(userId, cancellationToken);

// Soft delete (default)
await _work.Users.Delete(user, cancellationToken);

// Hard delete
await _work.Users.Delete(user, cancellationToken, isSoftDeleted: false);

// Unit of work pattern
var newUser = new User { Username = "john", ... };
await _work.Users.Insert(newUser, cancellationToken);
await _work.SaveChanges(cancellationToken); // Saves all changes with audit fields
```

### Validation Pattern
```csharp
// Domain validator extension method
public static async Task Validate(this User user, IUnitOfWork work, CancellationToken cancellationToken)
{
    // String validation with extension
    user.Username.StringValidate(
        allowEmpty: false, 
        minLength: 3, 
        maxLength: 50, 
        propertyName: nameof(user.Username));
    
    // Domain-specific validation
    var existingUser = await work.Users.Get(u => u.Username == user.Username, 
        includeDeleted: false, cancellationToken);
    if (existingUser.Any())
        throw new ConfesslyValidationException(
            ConfesslyValidationMessages.UsernameAlreadyTakenMessage(user.Username));
}

// Usage in service
await user.Validate(_work, cancellationToken);
```

### Logging Usage
```csharp
// Structured logging with correlation ID (automatic)
_logger.LogInformation("User {UserId} performed {Action} at {Timestamp}", 
    userId, "Login", DateTime.UtcNow);

// Performance tracking with disposable timer
using var perfTracker = _performanceLogger.TrackOperation("UserCreation");
// ... expensive operation
// Automatically logs duration on disposal

// Scoped logging context
using var scope = _logger.BeginScope("CreateUser", correlationId);
_logger.LogInformation("Starting user creation");
```

### Controller Pattern
```csharp
[HttpPost]
public async Task<ActionResult<ConfesslyResponse<User>>> CreateUser(
    UserCreate request, CancellationToken cancellationToken)
{
    using var scope = _logger.BeginScope("CreateUser", request.Username);
    
    try
    {
        var user = await _userServices.CreateUserAsync(request, cancellationToken);
        _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
        
        return ApiOk(user); // Returns ConfesslyResponse<T>.Ok() with trace ID
    }
    catch (ConfesslyValidationException ex)
    {
        // Handled by ExceptionHandlingMiddleware -> HTTP 400
        _logger.LogWarning(ex, "Validation failed for user creation");
        throw; // Re-throw for middleware handling
    }
    // Other exceptions -> HTTP 500 via middleware
}
```

### Configuration Usage
```csharp
// Static configuration access (initialized in Program.cs)
var connectionString = ConfesslyConfiguration.ConnectionString("Default");
var jwtSecret = ConfesslyConfiguration.JWTSecret;
var jwtExpiry = ConfesslyConfiguration.JWTExpirationMinutes;
```

## Dependencies & Packages

### Key NuGet Packages
- **Entity Framework Core 10.0**: ORM for PostgreSQL database operations
- **Npgsql.EntityFrameworkCore.PostgreSQL**: PostgreSQL provider for EF Core
- **Serilog 4.3 + Serilog.AspNetCore 10.0**: Structured logging framework
- **Serilog.Sinks.ApplicationInsights**: Application Insights logging sink
- **Microsoft.AspNetCore.Identity**: Password hashing utilities (bcrypt-based)
- **System.IdentityModel.Tokens.Jwt**: JWT token generation and validation
- **Microsoft.AspNetCore**: Web API framework (.NET 10)

### Project Dependencies Architecture
```
Confessly.API → (All projects)
Confessly.Services → Messages, Repository, Validators
Confessly.Repository → Domain, Infrastructure, Logging
Confessly.Infrastructure → Configuration, Contracts, Domain + EF/JWT packages
Confessly.Contracts → Domain
Confessly.Validators → Messages
Confessly.Logging → Serilog packages only
Confessly.Domain, Messages, Configuration → No project references
```

## Environment Configuration

### Development Setup
- **Database**: PostgreSQL with Npgsql provider
- **Logging**: Serilog with console sink and custom output template
- **Authentication**: JWT with bcrypt password hashing
- **Request Logging**: Middleware logs all requests with correlation IDs
- **Performance Tracking**: Built-in operation timing and database performance logging
- **Swagger UI**: Available in development environment
- **HTTPS Redirection**: Enabled for all environments

### Configuration Structure
- **appsettings.json**: Base configuration with Serilog setup and JWT placeholders
- **appsettings.Development.json**: Development-specific overrides (connection strings, JWT secrets, extended expiry)
- **Static Config Access**: `ConfesslyConfiguration` singleton initialized in `Program.cs`
- **Environment Variables**: Can override JSON settings via standard ASP.NET Core configuration

### Logging Configuration
- **Enrichers**: Machine name, environment, process ID, thread ID, correlation ID
- **Correlation Tracking**: Automatic correlation ID generation and propagation
- **Performance Metrics**: Operation duration tracking with automatic logging
- **Exception Handling**: Global exception middleware with structured logging
- **Request Logging**: Start/completion logging with HTTP status codes and remote IP

## Best Practices for AI Assistance

When generating or modifying code:

1. **Follow established patterns** in the existing codebase (repository, validation, logging)
2. **Use time-ordered GUIDs** (`Guid.CreateVersion7()`) for all entity IDs
3. **Include comprehensive logging** with `ILoggingService` and structured properties
4. **Inherit from base classes** (`BaseEntity`, `EntityRepository<T>`, `BaseServices`, `ConfesslyBaseController`)
5. **Use partial classes** when functionality spans multiple files (like `ConfesslyValidationMessages`)
6. **Handle soft deletes** with `includeDeleted` parameters in all repository queries
7. **Include cancellation tokens** for all async database operations
8. **Use dependency injection** with interface abstractions (prefer interfaces over concrete classes)
9. **Access configuration** via `ConfesslyConfiguration` static singleton, never inject `IConfiguration`
10. **Implement domain validation** as extension methods in `Confessly.Services/Validation/`
11. **Use exception-based validation** with `ConfesslyValidationException` and message constants
12. **Include correlation IDs** automatically via logging infrastructure
13. **Follow PostgreSQL conventions** for database design and migrations
14. **Maintain clean architecture** with strict dependency direction enforcement
15. **Use performance tracking** for expensive operations with `IPerformanceLogger`

## File Naming Conventions
- **Controllers**: `{EntityName}Controller.cs` (e.g., `UserController.cs`, `AuthenticationController.cs`)
- **Entities**: `{EntityName}.cs` in Domain project (e.g., `User.cs`)
- **Repository Interfaces**: `IRepository<TEntity>` (generic) + specific interfaces when needed
- **Repository Implementations**: Use generic `EntityRepository<TEntity>`
- **Services**: `{EntityName}Services.cs` and `I{EntityName}Services.cs` interfaces
- **Base Classes**: Prefix with `Base` (e.g., `BaseEntity`, `BaseServices`, `BaseRepository<T>`)
- **Extensions**: `{Purpose}Extensions.cs` (e.g., `ClaimsPrincipalExtensions.cs`, `ServiceCollectionExtensions.cs`)
- **Validators**: Domain validators as `{Entity}Validator.cs` with extension methods
- **Configurations**: `{Entity}EntityTypeConfiguration.cs` for EF Core configurations
- **Messages**: `Confessly{Type}Messages.cs` with partial classes by domain (e.g., `ConfesslyValidationMessages.User.cs`)
- **Middleware**: `{Purpose}Middleware.cs` (e.g., `ExceptionHandlingMiddleware.cs`, `RequestLoggingMiddleware.cs`)

## Notable Implementation Patterns

### Inconsistencies to be Aware Of
- `UserController` injects concrete `UserServices` rather than `IUserServices` interface
- Some controllers use `readonly` fields while others use `private` fields for DI
- `RequestLoggingMiddleware` has commented-out performance tracking code

### Project-Specific Conventions
- All projects target **.NET 10.0**
- Nullable reference types enabled across all projects
- `global.json` pins SDK version for consistent builds
- EF Core migrations stored in `Confessly.Infrastructure/Database/Migrations/`
- Configuration accessed via static singleton rather than DI pattern
- Response envelope pattern: `ConfesslyResponse<T>` for all API responses
- Trace IDs included in all API responses for request correlation

Remember: This is a production-ready API with emphasis on clean architecture, comprehensive logging, security, and maintainability. Always consider the impact on the overall architecture and established patterns when making changes.