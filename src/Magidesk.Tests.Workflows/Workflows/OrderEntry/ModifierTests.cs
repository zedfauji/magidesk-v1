using System;
using System.Collections.Generic;
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
using Magidesk.Presentation.ViewModels.Dialogs;
using Moq;
using Xunit;

namespace Magidesk.Tests.Workflows.Workflows.OrderEntry
{
    public class ModifierTests : Infrastructure.WorkflowTestBase
    {
        [Fact]
        public async Task EditModifiers_ShouldInvokeHandler_WhenDialogIsConfirmed()
        {
            // GIVEN
            var ticketId = Guid.NewGuid();
            var orderLineId = Guid.NewGuid();
            var menuItemId = Guid.NewGuid();
            var orderLine = new OrderLineDto 
            { 
                Id = orderLineId, 
                TicketId = ticketId, 
                MenuItemId = menuItemId,
                MenuItemName = "Custom Burger",
                Quantity = 1
            };

            SetupActiveTicket(ticketId, new List<OrderLineDto> { orderLine });

            // Setup Dialog Service Result
            MockOrderEntryDialogService
                .Setup(x => x.ShowModifierSelectionAsync(It.IsAny<Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel>()))
                .Callback<Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel>(vm => 
                {
                    // Simulate user selecting modifiers
                    vm.ResultModifiers.Add(new OrderLineModifierDto { ModifierId = Guid.NewGuid(), Name = "Extra Cheese" });
                    
                    // Use reflection to set private setter
                    typeof(Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel)
                        .GetProperty(nameof(Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel.IsConfirmed))?
                        .SetValue(vm, true);
                })
                .Returns(Task.CompletedTask);

            var vm = GetViewModel<OrderEntryViewModel>();
            await vm.InitializeAsync(ticketId);

            // WHEN
            await ((IAsyncRelayCommand<OrderLineDto?>)vm.EditModifiersCommand).ExecuteAsync(orderLine);

            // THEN
            MockModifyOrderLineHandler.Verify(x => x.HandleAsync(It.Is<ModifyOrderLineCommand>(c => 
                c.TicketId == ticketId &&
                c.OrderLineId == orderLineId &&
                c.Modifiers != null && 
                c.Modifiers.Count == 1 &&
                c.Modifiers[0].Name == "Extra Cheese"
            ), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task AddCookingInstruction_ShouldUpdateLine_WhenConfirmed()
        {
            // GIVEN
            var ticketId = Guid.NewGuid();
            var orderLineId = Guid.NewGuid();
            var orderLine = new OrderLineDto 
            { 
                Id = orderLineId, 
                TicketId = ticketId, 
                MenuItemId = Guid.NewGuid(),
                Quantity = 1,
                Instructions = "None"
            };

            SetupActiveTicket(ticketId, new List<OrderLineDto> { orderLine });

            MockOrderEntryDialogService
                .Setup(x => x.ShowCookingInstructionAsync(It.IsAny<Magidesk.Presentation.ViewModels.Dialogs.CookingInstructionViewModel>()))
                .Callback<Magidesk.Presentation.ViewModels.Dialogs.CookingInstructionViewModel>(vm => 
                {
                    vm.SelectedInstructions = "Extra Spicy";
                    vm.ConfirmCommand.Execute(null);
                })
                .Returns(Task.CompletedTask);

            var vm = GetViewModel<OrderEntryViewModel>();
            await vm.InitializeAsync(ticketId);

            // WHEN
            await ((IAsyncRelayCommand<OrderLineDto>)vm.AddCookingInstructionCommand).ExecuteAsync(orderLine);

            // THEN
            MockModifyOrderLineHandler.Verify(x => x.HandleAsync(It.Is<ModifyOrderLineCommand>(c => 
                c.TicketId == ticketId &&
                c.OrderLineId == orderLineId &&
                c.Instructions == "Extra Spicy"
            ), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task EditPizzaModifiers_ShouldInvokeHandler_WhenDialogIsConfirmed()
        {
            // GIVEN
            var ticketId = Guid.NewGuid();
            var orderLineId = Guid.NewGuid();
            var menuItemId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var orderLine = new OrderLineDto 
            { 
                Id = orderLineId, 
                TicketId = ticketId, 
                MenuItemId = menuItemId,
                MenuItemName = "12\" Pepperoni",
                CategoryName = "Pizzas", // Informational
                Quantity = 1
            };

            SetupActiveTicket(ticketId, new List<OrderLineDto> { orderLine });

            // Mock repository for IsPizza check
            var pizzaItem = MenuItem.Create("12\" Pepperoni", new Magidesk.Domain.ValueObjects.Money(10m), 0m);
            typeof(MenuItem).GetProperty(nameof(MenuItem.Id))?.SetValue(pizzaItem, menuItemId);
            typeof(MenuItem).GetProperty(nameof(MenuItem.CategoryId))?.SetValue(pizzaItem, categoryId);

            MockMenuRepository.Setup(x => x.GetByIdAsync(menuItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pizzaItem);
            
            // Mock category in ViewModel (IsPizza checks vm.Categories)
            // OrderEntryViewModel calls GetVisibleAsync during InitializeAsync
            MockCategoryRepository.Setup(x => x.GetVisibleAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<MenuCategory> 
                { 
                    CreateCategoryWithId(categoryId, "Pizzas")
                });

            MockOrderEntryDialogService
                .Setup(x => x.ShowPizzaModifierAsync(It.IsAny<Magidesk.Presentation.ViewModels.Dialogs.PizzaModifierViewModel>()))
                .Callback<Magidesk.Presentation.ViewModels.Dialogs.PizzaModifierViewModel>(vm => 
                {
                    // Simulate user selecting toppings on halves
                    vm.ResultModifiers.Add(new OrderLineModifierDto { ModifierId = Guid.NewGuid(), Name = "Mushrooms", SectionName = "Left" });
                    vm.ResultModifiers.Add(new OrderLineModifierDto { ModifierId = Guid.NewGuid(), Name = "Onions", SectionName = "Right" });
                    
                    typeof(Magidesk.Presentation.ViewModels.Dialogs.PizzaModifierViewModel)
                        .GetProperty(nameof(Magidesk.Presentation.ViewModels.Dialogs.PizzaModifierViewModel.IsConfirmed))?
                        .SetValue(vm, true);
                })
                .Returns(Task.CompletedTask);

            var vm = GetViewModel<OrderEntryViewModel>();
            await vm.InitializeAsync(ticketId);

            // WHEN
            await ((IAsyncRelayCommand<OrderLineDto?>)vm.EditModifiersCommand).ExecuteAsync(orderLine);

            // THEN
            MockModifyOrderLineHandler.Verify(x => x.HandleAsync(It.Is<ModifyOrderLineCommand>(c => 
                c.TicketId == ticketId &&
                c.OrderLineId == orderLineId &&
                c.Modifiers != null && 
                c.Modifiers.Count == 2 &&
                c.Modifiers.Any(m => m.Name == "Mushrooms" && m.SectionName == "Left") &&
                c.Modifiers.Any(m => m.Name == "Onions" && m.SectionName == "Right")
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

        private MenuCategory CreateCategoryWithId(Guid id, string name)
        {
            var category = MenuCategory.Create(name, 0);
            typeof(MenuCategory).GetProperty(nameof(MenuCategory.Id))?.SetValue(category, id);
            return category;
        }
    }
}
