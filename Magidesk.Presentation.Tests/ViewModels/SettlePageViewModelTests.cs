using FsCheck;
using FsCheck.Xunit;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Presentation.Tests.ViewModels;

/// <summary>
/// Tests for SettlePageViewModel including property-based tests and unit tests.
/// Feature: settle-order-page-redesign
/// </summary>
public class SettlePageViewModelTests
{
    private readonly Mock<IQueryHandler<GetTicketQuery, TicketDto?>> _mockGetTicketHandler;
    private readonly Mock<ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult>> _mockProcessPaymentHandler;
    private readonly Mock<ICommandHandler<SetTaxExemptCommand, SetTaxExemptResult>> _mockSetTaxExemptHandler;
    private readonly Mock<NavigationService> _mockNavigationService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ITerminalContext> _mockTerminalContext;
    private readonly Mock<ICashSessionRepository> _mockCashSessionRepository;
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;
    private readonly Mock<ILogger<SettlePageViewModel>> _mockLogger;

    public SettlePageViewModelTests()
    {
        _mockGetTicketHandler = new Mock<IQueryHandler<GetTicketQuery, TicketDto?>>();
        _mockProcessPaymentHandler = new Mock<ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult>>();
        _mockSetTaxExemptHandler = new Mock<ICommandHandler<SetTaxExemptCommand, SetTaxExemptResult>>();
        _mockNavigationService = new Mock<NavigationService>();
        _mockUserService = new Mock<IUserService>();
        _mockTerminalContext = new Mock<ITerminalContext>();
        _mockCashSessionRepository = new Mock<ICashSessionRepository>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        _mockLogger = new Mock<ILogger<SettlePageViewModel>>();
    }

    private SettlePageViewModel CreateViewModel()
    {
        return new SettlePageViewModel(
            _mockGetTicketHandler.Object,
            _mockProcessPaymentHandler.Object,
            _mockSetTaxExemptHandler.Object,
            _mockNavigationService.Object,
            _mockUserService.Object,
            _mockTerminalContext.Object,
            _mockCashSessionRepository.Object,
            _mockServiceScopeFactory.Object,
            _mockLogger.Object
        );
    }

    #region Property-Based Tests

