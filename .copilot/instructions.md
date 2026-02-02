# Copilot Instructions for Confessly API Solution

## Project Overview

This is the **Confessly API** solution, a .NET 10 web API project following Clean Architecture principles with a focus on structured logging and data management. The solution consists of 5 projects organized in a layered architecture.

## Architecture & Project Structure

### Core Projects
- **Confessly.Domain**: Contains domain entities, database context, and core business models
- **Confessly.Repository**: Implements Repository and Unit of Work patterns for data access
- **Confessly.Contracts**: Contains interfaces and contracts for cross-cutting concerns
- **Confessly.Logging**: Comprehensive logging infrastructure using Serilog
- **Confessly.API**: Main Web API project with controllers and application entry point

### Key Architecture Patterns
1. **Repository Pattern**: All data access goes through repository abstractions
2. **Unit of Work Pattern**: Transaction management and coordinated repository operations
3. **Clean Architecture**: Clear separation of concerns with dependency inversion
4. **Structured Logging**: Comprehensive logging with correlation IDs and performance tracking

## Code Style & Conventions

### Naming Conventions
- **Classes**: PascalCase (e.g., `BaseEntity`, `UserRepository`)
- **Interfaces**: Start with 'I' (e.g., `IRepository<T>`, `ILoggingService`)
- **Methods**: PascalCase with descriptive names
- **Parameters**: camelCase
- **Constants**: PascalCase or UPPER_CASE for static readonly fields

### Entity Guidelines
- All entities inherit from `BaseEntity` with standard audit fields:
  - `Id`: Guid using `Guid.CreateVersion7()`
  - `IsDeleted`: Soft delete flag
  - `CreatedBy`, `CreatedTime`, `UpdatedBy`, `UpdatedTime`: Audit tracking
- Use soft deletes by default (`IsDeleted = true`)
- Include XML documentation for all public members

### Repository Pattern Guidelines
- All repositories implement `IRepository<TEntity>` interface
- Support for soft delete filtering with `includeDeleted` parameter
- Async operations with `CancellationToken` support
- Comprehensive CRUD operations with expression-based queries
- Use `EntityRepository` as base implementation

### Logging Guidelines
- Use the `ILoggingService` for all application logging
- Include correlation IDs automatically via `ICorrelationService`
- Use structured logging with meaningful context
- Performance logging available via `IPerformanceLogger`
- Request/response logging handled by middleware

## Development Guidelines

### When Creating New Features

1. **Domain Entities**
   - Inherit from `BaseEntity`
   - Include appropriate validation attributes
   - Add to `ConfesslyDbContext` as DbSet

2. **Repository Layer**
   - Create repository interface in `Confessly.Repository/Core`
   - Implement concrete repository inheriting from `BaseRepository<T>`
   - Register in dependency injection container

3. **API Controllers**
   - Use constructor injection for dependencies
   - Include proper HTTP status codes and responses
   - Add appropriate logging for operations
   - Handle cancellation tokens for async operations

4. **Logging Integration**
   - Use `ILoggingService` for application logs
   - Wrap expensive operations with performance logging
   - Add structured properties for context

### Code Quality Standards
- **Always** use async/await for database operations
- **Always** include CancellationToken parameters for async methods
- **Always** include proper error handling and logging
- **Never** expose internal implementation details in public APIs
- **Prefer** composition over inheritance where appropriate

### Testing Considerations
- Mock repositories and services in unit tests
- Test both success and failure scenarios
- Verify logging behavior in integration tests
- Test soft delete functionality explicitly

## Common Patterns & Examples

### Repository Usage
```csharp
// Getting entities with custom query
var users = await _userRepository.Get(q => 
    q.Where(u => u.IsActive)
     .OrderBy(u => u.CreatedTime)
     .Take(10));

// Soft delete
await _userRepository.Delete(user, isSoftDeleted: true);
```

### Logging Usage
```csharp
// Structured logging with context
_logger.LogInformation("User {UserId} performed {Action}", userId, action);

// Performance logging
using var perf = _performanceLogger.StartOperation("UserCreation");
// ... expensive operation
```

### Controller Pattern
```csharp
[HttpPost]
public async Task<IActionResult> CreateUser(CreateUserRequest request, CancellationToken cancellationToken)
{
    try
    {
        using var scope = _logger.BeginScope("CreateUser", request.Id?.ToString());
        
        // Implementation
        var user = await _userService.CreateAsync(request, cancellationToken);
        
        _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
        return Ok(user);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to create user");
        return StatusCode(500, "An error occurred while creating the user");
    }
}
```

## Dependencies & Packages

### Key NuGet Packages
- **Serilog**: Structured logging framework
- **Entity Framework Core**: ORM for database operations
- **Microsoft.AspNetCore**: Web API framework

### Project Dependencies
- Domain layer has minimal external dependencies
- Repository depends on Domain
- Contracts provides cross-cutting interfaces
- API orchestrates all layers
- Logging provides infrastructure services

## Environment Configuration

### Development Setup
- Uses Serilog for structured logging
- Request logging middleware enabled
- Swagger UI available in development
- HTTPS redirection enabled

### Logging Configuration
- Console and file sinks configured
- Correlation ID tracking across requests
- Performance metrics collection
- Exception logging with full context

## Best Practices for AI Assistance

When generating or modifying code:

1. **Follow the established patterns** in the existing codebase
2. **Include proper async/await** usage with CancellationTokens
3. **Add comprehensive logging** for new operations
4. **Inherit from base classes** (BaseEntity, BaseRepository) where appropriate
5. **Include XML documentation** for public APIs
6. **Handle soft deletes** appropriately in queries
7. **Use dependency injection** rather than direct instantiation
8. **Include proper error handling** and meaningful error messages
9. **Follow the repository pattern** for data access
10. **Maintain separation of concerns** across layers

## File Naming Conventions
- Controllers: `{EntityName}Controller.cs`
- Entities: `{EntityName}.cs` in Domain project
- Repositories: `{EntityName}Repository.cs` and `I{EntityName}Repository.cs`
- Services: `{ServiceName}Service.cs` and `I{ServiceName}.cs`
- Extensions: `{Purpose}Extensions.cs`

Remember: This is a professional-grade API with emphasis on maintainability, testability, and comprehensive logging. Always consider the impact on the overall architecture when making changes.