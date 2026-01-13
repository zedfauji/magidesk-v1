using System;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Moq;

namespace Magidesk.Application.Tests.Services;

/// <summary>
/// Property-based tests for ManagerOverrideService authorization and audit trail operations.
/// Feature: table-game-management, Property 7: Manager Authorization Enforcement
/// Feature: table-game-management, Property 8: Override Audit Trail Completeness
/// </summary>
public class ManagerOverrideServicePropertyTests
{
    private readonly Mock<ITableSessionRepository> _mockSessionRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IOverrideAuditRepository> _mockAuditRepository;
    private readonly Mock<ISecurityService> _mockSecurityService;
    private readonly Mock<IAesEncryptionService> _mockEncryptionService;
    private readonly ManagerOverrideService _managerOverrideService;

    public ManagerOverrideServicePropertyTests()
    {
        _mockSessionRepository = new Mock<ITableSessionRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockAuditRepository = new Mock<IOverrideAuditRepository>();
        _mockSecurityService = new Mock<ISecurityService>();
        _mockEncryptionService = new Mock<IAesEncryptionService>();
        
        _managerOverrideService = new ManagerOverrideService(
            _mockSessionRepository.Object,
            _mockUserRepository.Object,
            _mockAuditRepository.Object,
            _mockSecurityService.Object,
            _mockEncryptionService.Object);
    }

    /// <summary>
    /// Property 7: Manager Authorization Enforcement
    /// For any manager override operation (time adjustment, pricing override, force end), 
    /// valid manager credentials must be provided and all actions must be logged with complete audit information.
    /// Validates: Requirements 3.1, 3.2, 3.4, 3.5
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property7_ManagerAuthorizationEnforcement_ForAnyValidPin_AuthorizationSucceeds()
    {
        return Prop.ForAll(
            ValidPinGenerator(),
            ValidUserIdGenerator(),
            (pin, userId) =>
            {
                // Arrange - Setup valid manager with permissions
                var encryptedPin = $"encrypted_{pin}";
                var manager = CreateValidManagerUser(userId);
                
                _mockEncryptionService.Setup(e => e.Encrypt(pin)).Returns(encryptedPin);
                _mockSecurityService.Setup(s => s.GetUserByPinAsync(encryptedPin, default))
                    .ReturnsAsync(manager);

                // Act
                var result = _managerOverrideService.ValidateManagerAuthorizationAsync(pin, userId).Result;

                // Assert properties
                var authorizationSuccessful = result.IsSuccessful;
                var noErrorMessage = string.IsNullOrEmpty(result.ErrorMessage);
                
                return authorizationSuccessful && noErrorMessage;
            });
    }

    /// <summary>
    /// Property 7: Manager Authorization Enforcement - Invalid PIN
    /// For any invalid PIN, authorization should fail regardless of user ID.
    /// Validates: Requirements 3.1, 3.2
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property7_ManagerAuthorizationEnforcement_ForAnyInvalidPin_AuthorizationFails()
    {
        return Prop.ForAll(
            InvalidPinGenerator(),
            ValidUserIdGenerator(),
            (pin, userId) =>
            {
                // Arrange - Setup invalid PIN (no user found)
                var encryptedPin = $"encrypted_{pin}";
                
                _mockEncryptionService.Setup(e => e.Encrypt(pin)).Returns(encryptedPin);
                _mockSecurityService.Setup(s => s.GetUserByPinAsync(encryptedPin, default))
                    .ReturnsAsync((User?)null);

                // Act
                var result = _managerOverrideService.ValidateManagerAuthorizationAsync(pin, userId).Result;

                // Assert properties
                var authorizationFailed = !result.IsSuccessful;
                var hasErrorMessage = !string.IsNullOrEmpty(result.ErrorMessage);
                
                return authorizationFailed && hasErrorMessage;
            });
    }

