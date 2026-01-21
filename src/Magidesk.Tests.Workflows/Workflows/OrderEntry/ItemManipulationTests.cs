using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.ViewModels;
using Moq;
using Xunit;

namespace Magidesk.Tests.Workflows.Workflows.OrderEntry
{
    public class ItemManipulationTests : Infrastructure.WorkflowTestBase
    {
        [Fact]
        public async Task IncrementQuantity_ShouldInvokeCommand_WhenCalled()
        {
            // GIVEN
            var ticketId = Guid.NewGuid();
            var orderLineId = Guid.NewGuid();
            var initialQuantity = 1.0m;

            var orderLine = new OrderLineDto 
            { 
                Id = orderLineId, 
                TicketId = ticketId, 
                Quantity = initialQuantity,
                MenuItemId = Guid.NewGuid(),
                MenuItemName = "Test Item"
            };

            SetupActiveTicket(ticketId, new List<OrderLineDto> { orderLine });

            var vm = GetViewModel<OrderEntryViewModel>();
            await vm.InitializeAsync(ticketId);

            // WHEN
            // Increment logic usually adds 1.
            // Note: Command might be generic usage
            await ((AsyncRelayCommand<OrderLineDto>)vm.IncrementQuantityCommand).ExecuteAsync(orderLine);

            // THEN
            MockModifyOrderLineHandler.Verify(x => x.HandleAsync(It.Is<ModifyOrderLineCommand>(c => 
                c.TicketId == ticketId &&
                c.OrderLineId == orderLineId &&
                c.Quantity == initialQuantity + 1
            ), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task RemoveItem_ShouldInvokeCommand_WhenCalled()
        {
            // GIVEN
            var ticketId = Guid.NewGuid();
            var orderLineId = Guid.NewGuid();
            var orderLine = new OrderLineDto 
            { 
                Id = orderLineId, 
                TicketId = ticketId, 
                Quantity = 1,
                MenuItemId = Guid.NewGuid(), 
                MenuItemName = "Test Item"
            };

            SetupActiveTicket(ticketId, new List<OrderLineDto> { orderLine });

            var vm = GetViewModel<OrderEntryViewModel>();
            await vm.InitializeAsync(ticketId);

            // WHEN
            await ((AsyncRelayCommand<OrderLineDto>)vm.RemoveItemCommand).ExecuteAsync(orderLine);

            // THEN
            MockRemoveOrderLineHandler.Verify(x => x.HandleAsync(It.Is<RemoveOrderLineCommand>(c => 
                c.TicketId == ticketId &&
                c.OrderLineId == orderLineId
            ), It.IsAny<CancellationToken>()), Times.Once());
        }

        private void SetupActiveTicket(Guid ticketId, List<OrderLineDto> orderLines)
        {
            MockGetTicketHandler.Setup(x => x.HandleAsync(It.Is<GetTicketQuery>(q => q.TicketId == ticketId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TicketDto 
                { 
                    Id = ticketId, 
                    TicketNumber = 101,
                    Status = TicketStatus.Open,
                    OrderLines = orderLines
                });
        }
    }
}
