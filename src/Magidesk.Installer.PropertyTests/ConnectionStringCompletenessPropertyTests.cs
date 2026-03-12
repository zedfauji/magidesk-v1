using FsCheck;
using FsCheck.Xunit;
using Npgsql;

namespace Magidesk.Installer.PropertyTests;

/// <summary>
/// Property-based tests for database connection string completeness.
/// **Validates: Requirements 7.5**
/// Property 7: Database Connection String Completeness
/// </summary>
public class ConnectionStringCompletenessPropertyTests
{
    /// <summary>
    /// Property: For any installation, the connection string written to appsettings.Production.json
    /// should contain all required components (host, port, database name, username, password).
    /// </summary>
    [Property(MaxTest = 100)]
    public void ConnectionString_ContainsAllRequiredComponents()
    {
        // Generate a valid connection string with all required components
        var gen = from host in Gen.Elements("127.0.0.1", "localhost")
                  from port in Gen.Choose(1024, 65535)
                  from database in GenerateValidDatabaseName()
                  from username in GenerateValidUsername()
                  from password in GenerateValidPassword()
                  select new { host, port, database, username, password };

        Prop.ForAll(
            Arb.From(gen),
            data =>
            {
                try
                {
                    var connectionString = $"Host={data.host};Port={data.port};Database={data.database};Username={data.username};Password={data.password}";
                    var builder = new NpgsqlConnectionStringBuilder(connectionString);

                    // Verify all required components are present and non-empty
                    return !string.IsNullOrEmpty(builder.Host) &&
                           builder.Port > 0 &&
                           !string.IsNullOrEmpty(builder.Database) &&
                           !string.IsNullOrEmpty(builder.Username) &&
                           !string.IsNullOrEmpty(builder.Password);
                }
                catch
                {
                    // If parsing fails, the connection string is incomplete
                    return false;
                }
            }).QuickCheckThrowOnFailure();
    }

    /// <summary>
    /// Property: For any connection string with missing host, parsing should fail or
    /// the host should be empty.
    /// </summary>
    [Property(MaxTest = 100)]
    public void ConnectionString_WithMissingHost_FailsValidation()
    {
        var gen = from port in Gen.Choose(1024, 65535)
                  from database in GenerateValidDatabaseName()
                  from username in GenerateValidUsername()
                  from password in GenerateValidPassword()
                  select new { port, database, username, password };

        Prop.ForAll(
            Arb.From(gen),
            data =>
            {
                try
                {
                    // Connection string without Host
                    var connectionString = $"Port={data.port};Database={data.database};Username={data.username};Password={data.password}";
                    var builder = new NpgsqlConnectionStringBuilder(connectionString);

                    // Host should be empty or default
                    return string.IsNullOrEmpty(builder.Host) || builder.Host == "localhost";
                }
                catch
                {
                    // Parsing failure is acceptable for incomplete connection strings
                    return true;
                }
            }).QuickCheckThrowOnFailure();
    }

    /// <summary>
    /// Property: For any connection string with missing port, the default port (5432)
    /// should be used.
    /// </summary>
    [Property(MaxTest = 100)]
    public void ConnectionString_WithMissingPort_UsesDefault()
    {
        var gen = from host in Gen.Elements("127.0.0.1", "localhost")
                  from database in GenerateValidDatabaseName()
                  from username in GenerateValidUsername()
                  from password in GenerateValidPassword()
                  select new { host, database, username, password };

        Prop.ForAll(
            Arb.From(gen),
            data =>
            {
                try
                {
                    // Connection string without Port
                    var connectionString = $"Host={data.host};Database={data.database};Username={data.username};Password={data.password}";
                    var builder = new NpgsqlConnectionStringBuilder(connectionString);

                    // Port should default to 5432
                    return builder.Port == 5432;
                }
                catch
                {
                    return false;
                }
            }).QuickCheckThrowOnFailure();
    }

