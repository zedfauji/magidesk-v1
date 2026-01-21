using System;
using Xunit;
using Magidesk.Domain.Entities;

namespace Magidesk.Domain.Tests.Entities;

/// <summary>
/// Unit tests for TableType entity.
/// Tests all invariants, domain methods, and edge cases.
/// Target: ≥90% coverage
/// </summary>
public class TableTypeTests
{
    #region Create Tests

    [Fact]
    public void Create_WithValidParameters_CreatesTableType()
    {
        // Arrange
        var name = "Pool Table";
        var hourlyRate = 15.00m;
        var description = "Standard pool table";

        // Act
        var tableType = TableType.Create(name, hourlyRate, description);

        // Assert
        Assert.NotNull(tableType);
        Assert.NotEqual(Guid.Empty, tableType.Id);
        Assert.Equal(name, tableType.Name);
        Assert.Equal(hourlyRate, tableType.HourlyRate);
        Assert.Equal(description, tableType.Description);
        Assert.Null(tableType.FirstHourRate);
        Assert.Equal(0, tableType.MinimumMinutes);
        Assert.Equal(1, tableType.RoundingMinutes);
        Assert.True(tableType.IsActive);
    }

    [Fact]
    public void Create_WithoutDescription_CreatesTableTypeWithEmptyDescription()
    {
        // Act
        var tableType = TableType.Create("Snooker Table", 20.00m);

        // Assert
        Assert.Equal(string.Empty, tableType.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ThrowsArgumentException(string invalidName)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            TableType.Create(invalidName, 15.00m));
        Assert.Contains("name", exception.Message.ToLower());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.50)]
    public void Create_WithInvalidHourlyRate_ThrowsArgumentException(decimal invalidRate)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            TableType.Create("Pool Table", invalidRate));
        Assert.Contains("hourly rate", exception.Message.ToLower());
    }

    [Fact]
    public void Create_TrimsNameAndDescription()
    {
        // Act
        var tableType = TableType.Create("  Pool Table  ", 15.00m, "  Description  ");

        // Assert
        Assert.Equal("Pool Table", tableType.Name);
        Assert.Equal("Description", tableType.Description);
    }

    #endregion

    #region UpdateRates Tests

    [Fact]
    public void UpdateRates_WithValidRates_UpdatesRates()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);
        var newHourlyRate = 18.00m;
        var newFirstHourRate = 20.00m;

        // Act
        tableType.UpdateRates(newHourlyRate, newFirstHourRate);

        // Assert
        Assert.Equal(newHourlyRate, tableType.HourlyRate);
        Assert.Equal(newFirstHourRate, tableType.FirstHourRate);
    }

    [Fact]
    public void UpdateRates_WithNullFirstHourRate_ClearsFirstHourRate()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);
        tableType.UpdateRates(15.00m, 20.00m); // Set first hour rate

        // Act
        tableType.UpdateRates(18.00m, null); // Clear it

        // Assert
        Assert.Equal(18.00m, tableType.HourlyRate);
        Assert.Null(tableType.FirstHourRate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5.50)]
    public void UpdateRates_WithInvalidHourlyRate_ThrowsArgumentException(decimal invalidRate)
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            tableType.UpdateRates(invalidRate));
        Assert.Contains("hourly rate", exception.Message.ToLower());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5.50)]
    public void UpdateRates_WithInvalidFirstHourRate_ThrowsArgumentException(decimal invalidRate)
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            tableType.UpdateRates(15.00m, invalidRate));
        Assert.Contains("first hour rate", exception.Message.ToLower());
    }

    #endregion

    #region SetRounding Tests

    [Fact]
    public void SetRounding_WithValidValues_UpdatesRounding()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);

        // Act
        tableType.SetRounding(minimumMinutes: 30, roundingMinutes: 15);

        // Assert
        Assert.Equal(30, tableType.MinimumMinutes);
        Assert.Equal(15, tableType.RoundingMinutes);
    }

    [Fact]
    public void SetRounding_WithZeroMinimum_IsValid()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);

        // Act
        tableType.SetRounding(minimumMinutes: 0, roundingMinutes: 5);

        // Assert
        Assert.Equal(0, tableType.MinimumMinutes);
    }

    [Fact]
    public void SetRounding_WithNegativeMinimum_ThrowsArgumentException()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            tableType.SetRounding(minimumMinutes: -1, roundingMinutes: 5));
        Assert.Contains("minimum minutes", exception.Message.ToLower());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void SetRounding_WithInvalidRoundingMinutes_ThrowsArgumentException(int invalidRounding)
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            tableType.SetRounding(minimumMinutes: 0, roundingMinutes: invalidRounding));
        Assert.Contains("rounding minutes", exception.Message.ToLower());
    }

    #endregion

    #region UpdateDetails Tests

    [Fact]
    public void UpdateDetails_WithValidValues_UpdatesNameAndDescription()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);

        // Act
        tableType.UpdateDetails("Billiards Table", "Professional billiards table");

        // Assert
        Assert.Equal("Billiards Table", tableType.Name);
        Assert.Equal("Professional billiards table", tableType.Description);
    }

    [Fact]
    public void UpdateDetails_TrimsValues()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);

        // Act
        tableType.UpdateDetails("  Snooker  ", "  Full size  ");

        // Assert
        Assert.Equal("Snooker", tableType.Name);
        Assert.Equal("Full size", tableType.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateDetails_WithEmptyName_ThrowsArgumentException(string invalidName)
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            tableType.UpdateDetails(invalidName));
        Assert.Contains("name", exception.Message.ToLower());
    }

    #endregion

    #region Activate/Deactivate Tests

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);
        Assert.True(tableType.IsActive);

        // Act
        tableType.Deactivate();

        // Assert
        Assert.False(tableType.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);
        tableType.Deactivate();
        Assert.False(tableType.IsActive);

        // Act
        tableType.Activate();

        // Assert
        Assert.True(tableType.IsActive);
    }

    #endregion

    #region Edge Cases and Invariants

    [Fact]
    public void Create_SetsCreatedAtAndUpdatedAt()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var tableType = TableType.Create("Pool Table", 15.00m);

        // Assert
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.InRange(tableType.CreatedAt, before, after);
        Assert.InRange(tableType.UpdatedAt, before, after);
    }

    [Fact]
    public void UpdateRates_UpdatesUpdatedAt()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);
        var originalUpdatedAt = tableType.UpdatedAt;
        System.Threading.Thread.Sleep(10); // Small delay to ensure time difference

        // Act
        tableType.UpdateRates(18.00m);

        // Assert
        Assert.True(tableType.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void SetRounding_UpdatesUpdatedAt()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);
        var originalUpdatedAt = tableType.UpdatedAt;
        System.Threading.Thread.Sleep(10);

        // Act
        tableType.SetRounding(30, 15);

        // Assert
        Assert.True(tableType.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void UpdateDetails_UpdatesUpdatedAt()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);
        var originalUpdatedAt = tableType.UpdatedAt;
        System.Threading.Thread.Sleep(10);

        // Act
        tableType.UpdateDetails("Snooker Table");

        // Assert
        Assert.True(tableType.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void Deactivate_UpdatesUpdatedAt()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);
        var originalUpdatedAt = tableType.UpdatedAt;
        System.Threading.Thread.Sleep(10);

        // Act
        tableType.Deactivate();

        // Assert
        Assert.True(tableType.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void Activate_UpdatesUpdatedAt()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 15.00m);
        tableType.Deactivate();
        var originalUpdatedAt = tableType.UpdatedAt;
        System.Threading.Thread.Sleep(10);

        // Act
        tableType.Activate();

        // Assert
        Assert.True(tableType.UpdatedAt > originalUpdatedAt);
    }

    #endregion
}
