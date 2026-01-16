# Agent Instructions for function

This is a .NET 9 Azure Functions application for background processing and serverless compute tasks in the pet store accessories application.

## Project Overview

- **Framework**: .NET 9.0 (net9.0)
- **Type**: Azure Functions v4 with Isolated Worker Process
- **Purpose**: Background jobs, scheduled tasks, event processing
- **Port**: <http://localhost:7222> (local development)
- **Runtime**: dotnet-isolated worker

## Technology Stack

- **Azure Functions v4**: Latest Azure Functions runtime
- **Isolated Worker Process**: Runs in a separate process from the Functions host
- **Application Insights**: Telemetry and monitoring integration
- **ASP.NET Core Integration**: HTTP extensions for ASP.NET Core features
- **Language**: C# 13 with nullable reference types enabled

## Project Structure

```plaintext
function/
├── Program.cs               # Function host configuration
├── function.csproj          # Project file with Azure Functions packages
├── host.json                # Functions runtime configuration
├── local.settings.json      # Local development settings (not in source control)
├── .gitignore               # Excludes local.settings.json and bin/obj
└── Properties/
    └── launchSettings.json  # Launch profile with port 7222
```

## Running the Application

```bash
cd function
dotnet run
```

Or use Azure Functions Core Tools:

```bash
cd function
func start
```

The function app will start on <http://localhost:7222>

## Development Guidelines

1. **Function Types**:
   - **HTTP Triggers**: REST endpoints for synchronous operations
   - **Timer Triggers**: Scheduled background jobs (cron expressions)
   - **Queue Triggers**: Process messages from Azure Storage Queues
   - **Blob Triggers**: React to file uploads/changes
   - **Event Grid/Hub Triggers**: Handle event-driven workflows

2. **Creating Functions**:
   - Create separate .cs files for each function or group of related functions
   - Use `[Function("FunctionName")]` attribute
   - Use appropriate trigger attributes (`HttpTrigger`, `TimerTrigger`, etc.)

3. **Dependency Injection**:
   - Register services in `Program.cs` using `builder.Services`
   - Inject dependencies via constructor parameters
   - Use standard ASP.NET Core DI patterns

4. **Configuration**:
   - Local settings in `local.settings.json` (not committed)
   - Access via `IConfiguration` or `Environment.GetEnvironmentVariable()`
   - Azure settings configured in Function App settings

5. **Logging**:
   - Inject `ILogger<T>` for structured logging
   - Application Insights automatically collects telemetry
   - Sampling configured in `host.json`

6. **Isolated Worker Process**:
   - Runs in separate process from Functions host
   - Better performance and fewer version conflicts
   - Use `Microsoft.Azure.Functions.Worker` packages (not `Microsoft.NET.Sdk.Functions`)

## Common Patterns

### HTTP Triggered Function

```csharp
public class HttpFunctions
{
    [Function("GetData")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "data")] HttpRequest req,
        FunctionContext context)
    {
        var logger = context.GetLogger<HttpFunctions>();
        logger.LogInformation("Processing request");
        
        return new OkObjectResult(new { message = "Hello" });
    }
}
```

### Timer Triggered Function

```csharp
public class ScheduledFunctions
{
    [Function("DailyCleanup")]
    public void Run(
        [TimerTrigger("0 0 2 * * *")] TimerInfo timer,
        FunctionContext context)
    {
        var logger = context.GetLogger<ScheduledFunctions>();
        logger.LogInformation("Running daily cleanup");
    }
}
```

### Queue Triggered Function

```csharp
public class QueueFunctions
{
    [Function("ProcessOrder")]
    public void Run(
        [QueueTrigger("orders")] string message,
        FunctionContext context)
    {
        var logger = context.GetLogger<QueueFunctions>();
        logger.LogInformation($"Processing: {message}");
    }
}
```

## Configuration Files

- **host.json**: Runtime settings, logging, Application Insights sampling
- **local.settings.json**: Local development only (gitignored)
  - `AzureWebJobsStorage`: Connection string for storage account
  - `FUNCTIONS_WORKER_RUNTIME`: Set to "dotnet-isolated"
- **launchSettings.json**: Port configuration (7222) and startup settings

## Local Development

1. **Azure Storage Emulator**: Set `AzureWebJobsStorage` to `UseDevelopmentStorage=true`
2. **Azurite**: Modern storage emulator (recommended)
3. **Local Debugging**: Full debugging support in VS Code/Visual Studio

## Integration Points

- **backend**: May call function endpoints or trigger queue processing
- **webapp**: Can invoke HTTP-triggered functions
- **Azure Services**: Can integrate with Storage, Event Grid, Service Bus, etc.

## Notes for AI Agents

- This uses **Isolated Worker Process**, not In-Process model
- Package references should be `Microsoft.Azure.Functions.Worker.*`
- Do NOT use `Microsoft.NET.Sdk.Functions` SDK (that's for in-process)
- Function signatures differ from in-process functions
- Use `FunctionContext` instead of `ExecutionContext`
- HTTP functions use ASP.NET Core types (`HttpRequest`, `IActionResult`)
- Application Insights is pre-configured for telemetry
- Follow Azure Functions best practices for stateless, idempotent operations