    /// <summary>
    /// Property 7: Manager Authorization Enforcement - Insufficient Permissions
    /// For any user without manager permissions, authorization should fail.
    /// Validates: Requirements 3.1, 3.2
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property7_ManagerAuthorizationEnforcement_ForAnyUserWithoutPermissions_AuthorizationFails()
    {
        return Prop.ForAll(
            ValidPinGenerator(),
            ValidUserIdGenerator(),
            (pin, userId) =>
            {
                // Arrange - Setup user without manager permissions
                var encryptedPin = $"encrypted_{pin}";
                var userWithoutPermissions = CreateUserWithoutManagerPermissions(userId);
                
                _mockEncryptionService.Setup(e => e.Encrypt(pin)).Returns(encryptedPin);
                _mockSecurityService.Setup(s => s.GetUserByPinAsync(encryptedPin, default))
                    .ReturnsAsync(userWithoutPermissions);

                // Act
                var result = _managerOverrideService.ValidateManagerAuthorizationAsync(pin, userId).Result;

                // Assert properties
                var authorizationFailed = !result.IsSuccessful;
                var hasErrorMessage = !string.IsNullOrEmpty(result.ErrorMessage);
                
                return authorizationFailed && hasErrorMessage;
            });
    }

    /// <summary>
    /// Property 8: Override Audit Trail Completeness - Time Adjustment
    /// For any time adjustment override, the audit trail must include timestamp, manager ID, 
    /// original value, new value, reason, and session context, and records must be immutable.
    /// Validates: Requirements 3.4, 3.5
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property8_OverrideAuditTrailCompleteness_ForAnyTimeAdjustment_AuditTrailComplete()
    {
        return Prop.ForAll(
            TimeAdjustmentTestDataGenerator(),
            (testData) =>
            {
                // Arrange - Setup valid session and manager
                var session = CreateValidActiveSession(testData.SessionId);
                var manager = CreateValidManagerUser(testData.ManagerId);
                
                _mockSessionRepository.Setup(r => r.GetByIdAsync(testData.SessionId)).ReturnsAsync(session);
                _mockUserRepository.Setup(r => r.GetByIdAsync(testData.ManagerId, default)).ReturnsAsync(manager);
                _mockSessionRepository.Setup(r => r.UpdateAsync(It.IsAny<TableSession>())).Returns(Task.CompletedTask);
                
                OverrideAuditEntry? capturedAuditEntry = null;
                _mockAuditRepository.Setup(r => r.AddAsync(It.IsAny<OverrideAuditEntry>(), default))
                    .Callback<OverrideAuditEntry, CancellationToken>((entry, _) => capturedAuditEntry = entry)
                    .Returns(Task.CompletedTask);

                // Act
                var result = _managerOverrideService.ApplyTimeAdjustmentAsync(testData.SessionId, testData.Adjustment, testData.Reason, testData.ManagerId).Result;

                // Assert properties
                var operationSuccessful = result.IsSuccessful;
                var auditEntryCreated = capturedAuditEntry != null;
                var auditHasAllRequiredFields = capturedAuditEntry != null &&
                    capturedAuditEntry.SessionId == testData.SessionId &&
                    capturedAuditEntry.ManagerId == testData.ManagerId &&
                    capturedAuditEntry.OverrideType == OverrideType.TimeAdjustment &&
                    !string.IsNullOrEmpty(capturedAuditEntry.Reason) &&
                    !string.IsNullOrEmpty(capturedAuditEntry.OriginalValue) &&
                    !string.IsNullOrEmpty(capturedAuditEntry.NewValue) &&
                    capturedAuditEntry.Timestamp > DateTime.MinValue;
                
                return operationSuccessful && auditEntryCreated && auditHasAllRequiredFields;
            });
    }

