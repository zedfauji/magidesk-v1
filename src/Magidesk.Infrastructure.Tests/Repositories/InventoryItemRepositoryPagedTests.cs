using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.Repositories;

namespace Magidesk.Infrastructure.Tests.Repositories;

/// <summary>
/// Integration tests for InventoryItemRepository.GetPagedAsync paging, search, and filter functionality.
/// NOTE: Requires PostgreSQL test database — run in CI with test DB provisioned at magidesk_test
/// </summary>
[Collection("Database Tests")]
public class InventoryItemRepositoryPagedTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly InventoryItemRepository _repository;

    public InventoryItemRepositoryPagedTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres;")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new InventoryItemRepository(_context);

        // Ensure a clean schema for each test run
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetPagedAsync_NoFilters_ReturnsPaginatedResults()
    {
        // Arrange
        var items = Enumerable.Range(1, 25)
            .Select(i => InventoryItem.Create($"Item {i:D2}", "unit", 10m + i, 5m + i))
            .ToList();

        foreach (var item in items)
        {
            await _repository.AddAsync(item);
        }

        // Act
        var (pagedItems, totalCount) = await _repository.GetPagedAsync(null, InventoryFilterType.None, null, 0, 10);

        // Assert
        Assert.Equal(25, totalCount);
        Assert.Equal(10, pagedItems.Count);
    }

    [Fact]
    public async Task GetPagedAsync_SearchByName_ReturnsMatchingItems()
    {
        // Arrange
        var testItem = InventoryItem.Create("TestSugar", "kg", 50m, 20m);
        var otherItem = InventoryItem.Create("Flour", "kg", 40m, 15m);

        await _repository.AddAsync(testItem);
        await _repository.AddAsync(otherItem);

        // Act
        var (results, totalCount) = await _repository.GetPagedAsync("sugar", InventoryFilterType.None, null, 0, 10);

        // Assert
        Assert.Single(results);
        Assert.Equal(1, totalCount);
        Assert.Equal("TestSugar", results.First().Name);
    }

    [Fact]
    public async Task GetPagedAsync_SearchBySkuCode_ReturnsMatchingItems()
    {
        // Arrange
        var item1 = InventoryItem.Create("Coffee", "kg", 30m, 10m, "SKU-FIND-ME");
        var item2 = InventoryItem.Create("Tea", "kg", 20m, 8m, "SKU-OTHER");

        await _repository.AddAsync(item1);
        await _repository.AddAsync(item2);

        // Act
        var (results, totalCount) = await _repository.GetPagedAsync("FIND-ME", InventoryFilterType.None, null, 0, 10);

        // Assert
        Assert.Single(results);
        Assert.Equal(1, totalCount);
        Assert.Equal("SKU-FIND-ME", results.First().SkuCode);
    }

    [Fact]
    public async Task GetPagedAsync_LowStockFilter_ReturnsOnlyLowStockItems()
    {
        // Arrange
        var lowStockItem = InventoryItem.Create("LowStock Item", "unit", 3m, 5m);     // 3 <= 5 and > 0
        var normalItem = InventoryItem.Create("Normal Item", "unit", 15m, 5m);         // 15 > 5
        var outOfStockItem = InventoryItem.Create("OutOfStock Item", "unit", 0m, 5m); // 0 == 0, not low stock

        await _repository.AddAsync(lowStockItem);
        await _repository.AddAsync(normalItem);
        await _repository.AddAsync(outOfStockItem);

        // Act
        var (results, totalCount) = await _repository.GetPagedAsync(null, InventoryFilterType.LowStock, null, 0, 10);

        // Assert
        Assert.Single(results);
        Assert.Equal(1, totalCount);
        Assert.Equal("LowStock Item", results.First().Name);
    }

    [Fact]
    public async Task GetPagedAsync_OutOfStockFilter_ReturnsOnlyZeroStockItems()
    {
        // Arrange
        var outOfStockItem1 = InventoryItem.Create("Empty Item 1", "unit", 0m, 5m);
        var outOfStockItem2 = InventoryItem.Create("Empty Item 2", "unit", 0m, 10m);
        var inStockItem = InventoryItem.Create("In Stock Item", "unit", 1m, 5m);

        await _repository.AddAsync(outOfStockItem1);
        await _repository.AddAsync(outOfStockItem2);
        await _repository.AddAsync(inStockItem);

        // Act
        var (results, totalCount) = await _repository.GetPagedAsync(null, InventoryFilterType.OutOfStock, null, 0, 10);

        // Assert
        Assert.Equal(2, totalCount);
        Assert.Equal(2, results.Count);
        Assert.All(results, item => Assert.Equal(0m, item.StockQuantity));
    }

    [Fact]
    public async Task GetPagedAsync_CategoryFilter_ReturnsOnlyItemsInCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var otherCategoryId = Guid.NewGuid();

        var itemInCategory = InventoryItem.Create("Coffee", "kg", 30m, 10m, null, categoryId);
        var itemInOtherCategory = InventoryItem.Create("Tea", "kg", 20m, 8m, null, otherCategoryId);
        var itemWithoutCategory = InventoryItem.Create("Sugar", "kg", 50m, 20m);

        await _repository.AddAsync(itemInCategory);
        await _repository.AddAsync(itemInOtherCategory);
        await _repository.AddAsync(itemWithoutCategory);

        // Act
        var (results, totalCount) = await _repository.GetPagedAsync(null, InventoryFilterType.None, categoryId, 0, 10);

        // Assert
        Assert.Single(results);
        Assert.Equal(1, totalCount);
        Assert.Equal("Coffee", results.First().Name);
        Assert.Equal(categoryId, results.First().CategoryId);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
