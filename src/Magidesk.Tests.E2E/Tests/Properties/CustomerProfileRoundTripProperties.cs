using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Magidesk.Tests.Workflows.Infrastructure;
using Xunit;
using Xunit.Abstracts;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for customer profile serialization round-trip.
/// Validates that customer profile data can be serialized and deserialized without data loss.
/// 
/// Feature: e2e-testing-comprehensive-scenarios
/// Property 11: Customer profile serialization round-trip
/// Validates: Requirements 11.1, 21.4
/// </summary>
[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class CustomerProfileRoundTripProperties : BaseE2ETest
{
    public CustomerProfileRoundTripProperties(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Property 11: Customer profile serialization round-trip
    /// Validates: Requirements 11.1, 21.4
    /// 
    /// For any customer profile, deserialize(serialize(profile)) must equal the original profile.
    /// This property verifies that customer profile data can be saved and retrieved without data loss,
    /// ensuring data integrity for customer management operations.
    /// </summary>
    [Property(MaxTest = 10)]
    public Property CustomerProfile_RoundTripPreservesAllData()
    {
        return Prop.ForAll(
            TestDataGenerators.CustomerGenerator(),
            customerData =>
            {
                try
                {
                    // Arrange
                    var loginPage = new LoginPage(MainWindow!);
                    var switchboard = new SwitchboardPage(MainWindow!);
                    var customerPage = new CustomerPage(MainWindow!);

                    // Act - Login and navigate to customer management
                    loginPage.LoginWithPin("1234");
                    Thread.Sleep(1000);
                    switchboard.NavigateToCustomerManagement();
                    Thread.Sleep(1000);

                    // Act - Create customer profile (serialize operation)
                    customerPage.CreateCustomer(customerData.Name, customerData.Phone, customerData.Email);
                    Thread.Sleep(1000);

                    // Act - Search for the customer by name (deserialize operation)
                    customerPage.SearchCustomer(customerData.Name);
                    Thread.Sleep(1000);

                    // Act - Retrieve customer profile data
                    var retrievedName = customerPage.GetCustomerName();
                    var retrievedPhone = customerPage.GetCustomerPhone();
                    var retrievedEmail = customerPage.GetCustomerEmail();

                    // Assert - Verify round-trip preserves all data
                    var nameMatches = retrievedName == customerData.Name;
                    var phoneMatches = retrievedPhone == customerData.Phone;
                    var emailMatches = retrievedEmail == customerData.Email;

                    if (!nameMatches)
                    {
                        return false.ToProperty()
                            .Label($"Customer name should be preserved in round-trip. " +
                                   $"Original: '{customerData.Name}', Retrieved: '{retrievedName}'");
                    }

                    if (!phoneMatches)
                    {
                        return false.ToProperty()
                            .Label($"Customer phone should be preserved in round-trip. " +
                                   $"Original: '{customerData.Phone}', Retrieved: '{retrievedPhone}'");
                    }

                    if (!emailMatches)
                    {
                        return false.ToProperty()
                            .Label($"Customer email should be preserved in round-trip. " +
                                   $"Original: '{customerData.Email}', Retrieved: '{retrievedEmail}'");
                    }

                    return (nameMatches && phoneMatches && emailMatches)
                        .ToProperty()
                        .Label("Customer profile round-trip preserves all data");
                }
                catch (Exception ex)
                {
                    // Mark test as failed for proper artifact capture
                    MarkTestFailed(ex);
                    
                    return false.ToProperty()
                        .Label($"Customer profile round-trip check failed: {ex.Message}");
                }
            });
    }

    /// <summary>
    /// Validates that customer profile creation persists data correctly.
    /// </summary>
    [Fact]
    public void CustomerProfile_CreationPersistsData()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var customerPage = new CustomerPage(MainWindow!);

        var customerName = "Test Customer";
        var customerPhone = "2025551234";
        var customerEmail = "test@example.com";

        // Act - Login and navigate to customer management
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCustomerManagement();
        Thread.Sleep(1000);

        // Act - Create customer profile
        customerPage.CreateCustomer(customerName, customerPhone, customerEmail);
        Thread.Sleep(1000);

        // Act - Search for the customer
        customerPage.SearchCustomer(customerName);
        Thread.Sleep(1000);

        // Assert - Verify customer data is persisted
        var retrievedName = customerPage.GetCustomerName();
        var retrievedPhone = customerPage.GetCustomerPhone();
        var retrievedEmail = customerPage.GetCustomerEmail();

        Assert.Equal(customerName, retrievedName);
        Assert.Equal(customerPhone, retrievedPhone);
        Assert.Equal(customerEmail, retrievedEmail);
    }

    /// <summary>
    /// Validates that customer profile search returns correct results.
    /// </summary>
    [Fact]
    public void CustomerProfile_SearchReturnsCorrectResults()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var customerPage = new CustomerPage(MainWindow!);

        var customer1Name = "Alice Johnson";
        var customer1Phone = "2025551111";
        var customer1Email = "alice@example.com";

        var customer2Name = "Bob Smith";
        var customer2Phone = "2025552222";
        var customer2Email = "bob@example.com";

        // Act - Login and navigate to customer management
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCustomerManagement();
        Thread.Sleep(1000);

        // Act - Create first customer
        customerPage.CreateCustomer(customer1Name, customer1Phone, customer1Email);
        Thread.Sleep(1000);

        // Act - Create second customer
        customerPage.CreateCustomer(customer2Name, customer2Phone, customer2Email);
        Thread.Sleep(1000);

        // Act - Search for first customer by name
        customerPage.SearchCustomer(customer1Name);
        Thread.Sleep(1000);

        // Assert - Verify correct customer is returned
        var retrievedName = customerPage.GetCustomerName();
        Assert.Equal(customer1Name, retrievedName);

        // Act - Search for second customer by phone
        customerPage.SearchCustomer(customer2Phone);
        Thread.Sleep(1000);

        // Assert - Verify correct customer is returned
        retrievedName = customerPage.GetCustomerName();
        Assert.Equal(customer2Name, retrievedName);
    }

    /// <summary>
    /// Validates that customer profile data is not corrupted by special characters.
    /// </summary>
    [Fact]
    public void CustomerProfile_HandlesSpecialCharactersCorrectly()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var customerPage = new CustomerPage(MainWindow!);

        var customerName = "O'Brien-Smith";
        var customerPhone = "202-555-1234";
        var customerEmail = "test+tag@example.com";

        // Act - Login and navigate to customer management
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCustomerManagement();
        Thread.Sleep(1000);

        // Act - Create customer with special characters
        customerPage.CreateCustomer(customerName, customerPhone, customerEmail);
        Thread.Sleep(1000);

        // Act - Search for the customer
        customerPage.SearchCustomer(customerName);
        Thread.Sleep(1000);

        // Assert - Verify special characters are preserved
        var retrievedName = customerPage.GetCustomerName();
        var retrievedPhone = customerPage.GetCustomerPhone();
        var retrievedEmail = customerPage.GetCustomerEmail();

        Assert.Equal(customerName, retrievedName);
        Assert.Equal(customerPhone, retrievedPhone);
        Assert.Equal(customerEmail, retrievedEmail);
    }

    /// <summary>
    /// Validates that customer profile fields are not truncated.
    /// </summary>
    [Fact]
    public void CustomerProfile_DoesNotTruncateFields()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var customerPage = new CustomerPage(MainWindow!);

        var customerName = "Very Long Customer Name With Many Words";
        var customerPhone = "2025551234567890";
        var customerEmail = "very.long.email.address@example.com";

        // Act - Login and navigate to customer management
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCustomerManagement();
        Thread.Sleep(1000);

        // Act - Create customer with long fields
        customerPage.CreateCustomer(customerName, customerPhone, customerEmail);
        Thread.Sleep(1000);

        // Act - Search for the customer
        customerPage.SearchCustomer(customerName);
        Thread.Sleep(1000);

        // Assert - Verify fields are not truncated
        var retrievedName = customerPage.GetCustomerName();
        var retrievedPhone = customerPage.GetCustomerPhone();
        var retrievedEmail = customerPage.GetCustomerEmail();

        Assert.Equal(customerName, retrievedName);
        Assert.Equal(customerPhone, retrievedPhone);
        Assert.Equal(customerEmail, retrievedEmail);
    }

    /// <summary>
    /// Validates that multiple customer profiles can be created and retrieved independently.
    /// </summary>
    [Fact]
    public void CustomerProfile_MultipleProfilesIndependent()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var customerPage = new CustomerPage(MainWindow!);

        var customers = new[]
        {
            ("Customer One", "2025551001", "one@example.com"),
            ("Customer Two", "2025551002", "two@example.com"),
            ("Customer Three", "2025551003", "three@example.com")
        };

        // Act - Login and navigate to customer management
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCustomerManagement();
        Thread.Sleep(1000);

        // Act - Create multiple customers
        foreach (var (name, phone, email) in customers)
        {
            customerPage.CreateCustomer(name, phone, email);
            Thread.Sleep(500);
        }

        // Assert - Verify each customer can be retrieved independently
        foreach (var (name, phone, email) in customers)
        {
            customerPage.SearchCustomer(name);
            Thread.Sleep(500);

            var retrievedName = customerPage.GetCustomerName();
            var retrievedPhone = customerPage.GetCustomerPhone();
            var retrievedEmail = customerPage.GetCustomerEmail();

            Assert.Equal(name, retrievedName);
            Assert.Equal(phone, retrievedPhone);
            Assert.Equal(email, retrievedEmail);
        }
    }

    /// <summary>
    /// Validates that customer profile update preserves data integrity.
    /// </summary>
    [Fact]
    public void CustomerProfile_UpdatePreservesDataIntegrity()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var customerPage = new CustomerPage(MainWindow!);

        var originalName = "Original Name";
        var originalPhone = "2025551234";
        var originalEmail = "original@example.com";

        var updatedPhone = "2025559999";
        var updatedEmail = "updated@example.com";

        // Act - Login and navigate to customer management
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCustomerManagement();
        Thread.Sleep(1000);

        // Act - Create customer
        customerPage.CreateCustomer(originalName, originalPhone, originalEmail);
        Thread.Sleep(1000);

        // Act - Search and update customer
        customerPage.SearchCustomer(originalName);
        Thread.Sleep(1000);
        customerPage.UpdateCustomer(originalName, updatedPhone, updatedEmail);
        Thread.Sleep(1000);

        // Act - Retrieve updated customer
        customerPage.SearchCustomer(originalName);
        Thread.Sleep(1000);

        // Assert - Verify updated data is preserved
        var retrievedName = customerPage.GetCustomerName();
        var retrievedPhone = customerPage.GetCustomerPhone();
        var retrievedEmail = customerPage.GetCustomerEmail();

        Assert.Equal(originalName, retrievedName); // Name unchanged
        Assert.Equal(updatedPhone, retrievedPhone); // Phone updated
        Assert.Equal(updatedEmail, retrievedEmail); // Email updated
    }
}
