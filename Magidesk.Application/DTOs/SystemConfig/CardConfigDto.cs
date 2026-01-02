using System;

namespace Magidesk.Application.DTOs.SystemConfig;

public class CardConfigDto
{
    public Guid Id { get; set; }
    public Guid TerminalId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty; // Raw API Key for UI, encrypted in domain/infra
    public string GatewayUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public string CardTypesAccepted { get; set; } = "VISA,MC,AMEX,DISC";
    public decimal SignatureThreshold { get; set; } = 25.00m;
    public bool AllowTipAdjustment { get; set; } = true;
    public bool IsExternalTerminal { get; set; } = false;
    public bool AllowManualEntry { get; set; } = true;
    public bool EnablePreAuth { get; set; } = false;
}
