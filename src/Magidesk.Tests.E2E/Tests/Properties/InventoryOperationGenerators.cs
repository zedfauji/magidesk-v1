using FsCheck;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// FsCheck generators for inventory operation property testing.
/// </summary>
public static class InventoryOperationGenerators
{
    /// <summary>
    /// Generates sequences of inventory operations for property testing.
    /// </summary>
    public static Arbitrary<List<InventoryOperation>> GenerateInventoryOperations()
    {
        var itemNames = new[] { "Coffee", "Tea", "Burger", "Fries" };
        
        var saleGen = from itemName in Gen.Elements(itemNames)
                     select new InventoryOperation 
                     { 
                         Type = InventoryOperationType.Sale, 
                         ItemName = itemName,
                         Quantity = 1
                     };

        var adjustmentGen = from itemName in Gen.Elements(itemNames)
                           from quantity in Gen.Choose(-10, 10)
                           select new InventoryOperation 
                           { 
                               Type = InventoryOperationType.Adjustment, 
                               ItemName = itemName,
                               Quantity = quantity
                           };

        var receiptGen = from itemName in Gen.Elements(itemNames)
                        from quantity in Gen.Choose(10, 50)
                        select new InventoryOperation 
                        { 
                            Type = InventoryOperationType.Receipt, 
                            ItemName = itemName,
                            Quantity = quantity
                        };

        var operationGen = Gen.OneOf(saleGen, adjustmentGen, receiptGen);
        
        // Generate 1-5 operations per test
        var operationsGen = from count in Gen.Choose(1, 5)
                           from operations in Gen.ListOf(count, operationGen)
                           select operations;

        return Arb.From(operationsGen);
    }
}

/// <summary>
/// Represents an inventory operation for property testing.
/// </summary>
public class InventoryOperation
{
    public InventoryOperationType Type { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

/// <summary>
/// Types of inventory operations.
/// </summary>
public enum InventoryOperationType
{
    Sale,
    Adjustment,
    Receipt
}
