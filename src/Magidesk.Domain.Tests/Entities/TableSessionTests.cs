using System;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Entities;

/// <summary>
/// Unit tests for TableSession entity.
/// Tests all invariants, domain methods, and edge cases.
/// Target: ≥90% coverage
/// </summary>
public class TableSessionTests
{
    #region Start Tests

    [Fact]
    public void Start_WithValidParameters_CreatesTableSession()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        var hourlyRate = 15.00m;
        var guestCount = 4;
        var customerId = Guid.NewGuid();

        // Act
        var session = TableSession.Start(tableId, tableTypeId, hourlyRate, guestCount, customerId);

        // Assert
        Assert.NotNull(session);
        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal(tableId, session.TableId);
        Assert.Equal(tableTypeId, session.TableTypeId);
        Assert.Equal(hourlyRate, session.HourlyRate);
        Assert.Equal(guestCount, session.GuestCount);
        Assert.Equal(customerId, session.CustomerId);
        Assert.Equal(TableSessionStatus.Active, session.Status);
        Assert.Equal(TimeSpan.Zero, session.TotalPausedDuration);
        Assert.Null(session.EndTime);
        Assert.Null(session.PausedAt);
    }

    [Fact]
    public void Start_WithoutCustomerId_CreatesTableSession()
    {
        // Act
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);

        // Assert
        Assert.Null(session.CustomerId);
    }

    [Fact]
    public void Start_WithEmptyTableId_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TableSession.Start(Guid.Empty, Guid.NewGuid(), 15.00m, 2));
        Assert.Contains("table id", exception.Message.ToLower());
    }

    [Fact]
    public void Start_WithEmptyTableTypeId_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TableSession.Start(Guid.NewGuid(), Guid.Empty, 15.00m, 2));
        Assert.Contains("table type id", exception.Message.ToLower());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.50)]
    public void Start_WithInvalidHourlyRate_ThrowsArgumentException(decimal invalidRate)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), invalidRate, 2));
        Assert.Contains("hourly rate", exception.Message.ToLower());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Start_WithInvalidGuestCount_ThrowsArgumentException(int invalidCount)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, invalidCount));
        Assert.Contains("guest count", exception.Message.ToLower());
    }

    #endregion

    #region Pause Tests

    [Fact]
    public void Pause_ActiveSession_PausesSuccessfully()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);

        // Act
        session.Pause();

        // Assert
        Assert.Equal(TableSessionStatus.Paused, session.Status);
        Assert.NotNull(session.PausedAt);
    }

    [Fact]
    public void Pause_AlreadyPausedSession_ThrowsInvalidOperationException()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        session.Pause();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => session.Pause());
        Assert.Contains("already paused", exception.Message.ToLower());
    }

    [Fact]
    public void Pause_EndedSession_ThrowsInvalidOperationException()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        session.End(Money.Zero());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => session.Pause());
        Assert.Contains("ended", exception.Message.ToLower());
    }

    #endregion

    #region Resume Tests

    [Fact]
    public void Resume_PausedSession_ResumesSuccessfully()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        session.Pause();
        System.Threading.Thread.Sleep(10); // Small delay to ensure time difference

        // Act
        session.Resume();

        // Assert
        Assert.Equal(TableSessionStatus.Active, session.Status);
        Assert.Null(session.PausedAt);
        Assert.True(session.TotalPausedDuration > TimeSpan.Zero);
    }

    [Fact]
    public void Resume_ActiveSession_ThrowsInvalidOperationException()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => session.Resume());
        Assert.Contains("paused session", exception.Message.ToLower());
    }

    #endregion

    #region End Tests

    [Fact]
    public void End_ActiveSession_EndsSuccessfully()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        var charge = new Money(25.50m);

        // Act
        session.End(charge);

        // Assert
        Assert.Equal(TableSessionStatus.Ended, session.Status);
        Assert.NotNull(session.EndTime);
        Assert.Equal(charge, session.TotalCharge);
    }

    [Fact]
    public void End_WithNullCharge_ThrowsArgumentNullException()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => session.End(null!));
    }

    [Fact]
    public void End_PausedSession_ThrowsInvalidOperationException()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        session.Pause();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => session.End(Money.Zero()));
        Assert.Contains("paused", exception.Message.ToLower());
    }

    [Fact]
    public void End_AlreadyEndedSession_ThrowsInvalidOperationException()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        session.End(Money.Zero());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => session.End(Money.Zero()));
        Assert.Contains("already ended", exception.Message.ToLower());
    }

    #endregion

    #region GetBillableTime Tests

    [Fact]
    public void GetBillableTime_ActiveSession_ReturnsElapsedTime()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        System.Threading.Thread.Sleep(100); // Small delay

        // Act
        var billableTime = session.GetBillableTime();

        // Assert
        Assert.True(billableTime > TimeSpan.Zero);
        Assert.True(billableTime < TimeSpan.FromSeconds(1)); // Should be very small
    }

    [Fact]
    public void GetBillableTime_PausedSession_ExcludesPausedTime()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        System.Threading.Thread.Sleep(50);
        session.Pause();
        System.Threading.Thread.Sleep(100); // Paused time
        
        // Act
        var billableTime = session.GetBillableTime();

        // Assert
        // Billable time should be close to 50ms, not 150ms
        Assert.True(billableTime < TimeSpan.FromMilliseconds(80));
    }

    [Fact]
    public void GetBillableTime_WithMultiplePauses_ExcludesAllPausedTime()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        System.Threading.Thread.Sleep(50);
        
        session.Pause();
        System.Threading.Thread.Sleep(50); // First pause
        session.Resume();
        
        System.Threading.Thread.Sleep(50);
        
        session.Pause();
        System.Threading.Thread.Sleep(50); // Second pause
        session.Resume();

        // Act
        var billableTime = session.GetBillableTime();

        // Assert
        // Should exclude both pauses (~100ms total), but allow for timing variance
        Assert.True(billableTime < TimeSpan.FromMilliseconds(150));
    }

    [Fact]
    public void GetBillableTime_EndedSession_ReturnsCorrectTime()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        System.Threading.Thread.Sleep(100);
        session.End(Money.Zero());

        // Act
        var billableTime = session.GetBillableTime();

        // Assert
        Assert.True(billableTime > TimeSpan.Zero);
    }

    #endregion

    #region LinkToTicket Tests

    [Fact]
    public void LinkToTicket_WithValidTicketId_LinksSuccessfully()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        var ticketId = Guid.NewGuid();

        // Act
        session.LinkToTicket(ticketId);

        // Assert
        Assert.Equal(ticketId, session.TicketId);
    }

    [Fact]
    public void LinkToTicket_WithEmptyTicketId_ThrowsArgumentException()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => session.LinkToTicket(Guid.Empty));
        Assert.Contains("ticket id", exception.Message.ToLower());
    }

    #endregion

    #region UpdateGuestCount Tests

    [Fact]
    public void UpdateGuestCount_WithValidCount_UpdatesSuccessfully()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);

        // Act
        session.UpdateGuestCount(5);

        // Assert
        Assert.Equal(5, session.GuestCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void UpdateGuestCount_WithInvalidCount_ThrowsArgumentException(int invalidCount)
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => session.UpdateGuestCount(invalidCount));
        Assert.Contains("guest count", exception.Message.ToLower());
    }

    [Fact]
    public void UpdateGuestCount_OnEndedSession_ThrowsInvalidOperationException()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        session.End(Money.Zero());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => session.UpdateGuestCount(5));
        Assert.Contains("ended", exception.Message.ToLower());
    }

    #endregion

    #region Edge Cases and Invariants

    [Fact]
    public void Start_SetsCreatedAtAndUpdatedAt()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);

        // Assert
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.InRange(session.CreatedAt, before, after);
        Assert.InRange(session.UpdatedAt, before, after);
        Assert.InRange(session.StartTime, before, after);
    }

    [Fact]
    public void Pause_UpdatesUpdatedAt()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        var originalUpdatedAt = session.UpdatedAt;
        System.Threading.Thread.Sleep(10);

        // Act
        session.Pause();

        // Assert
        Assert.True(session.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void Resume_UpdatesUpdatedAt()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        session.Pause();
        var originalUpdatedAt = session.UpdatedAt;
        System.Threading.Thread.Sleep(10);

        // Act
        session.Resume();

        // Assert
        Assert.True(session.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void End_UpdatesUpdatedAt()
    {
        // Arrange
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);
        var originalUpdatedAt = session.UpdatedAt;
        System.Threading.Thread.Sleep(10);

        // Act
        session.End(Money.Zero());

        // Assert
        Assert.True(session.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void TotalCharge_InitializesToZero()
    {
        // Act
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15.00m, 2);

        // Assert
        Assert.Equal(Money.Zero(), session.TotalCharge);
    }

    #endregion
}
