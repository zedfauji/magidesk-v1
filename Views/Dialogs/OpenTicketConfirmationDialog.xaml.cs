using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class OpenTicketConfirmationDialog : ContentDialog
{
    public string TableNumber { get; set; } = string.Empty;
    public bool HasExistingTicket { get; set; }
    public Guid? ExistingTicketId { get; set; }

    public OpenTicketConfirmationDialog()
    {
        this.InitializeComponent();
    }

    public void Initialize(string tableNumber, bool hasExistingTicket = false, Guid? existingTicketId = null)
    {
        TableNumber = tableNumber;
        HasExistingTicket = hasExistingTicket;
        ExistingTicketId = existingTicketId;

        TableInfoText.Text = $"Table: {tableNumber}";

        if (hasExistingTicket)
        {
            ExistingTicketWarning.IsOpen = true;
            Title = "Existing Ticket Found";
            PrimaryButtonText = "Open Existing Ticket";
            SecondaryButtonText = "Cancel";
        }
    }
}
