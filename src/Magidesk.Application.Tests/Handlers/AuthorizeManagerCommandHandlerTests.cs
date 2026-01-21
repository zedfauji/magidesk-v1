using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands.Security;
using Magidesk.Application.DTOs.Security;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services.Security;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Handlers;

/// <summary>
/// Unit tests for AuthorizeManagerCommandHandler.
/// Tests PIN validation, permission checks, and audit logging.
/// </summary>
public class AuthorizeManagerCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IAesEncryptionService> _mockEncryptionService;
    private readonly Mock<ILogger<AuthorizeManagerCommandHandler>> _mockLogger;
    private readonly AuthorizeManagerCommandHandler _handler;

    public AuthorizeManagerCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEncryptionService = new Mock<IAesEncryptionService>();
        _mockLogger = new Mock<ILogger<AuthorizeManagerCommandHandler>>();
        
        _handler = new AuthorizeManagerCommandHandler(
            _mockUserRepository.Object,
            _mockEncryptionService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task AuthorizeManager_ValidPinAndManagerRole_ReturnsAuthorized()
    {
        // Arrange
        var pin = "1234";
        var encryptedPin = "encrypted_1234";
        var command = new AuthorizeManagerCommand(pin, "VoidTicket");

        var managerRole = Role.Create("Manager", UserPermission.VoidTicket | UserPermission.RefundPayment);
        var managerUser = User.Create("manager", "John", "Doe", managerRole.Id, encryptedPin);
        
        // Use reflection to set Role navigation property (private setter)
        var roleProperty = typeof(User).GetProperty("Role");
        roleProperty?.SetValue(managerUser, managerRole);

        _mockEncryptionService.Setup(x => x.Encrypt(pin)).Returns(encryptedPin);
        _mockUserRepository.Setup(x => x.GetByPinAsync(encryptedPin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(managerUser);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.Authorized);
        Assert.Equal(managerUser.Id, result.AuthorizingUserId);
        Assert.Equal("John Doe", result.AuthorizingUserName);
        Assert.NotNull(result.ExpiresAt);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
        Assert.Null(result.FailureReason);
    }

    [Fact]
    public async Task AuthorizeManager_InvalidPin_ReturnsUnauthorized()
    {
        // Arrange
        var pin = "9999";
        var encryptedPin = "encrypted_9999";
        var command = new AuthorizeManagerCommand(pin, "VoidTicket");

        _mockEncryptionService.Setup(x => x.Encrypt(pin)).Returns(encryptedPin);
        _mockUserRepository.Setup(x => x.GetByPinAsync(encryptedPin, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.Authorized);
        Assert.Null(result.AuthorizingUserId);
        Assert.Null(result.AuthorizingUserName);
        Assert.Null(result.ExpiresAt);
        Assert.Equal("Invalid PIN.", result.FailureReason);
    }

    [Fact]
    public async Task AuthorizeManager_ValidPinButNotManager_ReturnsUnauthorized()
    {
        // Arrange
        var pin = "5678";
        var encryptedPin = "encrypted_5678";
        var command = new AuthorizeManagerCommand(pin, "VoidTicket");

        var cashierRole = Role.Create("Cashier", UserPermission.CreateTicket | UserPermission.TakePayment);
        var cashierUser = User.Create("cashier", "Jane", "Smith", cashierRole.Id, encryptedPin);
        
        // Set Role navigation property
        var roleProperty = typeof(User).GetProperty("Role");
        roleProperty?.SetValue(cashierUser, cashierRole);

        _mockEncryptionService.Setup(x => x.Encrypt(pin)).Returns(encryptedPin);
        _mockUserRepository.Setup(x => x.GetByPinAsync(encryptedPin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cashierUser);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.Authorized);
        Assert.Null(result.AuthorizingUserId);
        Assert.Null(result.AuthorizingUserName);
        Assert.Null(result.ExpiresAt);
        Assert.Equal("Insufficient permissions. Manager authorization required.", result.FailureReason);
    }

    [Fact]
    public async Task AuthorizeManager_EmptyPin_ReturnsUnauthorized()
    {
        // Arrange
        var command = new AuthorizeManagerCommand("", "VoidTicket");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.Authorized);
        Assert.Equal("PIN is required.", result.FailureReason);
    }

    [Fact]
    public async Task AuthorizeManager_InactiveUser_ReturnsUnauthorized()
    {
        // Arrange
        var pin = "1234";
        var encryptedPin = "encrypted_1234";
        var command = new AuthorizeManagerCommand(pin, "VoidTicket");

        var managerRole = Role.Create("Manager", UserPermission.VoidTicket);
        var inactiveUser = User.Create("manager", "John", "Doe", managerRole.Id, encryptedPin);
        inactiveUser.Deactivate();
        
        var roleProperty = typeof(User).GetProperty("Role");
        roleProperty?.SetValue(inactiveUser, managerRole);

        _mockEncryptionService.Setup(x => x.Encrypt(pin)).Returns(encryptedPin);
        _mockUserRepository.Setup(x => x.GetByPinAsync(encryptedPin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveUser);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.Authorized);
        Assert.Equal("User account is inactive.", result.FailureReason);
    }

    [Fact]
    public async Task AuthorizeManager_LogsSuccessfulAttempt()
    {
        // Arrange
        var pin = "1234";
        var encryptedPin = "encrypted_1234";
        var command = new AuthorizeManagerCommand(pin, "VoidTicket");

        var managerRole = Role.Create("Manager", UserPermission.VoidTicket);
        var managerUser = User.Create("manager", "John", "Doe", managerRole.Id, encryptedPin);
        
        var roleProperty = typeof(User).GetProperty("Role");
        roleProperty?.SetValue(managerUser, managerRole);

        _mockEncryptionService.Setup(x => x.Encrypt(pin)).Returns(encryptedPin);
        _mockUserRepository.Setup(x => x.GetByPinAsync(encryptedPin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(managerUser);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Authorization successful")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task AuthorizeManager_LogsFailedAttempt()
    {
        // Arrange
        var pin = "9999";
        var encryptedPin = "encrypted_9999";
        var command = new AuthorizeManagerCommand(pin, "VoidTicket");

        _mockEncryptionService.Setup(x => x.Encrypt(pin)).Returns(encryptedPin);
        _mockUserRepository.Setup(x => x.GetByPinAsync(encryptedPin, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Authorization failed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