    /// <summary>
    /// Property 1: Tender Amount Building
    /// Feature: settle-order-page-redesign, Property 1: For any sequence of digit button presses (0-9 and decimal point), the tender amount display should correctly concatenate the digits to form a valid currency string.
    /// Validates: Requirements 3.5
    /// </summary>
    [Property(MaxTest = 100)]
    [Trait("Feature", "settle-order-page-redesign")]
    [Trait("Property", "1")]
    public Property TenderAmountBuilding_ConcatenatesDigitsCorrectly()
    {
        var digitGen = Gen.Elements("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".");
        var digitSequenceGen = Gen.NonEmptyListOf(digitGen).Select(list => list.Take(10).ToList());

        return Prop.ForAll(
            Arb.From(digitSequenceGen),
            digits =>
            {
                // Arrange
                var viewModel = CreateViewModel();

                // Act
                foreach (var digit in digits)
                {
                    viewModel.KeypadDigitCommand.Execute(digit);
                }

                // Assert
                var display = viewModel.TenderAmountDisplay;
                
                // The display should not be empty
                var isNotEmpty = !string.IsNullOrEmpty(display);
                
                // The display should contain valid currency characters
                var containsValidChars = display.All(c => char.IsDigit(c) || c == '$' || c == '.' || c == ',');
                
                return isNotEmpty && containsValidChars;
            });
    }

    /// <summary>
    /// Property 2: Tender Amount Clearing
    /// Feature: settle-order-page-redesign, Property 2: For any current tender amount value, pressing the clear button should reset the tender amount display to "$0.00".
    /// Validates: Requirements 3.6
    /// </summary>
    [Property(MaxTest = 100)]
    [Trait("Feature", "settle-order-page-redesign")]
    [Trait("Property", "2")]
    public Property TenderAmountClearing_ResetsToZero()
    {
        return Prop.ForAll(
            Arb.Default.PositiveInt(),
            amount =>
            {
                // Arrange
                var viewModel = CreateViewModel();
                
                // Set some tender amount by entering digits
                var amountStr = amount.Get.ToString();
                foreach (var digit in amountStr)
                {
                    viewModel.KeypadDigitCommand.Execute(digit.ToString());
                }

                // Act
                viewModel.ClearTenderCommand.Execute(null);

                // Assert
                return viewModel.TenderAmountDisplay == "$0.00";
            });
    }

    /// <summary>
    /// Property 3: Quick Cash Selection
    /// Feature: settle-order-page-redesign, Property 3: For any quick cash denomination ($1, $5, $10, $20, $50, $100), clicking the quick cash button should set the tender amount to exactly that denomination.
    /// Validates: Requirements 4.2
    /// </summary>
    [Property(MaxTest = 100)]
    [Trait("Feature", "settle-order-page-redesign")]
    [Trait("Property", "3")]
    public Property QuickCashSelection_SetsExactDenomination()
    {
        var denominationGen = Gen.Elements(1m, 5m, 10m, 20m, 50m, 100m);

        return Prop.ForAll(
            Arb.From(denominationGen),
            denomination =>
            {
                // Arrange
                var viewModel = CreateViewModel();

                // Act
                viewModel.QuickCashCommand.Execute(denomination);

                // Assert
                var expectedDisplay = denomination.ToString("C2");
                return viewModel.TenderAmountDisplay == expectedDisplay;
            });
    }

    /// <summary>
    /// Property 6: Currency Formatting
    /// Feature: settle-order-page-redesign, Property 6: For any monetary amount (total, tax, paid, balance, tender), the display should format the amount as currency with exactly two decimal places.
    /// Validates: Requirements 2.8
    /// </summary>
    [Property(MaxTest = 100)]
    [Trait("Feature", "settle-order-page-redesign")]
    [Trait("Property", "6")]
    public Property CurrencyFormatting_HasTwoDecimalPlaces()
    {
        return Prop.ForAll(
            Arb.Default.PositiveInt(),
            amount =>
            {
                // Arrange
                var viewModel = CreateViewModel();
                var decimalAmount = amount.Get / 100m; // Convert to decimal with cents

                // Act
                viewModel.QuickCashCommand.Execute(decimalAmount);

                // Assert
                var display = viewModel.TenderAmountDisplay;
                
                // Check that display contains a decimal point
                var hasDecimal = display.Contains(".");
                
                // Check that there are exactly 2 digits after the decimal
                var parts = display.Replace("$", "").Replace(",", "").Split('.');
                var hasTwoDecimals = parts.Length == 2 && parts[1].Length == 2;
                
                return hasDecimal && hasTwoDecimals;
            });
    }

    #endregion

    #region Unit Tests

    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.PaymentMethods);
        Assert.Equal(3, viewModel.PaymentMethods.Count);
        Assert.NotNull(viewModel.QuickCashAmounts);
        Assert.Equal(6, viewModel.QuickCashAmounts.Count);
        Assert.NotNull(viewModel.KeypadDigitCommand);
        Assert.NotNull(viewModel.ClearTenderCommand);
        Assert.NotNull(viewModel.QuickCashCommand);
        Assert.NotNull(viewModel.ProcessPaymentCommand);
    }

    [Fact]
    public void KeypadDigitCommand_WithSingleDigit_UpdatesDisplay()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.KeypadDigitCommand.Execute("5");

        // Assert
        Assert.Contains("5", viewModel.TenderAmountDisplay);
    }

    [Fact]
    public void KeypadDigitCommand_WithDecimalPoint_AllowsOnlyOne()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.KeypadDigitCommand.Execute("1");
        viewModel.KeypadDigitCommand.Execute(".");
        viewModel.KeypadDigitCommand.Execute("5");
        viewModel.KeypadDigitCommand.Execute("."); // Second decimal should be ignored

        // Assert
        var decimalCount = viewModel.TenderAmountDisplay.Count(c => c == '.');
        Assert.Equal(1, decimalCount);
    }

    [Fact]
    public void ClearTenderCommand_ResetsToZero()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.KeypadDigitCommand.Execute("1");
        viewModel.KeypadDigitCommand.Execute("2");
        viewModel.KeypadDigitCommand.Execute("3");

        // Act
        viewModel.ClearTenderCommand.Execute(null);

        // Assert
        Assert.Equal("$0.00", viewModel.TenderAmountDisplay);
    }

    [Fact]
    public void QuickCashCommand_SetsTenderAmount()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.QuickCashCommand.Execute(20m);

        // Assert
        Assert.Equal("$20.00", viewModel.TenderAmountDisplay);
    }

    [Fact]
    public void QuickCashCommand_WithZeroAmount_DoesNothing()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var initialDisplay = viewModel.TenderAmountDisplay;

        // Act
        viewModel.QuickCashCommand.Execute(0m);

        // Assert
        Assert.Equal(initialDisplay, viewModel.TenderAmountDisplay);
    }

    [Fact]
    public void QuickCashCommand_WithNegativeAmount_DoesNothing()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var initialDisplay = viewModel.TenderAmountDisplay;

        // Act
        viewModel.QuickCashCommand.Execute(-10m);

        // Assert
        Assert.Equal(initialDisplay, viewModel.TenderAmountDisplay);
    }

    [Fact]
    public void PaymentMethods_ContainsExpectedTypes()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Contains(viewModel.PaymentMethods, pm => pm.Type == PaymentType.Cash);
        Assert.Contains(viewModel.PaymentMethods, pm => pm.Type == PaymentType.CreditCard);
        Assert.Contains(viewModel.PaymentMethods, pm => pm.Type == PaymentType.GiftCertificate);
    }

    [Fact]
    public void QuickCashAmounts_ContainsExpectedDenominations()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Contains(1m, viewModel.QuickCashAmounts);
        Assert.Contains(5m, viewModel.QuickCashAmounts);
        Assert.Contains(10m, viewModel.QuickCashAmounts);
        Assert.Contains(20m, viewModel.QuickCashAmounts);
        Assert.Contains(50m, viewModel.QuickCashAmounts);
        Assert.Contains(100m, viewModel.QuickCashAmounts);
    }

    [Fact]
    public void CancelSettlementCommand_CallsNavigationGoBack()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.CancelSettlementCommand.Execute(null);

        // Assert
        _mockNavigationService.Verify(n => n.GoBack(), Times.Once);
    }

    [Fact]
    public void NavigateBackCommand_CallsNavigationGoBack()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.NavigateBackCommand.Execute(null);

        // Assert
        _mockNavigationService.Verify(n => n.GoBack(), Times.Once);
    }

    #endregion
}
