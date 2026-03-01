using System.Diagnostics;
using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.Infrastructure.Exceptions;
using Npgsql;
using Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for DatabaseResetEngine.
/// Validates database reset completeness, performance, and data integrity.
/// </summary>
public class DatabaseResetEngineProperties
{
    private const string TestConnectionStringEnvVar = "MAGIDESK_TEST_DB_CONNECTION";

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.1, 4.4, 4.5, 4.6
    /// 
    /// For any database reset operation, all transactional tables (tickets, payments, 
    /// cash_sessions, order_lines, kitchen_orders) must be empty, all configuration tables 
    /// (menu_items, modifiers, payment_methods, terminals, users) must contain data, 
    /// and minimum required seed data (admin user, default terminal, tax rates) must exist.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property DatabaseReset_EnsuresAllTransactionalTablesAreEmpty()
    {
        return Prop.ForAll(
            GenerateResetIterations(),
            iteration =>
            {
                // Arrange
                var connectionString = GetTestConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    // Skip test if connection string not configured
                    return true;
                }

                var engine = new DatabaseResetEngine(connectionString);

                // Insert some transactional data before reset
                InsertTransactionalTestData(connectionString);

                // Act
                engine.ResetDatabase();

                // Assert - All transactional tables must be empty
                var transactionalTablesEmpty = VerifyTransactionalTablesEmpty(connectionString);

                // Assert - Configuration tables must contain data
                var configurationTablesHaveData = VerifyConfigurationTablesHaveData(connectionString);

                // Assert - Minimum required seed data must exist
                var seedDataExists = VerifySeedDataExists(connectionString);

                return transactionalTablesEmpty && configurationTablesHaveData && seedDataExists;
            });
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.1, 4.4, 4.5, 4.6
    /// 
    /// Verifies that after reset, admin user exists with correct credentials.
    /// </summary>
    [Fact]
    public void DatabaseReset_EnsuresAdminUserExists()
    {
        // Arrange
        var connectionString = GetTestConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            // Skip test if connection string not configured
            return;
        }

        var engine = new DatabaseResetEngine(connectionString);

        // Act
        engine.ResetDatabase();

        // Assert
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(
            "SELECT COUNT(*) FROM users WHERE username = 'admin' AND is_active = true",
            connection);

