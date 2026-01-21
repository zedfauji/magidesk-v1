using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Application.DTOs;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views.Components;

public sealed partial class CustomerSearchControl : UserControl
{
    public static readonly DependencyProperty SelectedCustomerProperty =
        DependencyProperty.Register(nameof(SelectedCustomer), typeof(CustomerSearchResultDto), typeof(CustomerSearchControl), new PropertyMetadata(null));

    public CustomerSearchResultDto? SelectedCustomer
    {
        get => (CustomerSearchResultDto?)GetValue(SelectedCustomerProperty);
        set => SetValue(SelectedCustomerProperty, value);
    }

    public CustomerSearchViewModel ViewModel { get; }

    public CustomerSearchControl()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<CustomerSearchViewModel>();
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CustomerSearchViewModel.SelectedDto))
        {
            SelectedCustomer = ViewModel.SelectedDto;
        }
    }

    private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is CustomerSearchResultViewModel result)
        {
            ViewModel.SelectCustomer(result);
        }
    }
}
