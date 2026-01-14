using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xunit;

namespace Magidesk.Presentation.Tests.ViewModels;

/// <summary>
/// Unit tests for ConfirmationDialogViewModel.
/// </summary>
public class ConfirmationDialogViewModelTests
{
    [Fact]
    public void Initialize_SetsAllProperties()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        var title = "Test Title";
        var message = "Test Message";
        var primaryButton = "Confirm";
        var secondaryButton = "Cancel";
        var icon = "⚠️";
        var severity = "Warning";
        var details = "Additional details";
        var warningMessage = "This is a warning";
        var detailItems = new Dictionary<string, string>
        {
            { "Ticket Number", "12345" },
            { "Amount", "$50.00" }
        };

        // Act
        viewModel.Initialize(title, message, primaryButton, secondaryButton, icon, severity, details, warningMessage, detailItems);

        // Assert
        Assert.Equal(title, viewModel.Title);
        Assert.Equal(message, viewModel.Message);
        Assert.Equal(primaryButton, viewModel.PrimaryButtonText);
        Assert.Equal(secondaryButton, viewModel.SecondaryButtonText);
        Assert.Equal(icon, viewModel.Icon);
        Assert.Equal(severity, viewModel.Severity);
        Assert.Equal(details, viewModel.Details);
        Assert.Equal(warningMessage, viewModel.WarningMessage);
        Assert.Equal(2, viewModel.DetailItems.Count);
        Assert.Equal("Ticket Number", viewModel.DetailItems[0].Label);
        Assert.Equal("12345", viewModel.DetailItems[0].Value);
        Assert.Equal("Amount", viewModel.DetailItems[1].Label);
        Assert.Equal("$50.00", viewModel.DetailItems[1].Value);
    }

    [Fact]
    public void InfoBarSeverity_ReturnsCorrectSeverity_ForWarning()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        viewModel.Initialize("Title", "Message", severity: "Warning");

        // Act
        var severity = viewModel.InfoBarSeverity;

        // Assert
        Assert.Equal(InfoBarSeverity.Warning, severity);
    }

    [Fact]
    public void InfoBarSeverity_ReturnsCorrectSeverity_ForError()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        viewModel.Initialize("Title", "Message", severity: "Error");

        // Act
        var severity = viewModel.InfoBarSeverity;

        // Assert
        Assert.Equal(InfoBarSeverity.Error, severity);
    }

    [Fact]
    public void InfoBarSeverity_ReturnsCorrectSeverity_ForInfo()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        viewModel.Initialize("Title", "Message", severity: "Info");

        // Act
        var severity = viewModel.InfoBarSeverity;

        // Assert
        Assert.Equal(InfoBarSeverity.Informational, severity);
    }

    [Fact]
    public void InfoBarSeverity_ReturnsCorrectSeverity_ForSuccess()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        viewModel.Initialize("Title", "Message", severity: "Success");

        // Act
        var severity = viewModel.InfoBarSeverity;

        // Assert
        Assert.Equal(InfoBarSeverity.Success, severity);
    }

    [Fact]
    public void HasDetails_ReturnsTrue_WhenDetailsProvided()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        viewModel.Initialize("Title", "Message", details: "Some details");

        // Act
        var hasDetails = viewModel.HasDetails;

        // Assert
        Assert.True(hasDetails);
    }

    [Fact]
    public void HasDetails_ReturnsFalse_WhenNoDetailsProvided()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        viewModel.Initialize("Title", "Message", details: "");

        // Act
        var hasDetails = viewModel.HasDetails;

        // Assert
        Assert.False(hasDetails);
    }

    [Fact]
    public void DetailsVisibility_ReturnsVisible_WhenDetailsProvided()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        viewModel.Initialize("Title", "Message", details: "Some details");

        // Act
        var visibility = viewModel.DetailsVisibility;

        // Assert
        Assert.Equal(Visibility.Visible, visibility);
    }

    [Fact]
    public void DetailsVisibility_ReturnsCollapsed_WhenNoDetailsProvided()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        viewModel.Initialize("Title", "Message", details: "");

        // Act
        var visibility = viewModel.DetailsVisibility;

        // Assert
        Assert.Equal(Visibility.Collapsed, visibility);
    }

    [Fact]
    public void HasDetailItems_ReturnsTrue_WhenDetailItemsProvided()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        var detailItems = new Dictionary<string, string>
        {
            { "Key1", "Value1" }
        };
        viewModel.Initialize("Title", "Message", detailItems: detailItems);

        // Act
        var hasDetailItems = viewModel.HasDetailItems;

        // Assert
        Assert.True(hasDetailItems);
    }

    [Fact]
    public void HasDetailItems_ReturnsFalse_WhenNoDetailItemsProvided()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        viewModel.Initialize("Title", "Message");

        // Act
        var hasDetailItems = viewModel.HasDetailItems;

        // Assert
        Assert.False(hasDetailItems);
    }

    [Fact]
    public void DetailCardVisibility_ReturnsVisible_WhenDetailItemsProvided()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        var detailItems = new Dictionary<string, string>
        {
            { "Key1", "Value1" }
        };
        viewModel.Initialize("Title", "Message", detailItems: detailItems);

        // Act
        var visibility = viewModel.DetailCardVisibility;

        // Assert
        Assert.Equal(Visibility.Visible, visibility);
    }

    [Fact]
    public void DetailCardVisibility_ReturnsCollapsed_WhenNoDetailItemsProvided()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();
        viewModel.Initialize("Title", "Message");

        // Act
        var visibility = viewModel.DetailCardVisibility;

        // Assert
        Assert.Equal(Visibility.Collapsed, visibility);
    }

    [Fact]
    public void Initialize_WithNullDetailItems_DoesNotThrow()
    {
        // Arrange
        var viewModel = new ConfirmationDialogViewModel();

        // Act & Assert
        var exception = Record.Exception(() => 
            viewModel.Initialize("Title", "Message", detailItems: null));
        
        Assert.Null(exception);
        Assert.Empty(viewModel.DetailItems);
    }
}
