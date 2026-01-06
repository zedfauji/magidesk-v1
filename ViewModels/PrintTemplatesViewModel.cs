using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.Views;

namespace Magidesk.Presentation.ViewModels;

public class PrintTemplatesViewModel : ViewModelBase
{
    private readonly IPrintTemplateRepository _repository;
    private readonly NavigationService _navigationService;

    public ObservableCollection<PrintTemplate> Templates { get; } = new();

    private PrintTemplate? _selectedTemplate;
    public PrintTemplate? SelectedTemplate
    {
        get => _selectedTemplate;
        set => SetProperty(ref _selectedTemplate, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }

    public PrintTemplatesViewModel(
        IPrintTemplateRepository repository,
        NavigationService navigationService)
    {
        _repository = repository;
        _navigationService = navigationService;
        
        Title = "Print Templates";

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        AddCommand = new AsyncRelayCommand(AddAsync);
        EditCommand = new AsyncRelayCommand<PrintTemplate>(EditAsync);
    }

    public async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            Templates.Clear();
            var items = await _repository.GetAllAsync();
            foreach (var item in items)
            {
                Templates.Add(item);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading templates: {ex.Message}";
            await _navigationService.ShowErrorAsync("Loading Failed", $"Could not load templates.\n{ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddAsync()
    {
        // Navigate to editor with new request
        _navigationService.Navigate(typeof(TemplateEditorPage)); 
    }

    private async Task EditAsync(PrintTemplate? template)
    {
        if (template == null) return;
        
        // Pass ID to editor
        _navigationService.Navigate(typeof(TemplateEditorPage), template.Id);
    }
}
