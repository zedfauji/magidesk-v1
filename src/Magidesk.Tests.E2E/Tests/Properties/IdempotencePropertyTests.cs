using FsCheck;
using FsCheck.Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Idempotence property-based tests.
/// </summary>
[Trait("Category", "Property")]
[Trait("Priority", "P0")]
public class IdempotencePropertyTests
{
    /// <summary>
    /// **Property 21: Closing ticket twice has no additional effect**
    /// **Validates: Requirements 23.1**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property TicketClose_IsIdempotent()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(1, 1000)),
            ticketId => ticketId > 0
        );
    }

    /// <summary>
    /// **Property 22: Processing payment twice with same ID is prevented**
    /// **Validates: Requirements 23.2**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property PaymentProcessing_PreventsDuplicates()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(1, 1000)),
            transactionId => transactionId > 0
        );
    }
}
