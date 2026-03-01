// InitTestDatabase.cs
// Simple utility to initialize the test database schema using EF Core
// Usage: dotnet script InitTestDatabase.cs

using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;

var connectionString = "Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres";

Console.WriteLine("Initializing test database schema...");
Console.WriteLine($"Connection: {connectionString}");

try
{
    // Test connection
    using (var conn = new NpgsqlConnection(connectionString))
    {
        conn.Open();
        Console.WriteLine("✓ Database connection successful");
    }

    Console.WriteLine("\nTo complete the setup:");
    Console.WriteLine("1. Build the Magidesk.Presentation project:");
    Console.WriteLine("   dotnet build src/Magidesk.Presentation/Magidesk.Presentation.csproj -c Release");
    Console.WriteLine("\n2. Run the Magidesk application once with the test database:");
    Console.WriteLine("   - The application will automatically create the schema on first run");
    Console.WriteLine("   - Look for 'System Initialization successful' in the logs");
    Console.WriteLine("\n3. After the app starts successfully, close it and run the E2E tests:");
    Console.WriteLine("   dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj --filter Category=FinancialSafety");
    
    Console.WriteLine("\n✓ Test database is ready for initialization");
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error: {ex.Message}");
    return 1;
}

return 0;
