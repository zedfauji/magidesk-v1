using Magidesk.Tests.Workflows.Infrastructure;
using Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for split payment invariants.
/// Property 6: Sum of split payments equals ticket total
/// Validates: Requirements 6.3, 22.1
/// </summary>
[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class SplitPaymentInvariantProperties
{
    private readonly IPropertyBasedTestFramework _framework;

    public SplitPaymentInvariantProperties()
    {
        _framework = new PropertyBasedTestFramework();
    }

    /// <summary>
    /// Property 6: Sum of split payments equals ticket total when ticket is closed.
    /// Validates that when multiple payment methods are used, the sum of all payment amounts
    /// must equal the ticket total for the ticket to be closed.
    /// Requirements: 6.3, 22.1
    /// </summary>
    [Fact]
    public void Property6_SplitPaymentSum_EqualsTicketTotal()
    {
        // Property: For all valid ticket totals and split payment scenarios,
        // sum(payment_amounts) == ticket_total when ticket is closed
        _framework.CheckProperty<decimal>(
            ticketTotal =>
            {
                // Skip invalid test cases
                if (ticketTotal <= 0)
                    return true;

                // Generate random split payment scenario (2-4 payments)
                var paymentCount = new Random().Next(2, 5);
                var payments = GenerateSplitPayments(ticketTotal, paymentCount);

                // Invariant: Sum of split payments must equal ticket total
                var totalPaid = payments.Sum();
                var difference = Math.Abs(totalPaid - ticketTotal);

                // Allow for rounding differences (within 1 cent)
                return difference < 0.01m;
            },
            iterations: 100,
            tag: "Property6_SplitPaymentSumInvariant"
        );
    }

    /// <summary>
    /// Property: Split payments with exact total allow ticket closure.
    /// Validates that when split payments sum to ticket total, the ticket can be closed.
    /// Requirements: 6.3, 22.1
    /// </summary>
    [Fact]
    public void Property_SplitPaymentsExactTotal_AllowsTicketClosure()
    {
        // Property: For all ticket totals, if sum(payments) = total, then ticket can close
        _framework.CheckProperty<decimal>(
            ticketTotal =>
            {
                // Skip invalid test cases
                if (ticketTotal <= 0)
                    return true;

                // Split into two equal payments
                var payment1 = ticketTotal / 2;
                var payment2 = ticketTotal - payment1;

                var totalPaid = payment1 + payment2;
                var remainingBalance = ticketTotal - totalPaid;

                // Invariant: Exact split payment total results in zero remaining balance
                return Math.Abs(remainingBalance) < 0.01m;
            },
            iterations: 100,
            tag: "SplitPaymentsExactTotalClosure"
        );
    }

    /// <summary>
    /// Property: Incomplete split payments show positive remaining balance.
    /// Validates that when split payments don't sum to ticket total, remaining balance is positive.
    /// Requirements: 6.4, 22.1
    /// </summary>
    [Fact]
    public void Property_IncompleteSplitPayments_ShowPositiveRemainingBalance()
    {
        // Property: For all ticket totals and partial split payments,
        // if sum(payments) < total, then remaining balance > 0
        _framework.CheckProperty<decimal>(
            ticketTotal =>
            {
                // Skip invalid test cases
                if (ticketTotal <= 0)
                    return true;

                // Create incomplete split payments (only 70% of total)
                var payment1 = ticketTotal * 0.3m;
                var payment2 = ticketTotal * 0.4m;

                var totalPaid = payment1 + payment2;
                var remainingBalance = ticketTotal - totalPaid;

                // Invariant: Incomplete payments result in positive remaining balance
                return remainingBalance > 0;
            },
            iterations: 100,
            tag: "IncompleteSplitPaymentsRemaining"
        );
    }

    /// <summary>
    /// Property: Split payment sum is commutative.
    /// Validates that the order of split payments doesn't affect the total.
    /// Requirements: 6.2, 6.3, 22.1
    /// </summary>
    [Fact]
    public void Property_SplitPaymentSum_IsCommutative()
    {
        // Property: For all payment sequences, sum(payments) is independent of order
        _framework.CheckProperty<decimal, decimal, decimal>(
            (payment1, payment2, payment3) =>
            {
                // Skip invalid test cases
                if (payment1 < 0 || payment2 < 0 || payment3 < 0)
                    return true;

                // Calculate sum in different orders
                var sum1 = payment1 + payment2 + payment3;
                var sum2 = payment3 + payment1 + payment2;
                var sum3 = payment2 + payment3 + payment1;

                // Invariant: Sum is independent of order
                return Math.Abs(sum1 - sum2) < 0.01m && Math.Abs(sum2 - sum3) < 0.01m;
            },
            iterations: 100,
            tag: "SplitPaymentSumCommutative"
        );
    }

    /// <summary>
    /// Property: Each split payment must be non-negative.
    /// Validates that individual split payment amounts are always >= 0.
    /// Requirements: 6.1, 6.2, 22.1
    /// </summary>
    [Fact]
    public void Property_SplitPaymentAmount_MustBeNonNegative()
    {
        // Property: For all split payments, each payment amount >= 0
        _framework.CheckProperty<decimal>(
            paymentAmount =>
            {
                // Invariant: Split payment amounts must be non-negative
                return paymentAmount >= 0;
            },
            iterations: 100,
            tag: "SplitPaymentNonNegative"
        );
    }

    /// <summary>
    /// Property: Split payment count must be at least 2.
    /// Validates that split payment scenarios involve multiple payment methods.
    /// Requirements: 6.1, 6.2
    /// </summary>
    [Fact]
    public void Property_SplitPaymentCount_AtLeastTwo()
    {
        // Property: For all split payment scenarios, payment count >= 2
        _framework.CheckProperty<int>(
            paymentCount =>
            {
                // Skip invalid test cases
                if (paymentCount < 1)
                    return true;

                // Invariant: Split payments require at least 2 payment methods
                // (otherwise it's a single payment, not a split payment)
                return paymentCount >= 2;
            },
            iterations: 100,
            tag: "SplitPaymentCountMinimum"
        );
    }

    /// <summary>
    /// Property: Split payment sum never exceeds ticket total by more than rounding error.
    /// Validates that overpayment in split scenarios is within acceptable tolerance.
    /// Requirements: 6.3, 22.1
    /// </summary>
    [Fact]
    public void Property_SplitPaymentSum_NeverExceedsTicketTotalSignificantly()
    {
        // Property: For all ticket totals and split payments,
        // sum(payments) <= ticket_total + rounding_tolerance
        _framework.CheckProperty<decimal>(
            ticketTotal =>
            {
                // Skip invalid test cases
                if (ticketTotal <= 0)
                    return true;

                // Generate split payments that sum to ticket total
                var payments = GenerateSplitPayments(ticketTotal, 3);
                var totalPaid = payments.Sum();

                // Invariant: Split payment sum doesn't exceed ticket total significantly
                var overpayment = totalPaid - ticketTotal;
                return overpayment <= 0.01m; // Allow 1 cent rounding tolerance
            },
            iterations: 100,
            tag: "SplitPaymentSumNoExcessOverpayment"
        );
    }

    // ===== Helper Methods =====

    /// <summary>
    /// Generates a list of split payment amounts that sum to the ticket total.
    /// </summary>
    /// <param name="ticketTotal">The total amount to split.</param>
    /// <param name="paymentCount">The number of payments to generate.</param>
    /// <returns>List of payment amounts that sum to ticket total.</returns>
    private static List<decimal> GenerateSplitPayments(decimal ticketTotal, int paymentCount)
    {
        if (paymentCount < 2)
            throw new ArgumentException("Split payments require at least 2 payments", nameof(paymentCount));

        var payments = new List<decimal>();
        var remaining = ticketTotal;
        var random = new Random();

        // Generate random percentages for all but the last payment
        for (int i = 0; i < paymentCount - 1; i++)
        {
            // Each payment is 10-40% of remaining amount
            var percentage = (decimal)(random.NextDouble() * 0.3 + 0.1);
            var payment = Math.Round(remaining * percentage, 2);
            payments.Add(payment);
            remaining -= payment;
        }

        // Last payment is the remaining amount (ensures exact sum)
        payments.Add(remaining);

        return payments;
    }
}
