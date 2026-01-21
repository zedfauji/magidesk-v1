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
using Magidesk.Presentation.ViewModels.Dialogs;
using Moq;
using Xunit;

namespace Magidesk.Tests.Workflows.Workflows.OrderEntry
{
    public class AddItemTests : Infrastructure.WorkflowTestBase
    {
        [Fact]
        public async Task AddManualPriceItem_ShouldPromptForPrice_AndUseEnteredPrice()
        {
            // GIVEN
            var ticketId = Guid.NewGuid();
            SetupActiveTicket(ticketId, new List<OrderLineDto>());

            var menuItemId = Guid.NewGuid();
            var menuItem = MenuItem.Create("Open Food", new Magidesk.Domain.ValueObjects.Money(0m), 0m);
            typeof(MenuItem).GetProperty(nameof(MenuItem.Id))?.SetValue(menuItem, menuItemId);
            
            // Set IsVariablePrice via properties dictionary
            var propsField = typeof(MenuItem).GetField("_properties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var props = (Dictionary<string, string>)propsField?.GetValue(menuItem)!;
            props["IsVariablePrice"] = "true";

            MockMenuRepository.Setup(x => x.GetByIdAsync(menuItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(menuItem);

            MockOrderEntryDialogService
                .Setup(x => x.ShowPriceEntryAsync(It.IsAny<PriceEntryViewModel>()))
                .Callback<PriceEntryViewModel>(vm => 
                {
                    vm.Price = 15.50m;
                    typeof(PriceEntryViewModel).GetProperty(nameof(PriceEntryViewModel.IsConfirmed))?.SetValue(vm, true);
                })
                .Returns(Task.CompletedTask);

            var vm = GetViewModel<OrderEntryViewModel>();
            await vm.InitializeAsync(ticketId);

            // WHEN
            await ((IAsyncRelayCommand<MenuItem?>)vm.AddItemCommand).ExecuteAsync(menuItem);

            // THEN
            MockAddOrderLineHandler.Verify(x => x.HandleAsync(It.Is<AddOrderLineCommand>(c => 
                c.TicketId == ticketId &&
                c.MenuItemId == menuItemId &&
                c.UnitPrice.Amount == 15.50m
            ), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task AddItemWithModifiers_ShouldPromptForModifiers_AndIncludeThem()
        {
            // GIVEN
            var ticketId = Guid.NewGuid();
            SetupActiveTicket(ticketId, new List<OrderLineDto>());

            var menuItemId = Guid.NewGuid();
            var menuItem = MenuItem.Create("Custom Burger", new Magidesk.Domain.ValueObjects.Money(10m), 0m);
            typeof(MenuItem).GetProperty(nameof(MenuItem.Id))?.SetValue(menuItem, menuItemId);
            
            // Give it a modifier group to trigger dialog
            var groupId = Guid.NewGuid();
            var group = ModifierGroup.Create("Add-ons");
            typeof(ModifierGroup).GetProperty(nameof(ModifierGroup.Id))?.SetValue(group, groupId);

            var itemGroupLine = MenuItemModifierGroup.Create(menuItem.Id, groupId);
            // We need to set the Navigation property ModifierGroup via reflection because IsPizza/AddItem uses it
            typeof(MenuItemModifierGroup).GetProperty(nameof(MenuItemModifierGroup.ModifierGroup))?.SetValue(itemGroupLine, group);

            var groupsField = typeof(MenuItem).GetField("_modifierGroups", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var groupsList = (List<MenuItemModifierGroup>)groupsField?.GetValue(menuItem)!;
            groupsList.Add(itemGroupLine);
            
            // Mock MenuRepository.GetByIdAsync for the modifier group if it's used elsewhere
            // Actually OrderEntryViewModel.AddItemAsync (line 1108) uses fullItem.ModifierGroups
            // It filters mg.ModifierGroup != null

            MockMenuRepository.Setup(x => x.GetByIdAsync(menuItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(menuItem);

            // Mock categories to ensure it's not a beverage
            MockCategoryRepository.Setup(x => x.GetVisibleAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<MenuCategory> { MenuCategory.Create("Burgers") });

            var modifierId = Guid.NewGuid();
            MockOrderEntryDialogService
                .Setup(x => x.ShowModifierSelectionAsync(It.IsAny<Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel>()))
                .Callback<Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel>(vm => 
                {
                    vm.ResultModifiers.Add(new OrderLineModifierDto { ModifierId = modifierId, Name = "Bacon" });
                    typeof(Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel).GetProperty(nameof(Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel.IsConfirmed))?.SetValue(vm, true);
                })
                .Returns(Task.CompletedTask);

            // Mock GetModifierByIdAsync
            var modifier = MenuModifier.Create("Bacon", ModifierType.Normal, new Magidesk.Domain.ValueObjects.Money(2m), null, null, 0m);
            typeof(MenuModifier).GetProperty(nameof(MenuModifier.Id))?.SetValue(modifier, modifierId);
            MockMenuRepository.Setup(x => x.GetModifierByIdAsync(modifierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(modifier);

            var vm = GetViewModel<OrderEntryViewModel>();
            await vm.InitializeAsync(ticketId);

            // WHEN
            await ((IAsyncRelayCommand<MenuItem?>)vm.AddItemCommand).ExecuteAsync(menuItem);

            // THEN
            MockAddOrderLineHandler.Verify(x => x.HandleAsync(It.Is<AddOrderLineCommand>(c => 
                c.TicketId == ticketId &&
                c.MenuItemId == menuItemId &&
                c.Modifiers != null &&
                c.Modifiers.Any(m => m.Id == modifierId)
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
