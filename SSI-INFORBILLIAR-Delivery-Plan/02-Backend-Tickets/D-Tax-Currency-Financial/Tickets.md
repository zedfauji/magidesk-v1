# Backend Tickets: Category D - Tax, Currency & Financial Rules

> [!NOTE]
> This category has 50% parity (3 full, 3 partial). Work focuses on completing partial implementations.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-D.2-01 | D.2 | Complete Multi-Tax Rate Support | P1 | NOT_STARTED |
| BE-D.4-01 | D.4 | Complete Tax Exemption Processing | P1 | NOT_STARTED |
| BE-D.5-01 | D.5 | Complete Receipt Tax Breakdown | P2 | NOT_STARTED |

---

## BE-D.2-01: Complete Multi-Tax Rate Support

**Ticket ID:** BE-D.2-01  
**Feature ID:** D.2  
**Type:** Backend  
**Title:** Complete Multi-Tax Rate Support  
**Priority:** P1

### Outcome (measurable, testable)
Support for multiple simultaneous tax rates (e.g., state + local + special).

### Scope
- Enhance TaxRate entity for stacking
- Calculate compound taxes correctly
- Apply different rates to different product categories
- Display breakdown on tickets

### Current State (Partial)
- Single tax rate works
- **Missing:** Multi-rate stacking, category-specific rates

### Implementation Notes
```csharp
public class TaxRate
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public decimal Percentage { get; private set; }
    public TaxType Type { get; private set; }  // State, Local, Special
    public bool IsCompound { get; private set; }  // Applied on top of other taxes
    public int ApplicationOrder { get; private set; }
    public ICollection<ProductCategory> ApplicableCategories { get; }
}

public interface ITaxCalculationService
{
    TaxBreakdown CalculateTaxes(Money subtotal, IEnumerable<OrderLine> lines);
}

public record TaxBreakdown(
    Money TotalTax,
    IEnumerable<TaxLineItem> TaxLines
);

public record TaxLineItem(
    string TaxName,
    decimal Rate,
    Money Amount
);
```

### Acceptance Criteria
- [ ] Multiple tax rates apply
- [ ] Compound taxes calculated correctly
- [ ] Category-specific rates work
- [ ] Breakdown available for display
- [ ] Tests verify calculations

---

## BE-D.4-01: Complete Tax Exemption Processing

**Ticket ID:** BE-D.4-01  
**Feature ID:** D.4  
**Type:** Backend  
**Title:** Complete Tax Exemption Processing  
**Priority:** P1

### Outcome (measurable, testable)
Proper tax exemption handling for eligible customers/members.

### Scope
- Add tax exemption flag to Customer/Member
- Store exemption certificate info
- Apply exemption during checkout
- Audit exemption usage

### Current State (Partial)
- Tax exemption concept exists
- **Missing:** Customer linkage, certificate storage, audit

### Implementation Notes
```csharp
// Add to Member or Customer
public bool IsTaxExempt { get; private set; }
public string TaxExemptCertificateNumber { get; private set; }
public DateTime? TaxExemptExpiration { get; private set; }

public record ApplyTaxExemptionCommand(
    Guid TicketId,
    Guid MemberId,
    string CertificateNumber
);

// Handler validates exemption and applies to ticket
```

### Acceptance Criteria
- [ ] Exemption stored on member
- [ ] Exemption applied to ticket
- [ ] Expired exemptions rejected
- [ ] Audit trail for exemptions
- [ ] Receipt shows exemption

---

## BE-D.5-01: Complete Receipt Tax Breakdown

**Ticket ID:** BE-D.5-01  
**Feature ID:** D.5  
**Type:** Backend  
**Title:** Complete Receipt Tax Breakdown  
**Priority:** P2

### Outcome (measurable, testable)
Detailed tax breakdown included in all receipt/report data.

### Scope
- Enhance Ticket totals to include tax breakdown
- Include in receipt generation
- Include in reports

### Current State (Partial)
- Single tax total exists
- **Missing:** Itemized breakdown

### Implementation Notes
```csharp
// Add to Ticket
public IReadOnlyCollection<TaxLineItem> TaxBreakdown { get; }

// Receipt DTO includes full breakdown
public record ReceiptDto(
    // ... other fields
    IEnumerable<TaxLineItem> TaxBreakdown
);
```

### Acceptance Criteria
- [ ] Tax breakdown stored on ticket
- [ ] Receipt shows itemized taxes
- [ ] Reports show tax breakdown
- [ ] Totals reconcile correctly

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P1 | 2 | NOT_STARTED |
| P2 | 1 | NOT_STARTED |
| **Total** | **3** | **NOT_STARTED** |

---

*Last Updated: 2026-01-08*
