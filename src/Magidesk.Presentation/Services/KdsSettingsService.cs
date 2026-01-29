using System;
using Windows.Storage;

namespace Magidesk.Presentation.Services;

public class KdsSettingsService : IKdsSettingsService
{
    private const string StationIdKey = "KdsSelectedStationId";
    private const string ApiBaseUrlKey = "KdsApiBaseUrl";
    private readonly Microsoft.Extensions.Configuration.IConfiguration? _configuration;

    // Optional injection in case used where config unavailable, though unlikely
    public KdsSettingsService(System.IServiceProvider serviceProvider)
    {
        // Resolve manually or allow IConfiguration via ctor if we registered it.
        // App.xaml.cs registers services using Host, which should provide IConfiguration.
        // But KdsSettingsService was registered as:
        // services.AddSingleton<IKdsSettingsService, KdsSettingsService>();
        // I'll assume constructor injection works.
        _configuration = serviceProvider.GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration)) as Microsoft.Extensions.Configuration.IConfiguration;
    }

    public Guid? GetSelectedStationId()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (localSettings.Values.TryGetValue(StationIdKey, out var value) && value is string guidString)
        {
            if (Guid.TryParse(guidString, out var stationId))
            {
                return stationId;
            }
        }
        return null;
    }

    public void SetSelectedStationId(Guid? stationId)
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (stationId.HasValue)
        {
            localSettings.Values[StationIdKey] = stationId.Value.ToString();
        }
        else
        {
            localSettings.Values.Remove(StationIdKey);
        }
    }

    public string GetApiBaseUrl()
    {
        // 1. Check Local Settings (Override)
        var localSettings = ApplicationData.Current.LocalSettings;
        if (localSettings.Values.TryGetValue(ApiBaseUrlKey, out var value) && value is string url && !string.IsNullOrWhiteSpace(url))
        {
            return url;
        }

        // 2. Check Configuration
        if (_configuration != null)
        {
            var configUrl = _configuration["Kds:ApiBaseUrl"]; 
            if (!string.IsNullOrWhiteSpace(configUrl)) return configUrl;
        }

        // 3. Fallback / Error
        // Strict adherence to "No localhost literals in code".
        // Configuration MUST be present in appsettings or LocalSettings.
        throw new InvalidOperationException("KDS API Base URL is not configured. please ensure 'Kds:ApiBaseUrl' is set in appsettings.json or LocalSettings.");
    }

    public void SetApiBaseUrl(string url)
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (!string.IsNullOrWhiteSpace(url))
        {
            localSettings.Values[ApiBaseUrlKey] = url;
        }
        else
        {
            localSettings.Values.Remove(ApiBaseUrlKey);
        }
    }
}
