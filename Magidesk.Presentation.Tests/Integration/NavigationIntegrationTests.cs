using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Presentation.Tests.Integration;

/// <summary>
/// Integration tests for navigation flows between Order Page and Settle Page.
/// Tests navigation, ticket state preservation, and dialog interactions.
/// </summary>
public class NavigationIntegrationTests
{
    private readonly Mock<IQueryHandler<GetTicketQuery, TicketDto?>> _mockGetTicketHandler;
    private readonly Mock<IQueryHandler<GetProductCatalogQuery, ProductCatalogDto>> _mockGetProductCatalogHandler;
    private readonly Mock<ICommandHandler<AddOrderLineCommand, AddOrderLineResult>> _mockAddOrderLineHandler;
    private readonly Mock<ICommandHandler<RemoveOrderLineCommand>> _mockRemoveOrderLineHandler;
    private readonly Mock<ICommandHandler<CreateTicketCommand, CreateTicketResult>> _mockCreateTicketHandler;
    private readonly Mock<NavigationService> _mockNavigationService;
    private readonly Mock<IDialogService> _mockDialogService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ITerminalContext> _mockTerminalContext;
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;
    private readonly Mock<ILogger<OrderPageViewModel>> _mockOrderLogger;
    private readonly Mock<ILogger<SettlePageViewModel>> _mockSettleLogger;

