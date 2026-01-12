using CommunityToolkit.Mvvm.ComponentModel;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using System;

namespace Magidesk.Presentation.ViewModels;

public class ItemPriceViewModel : ObservableObject
{
    public Guid PriceLevelId { get; }
    public string LevelName { get; }
    
    private string _amount = "0.00";
    public string Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    public ItemPriceViewModel(PriceLevel level, Money? existingPrice)
    {
        PriceLevelId = level.Id;
        LevelName = level.Name;
        if (existingPrice != null)
        {
            Amount = existingPrice.Amount.ToString("F2");
        }
    }
}
