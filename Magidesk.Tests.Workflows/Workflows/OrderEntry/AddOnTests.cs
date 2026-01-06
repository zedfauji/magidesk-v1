using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels;
using Magidesk.ViewModels.Dialogs;
using Moq;
using Xunit;

namespace Magidesk.Tests.Workflows.Workflows.OrderEntry
{
    public class AddOnTests : Infrastructure.WorkflowTestBase
    {
        [Fact]
        public async Task AddOnSelection_ShouldAddOrderLines_WhenConfirmed()
        {
            // GIVEN
            var ticketId = Guid.NewGuid();
            var orderLineId = Guid.NewGuid();
            var orderLine = new OrderLineDto 
            { 
                Id = orderLineId, 
                TicketId = ticketId, 
                MenuItemId = Guid.NewGuid(),
                MenuItemName = "Super Burger",
                Quantity = 1
            };

            SetupActiveTicket(ticketId, new List<OrderLineDto> { orderLine });

            var addOnId = Guid.NewGuid();
            var addOnName = "Extra Extra Cheese";
            var addOnPrice = 1.50m;

            MockOrderEntryDialogService
                .Setup(x => x.ShowAddOnSelectionAsync(It.IsAny<AddOnSelectionViewModel>()))
                .Callback<AddOnSelectionViewModel>(vm => 
                {
                    // Simulate user selecting an add-on
                    vm.ResultAddOns.Add(new OrderLineModifierDto 
                    { 
                        ModifierId = addOnId, 
                        Name = addOnName, 
                        UnitPrice = addOnPrice,
                        TaxRate = 0
                    });
                    
                    typeof(AddOnSelectionViewModel)
                        .GetProperty(nameof(AddOnSelectionViewModel.IsConfirmed))?
                        .SetValue(vm, true);
                })
                .Returns(Task.CompletedTask);

            var vm = GetViewModel<OrderEntryViewModel>();
            await vm.InitializeAsync(ticketId);

            // WHEN
            await ((IAsyncRelayCommand<OrderLineDto?>)vm.AddOnSelectionCommand).ExecuteAsync(orderLine);

            // THEN
            MockAddOrderLineHandler.Verify(x => x.HandleAsync(It.Is<AddOrderLineCommand>(c => 
                c.TicketId == ticketId &&
                c.MenuItemId == addOnId &&
                c.MenuItemName == addOnName &&
                c.UnitPrice.Amount == addOnPrice
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
