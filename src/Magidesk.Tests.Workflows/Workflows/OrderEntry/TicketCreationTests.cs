using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.ViewModels;
using Moq;
using Xunit;

namespace Magidesk.Tests.Workflows.Workflows.OrderEntry
{
    public class TicketCreationTests : Infrastructure.WorkflowTestBase
    {
        [Fact]
        public async Task Initialize_ShouldCreateNewTicket_WhenNoIdProvided()
        {
            // GIVEN
            var userId = Guid.NewGuid();
            var terminalId = Guid.NewGuid();
            var newTicketId = Guid.NewGuid();
            var ticketNumber = 101;

            SetupUser(userId);
            MockTerminalContext.Setup(x => x.TerminalId).Returns(terminalId);

            MockCreateTicketHandler.Setup(x => x.HandleAsync(It.IsAny<CreateTicketCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateTicketResult { TicketId = newTicketId, TicketNumber = ticketNumber });

            // Mock GetTicket for subsequent load
            MockGetTicketHandler.Setup(x => x.HandleAsync(It.Is<GetTicketQuery>(q => q.TicketId == newTicketId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TicketDto 
                { 
                    Id = newTicketId, 
                    TicketNumber = ticketNumber,
                    Status = TicketStatus.Open 
                });

            var vm = GetViewModel<OrderEntryViewModel>();

            // WHEN
            await vm.InitializeAsync(null);

            // THEN
            MockCreateTicketHandler.Verify(x => x.HandleAsync(It.Is<CreateTicketCommand>(c => 
                c.CreatedBy.Value == userId &&
                c.TerminalId == terminalId
            ), It.IsAny<CancellationToken>()), Times.Once());

            vm.HasTicket.Should().BeTrue();
            vm.Ticket.Should().NotBeNull();
            vm.Ticket!.Id.Should().Be(newTicketId);
            vm.Ticket.TicketNumber.Should().Be(ticketNumber);
        }

        [Fact]
        public async Task Initialize_ShouldLoadExistingTicket_WhenIdProvided()
        {
            // GIVEN
            var existingTicketId = Guid.NewGuid();
            var ticketNumber = 102;

            MockGetTicketHandler.Setup(x => x.HandleAsync(It.Is<GetTicketQuery>(q => q.TicketId == existingTicketId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TicketDto 
                { 
                    Id = existingTicketId, 
                    TicketNumber = ticketNumber,
                    Status = TicketStatus.Open 
                });

            var vm = GetViewModel<OrderEntryViewModel>();

            // WHEN
            await vm.InitializeAsync(existingTicketId);

            // THEN
            MockCreateTicketHandler.Verify(x => x.HandleAsync(It.IsAny<CreateTicketCommand>(), It.IsAny<CancellationToken>()), Times.Never());
            MockGetTicketHandler.Verify(x => x.HandleAsync(It.Is<GetTicketQuery>(q => q.TicketId == existingTicketId), It.IsAny<CancellationToken>()), Times.Once()); // Called twice in VM logic? Usually once.
            
            vm.HasTicket.Should().BeTrue();
            vm.Ticket.Should().NotBeNull();
            vm.Ticket!.Id.Should().Be(existingTicketId);
        }

        private void SetupUser(Guid userId)
        {
             MockUserService.Setup(x => x.CurrentUser).Returns(new Magidesk.Application.DTOs.UserDto { Id = userId });
        }
    }
}
