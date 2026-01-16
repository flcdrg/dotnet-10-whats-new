# Agent Instructions for adminapp

This is a .NET 9 Blazor Server application that serves as an intranet administration portal for the pet store accessories application.

## Project Overview

- **Framework**: .NET 9.0 (net9.0)
- **Type**: Blazor Server with Interactive Server Components
- **Purpose**: Internal administration and management interface
- **Ports**:
  - HTTP: <http://localhost:5196>
  - HTTPS: <https://localhost:7041>

## Technology Stack

- **UI**: Blazor Interactive Server Components
- **Rendering**: Server-side rendering with SignalR
- **Styling**: Bootstrap (included in wwwroot/lib/bootstrap)
- **Language**: C# 13 with nullable reference types enabled

## Project Structure

```plaintext
adminapp/
├── Components/
│   ├── Pages/          # Razor page components
│   │   ├── Counter.razor
│   │   ├── Home.razor
│   │   ├── Weather.razor
│   │   └── Error.razor
│   ├── Layout/         # Layout components
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── App.razor       # Root component
│   └── Routes.razor    # Routing configuration
├── wwwroot/            # Static assets
├── Properties/
│   └── launchSettings.json
├── appsettings.json    # Configuration
└── Program.cs          # Application startup
```

## Running the Application

```bash
cd adminapp
dotnet run
```

Or run with HTTPS:

```bash
cd adminapp
dotnet run --launch-profile https
```

## Development Guidelines

1. **Component Development**:
   - Place page components in `Components/Pages/`
   - Use `@page` directive to define routes
   - Use Interactive Server render mode for real-time updates

2. **Styling**:
   - Follow Bootstrap conventions
   - Custom styles in `wwwroot/app.css`
   - Component-specific styles use `.razor.css` files

3. **State Management**:
   - Use dependency injection for services
   - Server-side components maintain state automatically via SignalR

4. **Security**:
   - Intended for intranet use only
   - HSTS enabled in production
   - Antiforgery protection enabled

5. **Code Standards**:
   - Keep components focused and single-purpose
   - Use nullable reference types
   - Follow existing naming conventions
   - Leverage implicit usings

## Common Tasks

### Adding a New Page

1. Create a new `.razor` file in `Components/Pages/`
2. Add `@page "/route"` directive
3. Update `NavMenu.razor` if navigation link needed

### Adding a Service

1. Create service interface and implementation
2. Register in `Program.cs` using `builder.Services.Add*`
3. Inject into components via `@inject` directive

## Configuration

- `appsettings.json` - Production configuration
- `appsettings.Development.json` - Development overrides
- `launchSettings.json` - Launch profiles and port configuration

## Notes for AI Agents

- This is a Blazor Server app, not Blazor WebAssembly
- Uses Interactive Server Components (not WASM or Static SSR)
- SignalR connection required for interactivity
- All code runs server-side; no client-side C# compilation
- Follow Blazor component lifecycle (OnInitialized, OnParametersSet, etc.)
- Use `StateHasChanged()` when manually triggering UI updates
