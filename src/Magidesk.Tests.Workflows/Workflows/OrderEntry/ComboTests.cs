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
    public class ComboTests : Infrastructure.WorkflowTestBase
    {
        [Fact]
        public async Task ComboSelection_ShouldAddItemsAndRemoveParent_WhenConfirmed()
        {
            // GIVEN
            var ticketId = Guid.NewGuid();
            var comboLineId = Guid.NewGuid();
            var comboLine = new OrderLineDto 
            { 
                Id = comboLineId, 
                TicketId = ticketId, 
                MenuItemId = Guid.NewGuid(),
                MenuItemName = "Meal Combo",
                Quantity = 1
            };

            SetupActiveTicket(ticketId, new List<OrderLineDto> { comboLine });

            var sodaId = Guid.NewGuid();
            var friesId = Guid.NewGuid();

            MockOrderEntryDialogService
                .Setup(x => x.ShowComboSelectionAsync(It.IsAny<ComboSelectionViewModel>()))
                .Callback<ComboSelectionViewModel>(vm => 
                {
                    // Simulate user selecting combo items
                    vm.ResultSelections.Add(new ComboSelectionResultDto 
                    { 
                        MenuItemId = sodaId, 
                        ItemName = "Soda", 
                        BasePrice = 0, 
                        Upcharge = 0,
                        GroupName = "Drink"
                    });
                    vm.ResultSelections.Add(new ComboSelectionResultDto 
                    { 
                        MenuItemId = friesId, 
                        ItemName = "Fries", 
                        BasePrice = 0, 
                        Upcharge = 0,
                        GroupName = "Side"
                    });
                    
                    typeof(ComboSelectionViewModel)
                        .GetProperty(nameof(ComboSelectionViewModel.IsConfirmed))?
                        .SetValue(vm, true);
                })
                .Returns(Task.CompletedTask);

            var vm = GetViewModel<OrderEntryViewModel>();
            await vm.InitializeAsync(ticketId);

            // WHEN
            await ((IAsyncRelayCommand<OrderLineDto?>)vm.ComboSelectionCommand).ExecuteAsync(comboLine);

            // THEN
            // Verify AddOrderLine was called for both selections
            MockAddOrderLineHandler.Verify(x => x.HandleAsync(It.IsAny<AddOrderLineCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            
            MockAddOrderLineHandler.Verify(x => x.HandleAsync(It.Is<AddOrderLineCommand>(c => 
                c.TicketId == ticketId && c.MenuItemId == sodaId
            ), It.IsAny<CancellationToken>()), Times.Once());
            
            MockAddOrderLineHandler.Verify(x => x.HandleAsync(It.Is<AddOrderLineCommand>(c => 
                c.TicketId == ticketId && c.MenuItemId == friesId
            ), It.IsAny<CancellationToken>()), Times.Once());

            // Verify the original combo line was removed
            MockRemoveOrderLineHandler.Verify(x => x.HandleAsync(It.Is<RemoveOrderLineCommand>(c => 
                c.TicketId == ticketId && c.OrderLineId == comboLineId
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
