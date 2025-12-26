using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

public class FractionalModifier : MenuModifier
{
    public ModifierPortion Portion { get; private set; }
    public PriceStrategy PriceStrategy { get; private set; }

    protected FractionalModifier() : base() { }

    public FractionalModifier(
        string name, 
        Money price, 
        int sortOrder, 
        ModifierPortion portion, 
        PriceStrategy priceStrategy)
        : base(name, price, sortOrder)
    {
        Portion = portion;
        PriceStrategy = priceStrategy;
    }
}
