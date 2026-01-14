using FsCheck;
using FsCheck.Xunit;
using Magidesk.Presentation.Services;
using Moq;
using System.Windows.Input;
using Windows.System;
using Xunit;

namespace Magidesk.Presentation.Tests.Services;

/// <summary>
/// Property-based tests for KeyboardShortcutService.
/// Feature: ui-polish-optimization
/// </summary>
public class KeyboardShortcutServiceTests
{
    /// <summary>
    /// Property 5: Keyboard Shortcut Uniqueness
    /// Validates: Requirements 10.8
    /// 
    /// For any two keyboard shortcuts in the system, they should not have the same
    /// key combination to prevent conflicts.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property5_KeyboardShortcutUniqueness()
    {
        return Prop.ForAll(
            Arb.From(Gen.Elements(
                VirtualKey.F1, VirtualKey.F2, VirtualKey.F3, VirtualKey.F4,
                VirtualKey.F5, VirtualKey.F6, VirtualKey.F7, VirtualKey.F8
            )),
            Arb.From(Gen.Elements(
                VirtualKeyModifiers.None,
                VirtualKeyModifiers.Control,
                VirtualKeyModifiers.Shift,
                VirtualKeyModifiers.Menu
            )),
            (key, modifiers) =>
            {
                // Arrange
                var service = new KeyboardShortcutService();
                var command1 = new Mock<ICommand>().Object;
                var command2 = new Mock<ICommand>().Object;

                // Act - Register first shortcut
                service.RegisterShortcut(key, modifiers, "Action1", command1);

                // Try to register the same key combination again
                var exceptionThrown = false;
                try
                {
                    service.RegisterShortcut(key, modifiers, "Action2", command2);
                }
                catch (InvalidOperationException)
                {
                    exceptionThrown = true;
                }

                // Assert - Should throw exception for duplicate registration
                return exceptionThrown;
            }
        );
    }

    /// <summary>
    /// Unit test: Verify shortcut registration succeeds.
    /// </summary>
    [Fact]
    public void RegisterShortcut_AddsShortcutSuccessfully()
    {
        // Arrange
        var service = new KeyboardShortcutService();
        var command = new Mock<ICommand>().Object;

        // Act
        service.RegisterShortcut(VirtualKey.F1, VirtualKeyModifiers.None, "NewTicket", command);

        // Assert
        var shortcuts = service.GetAllShortcuts();
        Assert.Single(shortcuts);
    }

    /// <summary>
    /// Unit test: Verify duplicate shortcut registration throws exception.
    /// </summary>
    [Fact]
    public void RegisterShortcut_ThrowsExceptionForDuplicate()
    {
        // Arrange
        var service = new KeyboardShortcutService();
        var command1 = new Mock<ICommand>().Object;
        var command2 = new Mock<ICommand>().Object;

        service.RegisterShortcut(VirtualKey.F1, VirtualKeyModifiers.None, "Action1", command1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.RegisterShortcut(VirtualKey.F1, VirtualKeyModifiers.None, "Action2", command2)
        );
    }

    /// <summary>
    /// Unit test: Verify shortcut execution when command can execute.
    /// </summary>
    [Fact]
    public void HandleKeyPress_ExecutesCommandWhenCanExecute()
    {
        // Arrange
        var service = new KeyboardShortcutService();
        var commandMock = new Mock<ICommand>();
        commandMock.Setup(c => c.CanExecute(null)).Returns(true);

        service.RegisterShortcut(VirtualKey.F1, VirtualKeyModifiers.None, "TestAction", commandMock.Object);

        // Act
        var handled = service.HandleKeyPress(VirtualKey.F1, VirtualKeyModifiers.None);

        // Assert
        Assert.True(handled);
        commandMock.Verify(c => c.Execute(null), Times.Once);
    }

    /// <summary>
    /// Unit test: Verify shortcut does not execute when command cannot execute.
    /// </summary>
    [Fact]
    public void HandleKeyPress_DoesNotExecuteWhenCannotExecute()
    {
        // Arrange
        var service = new KeyboardShortcutService();
        var commandMock = new Mock<ICommand>();
        commandMock.Setup(c => c.CanExecute(null)).Returns(false);

        service.RegisterShortcut(VirtualKey.F1, VirtualKeyModifiers.None, "TestAction", commandMock.Object);

        // Act
        var handled = service.HandleKeyPress(VirtualKey.F1, VirtualKeyModifiers.None);

        // Assert
        Assert.False(handled);
        commandMock.Verify(c => c.Execute(null), Times.Never);
    }

    /// <summary>
    /// Unit test: Verify unregistered shortcut returns false.
    /// </summary>
    [Fact]
    public void HandleKeyPress_ReturnsFalseForUnregisteredShortcut()
    {
        // Arrange
        var service = new KeyboardShortcutService();

        // Act
        var handled = service.HandleKeyPress(VirtualKey.F1, VirtualKeyModifiers.None);

        // Assert
        Assert.False(handled);
    }

    /// <summary>
    /// Unit test: Verify shortcut unregistration.
    /// </summary>
    [Fact]
    public void UnregisterShortcut_RemovesShortcut()
    {
        // Arrange
        var service = new KeyboardShortcutService();
        var command = new Mock<ICommand>().Object;

        service.RegisterShortcut(VirtualKey.F1, VirtualKeyModifiers.None, "TestAction", command);

        // Act
        service.UnregisterShortcut(VirtualKey.F1, VirtualKeyModifiers.None);

        // Assert
        var shortcuts = service.GetAllShortcuts();
        Assert.Empty(shortcuts);
    }

    /// <summary>
    /// Unit test: Verify different modifiers allow same key.
    /// </summary>
    [Fact]
    public void RegisterShortcut_AllowsSameKeyWithDifferentModifiers()
    {
        // Arrange
        var service = new KeyboardShortcutService();
        var command1 = new Mock<ICommand>().Object;
        var command2 = new Mock<ICommand>().Object;

        // Act
        service.RegisterShortcut(VirtualKey.F1, VirtualKeyModifiers.None, "Action1", command1);
        service.RegisterShortcut(VirtualKey.F1, VirtualKeyModifiers.Control, "Action2", command2);

        // Assert
        var shortcuts = service.GetAllShortcuts();
        Assert.Equal(2, shortcuts.Count);
    }
}
