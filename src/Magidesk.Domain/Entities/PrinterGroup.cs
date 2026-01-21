using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a virtual printer destination (e.g., "Kitchen", "Receipt").
/// Menu items are routed to Printer Groups rather than physical devices directly.
/// </summary>
public class PrinterGroup
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public PrinterType Type { get; private set; }

    public CutBehavior CutBehavior { get; private set; }
    public bool ShowPrices { get; private set; }
    public int RetryCount { get; private set; }
    public int RetryDelayMs { get; private set; }
    public bool AllowReprint { get; private set; }
    public Guid? FallbackPrinterGroupId { get; private set; }

    // Template Links
    public Guid? ReceiptTemplateId { get; private set; }
    public virtual PrintTemplate? ReceiptTemplate { get; private set; }

    public Guid? KitchenTemplateId { get; private set; }
    public virtual PrintTemplate? KitchenTemplate { get; private set; }

    // Private constructor for EF Core
    private PrinterGroup()
    {
    }

    public static PrinterGroup Create(string name, PrinterType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Printer group name is required.");

        return new PrinterGroup
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            // Defaults
            CutBehavior = CutBehavior.Auto,
            ShowPrices = true,
            RetryCount = 0,
            RetryDelayMs = 0,
            AllowReprint = true
        };
    }

    public void Update(string name, PrinterType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Printer group name is required.");

        Name = name;
        Type = type;
    }

    public void UpdateBehavior(
        CutBehavior cutBehavior,
        bool showPrices,
        int retryCount,
        int retryDelayMs,
        bool allowReprint,
        Guid? fallbackPrinterGroupId)
    {
        CutBehavior = cutBehavior;
        ShowPrices = showPrices;
        RetryCount = retryCount;
        RetryDelayMs = retryDelayMs;
        AllowReprint = allowReprint;
        FallbackPrinterGroupId = fallbackPrinterGroupId;
    }
    public void SetTemplates(Guid? receiptTemplateId, Guid? kitchenTemplateId)
    {
        ReceiptTemplateId = receiptTemplateId;
        KitchenTemplateId = kitchenTemplateId;
    }
}
