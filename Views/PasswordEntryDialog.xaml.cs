using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace Magidesk.Presentation.Views;

public sealed partial class PasswordEntryDialog : ContentDialog
{
    public string Password => PasswordInput.Password;

    public PasswordEntryDialog()
    {
        this.InitializeComponent();
    }

    private void Numpad_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Content is string digit)
        {
            PasswordInput.Password += digit;
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        PasswordInput.Password = string.Empty;
    }

    private void Backspace_Click(object sender, RoutedEventArgs e)
    {
        if (PasswordInput.Password.Length > 0)
        {
            PasswordInput.Password = PasswordInput.Password[..^1];
        }
    }
}
