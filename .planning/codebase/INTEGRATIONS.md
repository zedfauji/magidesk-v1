# External Integrations

**Analysis Date:** 2026-03-23

## APIs & External Services

**GitHub API:**
- Service: GitHub Releases
  - What it's used for: Automated update checking and installer downloads
  - SDK/Client: Octokit 13.0.1
  - Implementation: `src/Magidesk.Infrastructure/Services/GithubUpdateService.cs`
  - Configuration: Repository owner and name from settings (`RepositoryOwner`, `RepositoryName`)
  - User Agent: "Magidesk-POS-Updater"

**Kitchen Notification Hub:**
- Service: Internal SignalR hub for real-time kitchen order communication
  - What it's used for: Publishing kitchen orders to KDS displays in real-time
  - Hub: `Magidesk.Api.Hubs.KitchenHub` mapped at `/hubs/kitchen`
  - Client: Desktop uses `Microsoft.AspNetCore.SignalR.Client 10.0.2`
  - Event Publisher: `Magidesk.Api.Services.SignalRKitchenNotificationPublisher` implements `IKitchenNotificationPublisher`

## Data Storage

**Databases:**
- PostgreSQL 12+
  - Connection (API): `Host=localhost;Database=Magidesk_Dev;Username=postgres;Password=password` (development)
  - Connection (Desktop): `Host=localhost;Database=magidesk_prod;Username=postgres;Password=password`
  - Client: Entity Framework Core 8.0.0 via Npgsql provider
  - DbContext: `ApplicationDbContext` at `src/Magidesk.Infrastructure/Data/ApplicationDbContext.cs`
  - Migrations: Managed via Magidesk.Migrations project

**File Storage:**
- Local filesystem only
  - Installer downloads: System temp directory (`Path.GetTempPath()`)
  - Log files: System temp directory
  - Configuration: Local `appsettings.json` files

**Caching:**
- In-memory caching (MemoryCache)
  - Registered in API via `builder.Services.AddMemoryCache()`
  - Enhanced caching service: `IEnhancedCachingService` at `src/Magidesk.Infrastructure/Services/EnhancedCachingService.cs`
  - Single-instance for performance-critical operations

## Authentication & Identity

**Auth Provider:**
- Custom JWT-based authentication (planned/partially implemented)
  - Implementation: JWT configuration in `appsettings.json`
  - Issuer: `http://localhost:5000`
  - Audience: `wpa-client`
  - Middleware: `.UseAuthentication()` and `.UseAuthorization()` configured in `src/Magidesk.Api/Program.cs`

**Desktop Client:**
- Terminal context via HTTP services: `HttpTerminalContext` and `HttpUserService`
- Scoped per request to API

## Payment Gateway

**Payment Processing:**
- Service: Mock Payment Gateway (production implementation pending)
  - Implementation: `MockPaymentGateway` at `src/Magidesk.Infrastructure/PaymentGateways/MockPaymentGateway.cs`
  - Supports: Authorization, Capture, Void, Refund, Tips adjustment, Batch close
  - Configuration: `MerchantGatewayConfiguration` entity stored in database
  - Repository: `IMerchantGatewayConfigurationRepository` at `src/Magidesk.Infrastructure/Repositories/MerchantGatewayConfigurationRepository.cs`

**Payment Methods:**
- Cash payments: Tracked via `CashPayment` entity
- Credit card payments: `CreditCardPayment` entity with gateway integration
- Debit card payments: `DebitCardPayment` entity
- Gift certificate payments: `GiftCertificatePayment` entity
- Custom payments: `CustomPaymentPayment` entity

## Monitoring & Observability

**Error Tracking:**
- Service: Custom error reporting service
  - Implementation: `IErrorReportingService` at `src/Magidesk.Infrastructure/Services/ErrorReportingService.cs`
  - Global exception handler: `GlobalExceptionHandler` in API

**Logs:**
- Approach: Built-in .NET logging infrastructure
  - Configuration: Log levels in `appsettings.json`
  - Default level: Information
  - Microsoft.AspNetCore level: Warning (reduces noise)
  - SQL logging: Enabled via `optionsBuilder.LogTo()` in `ApplicationDbContext.OnConfiguring()`

**Performance Monitoring:**
- Service: `IPerformanceMonitoringService` at `src/Magidesk.Infrastructure/Services/PerformanceMonitoringService.cs`
- Metrics storage: `PerformanceMetricEntity` in database
- Alert service: `IAlertService` at `src/Magidesk.Infrastructure/Services/AlertService.cs`

## CI/CD & Deployment

**Hosting:**
- Windows desktop application (WinUI 3) - deployed via MSIX installer
- ASP.NET Core API - ready for deployment on Windows Server or cloud (not actively deployed in current state)

**Deployment Package:**
- MSIX format with Windows App SDK
- Installer asset pattern: "Magidesk-Setup" (from update settings)
- Version control: Semantic versioning (0.1.0-beta current)
- Platforms: x86, x64, ARM64

**CI Pipeline:**
- Not detected - Manual build and deployment via Visual Studio

## Environment Configuration

**Required env vars (appsettings.json keys):**
- `ConnectionStrings.DefaultConnection` - PostgreSQL connection string (API)
- `ConnectionStrings.MagideskContext` - PostgreSQL connection string (Desktop)
- `Jwt.Issuer` - JWT issuer URL
- `Jwt.Audience` - JWT audience identifier
- `Jwt.Key` - JWT signing key (secret)
- `UpdateSettings.RepositoryOwner` - GitHub repo owner
- `UpdateSettings.RepositoryName` - GitHub repo name
- `UpdateSettings.CheckIntervalHours` - Update check frequency

**Secrets location:**
- `appsettings.json` for development (hardcoded values)
- Environment variables recommended for production
- AES encryption service available: `IAesEncryptionService` at `src/Magidesk.Infrastructure/Security/AesEncryptionService.cs`

## Webhooks & Callbacks

**Incoming:**
- Kitchen order notifications via SignalR hub (not traditional webhooks)
- No REST webhook receivers detected

**Outgoing:**
- GitHub release API polling for updates (one-way, no callbacks)
- No outgoing webhooks detected

## HTTP Client Configuration

**Named Clients:**
- `GitHubDownload` - For downloading release installers from GitHub
  - User Agent: "Magidesk-POS-Updater/1.0"
  - Timeout: 10 minutes
  - Configuration: `src/Magidesk.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` line 166-171

## CORS Configuration

**API CORS Policy:**
- Policy name: "AllowFrontend"
- Allowed origins: `http://localhost:5173` (frontend dev server)
- Allowed headers: Any
- Allowed methods: Any
- Credentials: Allowed
- Configuration: `src/Magidesk.Api/Program.cs` line 22-32

---

*Integration audit: 2026-03-23*
