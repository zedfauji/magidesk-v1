using System;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views;

public sealed partial class CashEntryDialog : ContentDialog
{
    public decimal Amount 
    {
        get 
        {
            if (decimal.TryParse(AmountBox.Text, out var val)) return val;
            return 0;
        }
    }
    public string Reason => ReasonBox.Text;

    public CashEntryDialog(string title, string message)
    {
        this.InitializeComponent();
        this.Title = title;
        MessageTextBlock.Text = message;
    }
}
