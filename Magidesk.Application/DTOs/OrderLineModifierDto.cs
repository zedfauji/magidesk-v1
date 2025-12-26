using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// Data transfer object for OrderLineModifier entity.
/// </summary>
public class OrderLineModifierDto
{
    public Guid Id { get; set; }
    public Guid OrderLineId { get; set; }
    public Guid? ModifierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ModifierType ModifierType { get; set; }
    public int ItemCount { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ShouldPrintToKitchen { get; set; }
    public DateTime CreatedAt { get; set; }
}

