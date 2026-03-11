using FsCheck;

namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Data model for ticket generation in property-based tests.
/// </summary>
public class TicketData
{
    public List<MenuItemData> Items { get; set; } = new();
    
    public decimal Total => Items.Sum(item => item.Price * item.Quantity);
}

/// <summary>
/// Data model for menu item generation in property-based tests.
/// </summary>
public class MenuItemData
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    public decimal LineTotal => Price * Quantity;
}

/// <summary>
/// Data model for payment generation in property-based tests.
/// </summary>
public class PaymentData
{
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

/// <summary>
/// Data model for customer profile generation in property-based tests.
/// </summary>
public class CustomerData
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Data model for cash session generation in property-based tests.
/// </summary>
public class CashSessionData
{
    public decimal StartingBalance { get; set; }
    public List<decimal> Receipts { get; set; } = new();
    public List<decimal> Disbursements { get; set; } = new();
    
    public decimal EndingBalance => StartingBalance + Receipts.Sum() - Disbursements.Sum();
}

/// <summary>
/// FsCheck generators for property-based testing of E2E scenarios.
/// Provides random data generation for tickets, menu items, payments, customers, and cash sessions.
/// </summary>
public static class TestDataGenerators
{
    /// <summary>
    /// Generate valid ticket with random items (1-20 items).
    /// </summary>
    public static Arbitrary<TicketData> TicketGenerator() =>
        Arb.From(
            from itemCount in Gen.Choose(1, 20)
            from items in Gen.ListOf(itemCount, MenuItemGenerator().Generator)
            select new TicketData { Items = items.ToList() }
        );
    
    /// <summary>
    /// Generate valid menu item with random name, price, and quantity.
    /// Prices range from $1.00 to $50.00, quantities from 1 to 10.
    /// </summary>
    public static Arbitrary<MenuItemData> MenuItemGenerator() =>
        Arb.From(
            from name in Gen.Elements("Coffee", "Tea", "Burger", "Fries", "Soda", "Pizza", "Salad", "Beer", "Wine", "Dessert")
            from price in Gen.Choose(100, 5000).Select(p => p / 100m)
            from quantity in Gen.Choose(1, 10)
            select new MenuItemData { Name = name, Price = price, Quantity = quantity }
        );
    
    /// <summary>
    /// Generate valid payment data with random method and amount.
    /// Amounts range from $1.00 to $100.00.
    /// </summary>
    public static Arbitrary<PaymentData> PaymentGenerator() =>
        Arb.From(
            from method in Gen.Elements("Cash", "Credit", "Debit", "GiftCertificate")
            from amount in Gen.Choose(100, 10000).Select(a => a / 100m)
            select new PaymentData { Method = method, Amount = amount }
        );
    
    /// <summary>
    /// Generate valid customer profile with random name, phone, and email.
    /// </summary>
    public static Arbitrary<CustomerData> CustomerGenerator() =>
        Arb.From(
            from name in Gen.Elements("John Doe", "Jane Smith", "Bob Johnson", "Alice Williams", "Charlie Brown", "Diana Prince", "Eve Davis", "Frank Miller")
            from phone in Gen.Choose(2000000000, 2099999999).Select(p => p.ToString())
            from email in Gen.Elements("test@example.com", "user@test.com", "customer@mail.com", "info@domain.com")
            select new CustomerData { Name = name, Phone = phone, Email = email }
        );
    
    /// <summary>
    /// Generate valid cash session data with random starting balance, receipts, and disbursements.
    /// Starting balance ranges from $100 to $1000.
    /// Receipts and disbursements are lists of 0-20 transactions each, ranging from $1 to $100.
    /// </summary>
    public static Arbitrary<CashSessionData> CashSessionGenerator() =>
        Arb.From(
            from startingBalance in Gen.Choose(10000, 100000).Select(b => b / 100m)
            from receiptCount in Gen.Choose(0, 20)
            from receipts in Gen.ListOf(receiptCount, Gen.Choose(100, 10000).Select(r => r / 100m))
            from disbursementCount in Gen.Choose(0, 20)
            from disbursements in Gen.ListOf(disbursementCount, Gen.Choose(100, 10000).Select(d => d / 100m))
            select new CashSessionData 
            { 
                StartingBalance = startingBalance,
                Receipts = receipts.ToList(),
                Disbursements = disbursements.ToList()
            }
        );
}
