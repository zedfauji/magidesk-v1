using FluentAssertions;
using Magidesk.Tests.Workflows.Infrastructure;
using Npgsql;

namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Unit tests for EnhancedDatabaseResetEngine seeding operations.
/// Uses test database for isolation and verifies all seeding methods create correct entities.
/// 
/// PREREQUISITES:
/// - PostgreSQL database 'magidesk_test' must exist
/// - All required tables must be created (tables, table_types, floors, menu_categories, menu_items, 
///   roles, users, inventory_items, customers, membership_tiers)
/// - Connection string can be overridden via MAGIDESK_TEST_DB_CONNECTION environment variable
/// 
/// Validates Requirements: 17.1
/// </summary>
[Trait("Category", "Integration")]
[Trait("Category", "Database")]
public class EnhancedDatabaseResetEngineTests : IAsyncLifetime
{
    private static readonly string TestConnectionString = 
        Environment.GetEnvironmentVariable("MAGIDESK_TEST_DB_CONNECTION") 
        ?? "Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres";
    
    private EnhancedDatabaseResetEngine _engine = null!;
    private NpgsqlConnection _connection = null!;

    public async Task InitializeAsync()
    {
        _engine = new EnhancedDatabaseResetEngine(TestConnectionString);
        _connection = new NpgsqlConnection(TestConnectionString);
        await _connection.OpenAsync();
        
        // Clean test data before each test
        await CleanTestDataAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanTestDataAsync();
        await _connection.DisposeAsync();
    }

    private async Task CleanTestDataAsync()
    {
        // Delete in correct order to respect foreign key constraints
        await ExecuteSqlAsync("DELETE FROM users WHERE username LIKE 'test_%'");
        await ExecuteSqlAsync("DELETE FROM tables WHERE table_number > 0");
        await ExecuteSqlAsync("DELETE FROM table_types WHERE name = 'Pool Table'");
        await ExecuteSqlAsync("DELETE FROM floors WHERE name IN ('Main Floor', 'Upper Floor')");
        await ExecuteSqlAsync("DELETE FROM menu_items WHERE name LIKE 'Drink %' OR name LIKE 'Starter %' OR name LIKE 'Entree %' OR name LIKE 'Sweet %' OR name LIKE 'Side %' OR name LIKE 'Item %'");
        await ExecuteSqlAsync("DELETE FROM menu_categories WHERE name IN ('Beverages', 'Appetizers', 'Main Courses', 'Desserts', 'Sides') OR name LIKE 'Category %'");
        await ExecuteSqlAsync("DELETE FROM inventory_items WHERE sku LIKE 'SKU-%'");
        await ExecuteSqlAsync("DELETE FROM customers WHERE email LIKE 'customer%@test.com'");
        await ExecuteSqlAsync("DELETE FROM membership_tiers WHERE name IN ('Bronze', 'Silver', 'Gold')");
        await ExecuteSqlAsync("DELETE FROM roles WHERE name IN ('Server', 'Manager', 'Admin')");
    }

