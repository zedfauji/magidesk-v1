using FluentAssertions;
using Magidesk.Domain.ValueObjects;
using System;
using Xunit;

namespace Magidesk.Domain.Tests.ValueObjects;

public class UserIdTests
{
    [Fact]
    public void Constructor_WithValidGuid_ShouldCreateUserId()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var userId = new UserId(guid);

        // Assert
        userId.Value.Should().Be(guid);
    }

    [Fact]
    public void Constructor_WithEmptyGuid_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new UserId(Guid.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("UserId cannot be empty Guid.*");
    }

    [Fact]
    public void ImplicitConversion_FromGuid_ShouldCreateUserId()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        UserId userId = guid;

        // Assert
        userId.Value.Should().Be(guid);
    }

    [Fact]
    public void ImplicitConversion_ToGuid_ShouldReturnGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        // Act
        Guid result = userId;

        // Assert
        result.Should().Be(guid);
    }

    [Fact]
    public void FromString_WithValidGuidString_ShouldCreateUserId()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var guidString = guid.ToString();

        // Act
        var userId = UserId.FromString(guidString);

        // Assert
        userId.Value.Should().Be(guid);
    }

    [Fact]
    public void FromString_WithNullString_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => UserId.FromString(null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("UserId string cannot be null or empty.*");
    }

    [Fact]
    public void FromString_WithEmptyString_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => UserId.FromString("");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("UserId string cannot be null or empty.*");
    }

    [Fact]
    public void FromString_WithInvalidGuidString_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => UserId.FromString("invalid-guid");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid Guid format*");
    }

    [Fact]
    public void Equality_WithSameGuid_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId1 = new UserId(guid);
        var userId2 = new UserId(guid);

        // Act & Assert
        (userId1 == userId2).Should().BeTrue();
        userId1.Equals(userId2).Should().BeTrue();
    }

    [Fact]
    public void Equality_WithDifferentGuid_ShouldNotBeEqual()
    {
        // Arrange
        var userId1 = new UserId(Guid.NewGuid());
        var userId2 = new UserId(Guid.NewGuid());

        // Act & Assert
        (userId1 == userId2).Should().BeFalse();
        userId1.Equals(userId2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        // Act
        var result = userId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }
}