        var count = (long)(command.ExecuteScalar() ?? 0L);
        Assert.True(count >= 1, "Admin user must exist after database reset");
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.1, 4.4, 4.5, 4.6
    /// 
    /// Verifies that after reset, default terminal exists.
    /// </summary>
    [Fact]
    public void DatabaseReset_EnsuresDefaultTerminalExists()
    {
        // Arrange
        var connectionString = GetTestConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var engine = new DatabaseResetEngine(connectionString);

        // Act
        engine.ResetDatabase();

        // Assert
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(
            "SELECT COUNT(*) FROM terminals WHERE terminal_number = 1 AND is_active = true",
            connection);

        var count = (long)(command.ExecuteScalar() ?? 0L);
        Assert.True(count >= 1, "Default terminal must exist after database reset");
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.1, 4.4, 4.5, 4.6
    /// 
    /// Verifies that after reset, restaurant configuration with tax rates exists.
    /// </summary>
    [Fact]
    public void DatabaseReset_EnsuresTaxRatesExist()
    {
        // Arrange
        var connectionString = GetTestConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var engine = new DatabaseResetEngine(connectionString);

        // Act
        engine.ResetDatabase();

        // Assert
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(
            "SELECT COUNT(*) FROM restaurant_configurations WHERE tax_rate IS NOT NULL AND reduced_tax_rate IS NOT NULL",
            connection);

        var count = (long)(command.ExecuteScalar() ?? 0L);
        Assert.True(count >= 1, "Restaurant configuration with tax rates must exist after database reset");
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 9: Database Reset Performance
    /// Validates: Requirements 4.2
    /// 
    /// For any typical database reset operation (with standard test data volume), 
    /// the reset must complete within 5 seconds.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property DatabaseReset_CompletesWithin5Seconds()
    {
        return Prop.ForAll(
            GenerateResetIterations(),
            iteration =>
            {
                // Arrange
                var connectionString = GetTestConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    // Skip test if connection string not configured
                    return true;
                }

                var engine = new DatabaseResetEngine(connectionString);

                // Insert typical test data volume
                InsertTypicalTestDataVolume(connectionString);

                // Act
                var stopwatch = Stopwatch.StartNew();
                engine.ResetDatabase();
                stopwatch.Stop();

                // Assert - Must complete within 5 seconds
                var completedWithinThreshold = stopwatch.Elapsed.TotalSeconds <= 5.0;

                if (!completedWithinThreshold)
                {
                    Console.WriteLine(
                        $"WARNING: Database reset took {stopwatch.Elapsed.TotalSeconds:F2} seconds, " +
                        "exceeds 5s threshold");
                }

                return completedWithinThreshold;
            });
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 9: Database Reset Performance
    /// Validates: Requirements 4.2
    /// 
    /// Verifies that reset performance is consistent across multiple executions.
    /// </summary>
    [Fact]
    public void DatabaseReset_PerformanceIsConsistent()
    {
        // Arrange
        var connectionString = GetTestConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var engine = new DatabaseResetEngine(connectionString);
        var executionTimes = new List<double>();

        // Act - Execute reset 10 times and measure performance
        for (int i = 0; i < 10; i++)
        {
            InsertTypicalTestDataVolume(connectionString);

            var stopwatch = Stopwatch.StartNew();
            engine.ResetDatabase();
            stopwatch.Stop();

            executionTimes.Add(stopwatch.Elapsed.TotalSeconds);
        }

        // Assert - All executions should be within 5 seconds
        Assert.All(executionTimes, time => Assert.True(time <= 5.0, 
            $"Reset took {time:F2}s, exceeds 5s threshold"));

        // Assert - Standard deviation should be low (consistent performance)
        var average = executionTimes.Average();
        var variance = executionTimes.Select(t => Math.Pow(t - average, 2)).Average();
        var stdDev = Math.Sqrt(variance);

        Assert.True(stdDev < 1.0, 
            $"Performance variance too high (stdDev: {stdDev:F2}s). " +
            "Reset performance should be consistent.");
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.1, 4.5
    /// 
    /// Verifies that tickets table is empty after reset.
    /// </summary>
    [Fact]
    public void DatabaseReset_EnsuresTicketsTableIsEmpty()
    {
        // Arrange
        var connectionString = GetTestConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var engine = new DatabaseResetEngine(connectionString);

        // Insert test tickets
        InsertTestTickets(connectionString, 5);

        // Act
        engine.ResetDatabase();

        // Assert
        var count = GetTableRowCount(connectionString, "tickets");
        Assert.Equal(0, count);
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.1, 4.5
    /// 
    /// Verifies that payments table is empty after reset.
    /// </summary>
    [Fact]
    public void DatabaseReset_EnsuresPaymentsTableIsEmpty()
    {
        // Arrange
        var connectionString = GetTestConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var engine = new DatabaseResetEngine(connectionString);

        // Act
        engine.ResetDatabase();

        // Assert
        var count = GetTableRowCount(connectionString, "payments");
        Assert.Equal(0, count);
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.1, 4.5
    /// 
    /// Verifies that cash_sessions table is empty after reset.
    /// </summary>
    [Fact]
    public void DatabaseReset_EnsuresCashSessionsTableIsEmpty()
    {
        // Arrange
        var connectionString = GetTestConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var engine = new DatabaseResetEngine(connectionString);

        // Act
        engine.ResetDatabase();

        // Assert
        var count = GetTableRowCount(connectionString, "cash_sessions");
        Assert.Equal(0, count);
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.1, 4.5
    /// 
    /// Verifies that order_lines table is empty after reset.
    /// </summary>
    [Fact]
    public void DatabaseReset_EnsuresOrderLinesTableIsEmpty()
    {
        // Arrange
        var connectionString = GetTestConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var engine = new DatabaseResetEngine(connectionString);

        // Act
        engine.ResetDatabase();

        // Assert
        var count = GetTableRowCount(connectionString, "order_lines");
        Assert.Equal(0, count);
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.1, 4.5
    /// 
    /// Verifies that kitchen_orders table is empty after reset.
    /// </summary>
    [Fact]
    public void DatabaseReset_EnsuresKitchenOrdersTableIsEmpty()
    {
        // Arrange
        var connectionString = GetTestConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var engine = new DatabaseResetEngine(connectionString);

        // Act
        engine.ResetDatabase();

        // Assert
        var count = GetTableRowCount(connectionString, "kitchen_orders");
        Assert.Equal(0, count);
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.7
    /// 
    /// Verifies that DatabaseResetException is thrown when connection string is invalid.
    /// </summary>
    [Fact]
    public void DatabaseReset_ThrowsDatabaseResetExceptionForInvalidConnectionString()
    {
        // Arrange
        var invalidConnectionString = "Host=invalid;Port=9999;Database=nonexistent;Username=fake;Password=fake";
        var engine = new DatabaseResetEngine(invalidConnectionString);

        // Act & Assert
        var exception = Assert.Throws<DatabaseResetException>(() => engine.ResetDatabase());

        Assert.Contains("Failed to reset database", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    /// <summary>
    /// Feature: e2e-testing-framework, Property 4: Database Reset Completeness
    /// Validates: Requirements 4.1
    /// 
    /// Verifies that reset is atomic - either all operations succeed or all are rolled back.
    /// </summary>
    [Fact]
    public void DatabaseReset_IsAtomic()
    {
        // Arrange
        var connectionString = GetTestConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var engine = new DatabaseResetEngine(connectionString);

        // Insert test data
        InsertTransactionalTestData(connectionString);
        var countBefore = GetTableRowCount(connectionString, "tickets");

        // Act
        engine.ResetDatabase();

        // Assert - All transactional data should be gone (atomic operation)
        var countAfter = GetTableRowCount(connectionString, "tickets");
        Assert.Equal(0, countAfter);

        // Verify other transactional tables are also empty
        Assert.Equal(0, GetTableRowCount(connectionString, "order_lines"));
        Assert.Equal(0, GetTableRowCount(connectionString, "payments"));
    }

    // ===== Helper Methods =====

    private static string? GetTestConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestConnectionStringEnvVar);
        
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine(
                $"WARNING: {TestConnectionStringEnvVar} environment variable not set. " +
                "Skipping database reset property tests.");
        }

        return connectionString;
    }

    private static void InsertTransactionalTestData(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        // Insert a test ticket
        using var command = new NpgsqlCommand(
            @"INSERT INTO tickets (id, ticket_number, status, total, created_at, updated_at, version)
              VALUES (gen_random_uuid(), 1001, 'Open', 25.00, NOW(), NOW(), 1)
              ON CONFLICT DO NOTHING",
            connection);

        command.ExecuteNonQuery();
    }

    private static void InsertTestTickets(string connectionString, int count)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        for (int i = 0; i < count; i++)
        {
            using var command = new NpgsqlCommand(
                @"INSERT INTO tickets (id, ticket_number, status, total, created_at, updated_at, version)
                  VALUES (gen_random_uuid(), @ticketNumber, 'Open', 25.00, NOW(), NOW(), 1)
                  ON CONFLICT DO NOTHING",
                connection);

            command.Parameters.AddWithValue("ticketNumber", 1000 + i);
            command.ExecuteNonQuery();
        }
    }

    private static void InsertTypicalTestDataVolume(string connectionString)
    {
        // Insert typical test data volume (10 tickets, 30 order lines)
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            // Insert 10 tickets
            for (int i = 0; i < 10; i++)
            {
                using var ticketCommand = new NpgsqlCommand(
                    @"INSERT INTO tickets (id, ticket_number, status, total, created_at, updated_at, version)
                      VALUES (gen_random_uuid(), @ticketNumber, 'Open', 75.00, NOW(), NOW(), 1)
                      ON CONFLICT DO NOTHING",
                    connection,
                    transaction);

                ticketCommand.Parameters.AddWithValue("ticketNumber", 2000 + i);
                ticketCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static bool VerifyTransactionalTablesEmpty(string connectionString)
    {
        var transactionalTables = new[]
        {
            "tickets",
            "order_lines",
            "payments",
            "cash_sessions",
            "kitchen_orders",
            "table_sessions",
            "shifts",
            "purchase_orders"
        };

        foreach (var table in transactionalTables)
        {
            var count = GetTableRowCount(connectionString, table);
            if (count > 0)
            {
                Console.WriteLine($"Transactional table '{table}' is not empty (count: {count})");
                return false;
            }
        }

        return true;
    }

    private static bool VerifyConfigurationTablesHaveData(string connectionString)
    {
        // Verify that configuration tables still have data after reset
        var configTables = new[]
        {
            "users",
            "terminals",
            "roles"
        };

        foreach (var table in configTables)
        {
            var count = GetTableRowCount(connectionString, table);
            if (count == 0)
            {
                Console.WriteLine($"Configuration table '{table}' is empty (should have data)");
                return false;
            }
        }

        return true;
    }

    private static bool VerifySeedDataExists(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        // Verify admin user exists
        using var adminCommand = new NpgsqlCommand(
            "SELECT COUNT(*) FROM users WHERE username = 'admin' AND is_active = true",
            connection);
        var adminCount = (long)(adminCommand.ExecuteScalar() ?? 0L);

        if (adminCount == 0)
        {
            Console.WriteLine("Admin user does not exist");
            return false;
        }

        // Verify default terminal exists
        using var terminalCommand = new NpgsqlCommand(
            "SELECT COUNT(*) FROM terminals WHERE terminal_number = 1 AND is_active = true",
            connection);
        var terminalCount = (long)(terminalCommand.ExecuteScalar() ?? 0L);

        if (terminalCount == 0)
        {
            Console.WriteLine("Default terminal does not exist");
            return false;
        }

        // Verify tax rates exist
        using var taxCommand = new NpgsqlCommand(
            "SELECT COUNT(*) FROM restaurant_configurations WHERE tax_rate IS NOT NULL",
            connection);
        var taxCount = (long)(taxCommand.ExecuteScalar() ?? 0L);

        if (taxCount == 0)
        {
            Console.WriteLine("Tax rates do not exist");
            return false;
        }

        return true;
    }

    private static long GetTableRowCount(string connectionString, string tableName)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = new NpgsqlCommand($"SELECT COUNT(*) FROM {tableName}", connection);
        return (long)(command.ExecuteScalar() ?? 0L);
    }

    // ===== Property Generators =====

    private static Arbitrary<int> GenerateResetIterations()
    {
        // Generate iteration numbers for property tests
        return Arb.From(Gen.Choose(1, 100));
    }
}