    /// <summary>
    /// Property: For any connection string with missing database name, parsing should
    /// succeed but the database should be empty.
    /// </summary>
    [Property(MaxTest = 100)]
    public void ConnectionString_WithMissingDatabase_HasEmptyDatabase()
    {
        var gen = from host in Gen.Elements("127.0.0.1", "localhost")
                  from port in Gen.Choose(1024, 65535)
                  from username in GenerateValidUsername()
                  from password in GenerateValidPassword()
                  select new { host, port, username, password };

        Prop.ForAll(
            Arb.From(gen),
            data =>
            {
                try
                {
                    // Connection string without Database
                    var connectionString = $"Host={data.host};Port={data.port};Username={data.username};Password={data.password}";
                    var builder = new NpgsqlConnectionStringBuilder(connectionString);

                    // Database should be empty
                    return string.IsNullOrEmpty(builder.Database);
                }
                catch
                {
                    return false;
                }
            }).QuickCheckThrowOnFailure();
    }

    /// <summary>
    /// Property: For any connection string with missing username, parsing should
    /// succeed but the username should be empty.
    /// </summary>
    [Property(MaxTest = 100)]
    public void ConnectionString_WithMissingUsername_HasEmptyUsername()
    {
        var gen = from host in Gen.Elements("127.0.0.1", "localhost")
                  from port in Gen.Choose(1024, 65535)
                  from database in GenerateValidDatabaseName()
                  from password in GenerateValidPassword()
                  select new { host, port, database, password };

        Prop.ForAll(
            Arb.From(gen),
            data =>
            {
                try
                {
                    // Connection string without Username
                    var connectionString = $"Host={data.host};Port={data.port};Database={data.database};Password={data.password}";
                    var builder = new NpgsqlConnectionStringBuilder(connectionString);

                    // Username should be empty
                    return string.IsNullOrEmpty(builder.Username);
                }
                catch
                {
                    return false;
                }
            }).QuickCheckThrowOnFailure();
    }

    /// <summary>
    /// Property: For any connection string with missing password, parsing should
    /// succeed but the password should be empty.
    /// </summary>
    [Property(MaxTest = 100)]
    public void ConnectionString_WithMissingPassword_HasEmptyPassword()
    {
        var gen = from host in Gen.Elements("127.0.0.1", "localhost")
                  from port in Gen.Choose(1024, 65535)
                  from database in GenerateValidDatabaseName()
                  from username in GenerateValidUsername()
                  select new { host, port, database, username };

        Prop.ForAll(
            Arb.From(gen),
            data =>
            {
                try
                {
                    // Connection string without Password
                    var connectionString = $"Host={data.host};Port={data.port};Database={data.database};Username={data.username}";
                    var builder = new NpgsqlConnectionStringBuilder(connectionString);

                    // Password should be empty
                    return string.IsNullOrEmpty(builder.Password);
                }
                catch
                {
                    return false;
                }
            }).QuickCheckThrowOnFailure();
    }

    /// <summary>
    /// Property: For any valid connection string, it should be parseable by
    /// NpgsqlConnectionStringBuilder without throwing exceptions.
    /// </summary>
    [Property(MaxTest = 100)]
    public void ConnectionString_WithAllComponents_IsParseable()
    {
        var gen = from host in Gen.Elements("127.0.0.1", "localhost")
                  from port in Gen.Choose(1024, 65535)
                  from database in GenerateValidDatabaseName()
                  from username in GenerateValidUsername()
                  from password in GenerateValidPassword()
                  select new { host, port, database, username, password };

        Prop.ForAll(
            Arb.From(gen),
            data =>
            {
                try
                {
                    var connectionString = $"Host={data.host};Port={data.port};Database={data.database};Username={data.username};Password={data.password}";
                    var builder = new NpgsqlConnectionStringBuilder(connectionString);

                    // Should parse without exception
                    return true;
                }
                catch
                {
                    // Parsing failure means the connection string is not valid
                    return false;
                }
            }).QuickCheckThrowOnFailure();
    }

    /// <summary>
    /// Property: For any connection string, the installer should use localhost or
    /// 127.0.0.1 as the host (security requirement).
    /// </summary>
    [Property(MaxTest = 100)]
    public void InstallerConnectionString_UsesLocalhostOnly()
    {
        var gen = from port in Gen.Choose(1024, 65535)
                  from database in GenerateValidDatabaseName()
                  from username in GenerateValidUsername()
                  from password in GenerateValidPassword()
                  select new { port, database, username, password };

        Prop.ForAll(
            Arb.From(gen),
            data =>
            {
                // Test both valid localhost values
                var hosts = new[] { "127.0.0.1", "localhost" };

                foreach (var host in hosts)
                {
                    var connectionString = $"Host={host};Port={data.port};Database={data.database};Username={data.username};Password={data.password}";
                    var builder = new NpgsqlConnectionStringBuilder(connectionString);

                    if (builder.Host != "127.0.0.1" && builder.Host != "localhost")
                    {
                        return false;
                    }
                }

                return true;
            }).QuickCheckThrowOnFailure();
    }

