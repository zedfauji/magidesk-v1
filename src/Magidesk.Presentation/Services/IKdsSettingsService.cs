using System;

namespace Magidesk.Presentation.Services;

public interface IKdsSettingsService
{
    Guid? GetSelectedStationId();
    void SetSelectedStationId(Guid? stationId);
    string GetApiBaseUrl();
    void SetApiBaseUrl(string url);
}
