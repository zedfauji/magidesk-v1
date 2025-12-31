namespace Magidesk.Application.DTOs.SystemConfig;

public class RestaurantConfigurationDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string ReceiptFooterMessage { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public bool IsKioskMode { get; set; }
}
