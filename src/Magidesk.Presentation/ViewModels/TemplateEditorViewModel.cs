using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation.ViewModels;

public class TemplateEditorViewModel : ViewModelBase
{
    private readonly IPrintTemplateRepository _repository;
    private readonly IRestaurantConfigurationRepository _configRepo;
    private readonly ITemplateEngine _engine;
    private readonly NavigationService _navigationService;

    private Guid? _templateId;
    private string _name = string.Empty;
    private string _content = string.Empty;
    private TemplateType _type = TemplateType.Receipt;
    private bool _isSystem;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    public TemplateType Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    public bool IsSystem
    {
        get => _isSystem;
        set => SetProperty(ref _isSystem, value);
    }

    public System.Collections.Generic.List<TemplateType> Types { get; } = Enum.GetValues<TemplateType>().ToList();

    public ICommand RequestSaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand RefreshPreviewCommand { get; }

    public TemplateEditorViewModel(
        IPrintTemplateRepository repository,
        IRestaurantConfigurationRepository configRepo, 
        ITemplateEngine engine,
        NavigationService navigationService)
    {
        _repository = repository;
        _configRepo = configRepo;
        _engine = engine;
        _navigationService = navigationService;

        Title = "Edit Template";

        RequestSaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(Cancel);
        RefreshPreviewCommand = new AsyncRelayCommand(UpdatePreviewAsync);
    }

    public async Task InitializeAsync(Guid? id)
    {
        try
        {
            _templateId = id;
            if (id.HasValue)
            {
                var template = await _repository.GetByIdAsync(id.Value);
                if (template != null)
                {
                    Name = template.Name;
                    Content = template.Content;
                    Type = template.Type;
                    IsSystem = template.IsSystem;
                    Title = $"Edit: {Name}";
                }
            }
            else
            {
                // Defaults
                Name = "New Template";
                Content = "{% comment %} Liquid Template {% endcomment %}";
                Type = TemplateType.Receipt;
                IsSystem = false;
                Title = "New Template";
            }
            
            // Trigger initial preview (safe)
            await UpdatePreviewAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Initialization Error: {ex.Message}";
            await _navigationService.ShowErrorAsync("Initialization Failed", ex.Message);
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            // 1. Validate Syntax
            if (!_engine.Validate(Content, out var error))
            {
                StatusMessage = $"Syntax Error: {error}";
                return;
            }

            if (_templateId.HasValue)
            {
                var template = await _repository.GetByIdAsync(_templateId.Value);
                if (template != null)
                {
                    if (template.IsSystem)
                    {
                        StatusMessage = "Cannot modify System Templates. Please Clone instead.";
                        return;
                    }
                    
                    template.UpdateName(Name);
                    template.UpdateContent(Content);
                    
                    await _repository.UpdateAsync(template);
                }
            }
            else
            {
                var newTemplate = PrintTemplate.Create(Name, Type, Content);
                await _repository.AddAsync(newTemplate);
            }

            _navigationService.GoBack();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save Failed: {ex.Message}";
        }
    }

    private void Cancel()
    {
        _navigationService.GoBack();
    }

    // TKT-P006: Preview Support
    private string _previewHtml = string.Empty;
    public string PreviewHtml
    {
        get => _previewHtml;
        set => SetProperty(ref _previewHtml, value);
    }

    // Hardcoded Mock Data for Preview (Base)
    private Magidesk.Application.DTOs.Printing.TicketPrintModel GetBaseMockModel()
    {
        return new Magidesk.Application.DTOs.Printing.TicketPrintModel
        {
            TicketNumber = "101",
            Date = DateTime.Now.ToString("MM/dd/yyyy"),
            Time = DateTime.Now.ToString("HH:mm"),
            ServerName = "John Doe",
            TableName = "Table 5",
            Subtotal = "$45.00",
            Tax = "$3.60",
            Total = "$48.60",
            BalanceDue = "$48.60",
            // Default fallback if DB is empty
            Restaurant = new Magidesk.Application.DTOs.Printing.RestaurantPrintModel 
            { 
                Name = "Magidesk Eats", 
                Address = "123 Tech Blvd, New York, NY", 
                Phone = "(212) 555-0199" 
            },
            Lines = new System.Collections.Generic.List<Magidesk.Application.DTOs.Printing.OrderLinePrintModel>
            {
                new() { Quantity = 1, Name = "Cheeseburger", Price = "12.00", Total = "12.00", Instructions = "No Onions", Modifiers = new() { "Extra Cheese (+$1.00)" } },
                new() { Quantity = 2, Name = "Coke", Price = "2.50", Total = "5.00" },
                new() { Quantity = 1, Name = "Fries (Large)", Price = "5.00", Total = "5.00" }
            },
            Payments = new System.Collections.Generic.List<Magidesk.Application.DTOs.Printing.PaymentPrintModel>
            {
                new() { Type = "Visa 1234", Amount = "$48.60" }
            }
        };
    }

    private async Task UpdatePreviewAsync()
    {
        try
        {
            // Prepare Model with Real Data
            var model = GetBaseMockModel();
            
            // Try fetch real config
            try 
            {
                var config = await _configRepo.GetConfigurationAsync();
                if (config != null)
                {
                    model.Restaurant.Name = config.Name;
                    model.Restaurant.Address = config.Address;
                    model.Restaurant.Phone = config.Phone;
                }
            }
            catch (Exception ex)
            {
                // Non-fatal, just use mock
                System.Diagnostics.Debug.WriteLine($"Preview config fetch failed: {ex.Message}");
            }
            
            var json = await _engine.RenderAsync(Content, model);
            
            // 2. Deserialize JSON to ADM (PrintDocument)
            // We need System.Text.Json here
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            // Simple robust check: if it starts with {, try parse.
            
            if (json.TrimStart().StartsWith("{"))
            {
                var doc = System.Text.Json.JsonSerializer.Deserialize<Magidesk.Infrastructure.Printing.Models.PrintDocument>(json, options);
                if (doc != null)
                {
                    // 3. Render ADM to HTML
                    var driver = new Magidesk.Infrastructure.Printing.Drivers.HtmlPreviewDriver();
                    PreviewHtml = driver.Render(doc);
                    return;
                }
            }
            
            // Fallback: If not JSON, just show raw text wrapped in pre
            PreviewHtml = $"<html><body><pre>{System.Net.WebUtility.HtmlEncode(json)}</pre></body></html>";

        }
        catch (Exception ex)
        {
            var msg = $"Preview Error: {ex.Message}";
            PreviewHtml = $"<html><body><h3 style='color:red'>Preview Error</h3><p>{ex.Message}</p></body></html>";
            await _navigationService.ShowErrorAsync("Preview Generation Failed", msg);
        }
    }
}
