using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Application.DTOs;
using Magidesk.Application.Queries;
using Magidesk.Application.Commands.Inventory;
using MediatR;
using System;
using System.Collections.Generic;

namespace Magidesk.Presentation.Views;

public sealed partial class InventoryPage : Page
{
    public InventoryViewModel ViewModel { get; }
    private readonly IMediator _mediator;

    public InventoryPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetService<InventoryViewModel>()!;
        _mediator = App.Services.GetService<IMediator>()!;

        // Subscribe to bulk edit requested event
        ViewModel.BulkEditRequested += OnBulkEditRequested;

        // Subscribe to CRUD events
        ViewModel.CreateItemRequested += OnCreateItemRequested;
        ViewModel.EditItemRequested += OnEditItemRequested;
        ViewModel.DeleteItemRequested += OnDeleteItemRequested;
        ViewModel.CategoryManagementRequested += OnCategoryManagementRequested;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadCategoriesCommand.ExecuteAsync(null);
        await ViewModel.LoadPageCommand.ExecuteAsync(null);
    }

    private async void OnBulkEditRequested(object? sender, IReadOnlyList<InventoryItemDto> selectedItems)
    {
        var bulkEditViewModel = new InventoryBulkEditViewModel(selectedItems);
        var dialog = new InventoryBulkEditDialog
        {
            ViewModel = bulkEditViewModel,
            XamlRoot = this.XamlRoot
        };

        // Wire up confirmation and cancellation
        bulkEditViewModel.Confirmed += async (_, entries) =>
        {
            dialog.Hide();
            await ViewModel.CommitBulkEditAsync(entries);
        };

        bulkEditViewModel.Cancelled += (_, _) =>
        {
            dialog.Hide();
        };

        await dialog.ShowAsync();
    }

    private async void OnCreateItemRequested(object? sender, EventArgs e)
    {
        var createViewModel = App.Services.GetService<CreateInventoryItemViewModel>()!;
        var dialog = new CreateInventoryItemDialog(createViewModel)
        {
            XamlRoot = this.XamlRoot
        };

        // Wire up events
        createViewModel.ItemCreated += async (_, itemId) =>
        {
            dialog.Hide();
            await ViewModel.OnItemCreatedAsync();
        };

        createViewModel.Cancelled += (_, _) =>
        {
            dialog.Hide();
        };

        await dialog.ShowAsync();
    }

    private async void OnEditItemRequested(object? sender, Guid itemId)
    {
        var editViewModel = App.Services.GetService<EditInventoryItemViewModel>()!;
        var dialog = new EditInventoryItemDialog(editViewModel, itemId)
        {
            XamlRoot = this.XamlRoot
        };

        // Wire up events
        editViewModel.ItemUpdated += async (_, updatedItemId) =>
        {
            dialog.Hide();
            await ViewModel.OnItemUpdatedAsync();
        };

        editViewModel.Cancelled += (_, _) =>
        {
            dialog.Hide();
        };

        await dialog.ShowAsync();
    }

    private async void OnDeleteItemRequested(object? sender, Guid itemId)
    {
        var confirmDialog = new ContentDialog
        {
            Title = "Delete Inventory Item",
            Content = "Are you sure you want to delete this item? This action cannot be undone.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot
        };

        var result = await confirmDialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            try
            {
                await _mediator.Send(new DeleteInventoryItemCommand(itemId));
                await ViewModel.OnItemDeletedAsync();
            }
            catch (InvalidOperationException ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Cannot Delete Item",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
    }

    private async void OnCategoryManagementRequested(object? sender, EventArgs e)
    {
        var categoryViewModel = App.Services.GetService<CategoryManagementViewModel>()!;
        var dialog = new CategoryManagementDialog(categoryViewModel)
        {
            XamlRoot = this.XamlRoot
        };

        // Wire up event
        categoryViewModel.CategoriesChanged += async (_, _) =>
        {
            await ViewModel.OnCategoriesChangedAsync();
        };

        await dialog.ShowAsync();

        // Refresh categories after dialog closes
        await ViewModel.OnCategoriesChangedAsync();
    }

    private void OnFilterAllClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.SetFilterCommand.Execute(InventoryFilterType.None);
    }

    private void OnFilterLowStockClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.SetFilterCommand.Execute(InventoryFilterType.LowStock);
    }

    private void OnFilterOutOfStockClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.SetFilterCommand.Execute(InventoryFilterType.OutOfStock);
    }

    private void OnFilterRecentlyAddedClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.SetFilterCommand.Execute(InventoryFilterType.RecentlyAdded);
    }

    private void OnItemCheckBoxClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.Tag is InventoryItemDto item)
        {
            ViewModel.ToggleItemSelectionCommand.Execute(item);
        }
    }

    private void OnCategorySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            ViewModel.SelectedCategory = listBox.SelectedItem as InventoryCategoryDto;
        }
    }

    private void OnItemRowClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid itemId)
        {
            ViewModel.OpenEditItemCommand.Execute(itemId);
        }
    }

    private void OnDeleteItemClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid itemId)
        {
            ViewModel.DeleteItemCommand.Execute(itemId);
        }
    }
}
