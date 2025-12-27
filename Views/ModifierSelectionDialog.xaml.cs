using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class ModifierSelectionDialog : ContentDialog
{
    private readonly MenuItem _menuItem;

    public ModifierSelectionViewModel ViewModel { get; }

    public ModifierSelectionDialog(MenuItem menuItem)
    {
        InitializeComponent();
        _menuItem = menuItem;
        ViewModel = App.Services.GetRequiredService<ModifierSelectionViewModel>();
        this.Loaded += ModifierSelectionDialog_Loaded;
        this.Closing += ModifierSelectionDialog_Closing;
    }

    private void ModifierSelectionDialog_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.LoadModifiers(_menuItem);
    }

    private void ModifierSelectionDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        if (args.Result == ContentDialogResult.Primary)
        {
            if (!ViewModel.ValidateSelections())
            {
                args.Cancel = true; // Prevent closing if validation fails
            }
        }
    }
}