    public NavigationIntegrationTests()
    {
        _mockGetTicketHandler = new Mock<IQueryHandler<GetTicketQuery, TicketDto?>>();
        _mockGetProductCatalogHandler = new Mock<IQueryHandler<GetProductCatalogQuery, ProductCatalogDto>>();
        _mockAddOrderLineHandler = new Mock<ICommandHandler<AddOrderLineCommand, AddOrderLineResult>>();
        _mockRemoveOrderLineHandler = new Mock<ICommandHandler<RemoveOrderLineCommand>>();
        _mockCreateTicketHandler = new Mock<ICommandHandler<CreateTicketCommand, CreateTicketResult>>();
        _mockNavigationService = new Mock<NavigationService>();
        _mockDialogService = new Mock<IDialogService>();
        _mockUserService = new Mock<IUserService>();
        _mockTerminalContext = new Mock<ITerminalContext>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        _mockOrderLogger = new Mock<ILogger<OrderPageViewModel>>();
        _mockSettleLogger = new Mock<ILogger<SettlePageViewModel>>();

        // Setup service scope factory
        var mockScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IQueryHandler<GetTicketQuery, TicketDto?>)))
            .Returns(_mockGetTicketHandler.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IQueryHandler<GetProductCatalogQuery, ProductCatalogDto>)))
            .Returns(_mockGetProductCatalogHandler.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(ICommandHandler<AddOrderLineCommand, AddOrderLineResult>)))
            .Returns(_mockAddOrderLineHandler.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(ICommandHandler<RemoveOrderLineCommand>)))
            .Returns(_mockRemoveOrderLineHandler.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(ICommandHandler<CreateTicketCommand, CreateTicketResult>)))
            .Returns(_mockCreateTicketHandler.Object);
        
        mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);
        _mockServiceScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

        // Setup default user and terminal
        _mockUserService.Setup(u => u.CurrentUser).Returns(new UserDto { Id = Guid.NewGuid(), Username = "TestUser" });
        _mockTerminalContext.Setup(t => t.TerminalId).Returns("TEST-TERMINAL");
    }

    [Fact]
    public async Task NavigateToSettle_WithTicketId_NavigatesToSettlePageView()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var ticket = CreateTestTicket(ticketId);
        
        _mockGetTicketHandler.Setup(h => h.HandleAsync(It.IsAny<GetTicketQuery>()))
            .ReturnsAsync(ticket);
        
        _mockGetProductCatalogHandler.Setup(h => h.HandleAsync(It.IsAny<GetProductCatalogQuery>()))
            .ReturnsAsync(new ProductCatalogDto { Categories = new List<ProductCategoryDto>() });

        var viewModel = CreateOrderPageViewModel();
        await viewModel.InitializeAsync(ticketId);

        // Act
        await viewModel.NavigateToSettleCommand.ExecuteAsync(null);

        // Assert
        _mockNavigationService.Verify(
            n => n.Navigate(typeof(Magidesk.Presentation.Views.SettlePageView), ticketId),
            Times.Once);
    }

    [Fact]
    public async Task NavigateBack_FromSettlePage_CallsGoBack()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var ticket = CreateTestTicket(ticketId);
        
        _mockGetTicketHandler.Setup(h => h.HandleAsync(It.IsAny<GetTicketQuery>()))
            .ReturnsAsync(ticket);

        var viewModel = CreateSettlePageViewModel();
        await viewModel.InitializeAsync(ticketId);

        // Act
        viewModel.NavigateBackCommand.Execute(null);

        // Assert
        _mockNavigationService.Verify(n => n.GoBack(), Times.Once);
    }

    [Fact]
    public async Task CancelSettlement_PreservesTicketState()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var ticket = CreateTestTicket(ticketId);
        
        _mockGetTicketHandler.Setup(h => h.HandleAsync(It.IsAny<GetTicketQuery>()))
            .ReturnsAsync(ticket);

        var viewModel = CreateSettlePageViewModel();
        await viewModel.InitializeAsync(ticketId);

        var originalBalanceDue = viewModel.BalanceDue;

        // Act
        viewModel.CancelSettlementCommand.Execute(null);

        // Assert
        Assert.Equal(originalBalanceDue, viewModel.BalanceDue);
        _mockNavigationService.Verify(n => n.GoBack(), Times.Once);
    }

    [Fact]
    public async Task RefreshTicket_AfterNavigatingBack_ReloadsTicketData()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var ticket = CreateTestTicket(ticketId);
        
        var callCount = 0;
        _mockGetTicketHandler.Setup(h => h.HandleAsync(It.IsAny<GetTicketQuery>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                // Simulate ticket being updated (e.g., payment applied)
                if (callCount > 1)
                {
                    ticket.PaidAmount = 50m;
                    ticket.DueAmount = 50m;
                }
                return ticket;
            });
        
        _mockGetProductCatalogHandler.Setup(h => h.HandleAsync(It.IsAny<GetProductCatalogQuery>()))
            .ReturnsAsync(new ProductCatalogDto { Categories = new List<ProductCategoryDto>() });

        var viewModel = CreateOrderPageViewModel();
        await viewModel.InitializeAsync(ticketId);

        var originalTotal = viewModel.Total;

        // Act - Simulate navigating back and refreshing
        await viewModel.RefreshTicketAsync();

        // Assert
        Assert.Equal(2, callCount); // Ticket loaded twice
        // Verify ticket data was reloaded (in real scenario, values would change)
    }

    [Fact]
    public async Task TicketStatePreservation_DuringNavigation_MaintainsTicketId()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var ticket = CreateTestTicket(ticketId);
        
        _mockGetTicketHandler.Setup(h => h.HandleAsync(It.IsAny<GetTicketQuery>()))
            .ReturnsAsync(ticket);
        
        _mockGetProductCatalogHandler.Setup(h => h.HandleAsync(It.IsAny<GetProductCatalogQuery>()))
            .ReturnsAsync(new ProductCatalogDto { Categories = new List<ProductCategoryDto>() });

        var orderViewModel = CreateOrderPageViewModel();
        await orderViewModel.InitializeAsync(ticketId);

        // Act - Navigate to settle
        await orderViewModel.NavigateToSettleCommand.ExecuteAsync(null);

        // Verify navigation was called with correct ticket ID
        _mockNavigationService.Verify(
            n => n.Navigate(typeof(Magidesk.Presentation.Views.SettlePageView), ticketId),
            Times.Once);

        // Create settle view model with same ticket ID
        var settleViewModel = CreateSettlePageViewModel();
        await settleViewModel.InitializeAsync(ticketId);

        // Assert - Ticket data is loaded in settle view model
        Assert.NotNull(settleViewModel.TicketNumber);
        Assert.True(settleViewModel.BalanceDue > 0);
    }

    private OrderPageViewModel CreateOrderPageViewModel()
    {
        return new OrderPageViewModel(
            _mockServiceScopeFactory.Object,
            _mockGetTicketHandler.Object,
            _mockGetProductCatalogHandler.Object,
            _mockAddOrderLineHandler.Object,
            _mockRemoveOrderLineHandler.Object,
            _mockCreateTicketHandler.Object,
            _mockNavigationService.Object,
            _mockDialogService.Object,
            _mockUserService.Object,
            _mockTerminalContext.Object,
            _mockOrderLogger.Object);
    }

    private SettlePageViewModel CreateSettlePageViewModel()
    {
        return new SettlePageViewModel(
            _mockServiceScopeFactory.Object,
            _mockNavigationService.Object,
            _mockDialogService.Object,
            _mockUserService.Object,
            _mockSettleLogger.Object);
    }

    private TicketDto CreateTestTicket(Guid ticketId)
    {
        return new TicketDto
        {
            Id = ticketId,
            TicketNumber = "12345",
            TableNumber = "10",
            Status = "Open",
            CreatedAt = DateTime.Now.AddMinutes(-30),
            OrderLines = new List<OrderLineDto>
            {
                new OrderLineDto
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = Guid.NewGuid(),
                    MenuItemName = "Test Item",
                    Quantity = 2,
                    UnitPrice = 25m,
                    TotalAmount = 50m,
                    TaxRate = 0.08m,
                    Modifiers = new List<OrderLineModifierDto>()
                }
            },
            Subtotal = 50m,
            TaxAmount = 4m,
            TotalAmount = 54m,
            PaidAmount = 0m,
            DueAmount = 54m,
            IsTaxExempt = false
        };
    }
}
