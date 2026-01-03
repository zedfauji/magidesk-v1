using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Presentation.Services;
using Windows.Storage.Pickers;
using Windows.Storage;
using MediatR;

namespace Magidesk.Presentation.ViewModels;

public partial class ExportImportManagementViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ITableLayoutRepository _tableLayoutRepository;
    private readonly TableLayoutExporter _exporter;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<TableLayoutDto> _layouts = new();

    [ObservableProperty]
    private TableLayoutDto? _selectedLayout;

    [ObservableProperty]
    private string _exportPath = string.Empty;

    [ObservableProperty]
    private string _importPath = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private LayoutValidationResult? _validationResult;

    [ObservableProperty]
    private bool _replaceExistingOnImport = false;

    public IRelayCommand ExportSingleLayoutCommand { get; }
    public IRelayCommand ExportAllLayoutsCommand { get; }
    public IRelayCommand ImportLayoutCommand { get; }
    public IRelayCommand ImportLayoutsCommand { get; }
    public IRelayCommand CreateBackupCommand { get; }
    public IRelayCommand RestoreBackupCommand { get; }
    public IRelayCommand ValidateFileCommand { get; }
    public IRelayCommand BrowseExportPathCommand { get; }
    public IRelayCommand BrowseImportPathCommand { get; }

    public ExportImportManagementViewModel(
        IMediator mediator,
        ITableLayoutRepository tableLayoutRepository,
        TableLayoutExporter exporter,
        NavigationService navigationService)
    {
        _mediator = mediator;
        _tableLayoutRepository = tableLayoutRepository;
        _exporter = exporter;
        _navigationService = navigationService;

        ExportSingleLayoutCommand = new AsyncRelayCommand(ExportSingleLayoutAsync);
        ExportAllLayoutsCommand = new AsyncRelayCommand(ExportAllLayoutsAsync);
        ImportLayoutCommand = new AsyncRelayCommand(ImportLayoutAsync);
        ImportLayoutsCommand = new AsyncRelayCommand(ImportLayoutsAsync);
        CreateBackupCommand = new AsyncRelayCommand(CreateBackupAsync);
        RestoreBackupCommand = new AsyncRelayCommand(RestoreBackupAsync);
        ValidateFileCommand = new AsyncRelayCommand(ValidateFileAsync);
        BrowseExportPathCommand = new AsyncRelayCommand(BrowseExportPathAsync);
        BrowseImportPathCommand = new AsyncRelayCommand(BrowseImportPathAsync);

        Title = "Export/Import Management";
        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            await LoadLayoutsAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadLayoutsAsync()
    {
        var layouts = await _tableLayoutRepository.GetAllAsync();
        Layouts.Clear();
        
        foreach (var layout in layouts)
        {
            Layouts.Add(new TableLayoutDto
            {
                Id = layout.Id,
                Name = layout.Name,
                FloorId = layout.FloorId,
                Tables = layout.Tables.Select(t => new TableDto
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    Capacity = t.Capacity,
                    X = t.X,
                    Y = t.Y,
                    Status = t.Status,
                    IsActive = t.IsActive
                }).ToList(),
                CreatedAt = layout.CreatedAt,
                UpdatedAt = layout.UpdatedAt,
                IsActive = layout.IsActive,
                Version = layout.Version
            });
        }
    }

    private async Task ExportSingleLayoutAsync()
    {
        if (SelectedLayout == null)
        {
            await ShowErrorAsync("Please select a layout to export.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ExportPath))
        {
            await ShowErrorAsync("Please specify an export path.");
            return;
        }

        IsBusy = true;
        try
        {
            var command = new ExportLayoutCommand(SelectedLayout.Id, ExportPath);
            var result = await _mediator.Send(command);
            
            await ShowSuccessAsync($"Layout '{SelectedLayout.Name}' exported successfully to {result}");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error exporting layout: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExportAllLayoutsAsync()
    {
        if (!Layouts.Any())
        {
            await ShowErrorAsync("No layouts available to export.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ExportPath))
        {
            await ShowErrorAsync("Please specify an export path.");
            return;
        }

        IsBusy = true;
        try
        {
            var command = new ExportAllLayoutsCommand(ExportPath);
            var result = await _mediator.Send(command);
            
            await ShowSuccessAsync($"All {Layouts.Count} layouts exported successfully to {result}");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error exporting layouts: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ImportLayoutAsync()
    {
        if (string.IsNullOrWhiteSpace(ImportPath))
        {
            await ShowErrorAsync("Please specify an import path.");
            return;
        }

        // Validate file first
        await ValidateFileAsync();

        if (ValidationResult == null || !ValidationResult.IsValid)
        {
            await ShowErrorAsync("File validation failed. Please check the file format.");
            return;
        }

        IsBusy = true;
        try
        {
            var command = new ImportLayoutCommand(ImportPath);
            var importedLayout = await _mediator.Send(command);
            
            // Add to layouts collection
            Layouts.Add(importedLayout);
            
            await ShowSuccessAsync($"Layout '{importedLayout.Name}' imported successfully.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error importing layout: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ImportLayoutsAsync()
    {
        if (string.IsNullOrWhiteSpace(ImportPath))
        {
            await ShowErrorAsync("Please specify an import path.");
            return;
        }

        // Validate file first
        await ValidateFileAsync();

        if (ValidationResult == null || !ValidationResult.IsValid)
        {
            await ShowErrorAsync("File validation failed. Please check the file format.");
            return;
        }

        IsBusy = true;
        try
        {
            var command = new ImportLayoutsCommand(ImportPath, ReplaceExistingOnImport);
            var importedLayouts = await _mediator.Send(command);
            
            if (ReplaceExistingOnImport)
            {
                Layouts.Clear();
            }
            
            foreach (var layout in importedLayouts)
            {
                Layouts.Add(layout);
            }
            
            await ShowSuccessAsync($"Successfully imported {importedLayouts.Count} layouts.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error importing layouts: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CreateBackupAsync()
    {
        if (!Layouts.Any())
        {
            await ShowErrorAsync("No layouts available to backup.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ExportPath))
        {
            await ShowErrorAsync("Please specify a backup path.");
            return;
        }

        IsBusy = true;
        try
        {
            var backupData = _exporter.CreateBackup(Layouts);
            await File.WriteAllTextAsync(ExportPath, backupData);
            
            await ShowSuccessAsync($"Backup created successfully with {Layouts.Count} layouts.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error creating backup: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RestoreBackupAsync()
    {
        if (string.IsNullOrWhiteSpace(ImportPath))
        {
            await ShowErrorAsync("Please specify a backup path.");
            return;
        }

        IsBusy = true;
        try
        {
            var backupData = await File.ReadAllTextAsync(ImportPath);
            var restoredLayouts = _exporter.RestoreFromBackup(backupData);
            
            if (ReplaceExistingOnImport)
            {
                Layouts.Clear();
            }
            
            foreach (var layout in restoredLayouts)
            {
                Layouts.Add(layout);
            }
            
            await ShowSuccessAsync($"Successfully restored {restoredLayouts.Count} layouts from backup.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error restoring backup: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ValidateFileAsync()
    {
        if (string.IsNullOrWhiteSpace(ImportPath))
        {
            ValidationResult = new LayoutValidationResult
            {
                IsValid = false,
                ErrorMessage = "No file path specified."
            };
            return;
        }

        IsBusy = true;
        try
        {
            var command = new ValidateLayoutFileCommand(ImportPath);
            ValidationResult = await _mediator.Send(command);
            
            StatusMessage = ValidationResult.IsValid 
                ? "File is valid for import." 
                : $"File validation failed: {ValidationResult.ErrorMessage}";
        }
        catch (Exception ex)
        {
            ValidationResult = new LayoutValidationResult
            {
                IsValid = false,
                ErrorMessage = ex.Message
            };
            StatusMessage = $"Validation error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task BrowseExportPathAsync()
    {
        var filePicker = new FileSavePicker();
        filePicker.FileTypeChoices.Add("JSON", new List<string> { ".json" });
        filePicker.SuggestedFileName = $"layout_export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        
        var file = await filePicker.PickSaveFileAsync();
        if (file != null)
        {
            ExportPath = file.Path;
        }
    }

    private async Task BrowseImportPathAsync()
    {
        var filePicker = new FileOpenPicker();
        filePicker.FileTypeFilter.Add(".json");
        
        var file = await filePicker.PickSingleFileAsync();
        if (file != null)
        {
            ImportPath = file.Path;
            await ValidateFileAsync();
        }
    }

    partial void OnSelectedLayoutChanged(TableLayoutDto? value)
    {
        // Update export path when layout changes
        if (value != null && string.IsNullOrWhiteSpace(ExportPath))
        {
            ExportPath = $"{value.Name}_export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        }
    }

    private async Task ShowErrorAsync(string message)
    {
        StatusMessage = message;
        // In a real implementation, this would show an error dialog
        System.Diagnostics.Debug.WriteLine($"Error: {message}");
    }

    private async Task ShowSuccessAsync(string message)
    {
        StatusMessage = message;
        // In a real implementation, this would show a success notification
        System.Diagnostics.Debug.WriteLine($"Success: {message}");
    }
}
