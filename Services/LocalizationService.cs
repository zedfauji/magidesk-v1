using CommunityToolkit.Mvvm.ComponentModel;
using Magidesk.Application.Interfaces;
using Microsoft.UI.Xaml;
using System.Globalization;

namespace Magidesk.Presentation.Services;

public partial class LocalizationService : ObservableObject
{
    private readonly IUserService _userService;
    private readonly Microsoft.Windows.ApplicationModel.Resources.ResourceManager _resourceManager;
    private readonly Microsoft.Windows.ApplicationModel.Resources.ResourceContext _resourceContext;
    private readonly Microsoft.Windows.ApplicationModel.Resources.ResourceMap _resourceMap;

    [ObservableProperty]
    private string _currentLanguageCode = "en-US";

    public LocalizationService(IUserService userService)
    {
        _userService = userService;
        
        // Initialize MRT Core
        _resourceManager = new Microsoft.Windows.ApplicationModel.Resources.ResourceManager();
        _resourceContext = _resourceManager.CreateResourceContext();
        _resourceMap = _resourceManager.MainResourceMap.GetSubtree("Resources");

        LoadLanguagePreference();
        
        // Apply initial language
        if (!string.IsNullOrEmpty(CurrentLanguageCode))
        {
             _resourceContext.QualifierValues["Language"] = CurrentLanguageCode;
            
            // Also ensure culture is set immediately
            try
            {
                CultureInfo.CurrentUICulture = new CultureInfo(CurrentLanguageCode);
                CultureInfo.CurrentCulture = new CultureInfo(CurrentLanguageCode);
            }
            catch { }
        }
    }

    private void LoadLanguagePreference()
    {
        try
        {
            var path = GetSettingsPath();
            if (System.IO.File.Exists(path))
            {
                var lang = System.IO.File.ReadAllText(path).Trim();
                if (!string.IsNullOrWhiteSpace(lang))
                {
                    CurrentLanguageCode = lang;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading language preference: {ex.Message}");
        }
    }

    public string this[string key]
    {
        get
        {
            try 
            {
                // Dynamic lookup using MRT Core ResourceMap with context
                var candidate = _resourceMap.GetValue(key, _resourceContext);
                return candidate != null ? candidate.ValueAsString : $"[{key}]";
            }
            catch
            {
                return $"[{key}]";
            }
        }
    }

    public async Task SetLanguageAsync(string languageCode)
    {
        CurrentLanguageCode = languageCode;

        // 1. Switch language context map
        try
        {
             _resourceContext.QualifierValues["Language"] = languageCode;
        }
        catch (Exception ex)
        {
             System.Diagnostics.Debug.WriteLine($"Failed to set ResourceContext language: {ex.Message}");
        }
        
        // 2. Persist System Preference (File-based)
        try
        {
            var path = GetSettingsPath();
            var dir = System.IO.Path.GetDirectoryName(path);
            if (dir != null && !System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            System.IO.File.WriteAllText(path, languageCode);
        }
        catch (Exception ex)
        {
             System.Diagnostics.Debug.WriteLine($"Error saving language preference: {ex.Message}");
        }

        // 3. Persist User Preference (if logged in)
        if (_userService.CurrentUser != null)
        {
            await _userService.UpdatePreferredLanguageAsync(languageCode);
        }

        // 4. Notify UI to update bindings
        OnPropertyChanged("Item[]"); 
        
        // 5. Update Thread Culture
        try 
        {
             CultureInfo.CurrentUICulture = new CultureInfo(languageCode);
             CultureInfo.CurrentCulture = new CultureInfo(languageCode);
        }
        catch { /* Ignore invalid culture */ }
    }

    private string GetSettingsPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return System.IO.Path.Combine(appData, "Magidesk", "settings", "language.txt");
    }
}
