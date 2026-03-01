using Magidesk.Tests.E2E.Infrastructure;
using Xunit;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// Smoke tests to verify basic application functionality.
/// </summary>
public class SmokeTests : BaseE2ETest
{
    [Fact]
    public void Application_Launches_And_MainWindow_Exists()
    {
        // Arrange & Act are handled by BaseE2ETest setup

        // Assert
        Assert.NotNull(MainWindow);
        Assert.False(MainWindow.IsOffscreen);
        Assert.True(MainWindow.IsAvailable);
    }

    [Fact]
    public void MainWindow_Has_Title()
    {
        // Arrange & Act
        var title = MainWindow?.Title;

        // Assert
        Assert.NotNull(title);
        Assert.NotEmpty(title);
    }
}
