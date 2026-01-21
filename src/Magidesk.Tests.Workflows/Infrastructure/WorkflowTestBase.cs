using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.Services;
using Magidesk.Application.Commands;
using Magidesk.Presentation.ViewModels;

using Magidesk.Application.Queries;
using Magidesk.Application.DTOs;

namespace Magidesk.Tests.Workflows.Infrastructure
{
    public abstract class WorkflowTestBase : IAsyncLifetime
    {
        protected IServiceProvider ServiceProvider { get; private set; }
        protected Mock<ISecurityService> MockSecurityService { get; } = new();
        protected Mock<IAesEncryptionService> MockEncryptionService { get; } = new();
        protected Mock<NavigationService> MockNavigationService { get; private set; }
        protected Mock<IUserService> MockUserService { get; } = new();
        protected Mock<ICommandHandler<ClockInCommand>> MockClockInHandler { get; } = new();
        protected Mock<ICommandHandler<ClockOutCommand>> MockClockOutHandler { get; } = new();
        protected Mock<IAttendanceRepository> MockAttendanceRepository { get; } = new();
        protected Mock<IDefaultViewRoutingService> MockDefaultRoutingService { get; } = new();
        protected Mock<ITerminalContext> MockTerminalContext { get; } = new();
        
        // Session & Shift Mocks
        protected Mock<IShiftRepository> MockShiftRepository { get; } = new();
        protected Mock<IQueryHandler<GetCurrentCashSessionQuery, GetCurrentCashSessionResult>> MockGetCurrentSessionHandler { get; } = new();
        protected Mock<ICommandHandler<OpenCashSessionCommand, OpenCashSessionResult>> MockOpenSessionHandler { get; } = new();
        protected Mock<ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult>> MockCloseSessionHandler { get; } = new();
        protected Mock<ICommandHandler<CreateShiftCommand, CreateShiftResult>> MockCreateShiftHandler { get; } = new();
        protected Mock<ICommandHandler<UpdateShiftCommand, UpdateShiftResult>> MockUpdateShiftHandler { get; } = new();
        protected Mock<ICashSessionRepository> MockCashSessionRepository { get; } = new();
        protected Mock<IOrderTypeRepository> MockOrderTypeRepository { get; } = new();
        protected Mock<ISwitchboardDialogService> MockSwitchboardDialogService { get; } = new();
        protected Mock<IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>>> MockGetOpenTicketsHandler { get; } = new();

        // Order Entry Mocks
        protected Mock<IMenuCategoryRepository> MockCategoryRepository { get; } = new();
        protected Mock<IMenuGroupRepository> MockGroupRepository { get; } = new();
        protected Mock<IMenuRepository> MockMenuRepository { get; } = new();
        protected Mock<IQueryHandler<GetTicketQuery, TicketDto?>> MockGetTicketHandler { get; } = new();
        protected Mock<ICommandHandler<CreateTicketCommand, CreateTicketResult>> MockCreateTicketHandler { get; } = new();
        protected Mock<ICommandHandler<AddOrderLineCommand, AddOrderLineResult>> MockAddOrderLineHandler { get; } = new();
        protected Mock<ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult>> MockPrintToKitchenHandler { get; } = new();
        protected Mock<ICommandHandler<ModifyOrderLineCommand>> MockModifyOrderLineHandler { get; } = new();
        protected Mock<ICommandHandler<RemoveOrderLineCommand>> MockRemoveOrderLineHandler { get; } = new();
        protected Mock<ICommandHandler<PayNowCommand>> MockPayNowHandler { get; } = new();
        protected Mock<ICommandHandler<SetServiceChargeCommand, SetServiceChargeResult>> MockSetServiceChargeHandler { get; } = new();
        protected Mock<ICommandHandler<SetDeliveryChargeCommand, SetDeliveryChargeResult>> MockSetDeliveryChargeHandler { get; } = new();
        protected Mock<ICommandHandler<SetAdjustmentCommand, SetAdjustmentResult>> MockSetAdjustmentHandler { get; } = new();
        protected Mock<IPrintingService> MockPrintingService { get; } = new();
        protected Mock<IKitchenRoutingService> MockKitchenRoutingService { get; } = new();
        protected Mock<ICommandHandler<ChangeSeatCommand>> MockChangeSeatHandler { get; } = new();
        protected Mock<ICommandHandler<MergeTicketsCommand>> MockMergeTicketsHandler { get; } = new();
        protected Mock<ICommandHandler<ChangeTableCommand, ChangeTableResult>> MockChangeTableHandler { get; } = new();
        protected Mock<ICommandHandler<SetCustomerCommand, SetCustomerResult>> MockSetCustomerHandler { get; } = new();
        protected Mock<IOrderEntryDialogService> MockOrderEntryDialogService { get; } = new();

