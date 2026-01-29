using System;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.Logging;
using Moq;

namespace Magidesk.Application.Tests.Services;

/// <summary>
/// Property-based tests for OrderNotificationService notification operations.
/// Feature: kds-realtime-notifications, Property 3: Notification Contains Correct Data
/// </summary>
public class OrderNotificationServicePropertyTests
{
    private readonly Mock<ILogger<OrderNotificationService>> _mockLogger;
    private readonly Mock<IKitchenNotificationPublisher> _mockPublisher;
    private readonly OrderNotificationService _notificationService;

    public OrderNotificationServicePropertyTests()
    {
        _mockLogger = new Mock<ILogger<OrderNotificationService>>();
        _mockPublisher = new Mock<IKitchenNotificationPublisher>();
        
        _notificationService = new OrderNotificationService(
            _mockLogger.Object,
            _mockPublisher.Object);
    }

    /// <summary>
    /// Property 3: Notification Contains Correct Data
    /// For any valid kitchen order ID and table number, the notification sent to SignalR 
    /// must contain the correct kitchen order ID, table number, notification type OrderCreated, 
    /// and a non-empty message.
    /// Validates: Requirements US-001.2
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property3_NotificationContainsCorrectData_ForAnyValidOrder_NotificationHasCorrectFields()
    {
        return Prop.ForAll(
            ValidKitchenOrderIdGenerator(),
            ValidTableNumberGenerator(),
            ValidServerNameGenerator(),
            (kitchenOrderId, tableNumber, serverName) =>
            {
                // Arrange
                OrderNotification? capturedNotification = null;
                _mockPublisher.Setup(p => p.PublishAsync(It.IsAny<OrderNotification>()))
                    .Callback<OrderNotification>(notification => capturedNotification = notification)
                    .Returns(Task.CompletedTask);

                // Act
                var task = _notificationService.NotifyOrderCreatedAsync(kitchenOrderId, tableNumber, serverName);
                task.Wait();

                // Assert properties
                var notificationCaptured = capturedNotification != null;
                var hasCorrectKitchenOrderId = capturedNotification?.KitchenOrderId == kitchenOrderId;
                var hasCorrectTableNumber = capturedNotification?.TableNumber == tableNumber;
                var hasCorrectServerName = capturedNotification?.ServerName == serverName;
                var hasCorrectType = capturedNotification?.Type == NotificationType.OrderCreated;
                var hasNonEmptyMessage = !string.IsNullOrEmpty(capturedNotification?.Message);
                var hasRecentTimestamp = capturedNotification?.Timestamp > DateTime.UtcNow.AddSeconds(-5);
                var hasValidId = capturedNotification?.Id != Guid.Empty;

                return notificationCaptured && 
                       hasCorrectKitchenOrderId && 
                       hasCorrectTableNumber && 
                       hasCorrectServerName &&
                       hasCorrectType && 
                       hasNonEmptyMessage && 
                       hasRecentTimestamp == true &&
                       hasValidId == true;
            });
    }

    /// <summary>
    /// Property 3: Notification Message Format
    /// For any valid table number, the notification message should contain the table number.
    /// Validates: Requirements US-001.2
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property3_NotificationMessageFormat_ForAnyTableNumber_MessageContainsTableNumber()
    {
        return Prop.ForAll(
            ValidKitchenOrderIdGenerator(),
            ValidTableNumberGenerator(),
            ValidServerNameGenerator(),
            (kitchenOrderId, tableNumber, serverName) =>
            {
                // Arrange
                OrderNotification? capturedNotification = null;
                _mockPublisher.Setup(p => p.PublishAsync(It.IsAny<OrderNotification>()))
                    .Callback<OrderNotification>(notification => capturedNotification = notification)
                    .Returns(Task.CompletedTask);

                // Act
                var task = _notificationService.NotifyOrderCreatedAsync(kitchenOrderId, tableNumber, serverName);
                task.Wait();

                // Assert properties
                var messageContainsTableNumber = capturedNotification?.Message.Contains(tableNumber) == true;
                var messageIsDescriptive = capturedNotification?.Message.Contains("order", StringComparison.OrdinalIgnoreCase) == true;

                return messageContainsTableNumber && messageIsDescriptive;
            });
    }

    /// <summary>
    /// Property 3: Notification Idempotency
    /// For any kitchen order ID, calling NotifyOrderCreatedAsync multiple times 
    /// should broadcast each notification independently without errors.
    /// Validates: Requirements US-002.1
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property3_NotificationIdempotency_ForAnyOrder_MultipleCallsSucceed()
    {
        return Prop.ForAll(
            NotificationIdempotencyTestDataGenerator(),
            (testData) =>
            {
                // Arrange
                var notificationCount = 0;
                _mockPublisher.Setup(p => p.PublishAsync(It.IsAny<OrderNotification>()))
                    .Callback<OrderNotification>(_ => notificationCount++)
                    .Returns(Task.CompletedTask);

                // Act
                var allSucceeded = true;
                for (int i = 0; i < testData.CallCount; i++)
                {
                    try
                    {
                        var task = _notificationService.NotifyOrderCreatedAsync(
                            testData.KitchenOrderId, 
                            testData.TableNumber, 
                            testData.ServerName);
                        task.Wait();
                    }
                    catch
                    {
                        allSucceeded = false;
                        break;
                    }
                }

                // Assert properties
                var correctNumberOfNotifications = notificationCount == testData.CallCount;

                return allSucceeded && correctNumberOfNotifications;
            });
    }

    #region Test Data Generators

    /// <summary>
    /// Test data for notification idempotency testing.
    /// </summary>
    public record NotificationIdempotencyTestData(Guid KitchenOrderId, string TableNumber, string ServerName, int CallCount);

    /// <summary>
    /// Generator for notification idempotency test data.
    /// </summary>
    public static Arbitrary<NotificationIdempotencyTestData> NotificationIdempotencyTestDataGenerator() =>
        Arb.From(
            from kitchenOrderId in ValidKitchenOrderIdGenerator().Generator
            from tableNumber in ValidTableNumberGenerator().Generator
            from serverName in ValidServerNameGenerator().Generator
            from callCount in Gen.Choose(1, 5)
            select new NotificationIdempotencyTestData(kitchenOrderId, tableNumber, serverName, callCount));

    /// <summary>
    /// Generator for valid kitchen order IDs (non-empty GUIDs).
    /// </summary>
    public static Arbitrary<Guid> ValidKitchenOrderIdGenerator() =>
        Arb.From(Gen.Fresh(() => Guid.NewGuid()));

    /// <summary>
    /// Generator for valid table numbers (1-99 or special formats).
    /// </summary>
    public static Arbitrary<string> ValidTableNumberGenerator() =>
        Arb.From(Gen.OneOf(
            Gen.Choose(1, 99).Select(x => x.ToString()),
            Gen.Choose(1, 99).Select(x => $"T{x}"),
            Gen.Choose(1, 99).Select(x => $"Table {x}"),
            Gen.Constant("Bar"),
            Gen.Constant("Patio-1"),
            Gen.Constant("VIP-A")
        ));

    /// <summary>
    /// Generator for valid server names.
    /// </summary>
    public static Arbitrary<string> ValidServerNameGenerator() =>
        Arb.From(Gen.OneOf(
            Gen.Constant("Server"),
            Gen.Constant("John"),
            Gen.Constant("Jane"),
            Gen.Constant("Mike"),
            Gen.Constant("Sarah"),
            Gen.Constant("Server 1"),
            Gen.Constant("Server 2")
        ));

    #endregion
}