    private async Task ExecuteSqlAsync(string sql)
    {
        await using var command = new NpgsqlCommand(sql, _connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task<int> GetCountAsync(string tableName, string? whereClause = null)
    {
        var sql = $"SELECT COUNT(*) FROM {tableName}";
        if (!string.IsNullOrEmpty(whereClause))
        {
            sql += $" WHERE {whereClause}";
        }

        await using var command = new NpgsqlCommand(sql, _connection);
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullConnectionString_ThrowsArgumentException()
    {
        // Act
        var act = () => new EnhancedDatabaseResetEngine(null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("connectionString");
    }

    [Fact]
    public void Constructor_WithEmptyConnectionString_ThrowsArgumentException()
    {
        // Act
        var act = () => new EnhancedDatabaseResetEngine(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("connectionString");
    }

    [Fact]
    public void Constructor_WithWhitespaceConnectionString_ThrowsArgumentException()
    {
        // Act
        var act = () => new EnhancedDatabaseResetEngine("   ");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("connectionString");
    }

    #endregion

    #region SeedPoolTablesAsync Tests

    [Fact]
    public async Task SeedPoolTablesAsync_CreatesCorrectNumberOfTables()
    {
        // Arrange
        const int expectedCount = 5;

        // Act
        await _engine.SeedPoolTablesAsync(expectedCount);

        // Assert
        var actualCount = await GetCountAsync("tables", "table_type_id IS NOT NULL");
        actualCount.Should().Be(expectedCount);
    }

    [Fact]
    public async Task SeedPoolTablesAsync_CreatesTablesWithPoolTableType()
    {
        // Arrange
        const int count = 3;

        // Act
        await _engine.SeedPoolTablesAsync(count);

        // Assert
        // Verify table type was created
        var tableTypeCount = await GetCountAsync("table_types", "name = 'Pool Table'");
        tableTypeCount.Should().Be(1);

        // Verify table type has hourly rate
        const string sql = "SELECT hourly_rate FROM table_types WHERE name = 'Pool Table'";
        await using var command = new NpgsqlCommand(sql, _connection);
        var hourlyRate = await command.ExecuteScalarAsync();
        hourlyRate.Should().NotBeNull();
        Convert.ToDecimal(hourlyRate).Should().Be(15.00m);
    }

    [Fact]
    public async Task SeedPoolTablesAsync_CreatesTablesWithSequentialNumbers()
    {
        // Arrange
        const int count = 5;

        // Act
        await _engine.SeedPoolTablesAsync(count);

        // Assert
        for (int i = 1; i <= count; i++)
        {
            var tableCount = await GetCountAsync("tables", $"table_number = {i}");
            tableCount.Should().Be(1, $"table number {i} should exist");
        }
    }

    [Fact]
    public async Task SeedPoolTablesAsync_CreatesTablesWithCorrectCapacity()
    {
        // Arrange
        const int count = 3;

        // Act
        await _engine.SeedPoolTablesAsync(count);

        // Assert
        const string sql = "SELECT capacity FROM tables WHERE table_type_id IS NOT NULL LIMIT 1";
        await using var command = new NpgsqlCommand(sql, _connection);
        var capacity = await command.ExecuteScalarAsync();
        Convert.ToInt32(capacity).Should().Be(4);
    }

    [Fact]
    public async Task SeedPoolTablesAsync_WithZeroCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedPoolTablesAsync(0);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("count");
    }

    [Fact]
    public async Task SeedPoolTablesAsync_WithNegativeCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedPoolTablesAsync(-1);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("count");
    }

    #endregion

    #region SeedDiningTablesAsync Tests

    [Fact]
    public async Task SeedDiningTablesAsync_CreatesCorrectNumberOfTables()
    {
        // Arrange
        const int expectedCount = 10;

        // Act
        await _engine.SeedDiningTablesAsync(expectedCount);

        // Assert
        var actualCount = await GetCountAsync("tables", "floor_id IS NOT NULL");
        actualCount.Should().Be(expectedCount);
    }

    [Fact]
    public async Task SeedDiningTablesAsync_CreatesTablesWithCorrectFloorAssignments()
    {
        // Arrange
        const int count = 10;

        // Act
        await _engine.SeedDiningTablesAsync(count);

        // Assert
        // Verify two floors were created
        var floorCount = await GetCountAsync("floors", "name IN ('Main Floor', 'Upper Floor')");
        floorCount.Should().Be(2);

        // Verify tables are distributed across both floors
        var mainFloorCount = await GetCountAsync("tables", 
            "floor_id = (SELECT id FROM floors WHERE name = 'Main Floor')");
        var upperFloorCount = await GetCountAsync("tables", 
            "floor_id = (SELECT id FROM floors WHERE name = 'Upper Floor')");

        mainFloorCount.Should().BeGreaterThan(0);
        upperFloorCount.Should().BeGreaterThan(0);
        (mainFloorCount + upperFloorCount).Should().Be(count);
    }

    [Fact]
    public async Task SeedDiningTablesAsync_CreatesTablesWithSequentialNumbers()
    {
        // Arrange
        const int count = 10;

        // Act
        await _engine.SeedDiningTablesAsync(count);

        // Assert
        for (int i = 1; i <= count; i++)
        {
            var tableCount = await GetCountAsync("tables", $"table_number = {i}");
            tableCount.Should().Be(1, $"table number {i} should exist");
        }
    }

    [Fact]
    public async Task SeedDiningTablesAsync_CreatesTablesWithCorrectCapacity()
    {
        // Arrange
        const int count = 5;

        // Act
        await _engine.SeedDiningTablesAsync(count);

        // Assert
        const string sql = "SELECT capacity FROM tables WHERE floor_id IS NOT NULL LIMIT 1";
        await using var command = new NpgsqlCommand(sql, _connection);
        var capacity = await command.ExecuteScalarAsync();
        Convert.ToInt32(capacity).Should().Be(4);
    }

    [Fact]
    public async Task SeedDiningTablesAsync_WithZeroCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedDiningTablesAsync(0);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("count");
    }

    [Fact]
    public async Task SeedDiningTablesAsync_WithNegativeCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedDiningTablesAsync(-1);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("count");
    }

    #endregion

    #region SeedMenuItemsAsync Tests

    [Fact]
    public async Task SeedMenuItemsAsync_CreatesCorrectNumberOfCategories()
    {
        // Arrange
        const int categoryCount = 5;
        const int itemsPerCategory = 10;

        // Act
        await _engine.SeedMenuItemsAsync(categoryCount, itemsPerCategory);

        // Assert
        var actualCount = await GetCountAsync("menu_categories", 
            "name IN ('Beverages', 'Appetizers', 'Main Courses', 'Desserts', 'Sides')");
        actualCount.Should().Be(categoryCount);
    }

    [Fact]
    public async Task SeedMenuItemsAsync_CreatesItemsWithCorrectCategories()
    {
        // Arrange
        const int categoryCount = 3;
        const int itemsPerCategory = 5;

        // Act
        await _engine.SeedMenuItemsAsync(categoryCount, itemsPerCategory);

        // Assert
        // Verify each category has the correct number of items
        var categories = new[] { "Beverages", "Appetizers", "Main Courses" };
        foreach (var category in categories)
        {
            const string sql = @"
                SELECT COUNT(*) 
                FROM menu_items mi
                JOIN menu_categories mc ON mi.category_id = mc.id
                WHERE mc.name = @categoryName";

            await using var command = new NpgsqlCommand(sql, _connection);
            command.Parameters.AddWithValue("@categoryName", category);
            var itemCount = await command.ExecuteScalarAsync();
            Convert.ToInt32(itemCount).Should().Be(itemsPerCategory);
        }
    }

    [Fact]
    public async Task SeedMenuItemsAsync_CreatesItemsWithCorrectPrices()
    {
        // Arrange
        const int categoryCount = 1;
        const int itemsPerCategory = 3;

        // Act
        await _engine.SeedMenuItemsAsync(categoryCount, itemsPerCategory);

        // Assert
        // Verify items have increasing prices (5.00 + i * 2.00)
        const string sql = @"
            SELECT price_amount 
            FROM menu_items 
            WHERE name LIKE 'Drink %'
            ORDER BY display_order";

        await using var command = new NpgsqlCommand(sql, _connection);
        await using var reader = await command.ExecuteReaderAsync();

        var expectedPrices = new[] { 7.00m, 9.00m, 11.00m };
        var actualPrices = new List<decimal>();

        while (await reader.ReadAsync())
        {
            actualPrices.Add(reader.GetDecimal(0));
        }

        actualPrices.Should().BeEquivalentTo(expectedPrices);
    }

    [Fact]
    public async Task SeedMenuItemsAsync_CreatesItemsWithCorrectDisplayOrder()
    {
        // Arrange
        const int categoryCount = 1;
        const int itemsPerCategory = 5;

        // Act
        await _engine.SeedMenuItemsAsync(categoryCount, itemsPerCategory);

        // Assert
        const string sql = @"
            SELECT display_order 
            FROM menu_items 
            WHERE name LIKE 'Drink %'
            ORDER BY display_order";

        await using var command = new NpgsqlCommand(sql, _connection);
        await using var reader = await command.ExecuteReaderAsync();

        var displayOrders = new List<int>();
        while (await reader.ReadAsync())
        {
            displayOrders.Add(reader.GetInt32(0));
        }

        displayOrders.Should().BeInAscendingOrder();
        displayOrders.Should().BeEquivalentTo(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public async Task SeedMenuItemsAsync_WithZeroCategoryCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedMenuItemsAsync(0, 10);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("categoryCount");
    }

    [Fact]
    public async Task SeedMenuItemsAsync_WithZeroItemsPerCategory_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedMenuItemsAsync(5, 0);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("itemsPerCategory");
    }

    #endregion

    #region SeedTestUsersAsync Tests

    [Fact]
    public async Task SeedTestUsersAsync_CreatesUsersWithCorrectRoles()
    {
        // Arrange
        var users = new[]
        {
            ("test_server1", "Server"),
            ("test_manager1", "Manager"),
            ("test_admin1", "Admin")
        };

        // Act
        await _engine.SeedTestUsersAsync(users);

        // Assert
        foreach (var (username, role) in users)
        {
            const string sql = @"
                SELECT r.name 
                FROM users u
                JOIN roles r ON u.role_id = r.id
                WHERE u.username = @username";

            await using var command = new NpgsqlCommand(sql, _connection);
            command.Parameters.AddWithValue("@username", username);
            var actualRole = await command.ExecuteScalarAsync();
            actualRole.Should().NotBeNull();
            actualRole!.ToString().Should().Be(role);
        }
    }

    [Fact]
    public async Task SeedTestUsersAsync_CreatesCorrectNumberOfUsers()
    {
        // Arrange
        var users = new[]
        {
            ("test_user1", "Server"),
            ("test_user2", "Manager"),
            ("test_user3", "Admin")
        };

        // Act
        await _engine.SeedTestUsersAsync(users);

        // Assert
        var actualCount = await GetCountAsync("users", "username LIKE 'test_user%'");
        actualCount.Should().Be(3);
    }

    [Fact]
    public async Task SeedTestUsersAsync_CreatesUsersWithCorrectFields()
    {
        // Arrange
        var users = new[] { ("test_complete", "Server") };

        // Act
        await _engine.SeedTestUsersAsync(users);

        // Assert
        const string sql = @"
            SELECT username, first_name, last_name, encrypted_pin, is_active
            FROM users
            WHERE username = 'test_complete'";

        await using var command = new NpgsqlCommand(sql, _connection);
        await using var reader = await command.ExecuteReaderAsync();

        reader.Read().Should().BeTrue();
        reader.GetString(0).Should().Be("test_complete");
        reader.GetString(1).Should().Be("test_complete");
        reader.GetString(2).Should().Be("Test");
        reader.GetString(3).Should().Be("1234");
        reader.GetBoolean(4).Should().BeTrue();
    }

    [Fact]
    public async Task SeedTestUsersAsync_WithNullUsers_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedTestUsersAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("users");
    }

