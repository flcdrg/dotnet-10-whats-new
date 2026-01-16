# Agent Instructions for dotnet-10-whats-new

## Repository Structure

- `backend` - .NET 9 Minimal API application
- `function` - .NET 9 Azure Function application
- `webapp` - Vite React web app
- `adminapp` - .NET 9 Blazor application

### Prerequisites

- .NET 9.0 SDK (version 9.0.309 or later) must be installed
- The SDK version is specified in `global.json`

## Agent Guidelines

When making changes to this project:

1. Always test that `dotnet run` works after modifications
4. Keep the target framework as `net9.0` in the csproj file
5. Ensure any new dependencies are compatible with .NET 9.0
6. Don't use `StringComparison.OrdinalIgnoreCase` for code that is used by EF Core
