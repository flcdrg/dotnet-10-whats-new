# Agent Instructions for dotnet-10-whats-new

## Running the Application

This project is a .NET 10.0 web application that demonstrates new features in .NET 10.

### Prerequisites
- .NET 10.0 SDK (version 10.0.101 or later) must be installed
- The SDK version is specified in `global.json`

### How to Run the Application

**IMPORTANT**: Always run commands from the `webapp` directory, not the root directory.

To launch the webapp correctly, use one of the following commands:

```powershell
# Navigate to the webapp directory first
cd webapp

# Then run the application
dotnet run
```

Or from the root directory:

```powershell
dotnet run --project webapp/webapp.csproj
```

### Expected Behavior
- The application will start and listen on:
  - HTTP: http://localhost:5152
  - HTTPS: https://localhost:7163
- A browser should launch automatically pointing to the application
- The application runs in Development mode by default

### Project Structure
- **Project File**: `webapp/webapp.csproj` - Main project file
- **Solution File**: `webapp/webapp.sln` - Solution file
- **Entry Point**: `webapp/Program.cs` - Application startup
- **Target Framework**: .NET 10.0 (`net10.0`)

### Key Dependencies
- Mediator.SourceGenerator (3.0.0)
- Mediator.Abstractions (3.0.0)
- Microsoft.EntityFrameworkCore.Sqlite (10.0.0)
- Microsoft.EntityFrameworkCore.Design (10.0.0)

### Common Issues and Solutions

1. **"Could not execute because the specified command or file was not found"**
   - Ensure you're in the `webapp` directory
   - Or use `dotnet run --project webapp/webapp.csproj` from the root

2. **SDK Version Mismatch**
   - Verify .NET 10.0 SDK is installed: `dotnet --list-sdks`
   - The project requires SDK version 10.0.101 or later

3. **Port Already in Use**
   - Change the port in `webapp/Properties/launchSettings.json`
   - Or specify a different port: `dotnet run --urls "http://localhost:5000"`

### Development Commands

```powershell
# Restore dependencies
dotnet restore webapp/webapp.csproj

# Build the project
dotnet build webapp/webapp.csproj

# Run with specific profile
dotnet run --project webapp/webapp.csproj --launch-profile https

# Clean build artifacts
dotnet clean webapp/webapp.csproj
```

## Agent Guidelines

When making changes to this project:

1. Always test that `dotnet run` works after modifications
2. Maintain the working directory context (run from `webapp` folder)
3. Preserve the launch settings in `Properties/launchSettings.json`
4. Keep the target framework as `net10.0` in the csproj file
5. Ensure any new dependencies are compatible with .NET 10.0
6. Don't use `StringComparison.OrdinalIgnoreCase` for code that is used by EF Core