    [Fact]
    public async Task SeedTestUsersAsync_WithEmptyUsers_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedTestUsersAsync(Array.Empty<(string, string)>());

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("users");
    }

    [Fact]
    public async Task SeedTestUsersAsync_WithInvalidRole_ThrowsArgumentException()
    {
        // Arrange
        var users = new[] { ("test_invalid", "InvalidRole") };

        // Act
        var act = async () => await _engine.SeedTestUsersAsync(users);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Invalid role: InvalidRole*");
    }

    #endregion

    #region SeedInventoryItemsAsync Tests

    [Fact]
    public async Task SeedInventoryItemsAsync_CreatesCorrectNumberOfItems()
    {
        // Arrange
        const int count = 20;
        const int lowStockCount = 5;

        // Act
        await _engine.SeedInventoryItemsAsync(count, lowStockCount);

        // Assert
        var actualCount = await GetCountAsync("inventory_items", "sku LIKE 'SKU-%'");
        actualCount.Should().Be(count);
    }

    [Fact]
    public async Task SeedInventoryItemsAsync_CreatesCorrectNumberOfLowStockItems()
    {
        // Arrange
        const int count = 20;
        const int lowStockCount = 5;

        // Act
        await _engine.SeedInventoryItemsAsync(count, lowStockCount);

        // Assert
        // Low stock items have quantity_on_hand < minimum_level
        const string sql = "SELECT COUNT(*) FROM inventory_items WHERE quantity_on_hand < minimum_level AND sku LIKE 'SKU-%'";
        await using var command = new NpgsqlCommand(sql, _connection);
        var actualLowStockCount = await command.ExecuteScalarAsync();
        Convert.ToInt32(actualLowStockCount).Should().Be(lowStockCount);
    }

    [Fact]
    public async Task SeedInventoryItemsAsync_CreatesItemsWithCorrectQuantities()
    {
        // Arrange
        const int count = 10;
        const int lowStockCount = 3;

        // Act
        await _engine.SeedInventoryItemsAsync(count, lowStockCount);

        // Assert
        // First 3 items should have low stock (2.0), rest should have normal stock (50.0)
        const string sql = @"
            SELECT quantity_on_hand 
            FROM inventory_items 
            WHERE sku LIKE 'SKU-%'
            ORDER BY sku";

        await using var command = new NpgsqlCommand(sql, _connection);
        await using var reader = await command.ExecuteReaderAsync();

        var quantities = new List<decimal>();
        while (await reader.ReadAsync())
        {
            quantities.Add(reader.GetDecimal(0));
        }

        quantities.Take(lowStockCount).Should().OnlyContain(q => q == 2.0m);
        quantities.Skip(lowStockCount).Should().OnlyContain(q => q == 50.0m);
    }

    [Fact]
    public async Task SeedInventoryItemsAsync_WithZeroCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedInventoryItemsAsync(0, 0);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("count");
    }

    [Fact]
    public async Task SeedInventoryItemsAsync_WithNegativeLowStockCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedInventoryItemsAsync(20, -1);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("lowStockCount");
    }

    [Fact]
    public async Task SeedInventoryItemsAsync_WithLowStockCountGreaterThanCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedInventoryItemsAsync(10, 15);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("lowStockCount");
    }

    #endregion

    #region SeedCustomersAsync Tests

    [Fact]
    public async Task SeedCustomersAsync_CreatesCorrectNumberOfCustomers()
    {
        // Arrange
        const int count = 10;
        const int membershipCount = 3;

        // Act
        await _engine.SeedCustomersAsync(count, membershipCount);

        // Assert
        var actualCount = await GetCountAsync("customers", "email LIKE 'customer%@test.com'");
        actualCount.Should().Be(count);
    }

    [Fact]
    public async Task SeedCustomersAsync_CreatesCorrectNumberOfMembershipTiers()
    {
        // Arrange
        const int count = 10;
        const int membershipCount = 3;

        // Act
        await _engine.SeedCustomersAsync(count, membershipCount);

        // Assert
        var actualCount = await GetCountAsync("membership_tiers", "name IN ('Bronze', 'Silver', 'Gold')");
        actualCount.Should().Be(membershipCount);
    }

    [Fact]
    public async Task SeedCustomersAsync_CreatesCustomersWithCorrectFields()
    {
        // Arrange
        const int count = 1;
        const int membershipCount = 1;

        // Act
        await _engine.SeedCustomersAsync(count, membershipCount);

        // Assert
        const string sql = @"
            SELECT first_name, last_name, email, phone, is_active
            FROM customers
            WHERE email = 'customer1@test.com'";

        await using var command = new NpgsqlCommand(sql, _connection);
        await using var reader = await command.ExecuteReaderAsync();

        reader.Read().Should().BeTrue();
        reader.GetString(0).Should().Be("Customer1");
        reader.GetString(1).Should().Be("Test");
        reader.GetString(2).Should().Be("customer1@test.com");
        reader.GetString(3).Should().Be("+15550000001");
        reader.GetBoolean(4).Should().BeTrue();
    }

    [Fact]
    public async Task SeedCustomersAsync_WithZeroCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedCustomersAsync(0, 3);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("count");
    }

    [Fact]
    public async Task SeedCustomersAsync_WithNegativeMembershipCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _engine.SeedCustomersAsync(10, -1);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("membershipCount");
    }

    #endregion
}
