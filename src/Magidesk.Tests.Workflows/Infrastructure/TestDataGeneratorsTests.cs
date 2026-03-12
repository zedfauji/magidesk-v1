using FluentAssertions;
using FsCheck;
using Xunit;

namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Unit tests for TestDataGenerators to verify FsCheck generators produce valid data.
/// </summary>
public class TestDataGeneratorsTests
{
    [Fact]
    public void TicketGenerator_ShouldGenerateValidTickets()
    {
        // Arrange
        var generator = TestDataGenerators.TicketGenerator();
        
        // Act
        var ticket = generator.Generator.Sample(1, 1).First();
        
        // Assert
        ticket.Should().NotBeNull();
        ticket.Items.Should().NotBeEmpty();
        ticket.Items.Count.Should().BeInRange(1, 20);
        ticket.Total.Should().BeGreaterThan(0);
        ticket.Total.Should().Be(ticket.Items.Sum(i => i.Price * i.Quantity));
    }
    
    [Fact]
    public void MenuItemGenerator_ShouldGenerateValidMenuItems()
    {
        // Arrange
        var generator = TestDataGenerators.MenuItemGenerator();
        
        // Act
        var menuItem = generator.Generator.Sample(1, 1).First();
        
        // Assert
        menuItem.Should().NotBeNull();
        menuItem.Name.Should().NotBeNullOrEmpty();
        menuItem.Price.Should().BeInRange(1.00m, 50.00m);
        menuItem.Quantity.Should().BeInRange(1, 10);
        menuItem.LineTotal.Should().Be(menuItem.Price * menuItem.Quantity);
    }
    
    [Fact]
    public void PaymentGenerator_ShouldGenerateValidPayments()
    {
        // Arrange
        var generator = TestDataGenerators.PaymentGenerator();
        
        // Act
        var payment = generator.Generator.Sample(1, 1).First();
        
        // Assert
        payment.Should().NotBeNull();
        payment.Method.Should().NotBeNullOrEmpty();
        payment.Method.Should().BeOneOf("Cash", "Credit", "Debit", "GiftCertificate");
        payment.Amount.Should().BeInRange(1.00m, 100.00m);
    }
    
    [Fact]
    public void CustomerGenerator_ShouldGenerateValidCustomers()
    {
        // Arrange
        var generator = TestDataGenerators.CustomerGenerator();
        
        // Act
        var customer = generator.Generator.Sample(1, 1).First();
        
        // Assert
        customer.Should().NotBeNull();
        customer.Name.Should().NotBeNullOrEmpty();
        customer.Phone.Should().NotBeNullOrEmpty();
        customer.Phone.Length.Should().Be(10);
        customer.Email.Should().NotBeNullOrEmpty();
        customer.Email.Should().Contain("@");
    }
    
    [Fact]
    public void CashSessionGenerator_ShouldGenerateValidCashSessions()
    {
        // Arrange
        var generator = TestDataGenerators.CashSessionGenerator();
        
        // Act
        var cashSession = generator.Generator.Sample(1, 1).First();
        
        // Assert
        cashSession.Should().NotBeNull();
        cashSession.StartingBalance.Should().BeInRange(100.00m, 1000.00m);
        cashSession.Receipts.Should().NotBeNull();
        cashSession.Receipts.Should().HaveCountLessOrEqualTo(20);
        cashSession.Disbursements.Should().NotBeNull();
        cashSession.Disbursements.Should().HaveCountLessOrEqualTo(20);
        
        // Verify ending balance calculation
        var expectedEndingBalance = cashSession.StartingBalance 
            + cashSession.Receipts.Sum() 
            - cashSession.Disbursements.Sum();
        cashSession.EndingBalance.Should().Be(expectedEndingBalance);
    }
    
    [Fact]
    public void TicketGenerator_ShouldGenerateMultipleUniqueTickets()
    {
        // Arrange
        var generator = TestDataGenerators.TicketGenerator();
        
        // Act
        var tickets = generator.Generator.Sample(10, 10).ToList();
        
        // Assert
        tickets.Should().HaveCount(10);
        tickets.Should().OnlyContain(t => t.Items.Count >= 1 && t.Items.Count <= 20);
    }
    
    [Fact]
    public void MenuItemGenerator_ShouldGenerateVarietyOfItems()
    {
        // Arrange
        var generator = TestDataGenerators.MenuItemGenerator();
        
        // Act
        var items = generator.Generator.Sample(50, 50).ToList();
        
        // Assert
        items.Should().HaveCount(50);
        var uniqueNames = items.Select(i => i.Name).Distinct().Count();
        uniqueNames.Should().BeGreaterThan(1, "should generate variety of menu items");
    }
    
    [Fact]
    public void CashSessionGenerator_EndingBalanceInvariant_ShouldAlwaysHold()
    {
        // Arrange
        var generator = TestDataGenerators.CashSessionGenerator();
        
        // Act - Generate 100 samples to verify invariant
        var sessions = generator.Generator.Sample(100, 100).ToList();
        
        // Assert - Verify ending balance calculation for all samples
        foreach (var session in sessions)
        {
            var expectedEndingBalance = session.StartingBalance 
                + session.Receipts.Sum() 
                - session.Disbursements.Sum();
            session.EndingBalance.Should().Be(expectedEndingBalance, 
                "ending balance invariant must hold for all generated sessions");
        }
    }
}
