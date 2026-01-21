using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public partial class TableSelectionViewModel : ObservableObject
{
    private readonly ITableRepository _tableRepository;

    private ObservableCollection<Table> _tables = new();
    public ObservableCollection<Table> Tables
    {
        get => _tables;
        set => SetProperty(ref _tables, value);
    }

    private Table? _selectedTable;
    public Table? SelectedTable
    {
        get => _selectedTable;
        set => SetProperty(ref _selectedTable, value);
    }

    public Action? CloseAction { get; set; }
    public bool IsConfirmed { get; private set; }

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public TableSelectionViewModel(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
        ConfirmCommand = new RelayCommand(Confirm);
        CancelCommand = new RelayCommand(Cancel);
    }

    public async Task InitializeAsync()
    {
        var activeTables = await _tableRepository.GetActiveAsync();
        Tables = new ObservableCollection<Table>(activeTables.OrderBy(t => t.TableNumber));
    }

    private void Confirm()
    {
        if (SelectedTable != null)
        {
            IsConfirmed = true;
            CloseAction?.Invoke();
        }
    }

    private void Cancel()
    {
        IsConfirmed = false;
        CloseAction?.Invoke();
    }
}
