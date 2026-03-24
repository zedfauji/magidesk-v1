using System.Text.RegularExpressions;
using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Moq;

namespace Magidesk.Application.Tests.Commands;

public class AuthorizeCardPaymentHandlerSecurityTests
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepo;
    private readonly Mock<IPaymentGateway> _mockPaymentGateway;
    private readonly Mock<IAuditEventRepository> _mockAuditRepo;
    private readonly Mock<IUserService> _mockUserService;
    private readonly AuthorizeCardPaymentCommandHandler _handler;
    private readonly CreditCardPayment _payment;
    private readonly AuthorizeCardPaymentCommand _manualAuthCommand;

    public AuthorizeCardPaymentHandlerSecurityTests()
    {
        _mockPaymentRepo = new Mock<IPaymentRepository>();
        _mockPaymentGateway = new Mock<IPaymentGateway>();
        _mockAuditRepo = new Mock<IAuditEventRepository>();
        _mockUserService = new Mock<IUserService>();

        _handler = new AuthorizeCardPaymentCommandHandler(
            _mockPaymentRepo.Object,
            _mockPaymentGateway.Object,
            _mockAuditRepo.Object,
            _mockUserService.Object);

        var userId = new UserId(Guid.NewGuid());
        _payment = CreditCardPayment.Create(
            ticketId: Guid.NewGuid(),
            amount: new Money(50m),
            processedBy: userId,
            terminalId: Guid.NewGuid());

        _mockPaymentRepo
            .Setup(r => r.GetByIdAsync(_payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_payment);

        _mockPaymentRepo
            .Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockAuditRepo
            .Setup(r => r.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserService
            .Setup(u => u.GetCurrentUserId())
            .Returns(userId.Value);

        _manualAuthCommand = new AuthorizeCardPaymentCommand
        {
            PaymentId = _payment.Id,
            ManualAuthCode = "AUTH123",
            ManagerId = Guid.NewGuid(),
            LastFourDigits = "4242",
            ProcessedBy = userId,
            CardToken = "tok_test"
        };
    }

    [Fact]
    public async Task AuthorizeCardPaymentHandler_WithoutManagerRole_ThrowsUnauthorized()
    {
        // Arrange
        _mockUserService
            .Setup(u => u.IsInRole("Manager"))
            .Returns(false);

        // Act
        var act = async () => await _handler.HandleAsync(_manualAuthCommand);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task AuthorizeCardPaymentHandler_WhenAuthorized_WritesAuditEvent()
    {
        // Arrange
        var capturedEvents = new List<AuditEvent>();
        _mockUserService
            .Setup(u => u.IsInRole("Manager"))
            .Returns(true);
        _mockAuditRepo
            .Setup(r => r.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()))
            .Callback<AuditEvent, CancellationToken>((e, _) => capturedEvents.Add(e))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(_manualAuthCommand);

        // Assert
        capturedEvents.Should().Contain(e => e.EventType == AuditEventType.ManualAuthorizationApplied);
    }

    [Fact]
    public async Task AuthorizeCardPaymentHandler_AuditEvent_DoesNotContainFullCardNumber()
    {
        // Arrange
        var capturedEvents = new List<AuditEvent>();
        _mockUserService
            .Setup(u => u.IsInRole("Manager"))
            .Returns(true);
        _mockAuditRepo
            .Setup(r => r.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()))
            .Callback<AuditEvent, CancellationToken>((e, _) => capturedEvents.Add(e))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(_manualAuthCommand);

        // Assert
        var manualAuthEvent = capturedEvents.First(e => e.EventType == AuditEventType.ManualAuthorizationApplied);
        Regex.IsMatch(manualAuthEvent.AfterState, @"\d{16}").Should().BeFalse();
    }
}
