using FluentAssertions;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace Magidesk.Infrastructure.Tests.Services;

public class CashBalanceTrackingServiceRefundTests
{
    private readonly UserId _userId = new UserId(Guid.NewGuid());
    private readonly Guid _terminalId = Guid.NewGuid();
    private readonly Guid _shiftId = Guid.NewGuid();

    [Fact]
    public async Task CashBalanceTrackingService_WhenRefundExists_ReducesExpectedDrawer()
    {
        // Arrange
        var session = CashSession.Open(_userId, _terminalId, _shiftId, new Money(100m));

        var sale = CreateSalePayment(session.Id, 200m);
        session.AddPayment(sale);

        var refund = CreateRefundPayment(sale, session.Id, 50m);
        session.AddPayment(refund);

        var service = BuildServiceWithSession(session);

        // Act
        var dto = await service.GetCurrentCashBalanceAsync(_terminalId);

        // Assert
        dto.Should().NotBeNull();
        dto!.TotalCashRefunds.Should().Be(50m);
        dto.CurrentBalance.Should().Be(250m); // 100 opening + 200 sale - 50 refund
    }

    [Fact]
    public async Task CashBalanceTrackingService_WhenNoRefunds_CurrentBalanceIncludesOnlySales()
    {
        // Arrange
        var session = CashSession.Open(_userId, _terminalId, _shiftId, new Money(100m));

        var sale = CreateSalePayment(session.Id, 200m);
        session.AddPayment(sale);

        var service = BuildServiceWithSession(session);

        // Act
        var dto = await service.GetCurrentCashBalanceAsync(_terminalId);

        // Assert
        dto.Should().NotBeNull();
        dto!.TotalCashRefunds.Should().Be(0m);
        dto.CurrentBalance.Should().Be(300m); // 100 opening + 200 sale
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private CashBalanceTrackingService BuildServiceWithSession(CashSession session)
    {
        var repoMock = new Mock<ICashSessionRepository>();
        repoMock
            .Setup(r => r.GetOpenSessionByTerminalIdAsync(_terminalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICashSessionRepository)))
            .Returns(repoMock.Object);

        var scopeMock = new Mock<IServiceScope>();
        scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);

        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);

        var logger = Mock.Of<ILogger<CashBalanceTrackingService>>();
        return new CashBalanceTrackingService(scopeFactoryMock.Object, logger);
    }

    private CashPayment CreateSalePayment(Guid cashSessionId, decimal amount)
    {
        var payment = CashPayment.Create(Guid.NewGuid(), new Money(amount), _userId, _terminalId);
        SetPaymentCashSessionId(payment, cashSessionId);
        return payment;
    }

    private static Payment CreateRefundPayment(Payment originalPayment, Guid cashSessionId, decimal refundAmount)
    {
        var refund = Payment.CreateRefund(
            originalPayment,
            new Money(refundAmount),
            originalPayment.ProcessedBy,
            originalPayment.TerminalId);
        SetPaymentCashSessionId(refund, cashSessionId);
        return refund;
    }

    private static void SetPaymentCashSessionId(Payment payment, Guid cashSessionId)
    {
        var prop = typeof(Payment).GetProperty(
            "CashSessionId",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        prop?.SetValue(payment, cashSessionId);
    }
}
