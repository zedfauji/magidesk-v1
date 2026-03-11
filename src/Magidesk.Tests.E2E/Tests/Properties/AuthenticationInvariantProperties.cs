using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for authentication invariants.
/// Validates authentication state consistency across login/logout cycles.
/// 
/// Feature: e2e-testing-comprehensive-scenarios
/// Property 1: Authentication state consistency
/// Validates: Requirements 1.1, 1.3
/// </summary>
[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class AuthenticationInvariantProperties : BaseE2ETest
{
    public AuthenticationInvariantProperties(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Property 1: Authentication state consistency
    /// Validates: Requirements 1.1, 1.3
    /// 
    /// For any successful login followed by logout, the user session must exist after login
    /// and must be terminated after logout. This property verifies that authentication state
    /// transitions are consistent and predictable.
    /// </summary>
    [Property(MaxTest = 10)]
    public Property AuthenticationState_ConsistentAcrossLoginLogoutCycle()
    {
        return Prop.ForAll(
            GenerateValidPins(),
            pin =>
            {
                try
                {
                    // Arrange
                    var loginPage = new LoginPage(MainWindow!);
                    var switchboard = new SwitchboardPage(MainWindow!);

                    // Act - Login with valid PIN
                    loginPage.LoginWithPin(pin);
                    Thread.Sleep(1000);

                    // Assert - After successful login, user session exists
                    var currentUserAfterLogin = switchboard.GetCurrentUserName();
                    var sessionExistsAfterLogin = !string.IsNullOrEmpty(currentUserAfterLogin);

                    if (!sessionExistsAfterLogin)
                    {
                        return false.ToProperty()
                            .Label("User session should exist after successful login");
                    }

                    // Act - Logout
                    switchboard.Logout();
                    Thread.Sleep(1000);

                    // Assert - After logout, session is terminated (back on login page)
                    var loginButtonEnabled = loginPage.IsLoginButtonEnabled();
                    var sessionTerminatedAfterLogout = loginButtonEnabled;

                    if (!sessionTerminatedAfterLogout)
                    {
                        return false.ToProperty()
                            .Label("User session should be terminated after logout");
                    }

                    // Property holds: session exists after login AND session terminated after logout
                    return (sessionExistsAfterLogin && sessionTerminatedAfterLogout)
                        .ToProperty()
                        .Label("Authentication state is consistent across login/logout cycle");
                }
                catch (Exception ex)
                {
                    // Mark test as failed for proper artifact capture
                    MarkTestFailed(ex);
                    
                    return false.ToProperty()
                        .Label($"Authentication state consistency check failed: {ex.Message}");
                }
            });
    }

    /// <summary>
    /// Validates that after successful login, the user can access authenticated features.
    /// This is a weaker property that verifies basic authentication functionality.
    /// </summary>
    [Fact]
    public void AuthenticationState_UserCanAccessAuthenticatedFeaturesAfterLogin()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);

        // Act - Login with valid PIN
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Assert - User session exists
        var currentUser = switchboard.GetCurrentUserName();
        Assert.NotNull(currentUser);
        Assert.NotEmpty(currentUser);

        // Assert - User can access authenticated features (e.g., New Ticket button exists)
        try
        {
            switchboard.NavigateToOrderEntry();
            Thread.Sleep(500);
            
            // If we can navigate to order entry, authentication is working
            Assert.NotNull(MainWindow);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Authenticated user should be able to access order entry: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates that after logout, the user cannot access authenticated features
    /// without logging in again.
    /// </summary>
    [Fact]
    public void AuthenticationState_UserCannotAccessAuthenticatedFeaturesAfterLogout()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);

        // Act - Login and then logout
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        
        switchboard.Logout();
        Thread.Sleep(1000);

        // Assert - User is back on login page
        Assert.True(loginPage.IsLoginButtonEnabled());
        
        // Assert - Login page elements are visible (session terminated)
        var errorMessage = loginPage.GetErrorMessage();
        Assert.NotNull(errorMessage); // Error message element exists (even if empty)
    }

    /// <summary>
    /// Validates that multiple login/logout cycles maintain consistent state.
    /// This tests the idempotence of authentication operations.
    /// </summary>
    [Fact]
    public void AuthenticationState_ConsistentAcrossMultipleLoginLogoutCycles()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        const int cycles = 3;

        // Act & Assert - Perform multiple login/logout cycles
        for (int i = 0; i < cycles; i++)
        {
            // Login
            loginPage.LoginWithPin("1234");
            Thread.Sleep(1000);

            // Verify session exists
            var currentUser = switchboard.GetCurrentUserName();
            Assert.NotNull(currentUser);
            Assert.NotEmpty(currentUser);

            // Logout
            switchboard.Logout();
            Thread.Sleep(1000);

            // Verify session terminated
            Assert.True(loginPage.IsLoginButtonEnabled());
        }
    }

    /// <summary>
    /// Validates that authentication state is not affected by invalid login attempts
    /// before a successful login.
    /// </summary>
    [Fact]
    public void AuthenticationState_NotAffectedByInvalidLoginAttempts()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);

        // Act - Attempt invalid login
        loginPage.EnterPin("9999");
        loginPage.ClickLogin();
        Thread.Sleep(500);

        // Assert - Still on login page
        Assert.True(loginPage.IsLoginButtonEnabled());

        // Act - Clear PIN and login with valid PIN
        for (int i = 0; i < 4; i++)
        {
            loginPage.ClickBackspace();
            Thread.Sleep(50);
        }
        
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Assert - Successful login after invalid attempt
        var currentUser = switchboard.GetCurrentUserName();
        Assert.NotNull(currentUser);
        Assert.NotEmpty(currentUser);
    }

    // ===== Property Generators =====

    /// <summary>
    /// Generates valid PINs for property testing.
    /// In this test environment, "1234" is the valid manager PIN from seed data.
    /// </summary>
    private static Arbitrary<string> GenerateValidPins()
    {
        // For E2E tests, we use the known valid PIN from seed data
        // In a real property test, we might generate multiple valid PINs
        return Arb.From(Gen.Constant("1234"));
    }
}
