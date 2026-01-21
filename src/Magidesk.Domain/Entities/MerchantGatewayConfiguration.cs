using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents the configuration for a merchant payment gateway.
/// Stores sensitive credentials in an encrypted format.
/// </summary>
public class MerchantGatewayConfiguration
{
    public Guid Id { get; private set; }
    public Guid TerminalId { get; private set; }
    public string ProviderName { get; private set; } = null!;
    public string MerchantId { get; private set; } = null!;
    public string EncryptedApiKey { get; private set; } = null!; // AES Encrypted
    public string GatewayUrl { get; private set; } = null!;
    public bool IsActive { get; private set; }

    // Parity with FloreantPOS (F-0107)
    public string CardTypesAccepted { get; private set; } = "VISA,MC,AMEX,DISC";
    public decimal SignatureThreshold { get; private set; } = 25.00m;
    public bool AllowTipAdjustment { get; private set; } = true;
    public bool IsExternalTerminal { get; private set; } = false;
    public bool AllowManualEntry { get; private set; } = true;
    public bool EnablePreAuth { get; private set; } = false;

    // Concurrency
    public int Version { get; private set; }

    private MerchantGatewayConfiguration() { }

    public static MerchantGatewayConfiguration Create(
        Guid terminalId,
        string providerName,
        string merchantId,
        string encryptedApiKey,
        string gatewayUrl)
    {
        return new MerchantGatewayConfiguration
        {
            Id = Guid.NewGuid(),
            TerminalId = terminalId,
            ProviderName = providerName,
            MerchantId = merchantId,
            EncryptedApiKey = encryptedApiKey, // Service layer must handle encryption before passing here
            GatewayUrl = gatewayUrl,
            IsActive = true,
            Version = 1
        };
    }

    public void UpdateCredentials(string merchantId, string encryptedApiKey, string gatewayUrl)
    {
        MerchantId = merchantId;
        EncryptedApiKey = encryptedApiKey;
        GatewayUrl = gatewayUrl;
        Version++;
    }

    public void UpdateSettings(
        string cardTypesAccepted,
        decimal signatureThreshold,
        bool allowTipAdjustment,
        bool isExternalTerminal,
        bool allowManualEntry,
        bool enablePreAuth)
    {
        CardTypesAccepted = cardTypesAccepted;
        SignatureThreshold = signatureThreshold;
        AllowTipAdjustment = allowTipAdjustment;
        IsExternalTerminal = isExternalTerminal;
        AllowManualEntry = allowManualEntry;
        EnablePreAuth = enablePreAuth;
        Version++;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        Version++;
    }
}
