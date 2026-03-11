namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Extends IDatabaseResetEngine with additional capabilities for comprehensive testing scenarios.
/// Provides methods to seed test data for specific scenarios including pool tables, dining tables,
/// menu items, test users, inventory items, and customers.
/// </summary>
public interface IEnhancedDatabaseResetEngine : IDatabaseResetEngine
{
    /// <summary>
    /// Seeds pool tables with hourly rates for testing pool table management scenarios.
    /// </summary>
    /// <param name="count">Number of pool tables to create. Default is 5.</param>
    Task SeedPoolTablesAsync(int count = 5);

    /// <summary>
    /// Seeds dining tables across multiple floors for testing table management scenarios.
    /// </summary>
    /// <param name="count">Number of dining tables to create. Default is 10.</param>
    Task SeedDiningTablesAsync(int count = 10);

    /// <summary>
    /// Seeds menu items organized by categories for testing order entry scenarios.
    /// </summary>
    /// <param name="categoryCount">Number of categories to create. Default is 5.</param>
    /// <param name="itemsPerCategory">Number of items per category. Default is 10.</param>
    Task SeedMenuItemsAsync(int categoryCount = 5, int itemsPerCategory = 10);

    /// <summary>
    /// Seeds test users with specified roles for testing authentication and authorization scenarios.
    /// </summary>
    /// <param name="users">Array of tuples containing username and role pairs.</param>
    Task SeedTestUsersAsync(params (string username, string role)[] users);

    /// <summary>
    /// Seeds inventory items with configurable low stock thresholds for testing inventory management scenarios.
    /// </summary>
    /// <param name="count">Total number of inventory items to create. Default is 20.</param>
    /// <param name="lowStockCount">Number of items to set with low stock levels. Default is 5.</param>
    Task SeedInventoryItemsAsync(int count = 20, int lowStockCount = 5);

    /// <summary>
    /// Seeds customer profiles with membership tiers for testing customer management scenarios.
    /// </summary>
    /// <param name="count">Total number of customers to create. Default is 10.</param>
    /// <param name="membershipCount">Number of membership tiers to create. Default is 3.</param>
    Task SeedCustomersAsync(int count = 10, int membershipCount = 3);
}
