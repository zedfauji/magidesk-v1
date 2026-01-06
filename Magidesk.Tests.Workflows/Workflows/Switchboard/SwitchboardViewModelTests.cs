using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Application.Interfaces;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views;
using Moq;
using Xunit;

namespace Magidesk.Tests.Workflows.Workflows.Switchboard
{
    public class SwitchboardViewModelTests : Infrastructure.WorkflowTestBase
    {
        [Fact]
        public async Task NewTicket_ShouldPromptForGuestCount_AndCreateTicket_WhenDineInSelected()
        {
            // GIVEN
            var orderType = OrderType.Create("DINE IN", false); // Changed to false to avoid "Table required" block
            var ticketId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var terminalId = Guid.NewGuid();

            MockTerminalContext.Setup(x => x.TerminalId).Returns(terminalId);

            MockOrderTypeRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OrderType> { orderType });

            MockUserService.Setup(x => x.CurrentUser)
                .Returns(new UserDto { Id = employeeId, FirstName = "Test", LastName = "User" });

            // Mock active session exists
            var shiftId = Guid.NewGuid();
            var session = CashSession.Open(employeeId, terminalId, shiftId, Money.Zero());
            MockCashSessionRepository.Setup(x => x.GetOpenSessionByTerminalIdAsync(terminalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);

            // Simulating Dialog results
            MockSwitchboardDialogService
                .Setup(x => x.ShowOrderTypeSelectionAsync(It.IsAny<OrderTypeSelectionViewModel>()))
                .Callback<OrderTypeSelectionViewModel>(vm => vm.SelectedOrderType = orderType)
                .Returns(Task.CompletedTask);

            MockSwitchboardDialogService
                .Setup(x => x.ShowGuestCountAsync(It.IsAny<GuestCountViewModel>()))
                .Callback<GuestCountViewModel>(vm => 
                {
                    vm.GuestCount = 4;
                    vm.ConfirmCommand.Execute(null);
                })
                .Returns(Task.CompletedTask);

            MockCreateTicketHandler.Setup(x => x.HandleAsync(It.IsAny<CreateTicketCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateTicketResult { TicketId = ticketId });

            var viewModel = GetViewModel<SwitchboardViewModel>();

            // WHEN
            await ((CommunityToolkit.Mvvm.Input.IAsyncRelayCommand)viewModel.NewTicketCommand).ExecuteAsync(null);

            // THEN
            MockCreateTicketHandler.Verify(x => x.HandleAsync(It.Is<CreateTicketCommand>(c => 
                c.OrderTypeId == orderType.Id && 
                c.NumberOfGuests == 4), It.IsAny<CancellationToken>()), Times.Once());
            
            MockNavigationService.Verify(x => x.Navigate(typeof(OrderEntryPage), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public async Task NewTicket_ShouldPromptForShiftStart_WhenNoActiveShift()
        {
            // GIVEN
            var orderType = OrderType.Create("Takeout", false);
            var employeeId = Guid.NewGuid();
            var terminalId = Guid.NewGuid();

            MockTerminalContext.Setup(x => x.TerminalId).Returns(terminalId);

            MockOrderTypeRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OrderType> { orderType });

            MockUserService.Setup(x => x.CurrentUser)
                .Returns(new UserDto { Id = employeeId, FirstName = "Test", LastName = "User" });

            // NO active shift
            MockShiftRepository.Setup(x => x.GetCurrentShiftAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((Shift?)null);

            MockSwitchboardDialogService
                .Setup(x => x.ShowOrderTypeSelectionAsync(It.IsAny<OrderTypeSelectionViewModel>()))
                .Callback<OrderTypeSelectionViewModel>(vm => vm.SelectedOrderType = orderType)
                .Returns(Task.CompletedTask);

            MockSwitchboardDialogService
                .Setup(x => x.ShowShiftStartAsync(It.IsAny<ShiftStartViewModel>()))
                .Callback<ShiftStartViewModel>(vm => 
                {
                    vm.ConfirmCommand.Execute(null);
                })
                .Returns(Task.CompletedTask);

            var viewModel = GetViewModel<SwitchboardViewModel>();

            // WHEN
            await ((CommunityToolkit.Mvvm.Input.IAsyncRelayCommand)viewModel.NewTicketCommand).ExecuteAsync(null);

            // THEN
            MockSwitchboardDialogService.Verify(x => x.ShowShiftStartAsync(It.IsAny<ShiftStartViewModel>()), Times.Once());
        }
    }
}
