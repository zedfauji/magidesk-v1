using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Interfaces;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Magidesk.Presentation.ViewModels;

public partial class LanguageSelectionViewModel : ObservableObject
{
    private readonly Services.LocalizationService _localizationService;
    
    [ObservableProperty]
    private LanguageOption _selectedLanguage;

    public ObservableCollection<LanguageOption> SupportedLanguages { get; } = new();

    public LanguageSelectionViewModel(Services.LocalizationService localizationService)
    {
        _localizationService = localizationService;
        LoadLanguages();
        
        // Select current language from service
        var currentCode = _localizationService.CurrentLanguageCode;
        SelectedLanguage = SupportedLanguages.FirstOrDefault(l => l.Code == currentCode) 
                           ?? SupportedLanguages.First();
    }

    private void LoadLanguages()
    {
        SupportedLanguages.Add(new LanguageOption("English", "en-US", "🇺🇸"));
        SupportedLanguages.Add(new LanguageOption("Español", "es-ES", "🇪🇸"));
        SupportedLanguages.Add(new LanguageOption("Français", "fr-FR", "🇫🇷"));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedLanguage != null)
        {
            await _localizationService.SetLanguageAsync(SelectedLanguage.Code);
        }
    }
}

public class LanguageOption
{
    public string Name { get; }
    public string Code { get; }
    public string Flag { get; }

    public LanguageOption(string name, string code, string flag)
    {
        Name = name;
        Code = code;
        Flag = flag;
    }
}