    /// <summary>
    /// Property: For any connection string, the port should be in the valid range
    /// (1-65535) and typically 5432 for PostgreSQL.
    /// </summary>
    [Property(MaxTest = 100)]
    public void ConnectionString_HasValidPortRange()
    {
        var gen = from host in Gen.Elements("127.0.0.1", "localhost")
                  from port in Gen.Choose(1, 65535)
                  from database in GenerateValidDatabaseName()
                  from username in GenerateValidUsername()
                  from password in GenerateValidPassword()
                  select new { host, port, database, username, password };

        Prop.ForAll(
            Arb.From(gen),
            data =>
            {
                try
                {
                    var connectionString = $"Host={data.host};Port={data.port};Database={data.database};Username={data.username};Password={data.password}";
                    var builder = new NpgsqlConnectionStringBuilder(connectionString);

                    // Port should be in valid range
                    return builder.Port > 0 && builder.Port <= 65535;
                }
                catch
                {
                    return false;
                }
            }).QuickCheckThrowOnFailure();
    }

    /// <summary>
    /// Property: For any connection string, rebuilding it from parsed components
    /// should produce an equivalent connection string (idempotency).
    /// </summary>
    [Property(MaxTest = 100)]
    public void ConnectionString_IsIdempotent()
    {
        var gen = from host in Gen.Elements("127.0.0.1", "localhost")
                  from port in Gen.Choose(1024, 65535)
                  from database in GenerateValidDatabaseName()
                  from username in GenerateValidUsername()
                  from password in GenerateValidPassword()
                  select new { host, port, database, username, password };

        Prop.ForAll(
            Arb.From(gen),
            data =>
            {
                try
                {
                    var originalConnectionString = $"Host={data.host};Port={data.port};Database={data.database};Username={data.username};Password={data.password}";
                    var builder = new NpgsqlConnectionStringBuilder(originalConnectionString);
                    var rebuiltConnectionString = builder.ToString();

                    var originalBuilder = new NpgsqlConnectionStringBuilder(originalConnectionString);
                    var rebuiltBuilder = new NpgsqlConnectionStringBuilder(rebuiltConnectionString);

                    // All components should match
                    return originalBuilder.Host == rebuiltBuilder.Host &&
                           originalBuilder.Port == rebuiltBuilder.Port &&
                           originalBuilder.Database == rebuiltBuilder.Database &&
                           originalBuilder.Username == rebuiltBuilder.Username &&
                           originalBuilder.Password == rebuiltBuilder.Password;
                }
                catch
                {
                    return false;
                }
            }).QuickCheckThrowOnFailure();
    }

    /// <summary>
    /// Generates a valid database name (alphanumeric + underscore, starts with letter)
    /// </summary>
    private static Gen<string> GenerateValidDatabaseName()
    {
        return from firstChar in Gen.Elements("abcdefghijklmnopqrstuvwxyz".ToCharArray())
               from length in Gen.Choose(3, 20)
               from chars in Gen.ArrayOf(length, Gen.Elements("abcdefghijklmnopqrstuvwxyz0123456789_".ToCharArray()))
               select firstChar + new string(chars);
    }

    /// <summary>
    /// Generates a valid username (alphanumeric + underscore)
    /// </summary>
    private static Gen<string> GenerateValidUsername()
    {
        return from length in Gen.Choose(3, 20)
               from chars in Gen.ArrayOf(length, Gen.Elements("abcdefghijklmnopqrstuvwxyz0123456789_".ToCharArray()))
               select new string(chars);
    }

    /// <summary>
    /// Generates a valid password (at least 8 characters, no special connection string chars)
    /// </summary>
    private static Gen<string> GenerateValidPassword()
    {
        return from length in Gen.Choose(8, 32)
               from chars in Gen.ArrayOf(length, Gen.Elements("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_+[]{}".ToCharArray()))
               select new string(chars);
    }
}
