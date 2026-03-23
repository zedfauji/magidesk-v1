# Technology Stack

**Analysis Date:** 2026-03-23

## Languages

**Primary:**
- C# 8.0+ - Windows desktop and backend API development
  - Used in all projects: `Magidesk.Domain`, `Magidesk.Application`, `Magidesk.Infrastructure`, `Magidesk.Presentation`, `Magidesk.Api`

## Runtime

**Environment:**
- .NET 8.0 - Cross-platform framework for all backend services and API
- .NET 8.0-windows10.0.19041.0 - Windows-specific runtime for desktop client (`Magidesk.Presentation`)

**Windows Platform:**
- Minimum Windows 10 (Build 17763)
- Target Windows 10+ (Build 19041)

## Frameworks

**Core Application:**
- WinUI 3 (Windows App SDK 1.6.240829007) - Desktop UI framework for `Magidesk.Presentation`
- ASP.NET Core - Backend API framework in `Magidesk.Api`

**Architecture & DI:**
- MediatR 12.2.0 - CQRS pattern implementation for commands/queries
- Microsoft.Extensions.DependencyInjection 8.0.0 - Dependency injection
- Microsoft.Extensions.Hosting 8.0.0 - Host configuration
- FluentValidation 12.1.1 - Input validation with DI extensions

**Data Access:**
- Entity Framework Core 8.0.0 - ORM framework
- Npgsql.EntityFrameworkCore.PostgreSQL 8.0.0 - PostgreSQL database provider
- Microsoft.EntityFrameworkCore.Design 8.0.0/8.0.11 - Design-time tools

**UI & MVVM:**
- CommunityToolkit.Mvvm 8.4.0 - MVVM pattern helpers for WinUI 3

**Real-time Communication:**
- Microsoft.AspNetCore.SignalR.Client 10.0.2 - WebSocket client for kitchen notifications
- SignalR (server-side in API) - Real-time pub/sub for kitchen order publishing

**Templating:**
- Fluid.Core 2.31.0 - Liquid template engine for receipt/report printing

**GitHub Integration:**
- Octokit 13.0.1 - GitHub API client for automated updates

**Testing:**
- xUnit (inferred from test project structure)
- Test project naming: `*.Tests` projects

## Key Dependencies

**Critical:**
- Entity Framework Core 8.0.0 - Database persistence across all layers via `ApplicationDbContext` at `src/Magidesk.Infrastructure/Data/ApplicationDbContext.cs`
- Npgsql 8.0.0+ - PostgreSQL connectivity, connection string example: `Host=localhost;Database=Magidesk_Dev;Username=postgres;Password=password`
- MediatR 12.2.0 - Command/query handling pipeline (required for Application layer architecture)
- Octokit 13.0.1 - GitHub release checking and installer downloads for auto-updates

**Infrastructure:**
- System.Drawing.Common 8.0.0 - Graphics operations for printing
- System.Security.Cryptography.ProtectedData 8.0.0 - Data protection API for sensitive configuration
- Microsoft.Web.WebView2 1.0.2792.45 - Embedded browser control for WinUI 3
- Swashbuckle.AspNetCore 10.1.0 - Swagger/OpenAPI documentation for API
- System.Collections.Immutable 8.0.0 - Immutable collections
- Microsoft.Windows.AppSDK 1.6.240829007 - Windows App SDK runtime components

## Configuration

**Environment:**
- Configuration via `appsettings.json` and environment-specific overrides (`appsettings.Development.json`)
- Connection strings: `DefaultConnection` (API), `MagideskContext` (Desktop)
- JWT configuration: Issuer, Audience, Key stored in appsettings
- Database connection: PostgreSQL with environment variables

**Build:**
- Project files: `*.csproj` (MSBuild format)
- Target frameworks: `net8.0` (standard) and `net8.0-windows10.0.19041.0` (desktop)
- Solution file: `src/Magidesk.sln`

**Desktop Configuration:**
- Located: `src/Magidesk.Presentation/Configuration/appsettings.defaults.json`
- Terminal configuration: `TerminalId`, `Language`, `Theme`
- API Base URL: `Kds.ApiBaseUrl`
- Update settings: GitHub repository configuration (`RepositoryOwner`, `RepositoryName`, check interval)

## Platform Requirements

**Development:**
- .NET 8.0 SDK
- Entity Framework Core CLI tools
- Visual Studio 2022 (Version 17.0+) for WinUI 3 support
- Windows 10+ with developer mode enabled

**Production:**
- Windows 10 Build 19041 or later
- .NET 8.0 Runtime (included in MSIX package)
- PostgreSQL 12+ database
- Network access to GitHub for auto-updates

## NuGet Dependencies Summary

| Package | Version | Purpose |
|---------|---------|---------|
| MediatR | 12.2.0 | CQRS pattern |
| FluentValidation.DependencyInjectionExtensions | 12.1.1 | Input validation |
| Microsoft.EntityFrameworkCore | 8.0.0 | ORM |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.0.0 | PostgreSQL provider |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM utilities |
| Microsoft.AspNetCore.SignalR.Client | 10.0.2 | Real-time communication |
| Octokit | 13.0.1 | GitHub API |
| Fluid.Core | 2.31.0 | Template engine |
| Swashbuckle.AspNetCore | 10.1.0 | Swagger/OpenAPI |
| Microsoft.WindowsAppSDK | 1.6.240829007 | WinUI 3 runtime |

---

*Stack analysis: 2026-03-23*
