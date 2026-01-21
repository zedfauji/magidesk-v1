using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

public class SetTaxExemptCommand
{
    public Guid TicketId { get; set; }
    public bool IsTaxExempt { get; set; }
    public UserId ModifiedBy { get; set; } = null!;
}

public class SetTaxExemptResult
{
    public bool Success { get; set; } = true;
    public string? Error { get; set; }
}
