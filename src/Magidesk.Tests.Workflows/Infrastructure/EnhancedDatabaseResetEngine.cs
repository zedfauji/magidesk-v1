using System;
using System.Threading.Tasks;
using Npgsql;

namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Extends DatabaseResetEngine with additional capabilities for comprehensive testing scenarios.
/// Provides concrete implementations to seed test data for specific scenarios including
/// pool tables, dining tables, menu items, test users, inventory items, and customers.
/// </summary>
public class EnhancedDatabaseResetEngine : IEnhancedDatabaseResetEngine
{
    private readonly string _connectionString;
    private const int CommandTimeoutSeconds = 30;

    /// <summary>
    /// Initializes a new instance of EnhancedDatabaseResetEngine.
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string.</param>
    /// <exception cref="ArgumentException">Thrown when connection string is null or empty.</exception>
    public EnhancedDatabaseResetEngine(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public void ResetDatabase()
    {
        // Delegate to existing DatabaseResetEngine logic if needed
        // For now, this is a placeholder as the base implementation is in a different project
        throw new NotImplementedException("ResetDatabase should be implemented or delegated to existing DatabaseResetEngine.");
    }

    /// <inheritdoc/>
    public async Task SeedPoolTablesAsync(int count = 5)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // Create a pool table type with hourly rate
            var tableTypeId = Guid.NewGuid();
            await ExecuteSqlAsync(connection, transaction, @"
                INSERT INTO table_types (id, name, description, hourly_rate, minimum_minutes, rounding_minutes, 
                                        minimum_charge_amount, minimum_charge_currency, rounding_rule, is_active, created_at, updated_at)
                VALUES (@id, @name, @description, @hourlyRate, 0, 1, 0, 'USD', 0, true, @now, @now)",
                new NpgsqlParameter("@id", tableTypeId),
                new NpgsqlParameter("@name", "Pool Table"),
                new NpgsqlParameter("@description", "Standard pool table with hourly billing"),
                new NpgsqlParameter("@hourlyRate", 15.00m),
                new NpgsqlParameter("@now", DateTime.UtcNow));

            // Create pool tables
            for (int i = 1; i <= count; i++)
            {
                var tableId = Guid.NewGuid();
                await ExecuteSqlAsync(connection, transaction, @"
                    INSERT INTO tables (id, table_number, capacity, x, y, width, height, shape, status, 
                                       table_type_id, is_active, created_at, updated_at, version)
                    VALUES (@id, @tableNumber, @capacity, @x, @y, @width, @height, @shape, @status, 
                           @tableTypeId, true, @now, @now, 1)",
                    new NpgsqlParameter("@id", tableId),
                    new NpgsqlParameter("@tableNumber", i),
                    new NpgsqlParameter("@capacity", 4),
                    new NpgsqlParameter("@x", (i - 1) * 150.0),
                    new NpgsqlParameter("@y", 100.0),
                    new NpgsqlParameter("@width", 120.0),
                    new NpgsqlParameter("@height", 80.0),
                    new NpgsqlParameter("@shape", 0), // Rectangle
                    new NpgsqlParameter("@status", 0), // Available
                    new NpgsqlParameter("@tableTypeId", tableTypeId),
                    new NpgsqlParameter("@now", DateTime.UtcNow));
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task SeedDiningTablesAsync(int count = 10)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // Create two floors
            var floor1Id = Guid.NewGuid();
            var floor2Id = Guid.NewGuid();

            await ExecuteSqlAsync(connection, transaction, @"
                INSERT INTO floors (id, name, description, width, height, background_color, is_active, created_at, updated_at, version)
                VALUES (@id, @name, @description, 2000, 2000, '#f8f8f8', true, @now, @now, 1)",
                new NpgsqlParameter("@id", floor1Id),
                new NpgsqlParameter("@name", "Main Floor"),
                new NpgsqlParameter("@description", "Primary dining area"),
                new NpgsqlParameter("@now", DateTime.UtcNow));

            await ExecuteSqlAsync(connection, transaction, @"
                INSERT INTO floors (id, name, description, width, height, background_color, is_active, created_at, updated_at, version)
                VALUES (@id, @name, @description, 2000, 2000, '#f8f8f8', true, @now, @now, 1)",
                new NpgsqlParameter("@id", floor2Id),
                new NpgsqlParameter("@name", "Upper Floor"),
                new NpgsqlParameter("@description", "Secondary dining area"),
                new NpgsqlParameter("@now", DateTime.UtcNow));

            // Create dining tables distributed across both floors
            int tablesPerFloor = count / 2;
            int remainder = count % 2;

            for (int i = 1; i <= count; i++)
            {
                var tableId = Guid.NewGuid();
                var floorId = i <= (tablesPerFloor + remainder) ? floor1Id : floor2Id;
                var tableNumberOnFloor = i <= (tablesPerFloor + remainder) ? i : i - (tablesPerFloor + remainder);

                await ExecuteSqlAsync(connection, transaction, @"
                    INSERT INTO tables (id, table_number, capacity, x, y, width, height, shape, floor_id, status, 
                                       is_active, created_at, updated_at, version)
                    VALUES (@id, @tableNumber, @capacity, @x, @y, @width, @height, @shape, @floorId, @status, 
                           true, @now, @now, 1)",
                    new NpgsqlParameter("@id", tableId),
                    new NpgsqlParameter("@tableNumber", i),
                    new NpgsqlParameter("@capacity", 4),
                    new NpgsqlParameter("@x", ((tableNumberOnFloor - 1) % 5) * 200.0),
                    new NpgsqlParameter("@y", ((tableNumberOnFloor - 1) / 5) * 200.0),
                    new NpgsqlParameter("@width", 150.0),
                    new NpgsqlParameter("@height", 150.0),
                    new NpgsqlParameter("@shape", 1), // Circle
                    new NpgsqlParameter("@floorId", floorId),
                    new NpgsqlParameter("@status", 0), // Available
                    new NpgsqlParameter("@now", DateTime.UtcNow));
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task SeedMenuItemsAsync(int categoryCount = 5, int itemsPerCategory = 10)
    {
        if (categoryCount <= 0)
        {
            throw new ArgumentException("Category count must be greater than zero.", nameof(categoryCount));
        }

        if (itemsPerCategory <= 0)
        {
            throw new ArgumentException("Items per category must be greater than zero.", nameof(itemsPerCategory));
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var categoryNames = new[] { "Beverages", "Appetizers", "Main Courses", "Desserts", "Sides" };
            var itemPrefixes = new[] { "Drink", "Starter", "Entree", "Sweet", "Side" };

            for (int c = 0; c < categoryCount; c++)
            {
                var categoryId = Guid.NewGuid();
                var categoryName = c < categoryNames.Length ? categoryNames[c] : $"Category {c + 1}";

                await ExecuteSqlAsync(connection, transaction, @"
                    INSERT INTO menu_categories (id, name, description, display_order, is_active, created_at, updated_at, version)
                    VALUES (@id, @name, @description, @displayOrder, true, @now, @now, 1)",
                    new NpgsqlParameter("@id", categoryId),
                    new NpgsqlParameter("@name", categoryName),
                    new NpgsqlParameter("@description", $"Test {categoryName}"),
                    new NpgsqlParameter("@displayOrder", c + 1),
                    new NpgsqlParameter("@now", DateTime.UtcNow));

                // Create items for this category
                var itemPrefix = c < itemPrefixes.Length ? itemPrefixes[c] : "Item";
                for (int i = 1; i <= itemsPerCategory; i++)
                {
                    var itemId = Guid.NewGuid();
                    var price = 5.00m + (i * 2.00m);

                    await ExecuteSqlAsync(connection, transaction, @"
                        INSERT INTO menu_items (id, name, description, price_amount, price_currency, tax_rate, 
                                               category_id, display_order, is_visible, is_available, show_in_kiosk, 
                                               is_stock_item, should_print_to_kitchen, track_stock, stock_quantity, 
                                               minimum_stock_level, version, is_active)
                        VALUES (@id, @name, @description, @priceAmount, 'USD', 0.08, @categoryId, @displayOrder, 
                               true, true, true, false, true, false, 0, 0, 1, true)",
                        new NpgsqlParameter("@id", itemId),
                        new NpgsqlParameter("@name", $"{itemPrefix} {i}"),
                        new NpgsqlParameter("@description", $"Test {itemPrefix} item {i}"),
                        new NpgsqlParameter("@priceAmount", price),
                        new NpgsqlParameter("@categoryId", categoryId),
                        new NpgsqlParameter("@displayOrder", i),
                        new NpgsqlParameter("@now", DateTime.UtcNow));
                }
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task SeedTestUsersAsync(params (string username, string role)[] users)
    {
        if (users == null || users.Length == 0)
        {
            throw new ArgumentException("At least one user must be specified.", nameof(users));
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // Create roles if they don't exist
            var roleIds = new System.Collections.Generic.Dictionary<string, Guid>();
            var roleNames = new[] { "Server", "Manager", "Admin" };

            foreach (var roleName in roleNames)
            {
                var roleId = Guid.NewGuid();
                roleIds[roleName] = roleId;

                await ExecuteSqlAsync(connection, transaction, @"
                    INSERT INTO roles (id, name, description, is_active, created_at, updated_at)
                    VALUES (@id, @name, @description, true, @now, @now)
                    ON CONFLICT (name) DO NOTHING",
                    new NpgsqlParameter("@id", roleId),
                    new NpgsqlParameter("@name", roleName),
                    new NpgsqlParameter("@description", $"Test {roleName} role"),
                    new NpgsqlParameter("@now", DateTime.UtcNow));
            }

            // Fetch actual role IDs (in case they already existed)
            foreach (var roleName in roleNames)
            {
                await using var cmd = new NpgsqlCommand("SELECT id FROM roles WHERE name = @name", connection, transaction);
                cmd.Parameters.AddWithValue("@name", roleName);
                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    roleIds[roleName] = (Guid)result;
                }
            }

            // Create users
            foreach (var (username, role) in users)
            {
                if (!roleIds.TryGetValue(role, out var roleId))
                {
                    throw new ArgumentException($"Invalid role: {role}. Must be one of: Server, Manager, Admin", nameof(users));
                }

                var userId = Guid.NewGuid();
                var pin = "1234"; // Simple test PIN (should be hashed in production)

                await ExecuteSqlAsync(connection, transaction, @"
                    INSERT INTO users (id, username, first_name, last_name, encrypted_pin, role_id, 
                                      hourly_rate_amount, hourly_rate_currency, preferred_language, is_active)
                    VALUES (@id, @username, @firstName, @lastName, @pin, @roleId, 15.00, 'USD', 'en-US', true)",
                    new NpgsqlParameter("@id", userId),
                    new NpgsqlParameter("@username", username),
                    new NpgsqlParameter("@firstName", username),
                    new NpgsqlParameter("@lastName", "Test"),
                    new NpgsqlParameter("@pin", pin),
                    new NpgsqlParameter("@roleId", roleId));
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task SeedInventoryItemsAsync(int count = 20, int lowStockCount = 5)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        if (lowStockCount < 0 || lowStockCount > count)
        {
            throw new ArgumentException("Low stock count must be between 0 and total count.", nameof(lowStockCount));
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            for (int i = 1; i <= count; i++)
            {
                var itemId = Guid.NewGuid();
                var isLowStock = i <= lowStockCount;
                var quantity = isLowStock ? 2.0m : 50.0m;
                var minLevel = 10.0m;

                await ExecuteSqlAsync(connection, transaction, @"
                    INSERT INTO inventory_items (id, name, description, sku, unit_of_measure, quantity_on_hand, 
                                                minimum_level, reorder_point, unit_cost_amount, unit_cost_currency, 
                                                is_active, created_at, updated_at)
                    VALUES (@id, @name, @description, @sku, @unit, @quantity, @minLevel, @reorderPoint, 
                           @costAmount, 'USD', true, @now, @now)",
                    new NpgsqlParameter("@id", itemId),
                    new NpgsqlParameter("@name", $"Inventory Item {i}"),
                    new NpgsqlParameter("@description", $"Test inventory item {i}"),
                    new NpgsqlParameter("@sku", $"SKU-{i:D4}"),
                    new NpgsqlParameter("@unit", "EA"),
                    new NpgsqlParameter("@quantity", quantity),
                    new NpgsqlParameter("@minLevel", minLevel),
                    new NpgsqlParameter("@reorderPoint", minLevel),
                    new NpgsqlParameter("@costAmount", 5.00m + i),
                    new NpgsqlParameter("@now", DateTime.UtcNow));
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task SeedCustomersAsync(int count = 10, int membershipCount = 3)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        if (membershipCount < 0)
        {
            throw new ArgumentException("Membership count cannot be negative.", nameof(membershipCount));
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // Create membership tiers
            var tierIds = new System.Collections.Generic.List<Guid>();
            var tierNames = new[] { "Bronze", "Silver", "Gold" };

            for (int t = 0; t < membershipCount && t < tierNames.Length; t++)
            {
                var tierId = Guid.NewGuid();
                tierIds.Add(tierId);

                await ExecuteSqlAsync(connection, transaction, @"
                    INSERT INTO membership_tiers (id, name, description, discount_percentage, points_multiplier, 
                                                  is_active, created_at, updated_at)
                    VALUES (@id, @name, @description, @discount, @multiplier, true, @now, @now)
                    ON CONFLICT (name) DO NOTHING",
                    new NpgsqlParameter("@id", tierId),
                    new NpgsqlParameter("@name", tierNames[t]),
                    new NpgsqlParameter("@description", $"{tierNames[t]} membership tier"),
                    new NpgsqlParameter("@discount", (t + 1) * 5.0m),
                    new NpgsqlParameter("@multiplier", 1.0m + (t * 0.5m)),
                    new NpgsqlParameter("@now", DateTime.UtcNow));
            }

            // Create customers
            for (int i = 1; i <= count; i++)
            {
                var customerId = Guid.NewGuid();
                var phone = $"+1555{i:D7}";

                await ExecuteSqlAsync(connection, transaction, @"
                    INSERT INTO customers (id, first_name, last_name, email, phone, created_at, total_visits, 
                                          total_spent_amount, total_spent_currency, is_active)
                    VALUES (@id, @firstName, @lastName, @email, @phone, @now, 0, 0, 'USD', true)",
                    new NpgsqlParameter("@id", customerId),
                    new NpgsqlParameter("@firstName", $"Customer{i}"),
                    new NpgsqlParameter("@lastName", "Test"),
                    new NpgsqlParameter("@email", $"customer{i}@test.com"),
                    new NpgsqlParameter("@phone", phone),
                    new NpgsqlParameter("@now", DateTime.UtcNow));
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Executes a SQL command with parameters.
    /// </summary>
    private static async Task ExecuteSqlAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string sql,
        params NpgsqlParameter[] parameters)
    {
        await using var command = new NpgsqlCommand(sql, connection, transaction)
        {
            CommandTimeout = CommandTimeoutSeconds
        };

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        await command.ExecuteNonQueryAsync();
    }
}
