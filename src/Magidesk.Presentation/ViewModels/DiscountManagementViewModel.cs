using System.Collections.ObjectModel;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.ViewModels;

public class DiscountManagementViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetDiscountsQuery, IEnumerable<DiscountDto>> _getDiscounts;
    
    private ObservableCollection<DiscountDto> _discounts = new();
    private DiscountDto? _selectedDiscount;

    public DiscountManagementViewModel(IQueryHandler<GetDiscountsQuery, IEnumerable<DiscountDto>> getDiscounts)
    {
        _getDiscounts = getDiscounts;
        Title = "Discount Administration";

        LoadDiscountsCommand = new AsyncRelayCommand(LoadDiscountsAsync);
        ManageScheduleCommand = new AsyncRelayCommand<DiscountDto>(ManageScheduleAsync);
    }

    public ObservableCollection<DiscountDto> Discounts
    {
        get => _discounts;
        set => SetProperty(ref _discounts, value);
    }

    public DiscountDto? SelectedDiscount
    {
        get => _selectedDiscount;
        set
        {
            if (SetProperty(ref _selectedDiscount, value))
            {
                OnPropertyChanged(nameof(CanManageSchedule));
            }
        }
    }

    public bool CanManageSchedule => SelectedDiscount != null;

    public AsyncRelayCommand LoadDiscountsCommand { get; }
    public AsyncRelayCommand<DiscountDto> ManageScheduleCommand { get; }

    public async Task InitializeAsync()
    {
        await LoadDiscountsAsync();
    }

    private async Task LoadDiscountsAsync()
    {
        IsBusy = true;
        try
        {
            var discounts = await _getDiscounts.HandleAsync(new GetDiscountsQuery { IncludeInactive = true });
            Discounts = new ObservableCollection<DiscountDto>(discounts);
        }
        catch (Exception ex)
        {
            // Handle error (e.g., through a dialog service or status property)
            System.Diagnostics.Debug.WriteLine($"Error loading discounts: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ManageScheduleAsync(DiscountDto? discount)
    {
        var targetDiscount = discount ?? SelectedDiscount;
        if (targetDiscount == null) return;

        var dialog = new Views.Dialogs.PromotionScheduleDialog();
        // XamlRoot must be set for ContentDialog in WinUI 3
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        // Initialize ViewModel
        dialog.ViewModel.Initialize(targetDiscount);

        await dialog.ShowAsync();
    }
}
