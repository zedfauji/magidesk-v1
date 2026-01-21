using System;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Entities;

public class CustomerTests
{
    [Fact]
    public void Create_WithValidData_ReturnsCustomer()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var phone = "+1234567890";
        var email = "john.doe@example.com";

        // Act
        var customer = Customer.Create(firstName, lastName, phone, email);

        // Assert
        Assert.NotNull(customer);
        Assert.Equal(firstName, customer.FirstName);
        Assert.Equal(lastName, customer.LastName);
        Assert.Equal(phone, customer.Phone);
        Assert.Equal(email, customer.Email);
        Assert.True(customer.IsActive);
        Assert.Equal(Money.Zero(), customer.TotalSpent);
        Assert.Equal(0, customer.TotalVisits);
    }

    [Theory]
    [InlineData("", "Doe", "+1234567890")]
    [InlineData("John", "", "+1234567890")]
    [InlineData("John", "Doe", "")]
    [InlineData("John", "Doe", "invalid-phone")]
    public void Create_WithInvalidData_ThrowsBusinessRuleViolationException(string firstName, string lastName, string phone)
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() => Customer.Create(firstName, lastName, phone));
    }

    [Fact]
    public void UpdateContactInfo_WithValidData_UpdatesProperties()
    {
        // Arrange
        var customer = Customer.Create("John", "Doe", "+1234567890");
        var newEmail = "updated@example.com";
        var newPhone = "+9876543210";
        var address = "123 Main St";

        // Act
        customer.UpdateContactInfo(newEmail, newPhone, address);

        // Assert
        Assert.Equal(newEmail, customer.Email);
        Assert.Equal(newPhone, customer.Phone);
        Assert.Equal(address, customer.Address);
    }

    [Fact]
    public void RecordVisit_IncrementsCountersAndUpdatesTotalSpent()
    {
        // Arrange
        var customer = Customer.Create("John", "Doe", "+1234567890");
        var spent = new Money(50.00m);
        var visitTime = DateTime.UtcNow;

        // Act
        customer.RecordVisit(visitTime, spent);

        // Assert
        Assert.Equal(1, customer.TotalVisits);
        Assert.Equal(spent, customer.TotalSpent);
        Assert.Equal(visitTime, customer.LastVisitAt);
    }

    [Fact]
    public void RecordVisit_WithNegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        var customer = Customer.Create("John", "Doe", "+1234567890");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => customer.RecordVisit(DateTime.UtcNow, new Money(-10.00m)));
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var customer = Customer.Create("John", "Doe", "+1234567890");

        // Act
        customer.Deactivate();

        // Assert
        Assert.False(customer.IsActive);
    }
}
