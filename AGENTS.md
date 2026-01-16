# Agent Instructions for dotnet-10-whats-new

This is a pet store accessories web application with a number of different components.

**Important**: Ignore the repository name. All .NET projects are based on .NET 9.0

## Repository Structure

- `backend` - .NET 9 Minimal API application (see [backend/AGENTS.md](backend/AGENTS.md))
  - Runs on: <http://localhost:5148> (HTTP) or <https://localhost:7087> (HTTPS)
- `function` - .NET 9 Azure Function application (see [function/AGENTS.md](function/AGENTS.md))
  - Runs on: <http://localhost:7222>
- `webapp` - Vite React 19 web application with TypeScript (see [webapp/AGENTS.md](webapp/AGENTS.md))
  - Runs on: <http://localhost:5173> (default Vite port)
  - API base URL: <http://localhost:5148>
- `adminapp` - .NET 9 Blazor intranet administration application (see [adminapp/AGENTS.md](adminapp/AGENTS.md))
  - Runs on: <http://localhost:5196> (HTTP) or <https://localhost:7041> (HTTPS)

## Prerequisites

- .NET 9.0 SDK (version 9.0.309 or later) must be installed
  - The SDK version is specified in `global.json`
- Node.js 24.x (for webapp)
- pnpm 10.28.0+ (specified in webapp/package.json)

## Getting Started

### Build All Projects

```bash
dotnet build demo.sln
```

### Run Individual Projects

Do not use bash commands when running on Windows or in PowerShell.

**Backend API:**

```bash
cd backend
dotnet run
```

**Admin App:**

```bash
cd adminapp
dotnet run
```

**Azure Function:**

```bash
cd function
dotnet run
```

**Web App:**

```bash
cd webapp
pnpm install
pnpm dev
```

## Agent Guidelines

When making changes to this project:

1. **Testing**: Always verify that `dotnet run` works after modifications
2. **Target Framework**: Keep the target framework as `net9.0` in .csproj files
3. **Dependencies**: Ensure any new dependencies are compatible with .NET 9.0
4. **EF Core**: Don't use `StringComparison.OrdinalIgnoreCase` for code that is used by EF Core queries (causes runtime errors)
5. **Code Style**: Follow existing code conventions in each project
6. **Documentation**: Update relevant AGENTS.md files when making architectural changes. All Markdownlint warnings should be resolved.
7. **Port Configuration**: Maintain consistent ports defined in launchSettings.json files

## Common Patterns

- All .NET projects use C# 13 features where applicable
- Minimal APIs use endpoint filters and route groups
- Configuration is managed through `appsettings.json` and environment variables
- React webapp uses React 19 with TypeScript 5.9

## Architecture

- **Backend**: Provides REST API endpoints for the web application
- **Function**: Azure Functions for background processing
- **Webapp**: User-facing React application that consumes the backend API
- **Adminapp**: Internal Blazor application for administrative tasks

## Troubleshooting

- **Port conflicts**: If services fail to start, check if the configured ports are already in use
- **CORS issues**: Ensure backend CORS configuration includes webapp origin (<http://localhost:5173>)
- **Node version**: Webapp requires Node 24.x as specified in the AGENTS.md
