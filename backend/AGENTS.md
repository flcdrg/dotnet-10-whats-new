# Agent Instructions for backend

This is a .NET 9 Minimal API application that provides REST API endpoints for the pet store accessories web application.

## Project Overview

- **Framework**: .NET 9.0 (net9.0)
- **Type**: ASP.NET Core Minimal API
- **Purpose**: Backend REST API for webapp and adminapp
- **Ports**:
  - HTTP: <http://localhost:5148>
  - HTTPS: <https://localhost:7087>

## Technology Stack

- **API Style**: Minimal APIs (Program.cs only, no controllers)
- **Language**: C# 13 with nullable reference types enabled
- **Configuration**: appsettings.json with environment-specific overrides
- **Logging**: Built-in ASP.NET Core logging

## Project Structure

```plaintext
backend/
├── Program.cs               # API endpoints and app configuration
├── appsettings.json         # Production configuration
├── appsettings.Development.json  # Development overrides
├── Demo.Api.csproj          # Project file
└── Properties/
    └── launchSettings.json  # Launch profiles and port config
```

## Running the Application

```bash
cd backend
dotnet run
```

Or run with HTTPS:

```bash
cd backend
dotnet run --launch-profile https
```

## Development Guidelines

1. **API Endpoints**:
   - Define all endpoints in `Program.cs` using `app.MapGet()`, `app.MapPost()`, etc.
   - Use route groups for related endpoints: `var api = app.MapGroup("/api")`
   - Apply endpoint filters for cross-cutting concerns

2. **CORS Configuration**:
   - Must allow webapp origin: <http://localhost:5173>
   - Configure in `Program.cs` before `app.Build()`
   - Example: `builder.Services.AddCors(options => ...)`

3. **Request/Response**:
   - Use typed parameters for request binding
   - Return `Results` types (e.g., `Results.Ok()`, `Results.NotFound()`)
   - Use `IResult` return type for endpoints

4. **Validation**:
   - Use Data Annotations or FluentValidation
   - Apply endpoint filters for validation logic

5. **Error Handling**:
   - Use exception handling middleware in production
   - Return appropriate HTTP status codes
   - Include problem details for errors

6. **Database Access** (if applicable):
   - Use Entity Framework Core for data access
   - Register DbContext in `Program.cs`
   - **IMPORTANT**: Don't use `StringComparison.OrdinalIgnoreCase` in EF Core queries (causes runtime errors)
   - Use `.ToLowerInvariant()` for case-insensitive string comparisons in queries

## Common Patterns

### Adding a New Endpoint

```csharp
app.MapGet("/api/products", async () => 
{
    // Implementation
    return Results.Ok(products);
});
```

### Using Route Groups

```csharp
var products = app.MapGroup("/api/products");
products.MapGet("/", GetAllProducts);
products.MapGet("/{id}", GetProductById);
products.MapPost("/", CreateProduct);
```

### Adding Endpoint Filters

```csharp
app.MapPost("/api/products", CreateProduct)
   .AddEndpointFilter<ValidationFilter>();
```

## Configuration

- **appsettings.json**: Production settings (logging, allowed hosts)
- **appsettings.Development.json**: Development-specific overrides
- **launchSettings.json**: Launch profiles, ports, environment variables

## API Clients

- **webapp**: React application consuming this API (<http://localhost:5173>)
- **adminapp**: Blazor admin app (may consume some endpoints)

## Notes for AI Agents

- This uses Minimal APIs, not MVC Controllers
- All endpoint definitions are in `Program.cs`
- Use C# 13 features where applicable
- Implicit usings are enabled
- Keep endpoints focused and single-purpose
- Consider using separate handler classes for complex logic
- Ensure CORS is configured when webapp makes requests
- Test endpoints using browser, Postman, or HTTP files
