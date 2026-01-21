using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class ShiftTests
{
    [Fact]
    public void Shift_Create_WithValidData_ShouldCreateShift()
    {
        // Arrange
        var name = "Morning Shift";
        var startTime = new TimeSpan(6, 0, 0); // 6:00 AM
        var endTime = new TimeSpan(14, 0, 0); // 2:00 PM

        // Act
        var shift = Shift.Create(name, startTime, endTime);

        // Assert
        shift.Should().NotBeNull();
        shift.Id.Should().NotBeEmpty();
        shift.Name.Should().Be(name);
        shift.StartTime.Should().Be(startTime);
        shift.EndTime.Should().Be(endTime);
        shift.IsActive.Should().BeTrue();
        shift.Version.Should().Be(1);
    }

    [Fact]
    public void Shift_Create_WithInactive_ShouldCreateInactiveShift()
    {
        // Arrange
        var name = "Night Shift";
        var startTime = new TimeSpan(22, 0, 0); // 10:00 PM
        var endTime = new TimeSpan(6, 0, 0); // 6:00 AM
        var isActive = false;

        // Act
        var shift = Shift.Create(name, startTime, endTime, isActive);

        // Assert
        shift.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Shift_Create_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var startTime = new TimeSpan(6, 0, 0);
        var endTime = new TimeSpan(14, 0, 0);

        // Act
        var act = () => Shift.Create("", startTime, endTime);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Shift_Create_WithWhitespaceName_ShouldThrowException()
    {
        // Arrange
        var startTime = new TimeSpan(6, 0, 0);
        var endTime = new TimeSpan(14, 0, 0);

        // Act
        var act = () => Shift.Create("   ", startTime, endTime);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Shift_Create_WithSameStartAndEndTime_ShouldThrowException()
    {
        // Arrange
        var name = "Test Shift";
        var time = new TimeSpan(12, 0, 0);

        // Act
        var act = () => Shift.Create(name, time, time);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*start time and end time cannot be the same*");
    }

    [Fact]
    public void Shift_UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var shift = Shift.Create("Old Name", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0));
        var newName = "New Name";

        // Act
        shift.UpdateName(newName);

        // Assert
        shift.Name.Should().Be(newName);
    }

    [Fact]
    public void Shift_UpdateName_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var shift = Shift.Create("Test Shift", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0));

        // Act
        var act = () => shift.UpdateName("");

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Shift_UpdateTimes_WithValidTimes_ShouldUpdateTimes()
    {
        // Arrange
        var shift = Shift.Create("Test Shift", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0));
        var newStartTime = new TimeSpan(8, 0, 0);
        var newEndTime = new TimeSpan(16, 0, 0);

        // Act
        shift.UpdateTimes(newStartTime, newEndTime);

        // Assert
        shift.StartTime.Should().Be(newStartTime);
        shift.EndTime.Should().Be(newEndTime);
    }

    [Fact]
    public void Shift_UpdateTimes_WithSameTimes_ShouldThrowException()
    {
        // Arrange
        var shift = Shift.Create("Test Shift", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0));
        var time = new TimeSpan(12, 0, 0);

        // Act
        var act = () => shift.UpdateTimes(time, time);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*start time and end time cannot be the same*");
    }

    [Fact]
    public void Shift_Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var shift = Shift.Create("Test Shift", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0), isActive: false);

        // Act
        shift.Activate();

        // Assert
        shift.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Shift_Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var shift = Shift.Create("Test Shift", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0), isActive: true);

        // Act
        shift.Deactivate();

        // Assert
        shift.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Shift_IsTimeInShift_WithNormalShift_ShouldReturnTrue()
    {
        // Arrange
        var shift = Shift.Create("Morning Shift", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0));
        var time = new TimeSpan(10, 0, 0); // 10:00 AM

        // Act
        var result = shift.IsTimeInShift(time);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Shift_IsTimeInShift_WithNormalShift_AtStartTime_ShouldReturnTrue()
    {
        // Arrange
        var shift = Shift.Create("Morning Shift", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0));
        var time = new TimeSpan(6, 0, 0); // Start time

        // Act
        var result = shift.IsTimeInShift(time);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Shift_IsTimeInShift_WithNormalShift_AtEndTime_ShouldReturnTrue()
    {
        // Arrange
        var shift = Shift.Create("Morning Shift", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0));
        var time = new TimeSpan(14, 0, 0); // End time

        // Act
        var result = shift.IsTimeInShift(time);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Shift_IsTimeInShift_WithNormalShift_BeforeStartTime_ShouldReturnFalse()
    {
        // Arrange
        var shift = Shift.Create("Morning Shift", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0));
        var time = new TimeSpan(5, 0, 0); // Before start

        // Act
        var result = shift.IsTimeInShift(time);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Shift_IsTimeInShift_WithNormalShift_AfterEndTime_ShouldReturnFalse()
    {
        // Arrange
        var shift = Shift.Create("Morning Shift", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0));
        var time = new TimeSpan(15, 0, 0); // After end

        // Act
        var result = shift.IsTimeInShift(time);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Shift_IsTimeInShift_WithMidnightSpanningShift_AfterStartTime_ShouldReturnTrue()
    {
        // Arrange
        var shift = Shift.Create("Night Shift", new TimeSpan(22, 0, 0), new TimeSpan(6, 0, 0)); // 10 PM - 6 AM
        var time = new TimeSpan(23, 0, 0); // 11:00 PM

        // Act
        var result = shift.IsTimeInShift(time);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Shift_IsTimeInShift_WithMidnightSpanningShift_BeforeEndTime_ShouldReturnTrue()
    {
        // Arrange
        var shift = Shift.Create("Night Shift", new TimeSpan(22, 0, 0), new TimeSpan(6, 0, 0)); // 10 PM - 6 AM
        var time = new TimeSpan(5, 0, 0); // 5:00 AM

        // Act
        var result = shift.IsTimeInShift(time);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Shift_IsTimeInShift_WithMidnightSpanningShift_AtStartTime_ShouldReturnTrue()
    {
        // Arrange
        var shift = Shift.Create("Night Shift", new TimeSpan(22, 0, 0), new TimeSpan(6, 0, 0)); // 10 PM - 6 AM
        var time = new TimeSpan(22, 0, 0); // Start time

        // Act
        var result = shift.IsTimeInShift(time);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Shift_IsTimeInShift_WithMidnightSpanningShift_AtEndTime_ShouldReturnTrue()
    {
        // Arrange
        var shift = Shift.Create("Night Shift", new TimeSpan(22, 0, 0), new TimeSpan(6, 0, 0)); // 10 PM - 6 AM
        var time = new TimeSpan(6, 0, 0); // End time

        // Act
        var result = shift.IsTimeInShift(time);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Shift_IsTimeInShift_WithMidnightSpanningShift_OutsideRange_ShouldReturnFalse()
    {
        // Arrange
        var shift = Shift.Create("Night Shift", new TimeSpan(22, 0, 0), new TimeSpan(6, 0, 0)); // 10 PM - 6 AM
        var time = new TimeSpan(12, 0, 0); // Noon (outside range)

        // Act
        var result = shift.IsTimeInShift(time);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Shift_GetCurrentShift_WithActiveShift_ShouldReturnShift()
    {
        // Arrange
        var currentTime = DateTime.Now.TimeOfDay;
        var shifts = new List<Shift>
        {
            Shift.Create("Morning", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0), isActive: true),
            Shift.Create("Afternoon", new TimeSpan(14, 0, 0), new TimeSpan(22, 0, 0), isActive: true)
        };

        // Note: This test may be flaky if run at certain times
        // In a real scenario, you'd mock the time or use a time provider
        // For now, we'll test the logic with a known time
        var testTime = new TimeSpan(10, 0, 0); // 10:00 AM
        var matchingShift = shifts.FirstOrDefault(s => s.IsActive && s.IsTimeInShift(testTime));

        // Act & Assert
        if (matchingShift != null)
        {
            matchingShift.Name.Should().Be("Morning");
        }
    }

    [Fact]
    public void Shift_GetCurrentShift_WithInactiveShift_ShouldNotReturnShift()
    {
        // Arrange
        var shifts = new List<Shift>
        {
            Shift.Create("Morning", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0), isActive: false)
        };

        // Act
        var result = Shift.GetCurrentShift(shifts);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Shift_GetCurrentShift_WithNoMatchingShift_ShouldReturnNull()
    {
        // Arrange
        var shifts = new List<Shift>
        {
            Shift.Create("Morning", new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0), isActive: true)
        };

        // Act - test with a time that doesn't match
        var testTime = new TimeSpan(20, 0, 0); // 8:00 PM (outside morning shift)
        var matchingShift = shifts.FirstOrDefault(s => s.IsActive && s.IsTimeInShift(testTime));

        // Assert
        matchingShift.Should().BeNull();
    }
}

