## Running Magidesk (Dev)

### Prereqs
- .NET 8 SDK
- Local PostgreSQL

### Configure DB connection
- **Recommended**: set env var `MAGIDESK_CONNECTION_STRING`

Example:

```powershell
$env:MAGIDESK_CONNECTION_STRING = "Host=localhost;Port=5432;Database=magidesk_pos;Username=postgres;Password="
```

### Apply migrations

```powershell
dotnet run --project Magidesk.Migrations
```

### Run WinUI app
Open `Magidesk.Presentation.sln` in Visual Studio and run the WinUI project.

## Running tests

```powershell
dotnet test Magidesk.sln
```

### Notes on integration tests
- `Magidesk.Infrastructure.Tests` uses `magidesk_test` and currently expects it to exist.
- Connection used in tests: `Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres;`
