using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for the CustomerSearchControl.
/// </summary>
public partial class CustomerSearchViewModel : ObservableObject
{
    private readonly IQueryHandler<SearchCustomersQuery, IEnumerable<CustomerSearchResultDto>> _searchHandler;
    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<CustomerSearchResultViewModel> _searchResults = new();

    [ObservableProperty]
    private CustomerSearchResultViewModel? _selectedCustomer;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedCustomer))]
    [NotifyPropertyChangedFor(nameof(SelectedCustomerName))]
    [NotifyPropertyChangedFor(nameof(SelectedCustomerPhone))]
    private CustomerSearchResultDto? _selectedDto;

    public bool HasSelectedCustomer => SelectedDto != null;
    public string SelectedCustomerName => SelectedDto?.FullName ?? string.Empty;
    public string SelectedCustomerPhone => SelectedDto?.Phone ?? string.Empty;

    public CustomerSearchViewModel(IQueryHandler<SearchCustomersQuery, IEnumerable<CustomerSearchResultDto>> searchHandler)
    {
        _searchHandler = searchHandler;
    }

    partial void OnSearchTextChanged(string value)
    {
        if (value.Length < 2)
        {
            SearchResults.Clear();
            return;
        }

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _ = PerformSearchAsync(value, _cts.Token);
    }

    private async Task PerformSearchAsync(string term, CancellationToken ct)
    {
        try
        {
            // Debounce
            await Task.Delay(300, ct);

            var query = new SearchCustomersQuery(term);
            var results = await _searchHandler.HandleAsync(query, ct);

            if (!ct.IsCancellationRequested)
            {
                SearchResults.Clear();
                foreach (var result in results)
                {
                    SearchResults.Add(new CustomerSearchResultViewModel(result));
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception)
        {
            // Logging would go here
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        SelectedDto = null;
        SearchText = string.Empty;
        SearchResults.Clear();
    }

    public void SelectCustomer(CustomerSearchResultViewModel result)
    {
        SelectedDto = result.Dto;
        SearchText = string.Empty;
        SearchResults.Clear();
    }
}
