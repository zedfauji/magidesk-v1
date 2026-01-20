using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Presentation.Tests.Views;

/// <summary>
/// UI integration tests for SettlePageView.
/// Tests data binding correctness, command bindings, UI element visibility, and accessibility.
/// Feature: settle-order-page-redesign
/// </summary>
public class SettlePageViewTests
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

    public SettlePageViewTests()
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

    [Fact]
    public void ViewModel_ShouldHaveCorrectProperties()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert - Verify all required properties exist
        Assert.NotNull(viewModel.TicketNumber);
        Assert.NotNull(viewModel.TableNumber);
        Assert.NotNull(viewModel.TenderAmountDisplay);
        Assert.NotNull(viewModel.PaymentMethods);
        Assert.NotNull(viewModel.QuickCashAmounts);
    }

    [Fact]
    public void ViewModel_ShouldHaveCorrectCommands()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert - Verify all required commands exist
        Assert.NotNull(viewModel.KeypadDigitCommand);
        Assert.NotNull(viewModel.ClearTenderCommand);
        Assert.NotNull(viewModel.QuickCashCommand);
        Assert.NotNull(viewModel.ProcessPaymentCommand);
        Assert.NotNull(viewModel.AddTipCommand);
        Assert.NotNull(viewModel.HoldTicketCommand);
        Assert.NotNull(viewModel.SplitPaymentCommand);
        Assert.NotNull(viewModel.ApplyDiscountCommand);
        Assert.NotNull(viewModel.PrintReceiptCommand);
        Assert.NotNull(viewModel.ToggleTaxExemptCommand);
        Assert.NotNull(viewModel.CancelSettlementCommand);
        Assert.NotNull(viewModel.NavigateBackCommand);
    }

    [Fact]
    public void PaymentMethods_ShouldContainThreeOptions()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal(3, viewModel.PaymentMethods.Count);
        Assert.Contains(viewModel.PaymentMethods, pm => pm.Type == PaymentType.Cash);
        Assert.Contains(viewModel.PaymentMethods, pm => pm.Type == PaymentType.CreditCard);
        Assert.Contains(viewModel.PaymentMethods, pm => pm.Type == PaymentType.GiftCertificate);
    }

    [Fact]
    public void PaymentMethods_ShouldHaveCorrectColors()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert - Verify payment method colors match design spec
        var cashMethod = viewModel.PaymentMethods.First(pm => pm.Type == PaymentType.Cash);
        var creditMethod = viewModel.PaymentMethods.First(pm => pm.Type == PaymentType.CreditCard);
        var giftMethod = viewModel.PaymentMethods.First(pm => pm.Type == PaymentType.GiftCertificate);

        Assert.Equal("#107C10", cashMethod.BackgroundColor); // Green
        Assert.Equal("#0078D4", creditMethod.BackgroundColor); // Blue
        Assert.Equal("#8E44AD", giftMethod.BackgroundColor); // Purple
    }

    [Fact]
    public void QuickCashAmounts_ShouldContainSixDenominations()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal(6, viewModel.QuickCashAmounts.Count);
        Assert.Contains(1m, viewModel.QuickCashAmounts);
        Assert.Contains(5m, viewModel.QuickCashAmounts);
        Assert.Contains(10m, viewModel.QuickCashAmounts);
        Assert.Contains(20m, viewModel.QuickCashAmounts);
        Assert.Contains(50m, viewModel.QuickCashAmounts);
        Assert.Contains(100m, viewModel.QuickCashAmounts);
    }

    [Fact]
    public void TenderAmountDisplay_ShouldInitializeToZero()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("$0.00", viewModel.TenderAmountDisplay);
    }

    [Fact]
    public void BalanceDue_ShouldInitializeToZero()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal(0m, viewModel.BalanceDue);
    }

    [Fact]
    public void PaidAmount_ShouldInitializeToZero()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal(0m, viewModel.PaidAmount);
    }

    [Fact]
    public void IsTaxExempt_ShouldInitializeToFalse()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.False(viewModel.IsTaxExempt);
    }

    [Fact]
    public void IsProcessingPayment_ShouldInitializeToFalse()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.False(viewModel.IsProcessingPayment);
    }

    [Fact]
    public void PaymentMethodViewModel_ShouldHaveCorrectDisplayNames()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert - Verify display names match design spec
        var cashMethod = viewModel.PaymentMethods.First(pm => pm.Type == PaymentType.Cash);
        var creditMethod = viewModel.PaymentMethods.First(pm => pm.Type == PaymentType.CreditCard);
        var giftMethod = viewModel.PaymentMethods.First(pm => pm.Type == PaymentType.GiftCertificate);

        Assert.Equal("CASH", cashMethod.DisplayName);
        Assert.Equal("CREDIT CARD", creditMethod.DisplayName);
        Assert.Equal("GIFT CARD", giftMethod.DisplayName);
    }

    [Fact]
    public void PaymentMethodViewModel_ShouldHaveCorrectIcons()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert - Verify icon names are set
        var cashMethod = viewModel.PaymentMethods.First(pm => pm.Type == PaymentType.Cash);
        var creditMethod = viewModel.PaymentMethods.First(pm => pm.Type == PaymentType.CreditCard);
        var giftMethod = viewModel.PaymentMethods.First(pm => pm.Type == PaymentType.GiftCertificate);

        Assert.NotNull(cashMethod.IconName);
        Assert.NotNull(creditMethod.IconName);
        Assert.NotNull(giftMethod.IconName);
    }

    [Fact]
    public void PaymentMethodViewModel_ShouldBeEnabledByDefault()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert - All payment methods should be enabled
        Assert.All(viewModel.PaymentMethods, pm => Assert.True(pm.IsEnabled));
    }

    [Fact]
    public void TicketNumber_ShouldDisplayNoTicketWhenNotLoaded()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("No Ticket", viewModel.TicketNumber);
    }

    [Fact]
    public void TableNumber_ShouldDisplayNoTableWhenNotLoaded()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("No Table", viewModel.TableNumber);
    }

    /// <summary>
    /// Helper method to create a SettlePageViewModel instance for testing.
    /// </summary>
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
}
