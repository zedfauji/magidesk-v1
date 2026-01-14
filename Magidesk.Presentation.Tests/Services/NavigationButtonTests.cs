using FsCheck;
using FsCheck.Xunit;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.Services;
using Xunit;

namespace Magidesk.Presentation.Tests.Services;

/// <summary>
/// Property-based tests for NavigationButton.
/// Feature: ui-polish-optimization
/// </summary>
public class NavigationButtonTests
{
    /// <summary>
    /// Property 12: Permission-Based Button Visibility
    /// Validates: Requirements 1.6
    /// 
    /// For any navigation button requiring specific permissions, the button should only be
    /// enabled when the current user has the required permission.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property12_PermissionBasedButtonVisibility()
    {
        // Generator for valid UserPermission values
        var permissionGen = Gen.Elements(
            UserPermission.None,
            UserPermission.CreateTicket,
            UserPermission.EditTicket,
            UserPermission.TakePayment,
            UserPermission.VoidTicket,
            UserPermission.RefundPayment,
            UserPermission.OpenDrawer,
            UserPermission.CloseBatch,
            UserPermission.ApplyDiscount,
            UserPermission.ManageUsers,
            UserPermission.ManageTableLayout,
            UserPermission.ManageMenu,
            UserPermission.ViewReports,
            UserPermission.SystemConfiguration,
            UserPermission.AdjustSessionTime,
            UserPermission.RefundTicket
        );

        return Prop.ForAll(
            Arb.From(permissionGen),
            Arb.From(permissionGen),
            (requiredPermission, userPermission) =>
            {
                // Arrange
                var button = new NavigationButton
                {
                    Label = "Test Button",
                    Icon = "\uE8F4",
                    Route = "TestRoute",
                    Category = "Operations",
                    IsEnabled = true,
                    RequiredPermission = requiredPermission,
                    KeyboardShortcut = "F1"
                };

                // Act
                var isVisible = button.IsVisibleForUser(userPermission);
                var isEnabled = button.IsEnabledForUser(userPermission);

                // Assert
                if (requiredPermission == UserPermission.None)
                {
                    // Buttons with no permission requirement should always be visible
                    return isVisible && isEnabled;
                }
                else if (userPermission.HasFlag(requiredPermission))
                {
                    // User has the required permission - button should be visible and enabled
                    return isVisible && isEnabled;
                }
                else
                {
                    // User does not have the required permission - button should not be visible
                    return !isVisible && !isEnabled;
                }
            }
        );
    }

    /// <summary>
    /// Unit test: Verify button with no permission requirement is always visible.
    /// </summary>
    [Fact]
    public void IsVisibleForUser_ReturnsTrue_WhenNoPermissionRequired()
    {
        // Arrange
        var button = new NavigationButton
        {
            RequiredPermission = UserPermission.None
        };

        // Act
        var isVisible = button.IsVisibleForUser(UserPermission.None);

        // Assert
        Assert.True(isVisible);
    }

    /// <summary>
    /// Unit test: Verify button is visible when user has required permission.
    /// </summary>
    [Fact]
    public void IsVisibleForUser_ReturnsTrue_WhenUserHasPermission()
    {
        // Arrange
        var button = new NavigationButton
        {
            RequiredPermission = UserPermission.VoidTicket
        };

        // Act
        var isVisible = button.IsVisibleForUser(UserPermission.VoidTicket);

        // Assert
        Assert.True(isVisible);
    }

    /// <summary>
    /// Unit test: Verify button is not visible when user lacks required permission.
    /// </summary>
    [Fact]
    public void IsVisibleForUser_ReturnsFalse_WhenUserLacksPermission()
    {
        // Arrange
        var button = new NavigationButton
        {
            RequiredPermission = UserPermission.VoidTicket
        };

        // Act
        var isVisible = button.IsVisibleForUser(UserPermission.CreateTicket);

        // Assert
        Assert.False(isVisible);
    }

    /// <summary>
    /// Unit test: Verify button is visible when user has multiple permissions including required.
    /// </summary>
    [Fact]
    public void IsVisibleForUser_ReturnsTrue_WhenUserHasMultiplePermissionsIncludingRequired()
    {
        // Arrange
        var button = new NavigationButton
        {
            RequiredPermission = UserPermission.VoidTicket
        };

        var userPermissions = UserPermission.CreateTicket | UserPermission.VoidTicket | UserPermission.RefundPayment;

        // Act
        var isVisible = button.IsVisibleForUser(userPermissions);

        // Assert
        Assert.True(isVisible);
    }

    /// <summary>
    /// Unit test: Verify button is enabled when user has permission and button is enabled.
    /// </summary>
    [Fact]
    public void IsEnabledForUser_ReturnsTrue_WhenUserHasPermissionAndButtonEnabled()
    {
        // Arrange
        var button = new NavigationButton
        {
            RequiredPermission = UserPermission.VoidTicket,
            IsEnabled = true
        };

        // Act
        var isEnabled = button.IsEnabledForUser(UserPermission.VoidTicket);

        // Assert
        Assert.True(isEnabled);
    }

    /// <summary>
    /// Unit test: Verify button is not enabled when button is disabled even if user has permission.
    /// </summary>
    [Fact]
    public void IsEnabledForUser_ReturnsFalse_WhenButtonDisabledEvenWithPermission()
    {
        // Arrange
        var button = new NavigationButton
        {
            RequiredPermission = UserPermission.VoidTicket,
            IsEnabled = false
        };

        // Act
        var isEnabled = button.IsEnabledForUser(UserPermission.VoidTicket);

        // Assert
        Assert.False(isEnabled);
    }

    /// <summary>
    /// Unit test: Verify button is not enabled when user lacks permission.
    /// </summary>
    [Fact]
    public void IsEnabledForUser_ReturnsFalse_WhenUserLacksPermission()
    {
        // Arrange
        var button = new NavigationButton
        {
            RequiredPermission = UserPermission.VoidTicket,
            IsEnabled = true
        };

        // Act
        var isEnabled = button.IsEnabledForUser(UserPermission.CreateTicket);

        // Assert
        Assert.False(isEnabled);
    }

    /// <summary>
    /// Unit test: Verify button with combined permissions requires all flags.
    /// </summary>
    [Fact]
    public void IsVisibleForUser_RequiresAllFlags_WhenMultiplePermissionsRequired()
    {
        // Arrange
        var button = new NavigationButton
        {
            RequiredPermission = UserPermission.VoidTicket | UserPermission.RefundPayment
        };

        // Act - User has only one of the required permissions
        var isVisiblePartial = button.IsVisibleForUser(UserPermission.VoidTicket);
        
        // Act - User has both required permissions
        var isVisibleFull = button.IsVisibleForUser(UserPermission.VoidTicket | UserPermission.RefundPayment);

        // Assert
        Assert.False(isVisiblePartial); // Should not be visible with only partial permissions
        Assert.True(isVisibleFull); // Should be visible with all required permissions
    }
}
