using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class OrderTypeSelectionDialog : ContentDialog
{
    public OrderTypeSelectionViewModel ViewModel { get; }

    public OrderType? SelectedOrderType => ViewModel.SelectedOrderType;

    public OrderTypeSelectionDialog()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<OrderTypeSelectionViewModel>();
        this.Opened += OrderTypeSelectionDialog_Opened;
    }

    private async void OrderTypeSelectionDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.LoadOrderTypesAsync();
    }

    private void GridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is OrderType orderType)
        {
            ViewModel.SelectedOrderType = orderType;
            Hide(); // Closes the dialog with ContentDialogResult.None by default, but we have the result in the property
        }
    }

    private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Optional: Enable/Disable 'OK' button if we had one.
        // For now, single click selects and closes.
    }
}
