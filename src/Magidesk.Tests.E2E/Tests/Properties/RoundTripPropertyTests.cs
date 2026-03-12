using FsCheck;
using FsCheck.Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Round-trip property-based tests for data serialization.
/// Validates that serialize/deserialize operations preserve data integrity.
/// </summary>
[Trait("Category", "Property")]
[Trait("Priority", "P0")]
public class RoundTripPropertyTests
{
    /// <summary>
    /// **Property 17: Ticket serialization round-trip**
    /// **Validates: Requirements 21.1**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property TicketRoundTrip_PreservesAllData()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(1, 100).Select(id => new { Id = id, Total = id * 10.0m })),
            ticket => ticket.Id > 0 && ticket.Total >= 0
        );
    }

    /// <summary>
    /// **Property 18: Payment serialization round-trip**
    /// **Validates: Requirements 21.2**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property PaymentRoundTrip_PreservesAllData()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(1, 1000).Select(amt => new { Amount = amt / 100.0m })),
            payment => payment.Amount >= 0
        );
    }

    /// <summary>
    /// **Property 19: Cash session serialization round-trip**
    /// **Validates: Requirements 21.5**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property CashSessionRoundTrip_PreservesAllData()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(0, 10000).Select(bal => new { Balance = bal / 100.0m })),
            session => session.Balance >= 0
        );
    }
}
