using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.ViewModels;
using Moq;
using Xunit;

namespace Magidesk.Tests.Workflows.Workflows.Foundation
{
    public class ShiftTests : Infrastructure.WorkflowTestBase
    {
        [Fact]
        public async Task CreateShift_ShouldInvokeCommand_WhenAddIsExecuted()
        {
            // GIVEN
            var resultShiftId = Guid.NewGuid();
            MockCreateShiftHandler.Setup(x => x.HandleAsync(It.IsAny<CreateShiftCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateShiftResult { ShiftId = resultShiftId });

            MockShiftRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new System.Collections.Generic.List<Shift>());

            var vm = GetViewModel<ShiftExplorerViewModel>();

            // WHEN
            // Execute AddCommand
            await ExtractTaskFromCommand(vm.AddCommand);

            // THEN
            MockCreateShiftHandler.Verify(x => x.HandleAsync(It.IsAny<CreateShiftCommand>(), It.IsAny<CancellationToken>()), Times.Once());
            vm.StatusMessage.Should().Be("Shift created.");
        }

        [Fact]
        public async Task OpenCashSession_ShouldFail_WhenInputIsInvalid()
        {
            // GIVEN
            var vm = GetViewModel<CashSessionViewModel>();
            vm.UserIdText = "InvalidGuid";

            // WHEN
            await ExtractTaskFromCommand(vm.OpenCommand);

            // THEN
            vm.HasError.Should().BeTrue();
            vm.Error.Should().Contain("Invalid UserId");
            MockOpenSessionHandler.Verify(x => x.HandleAsync(It.IsAny<OpenCashSessionCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task OpenCashSession_ShouldInvokeCommand_WhenInputIsValid()
        {
            // GIVEN
            var userId = Guid.NewGuid();
            var terminalId = Guid.NewGuid();
            var shiftId = Guid.NewGuid();
            
            var vm = GetViewModel<CashSessionViewModel>();
            vm.UserIdText = userId.ToString();
            vm.TerminalIdText = terminalId.ToString();
            vm.ShiftIdText = shiftId.ToString();
            vm.OpeningBalanceText = "100.00";

            OpenCashSessionCommand capturedCommand = null;
            MockOpenSessionHandler.Setup(x => x.HandleAsync(It.IsAny<OpenCashSessionCommand>(), It.IsAny<CancellationToken>()))
                .Callback<OpenCashSessionCommand, CancellationToken>((c, t) => capturedCommand = c)
                .ReturnsAsync(new OpenCashSessionResult { CashSessionId = Guid.NewGuid() });

            MockGetCurrentSessionHandler.Setup(x => x.HandleAsync(It.IsAny<GetCurrentCashSessionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCurrentCashSessionResult { CashSession = null }); // Return valid result container

            // WHEN
            await ExtractTaskFromCommand(vm.OpenCommand);

            var expectedOpening = new Money(100.00m);
            var expectedUserId = new UserId(userId);

            // THEN
            vm.HasError.Should().BeFalse($"because no error should occur, but found: {vm.Error}");
            capturedCommand.Should().NotBeNull();
            
            capturedCommand.UserId.Value.Should().Be(userId);
            capturedCommand.TerminalId.Should().Be(terminalId);
            capturedCommand.ShiftId.Should().Be(shiftId);
            capturedCommand.OpeningBalance.Should().Be(expectedOpening);
        }

        [Fact]
        public async Task CloseCashSession_ShouldInvokeCommand_WhenActiveSessionExists()
        {
            // GIVEN
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var activeSession = new CashSessionDto 
            { 
                Id = sessionId, 
                OpenedAt = DateTime.Now,
                ExpectedCash = 500.00m
            };

            MockGetCurrentSessionHandler.Setup(x => x.HandleAsync(It.IsAny<GetCurrentCashSessionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCurrentCashSessionResult { CashSession = activeSession });

            var vm = GetViewModel<CashSessionViewModel>();
            await vm.RefreshAsync(); // Load the active session

            vm.UserIdText = userId.ToString();
            vm.ActualCashText = "500.00";

            // WHEN
            await ExtractTaskFromCommand(vm.CloseCommand);

            var expectedActual = new Money(500.00m);

            // THEN
            vm.HasError.Should().BeFalse();
            MockCloseSessionHandler.Verify(x => x.HandleAsync(It.Is<CloseCashSessionCommand>(c => 
                c.CashSessionId == sessionId &&
                c.ActualCash == expectedActual
            ), It.IsAny<CancellationToken>()), Times.Once());
        }

        // Helper
        private async Task ExtractTaskFromCommand(System.Windows.Input.ICommand command)
        {
            if (command is CommunityToolkit.Mvvm.Input.IAsyncRelayCommand asyncCmd)
            {
                await asyncCmd.ExecuteAsync(null);
            }
            else
            {
                command.Execute(null);
            }
        }
    }
}