    /// <summary>
    /// Property 8: Override Audit Trail Completeness - Pricing Override
    /// For any pricing override, the audit trail must include all required fields.
    /// Validates: Requirements 3.4, 3.5
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property8_OverrideAuditTrailCompleteness_ForAnyPricingOverride_AuditTrailComplete()
    {
        return Prop.ForAll(
            PricingOverrideTestDataGenerator(),
            (testData) =>
            {
                // Arrange - Setup valid session and manager
                var session = CreateValidActiveSession(testData.SessionId);
                var manager = CreateValidManagerUser(testData.ManagerId);
                var money = new Money(testData.OverrideAmount);
                
                _mockSessionRepository.Setup(r => r.GetByIdAsync(testData.SessionId)).ReturnsAsync(session);
                _mockUserRepository.Setup(r => r.GetByIdAsync(testData.ManagerId, default)).ReturnsAsync(manager);
                
                OverrideAuditEntry? capturedAuditEntry = null;
                _mockAuditRepository.Setup(r => r.AddAsync(It.IsAny<OverrideAuditEntry>(), default))
                    .Callback<OverrideAuditEntry, CancellationToken>((entry, _) => capturedAuditEntry = entry)
                    .Returns(Task.CompletedTask);

                // Act
                var result = _managerOverrideService.ApplyPricingOverrideAsync(testData.SessionId, money, testData.Reason, testData.ManagerId).Result;

                // Assert properties
                var operationSuccessful = result.IsSuccessful;
                var auditEntryCreated = capturedAuditEntry != null;
                var auditHasAllRequiredFields = capturedAuditEntry != null &&
                    capturedAuditEntry.SessionId == testData.SessionId &&
                    capturedAuditEntry.ManagerId == testData.ManagerId &&
                    capturedAuditEntry.OverrideType == OverrideType.PricingOverride &&
                    !string.IsNullOrEmpty(capturedAuditEntry.Reason) &&
                    !string.IsNullOrEmpty(capturedAuditEntry.OriginalValue) &&
                    !string.IsNullOrEmpty(capturedAuditEntry.NewValue) &&
                    capturedAuditEntry.Timestamp > DateTime.MinValue;
                
                return operationSuccessful && auditEntryCreated && auditHasAllRequiredFields;
            });
    }

    /// <summary>
    /// Property 8: Override Audit Trail Completeness - Force End Session
    /// For any force end session override, the audit trail must include all required fields.
    /// Validates: Requirements 3.4, 3.5
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property8_OverrideAuditTrailCompleteness_ForAnyForceEndSession_AuditTrailComplete()
    {
        return Prop.ForAll(
            ValidSessionIdGenerator(),
            ValidReasonGenerator(),
            ValidManagerIdGenerator(),
            (sessionId, reason, managerId) =>
            {
                // Arrange - Setup valid session and manager
                var session = CreateValidActiveSession(sessionId);
                var manager = CreateValidManagerUser(managerId);
                
                _mockSessionRepository.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(session);
                _mockUserRepository.Setup(r => r.GetByIdAsync(managerId, default)).ReturnsAsync(manager);
                _mockSessionRepository.Setup(r => r.UpdateAsync(It.IsAny<TableSession>())).Returns(Task.CompletedTask);
                
                OverrideAuditEntry? capturedAuditEntry = null;
                _mockAuditRepository.Setup(r => r.AddAsync(It.IsAny<OverrideAuditEntry>(), default))
                    .Callback<OverrideAuditEntry, CancellationToken>((entry, _) => capturedAuditEntry = entry)
                    .Returns(Task.CompletedTask);

                // Act
                var result = _managerOverrideService.ForceEndSessionAsync(sessionId, reason, managerId).Result;

                // Assert properties
                var operationSuccessful = result.IsSuccessful;
                var auditEntryCreated = capturedAuditEntry != null;
                var auditHasAllRequiredFields = capturedAuditEntry != null &&
                    capturedAuditEntry.SessionId == sessionId &&
                    capturedAuditEntry.ManagerId == managerId &&
                    capturedAuditEntry.OverrideType == OverrideType.ForceEndSession &&
                    !string.IsNullOrEmpty(capturedAuditEntry.Reason) &&
                    !string.IsNullOrEmpty(capturedAuditEntry.OriginalValue) &&
                    !string.IsNullOrEmpty(capturedAuditEntry.NewValue) &&
                    capturedAuditEntry.Timestamp > DateTime.MinValue;
                
                return operationSuccessful && auditEntryCreated && auditHasAllRequiredFields;
            });
    }

    #region Test Data Generators

    /// <summary>
    /// Test data for time adjustment operations.
    /// </summary>
    public record TimeAdjustmentTestData(Guid SessionId, TimeSpan Adjustment, string Reason, Guid ManagerId);

    /// <summary>
    /// Test data for pricing override operations.
    /// </summary>
    public record PricingOverrideTestData(Guid SessionId, decimal OverrideAmount, string Reason, Guid ManagerId);

    /// <summary>
    /// Generator for time adjustment test data.
    /// </summary>
    public static Arbitrary<TimeAdjustmentTestData> TimeAdjustmentTestDataGenerator() =>
        Arb.From(
            from sessionId in ValidSessionIdGenerator().Generator
            from adjustment in ValidTimeAdjustmentGenerator().Generator
            from reason in ValidReasonGenerator().Generator
            from managerId in ValidManagerIdGenerator().Generator
            select new TimeAdjustmentTestData(sessionId, adjustment, reason, managerId));