        protected WorkflowTestBase()
        {
            var services = new ServiceCollection();
            MockNavigationService = new Mock<NavigationService>(MockUserService.Object);

            // Register Mocks
            services.AddSingleton(MockSecurityService.Object);
            services.AddSingleton(MockEncryptionService.Object);
            services.AddSingleton(MockNavigationService.Object);
            services.AddSingleton(MockUserService.Object);
            services.AddSingleton(MockClockInHandler.Object);
            services.AddSingleton(MockClockOutHandler.Object);
            services.AddSingleton(MockAttendanceRepository.Object);
            services.AddSingleton(MockDefaultRoutingService.Object);
            services.AddSingleton(MockTerminalContext.Object);
            
            // Register Session/Shift Mocks
            services.AddSingleton(MockShiftRepository.Object);
            services.AddSingleton(MockGetCurrentSessionHandler.Object);
            services.AddSingleton(MockOpenSessionHandler.Object);
            services.AddSingleton(MockCloseSessionHandler.Object);
            services.AddSingleton(MockCreateShiftHandler.Object);
            services.AddSingleton(MockUpdateShiftHandler.Object);
            services.AddSingleton(MockCashSessionRepository.Object);
            services.AddSingleton(MockOrderTypeRepository.Object);
            services.AddSingleton(MockSwitchboardDialogService.Object);
            services.AddSingleton(MockGetOpenTicketsHandler.Object);

            // Register Order Entry Services
            services.AddSingleton(MockCategoryRepository.Object);
            services.AddSingleton(MockGroupRepository.Object);
            services.AddSingleton(MockMenuRepository.Object);
            services.AddSingleton(MockGetTicketHandler.Object);
            services.AddSingleton(MockCreateTicketHandler.Object);
            services.AddSingleton(MockAddOrderLineHandler.Object);
            services.AddSingleton(MockPrintToKitchenHandler.Object);
            services.AddSingleton(MockModifyOrderLineHandler.Object);
            services.AddSingleton(MockRemoveOrderLineHandler.Object);
            services.AddSingleton(MockPayNowHandler.Object);
            services.AddSingleton(MockSetServiceChargeHandler.Object);
            services.AddSingleton(MockSetDeliveryChargeHandler.Object);
            services.AddSingleton(MockSetAdjustmentHandler.Object);
            services.AddSingleton(MockPrintingService.Object);
            services.AddSingleton(MockKitchenRoutingService.Object);
            services.AddSingleton(MockChangeSeatHandler.Object);
            services.AddSingleton(MockMergeTicketsHandler.Object);
            services.AddSingleton(MockChangeTableHandler.Object);
            services.AddSingleton(MockSetCustomerHandler.Object);
            services.AddSingleton(MockOrderEntryDialogService.Object);

            // Register ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<CashSessionViewModel>();
            services.AddTransient<ShiftExplorerViewModel>();
            services.AddTransient<OrderEntryViewModel>();
            services.AddTransient<SwitchboardViewModel>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public virtual Task InitializeAsync()
        {
            // Reset Mocks per test
            MockSecurityService.Reset();
            MockUserService.Reset();
            
            // Default Setup
            MockEncryptionService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => "ENC_" + s);
            
            return Task.CompletedTask;
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        protected T GetViewModel<T>() where T : notnull
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
