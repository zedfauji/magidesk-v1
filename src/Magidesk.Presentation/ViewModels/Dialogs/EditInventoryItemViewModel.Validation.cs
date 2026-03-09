namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// Validation logic for EditInventoryItemViewModel.
/// </summary>
public partial class EditInventoryItemViewModel
{
    private void ValidateName()
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            _validationErrors[nameof(Name)] = "Name is required.";
        }
        else if (_name.Length > 200)
        {
            _validationErrors[nameof(Name)] = "Name must not exceed 200 characters.";
        }
        else
        {
            _validationErrors.Remove(nameof(Name));
        }
    }

    private void ValidateUnit()
    {
        if (string.IsNullOrWhiteSpace(_unit))
        {
            _validationErrors[nameof(Unit)] = "Unit is required.";
        }
        else if (_unit.Length > 50)
        {
            _validationErrors[nameof(Unit)] = "Unit must not exceed 50 characters.";
        }
        else
        {
            _validationErrors.Remove(nameof(Unit));
        }
    }

    private void ValidateSkuCode()
    {
        if (!string.IsNullOrWhiteSpace(_skuCode) && _skuCode.Length > 50)
        {
            _validationErrors[nameof(SkuCode)] = "SKU code must not exceed 50 characters.";
        }
        else
        {
            _validationErrors.Remove(nameof(SkuCode));
        }
    }

    private void ValidateStockQuantity()
    {
        if (_stockQuantity < 0)
        {
            _validationErrors[nameof(StockQuantity)] = "Stock quantity must be non-negative.";
        }
        else
        {
            _validationErrors.Remove(nameof(StockQuantity));
        }
    }

    private void ValidateReorderPoint()
    {
        if (_reorderPoint < 0)
        {
            _validationErrors[nameof(ReorderPoint)] = "Reorder point must be non-negative.";
        }
        else
        {
            _validationErrors.Remove(nameof(ReorderPoint));
        }
    }
}
