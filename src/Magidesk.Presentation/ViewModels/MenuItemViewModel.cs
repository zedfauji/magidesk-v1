using CommunityToolkit.Mvvm.ComponentModel;
using Magidesk.Domain.Entities;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;

namespace Magidesk.Presentation.ViewModels;

public partial class MenuItemViewModel : ObservableObject
{
    private readonly MenuItem _model;

    public MenuItemViewModel(MenuItem model)
    {
        _model = model;
    }

    public MenuItem Model => _model;

    public Guid Id => _model.Id;
    public string Name => _model.Name;
    public decimal Price => _model.Price.Amount;
    public string ColorCode => _model.ColorCode ?? "#e6e6e6"; // Default gray

    // Stock Properties (G.2)
    public bool TrackStock => _model.TrackStock;
    public int StockQuantity => _model.StockQuantity;
    public int MinimumStockLevel => _model.MinimumStockLevel;

    public bool IsLowStock => TrackStock && StockQuantity <= MinimumStockLevel && StockQuantity > 0;
    public bool IsOutOfStock => TrackStock && StockQuantity <= 0;

    public string StockStatusText 
    {
        get
        {
            if (!TrackStock) return string.Empty;
            if (IsOutOfStock) return "Out of Stock";
            if (IsLowStock) return $"Low: {StockQuantity}";
            return string.Empty;
        }
    }

    public bool IsAvailable => _model.IsAvailable;  // Assuming Model has IsAvailable (need to verify model, but usually yes)
    
    // UI Enabled State: Must be Available AND (Not Tracking Stock OR Stock > 0)
    public bool IsEnabled => IsAvailable && (!TrackStock || StockQuantity > 0);

    public double Opacity => IsEnabled ? 1.0 : 0.6;
    
    // Commands or other UI logic can go here
}
