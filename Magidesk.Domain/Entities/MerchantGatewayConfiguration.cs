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

    public void UpdateCredentials(string merchantId, string encryptedApiKey)
    {
        MerchantId = merchantId;
        EncryptedApiKey = encryptedApiKey;
        Version++;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        Version++;
    }
}
