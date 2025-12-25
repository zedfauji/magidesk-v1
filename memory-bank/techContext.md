# Technical Context

## Database Configuration

### PostgreSQL Database
- **Database Name**: `magidesk_new`
- **Host**: `localhost`
- **Port**: `5432` (default)
- **Username**: `postgres`
- **Password**: `postgres`
- **Connection String**: `Host=localhost;Port=5432;Database=magidesk_new;Username=postgres;Password=postgres;`

### Important Notes
- This is a **NEW database** created specifically for the Magidesk POS system rebuild
- **DO NOT** mix with any legacy databases
- **Legacy database**: `magidesk_pos` - DO NOT USE OR MODIFY
- **New database**: `magidesk_new` - Use this for all new development
- Local development uses passwordless PostgreSQL authentication
- In production, credentials should come from secure configuration/secret management

## Technology Stack

### Backend
- **.NET 8.0**: Target framework
- **Entity Framework Core 8.0**: ORM for database access
- **Npgsql.EntityFrameworkCore.PostgreSQL 8.0**: PostgreSQL provider
- **PostgreSQL**: Database server

### Architecture
- **Clean Architecture**: Domain, Application, Infrastructure, Presentation layers
- **MVVM**: Model-View-ViewModel pattern for UI
- **CQRS**: Command Query Responsibility Segregation
- **Repository Pattern**: Data access abstraction

### Development Tools
- **EF Core Migrations**: Database schema management
- **xUnit**: Unit testing framework
- **Moq**: Mocking framework for tests

## Development Setup

### Database Connection
The connection string is configured in `Magidesk.Infrastructure/Data/DatabaseConnection.cs`

### Migrations
- Migrations are stored in `Magidesk.Infrastructure/Data/Migrations/`
- Use `dotnet ef migrations add <MigrationName>` to create migrations
- Use `dotnet ef database update` to apply migrations