    /// <summary>
    /// Generator for pricing override test data.
    /// </summary>
    public static Arbitrary<PricingOverrideTestData> PricingOverrideTestDataGenerator() =>
        Arb.From(
            from sessionId in ValidSessionIdGenerator().Generator
            from amount in ValidMoneyAmountGenerator().Generator
            from reason in ValidReasonGenerator().Generator
            from managerId in ValidManagerIdGenerator().Generator
            select new PricingOverrideTestData(sessionId, amount, reason, managerId));

    /// <summary>
    /// Generator for valid PINs (4-6 digit strings).
    /// </summary>
    public static Arbitrary<string> ValidPinGenerator() =>
        Arb.From(Gen.Choose(1000, 999999).Select(x => x.ToString()));

    /// <summary>
    /// Generator for invalid PINs (empty, null, or too short).
    /// </summary>
    public static Arbitrary<string> InvalidPinGenerator() =>
        Arb.From(Gen.OneOf(
            Gen.Constant(""),
            Gen.Constant("1"),
            Gen.Constant("12"),
            Gen.Constant("123")
        ));

    /// <summary>
    /// Generator for valid user IDs.
    /// </summary>
    public static Arbitrary<Guid> ValidUserIdGenerator() =>
        Arb.From(Gen.Fresh(() => Guid.NewGuid()));

    /// <summary>
    /// Generator for valid session IDs.
    /// </summary>
    public static Arbitrary<Guid> ValidSessionIdGenerator() =>
        Arb.From(Gen.Fresh(() => Guid.NewGuid()));

    /// <summary>
    /// Generator for valid manager IDs.
    /// </summary>
    public static Arbitrary<Guid> ValidManagerIdGenerator() =>
        Arb.From(Gen.Fresh(() => Guid.NewGuid()));

    /// <summary>
    /// Generator for valid time adjustments (-480 to 480 minutes).
    /// </summary>
    public static Arbitrary<TimeSpan> ValidTimeAdjustmentGenerator() =>
        Arb.From(Gen.Choose(-480, 480).Select(minutes => TimeSpan.FromMinutes(minutes)));

    /// <summary>
    /// Generator for valid money amounts ($0.01 to $999.99).
    /// </summary>
    public static Arbitrary<decimal> ValidMoneyAmountGenerator() =>
        Arb.From(Gen.Choose(1, 99999).Select(x => x / 100m));

    /// <summary>
    /// Generator for valid reasons (non-empty strings).
    /// </summary>
    public static Arbitrary<string> ValidReasonGenerator() =>
        Arb.From(Gen.OneOf(
            Gen.Constant("Customer complaint"),
            Gen.Constant("System error"),
            Gen.Constant("Manager discretion"),
            Gen.Constant("Emergency situation"),
            Gen.Constant("Technical issue")
        ));

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a valid manager user with appropriate permissions.
    /// </summary>
    private static User CreateValidManagerUser(Guid userId)
    {
        var role = Role.Create("Manager", UserPermission.AdjustSessionTime);
        var user = User.Create("manager", "Manager", "User", role.Id);
        
        // Set the role navigation property using reflection
        var roleProperty = typeof(User).GetProperty("Role");
        roleProperty?.SetValue(user, role);
        
        return user;
    }

    /// <summary>
    /// Creates a user without manager permissions.
    /// </summary>
    private static User CreateUserWithoutManagerPermissions(Guid userId)
    {
        var role = Role.Create("Server", UserPermission.CreateTicket | UserPermission.TakePayment);
        var user = User.Create("server", "Server", "User", role.Id);
        
        // Set the role navigation property using reflection
        var roleProperty = typeof(User).GetProperty("Role");
        roleProperty?.SetValue(user, role);
        
        return user;
    }

    /// <summary>
    /// Creates a valid active table session.
    /// </summary>
    private static TableSession CreateValidActiveSession(Guid sessionId)
    {
        var tableId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        var session = TableSession.Start(tableId, tableTypeId, 25.00m, 2);
        
        // Set the session ID using reflection
        var idProperty = typeof(TableSession).GetProperty("Id");
        idProperty?.SetValue(session, sessionId);
        
        return session;
    }

    #endregion
}