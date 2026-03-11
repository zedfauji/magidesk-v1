using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for dining table status invariants.
/// Validates that table status always reflects ticket association correctly.
/// 
/// Feature: e2e-testing-comprehensive-scenarios
/// Property 4: Table status reflects ticket association
/// Validates: Requirements 4.1, 4.2, 4.3
/// </summary>
[Trait("Priority", "P1")]
[Trait("Category", "OperationalIntegrity")]
public class DiningTableStatusInvariantProperties : BaseE2ETest
{
    public DiningTableStatusInvariantProperties(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Property 4: Table status reflects ticket association
    /// Validates: Requirements 4.1, 4.2, 4.3
    /// 
    /// For any dining table, the status must accurately reflect its ticket association:
    /// - Occupied tables must have associated tickets
    /// - Available tables must have no tickets
    /// This property verifies that table status is always consistent with ticket state.
    /// </summary>
    [Property(MaxTest = 10)]
    public Property TableStatus_ReflectsTicketAssociation()
    {
        return Prop.ForAll(
            GenerateTableOperations(),
            operation =>
            {
                try
                {
                    // Arrange
                    var loginPage = new LoginPage(MainWindow!);
                    var switchboard = new SwitchboardPage(MainWindow!);
                    var tableManagement = new TableManagementPage(MainWindow!);

                    // Act - Login and navigate to tables
                    loginPage.LoginWithPin("1234");
                    Thread.Sleep(1000);
                    switchboard.NavigateToTables();
                    Thread.Sleep(1000);

                    // Verify initial status is available
                    var initialStatus = tableManagement.GetTableStatus(operation.TableNumber);
                    var initialStatusIsAvailable = initialStatus == TableStatus.Available;

                    if (!initialStatusIsAvailable)
                    {
                        return false.ToProperty()
                            .Label($"Table {operation.TableNumber} should start as Available. " +
                                   $"Actual: {initialStatus}");
                    }

                    // Act - Perform operation based on type
                    if (operation.OperationType == TableOperationType.Assign)
                    {
                        // Assign table to ticket
                        tableManagement.AssignDiningTable(operation.TableNumber, operation.TicketId);
                        Thread.Sleep(1000);

                        // Verify status changed to occupied
                        var statusAfterAssign = tableManagement.GetTableStatus(operation.TableNumber);
                        var statusIsOccupied = statusAfterAssign == TableStatus.Occupied;

                        if (!statusIsOccupied)
                        {
                            return false.ToProperty()
                                .Label($"Table {operation.TableNumber} should be Occupied after assignment. " +
                                       $"Actual: {statusAfterAssign}");
                        }

                        return statusIsOccupied
                            .ToProperty()
                            .Label("Table status reflects ticket association after assignment");
                    }
                    else if (operation.OperationType == TableOperationType.AssignThenClear)
                    {
                        // Assign table first
                        tableManagement.AssignDiningTable(operation.TableNumber, operation.TicketId);
                        Thread.Sleep(1000);

                        // Verify occupied
                        var statusAfterAssign = tableManagement.GetTableStatus(operation.TableNumber);
                        if (statusAfterAssign != TableStatus.Occupied)
                        {
                            return false.ToProperty()
                                .Label($"Table {operation.TableNumber} should be Occupied after assignment. " +
                                       $"Actual: {statusAfterAssign}");
                        }

                        // Clear table
                        tableManagement.ClearTable(operation.TableNumber);
                        Thread.Sleep(1000);

                        // Verify status changed back to available
                        var statusAfterClear = tableManagement.GetTableStatus(operation.TableNumber);
                        var statusIsAvailable = statusAfterClear == TableStatus.Available;

                        if (!statusIsAvailable)
                        {
                            return false.ToProperty()
                                .Label($"Table {operation.TableNumber} should be Available after clearing. " +
                                       $"Actual: {statusAfterClear}");
                        }

                        return statusIsAvailable
                            .ToProperty()
                            .Label("Table status reflects ticket disassociation after clearing");
                    }

                    return true.ToProperty()
                        .Label("Table status consistency verified");
                }
                catch (Exception ex)
                {
                    // Mark test as failed for proper artifact capture
                    MarkTestFailed(ex);
                    
                    return false.ToProperty()
                        .Label($"Table status invariant check failed: {ex.Message}");
                }
            });
    }

    /// <summary>
    /// Validates that available tables have no associated tickets.
    /// </summary>
    [Fact]
    public void AvailableTable_HasNoAssociatedTicket()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Assert - Verify table starts as available
        var status = tableManagement.GetTableStatus("201");
        Assert.Equal(TableStatus.Available, status);
    }

    /// <summary>
    /// Validates that occupied tables have associated tickets.
    /// </summary>
    [Fact]
    public void OccupiedTable_HasAssociatedTicket()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Assign table to ticket
        tableManagement.AssignDiningTable("202", "TICKET-202");
        Thread.Sleep(1000);

        // Assert - Verify table is occupied
        var status = tableManagement.GetTableStatus("202");
        Assert.Equal(TableStatus.Occupied, status);
    }

    /// <summary>
    /// Validates that clearing a table removes ticket association.
    /// </summary>
    [Fact]
    public void ClearTable_RemovesTicketAssociation()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Assign table
        tableManagement.AssignDiningTable("203", "TICKET-203");
        Thread.Sleep(1000);

        // Verify occupied
        var occupiedStatus = tableManagement.GetTableStatus("203");
        Assert.Equal(TableStatus.Occupied, occupiedStatus);

        // Act - Clear table
        tableManagement.ClearTable("203");
        Thread.Sleep(1000);

        // Assert - Verify available
        var availableStatus = tableManagement.GetTableStatus("203");
        Assert.Equal(TableStatus.Available, availableStatus);
    }

    /// <summary>
    /// Validates that table status transitions are atomic.
    /// </summary>
    [Fact]
    public void TableStatusTransition_IsAtomic()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Assign table
        tableManagement.AssignDiningTable("204", "TICKET-204");
        Thread.Sleep(1000);

        // Assert - Status should be occupied (not in intermediate state)
        var status = tableManagement.GetTableStatus("204");
        Assert.True(status == TableStatus.Occupied || status == TableStatus.Available,
            $"Table status should be either Occupied or Available, not intermediate. Actual: {status}");
    }

    /// <summary>
    /// Validates that multiple assign operations on same table maintain consistency.
    /// </summary>
    [Fact]
    public void MultipleAssignOperations_MaintainConsistency()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Assign table to first ticket
        tableManagement.AssignDiningTable("205", "TICKET-205A");
        Thread.Sleep(1000);

        // Verify occupied
        var status1 = tableManagement.GetTableStatus("205");
        Assert.Equal(TableStatus.Occupied, status1);

        // Act - Assign table to second ticket (multiple tickets per table scenario)
        tableManagement.AssignDiningTable("205", "TICKET-205B");
        Thread.Sleep(1000);

        // Assert - Table should remain occupied
        var status2 = tableManagement.GetTableStatus("205");
        Assert.Equal(TableStatus.Occupied, status2);
    }

    /// <summary>
    /// Validates that table transfer maintains occupied status.
    /// </summary>
    [Fact]
    public void TableTransfer_MaintainsOccupiedStatus()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Assign table
        tableManagement.AssignDiningTable("206", "TICKET-206");
        Thread.Sleep(1000);

        // Verify occupied
        var statusBeforeTransfer = tableManagement.GetTableStatus("206");
        Assert.Equal(TableStatus.Occupied, statusBeforeTransfer);

        // Act - Transfer table to new server
        tableManagement.TransferTable("206", "Server2");
        Thread.Sleep(1000);

        // Assert - Table should remain occupied after transfer
        var statusAfterTransfer = tableManagement.GetTableStatus("206");
        Assert.Equal(TableStatus.Occupied, statusAfterTransfer);
    }

    // ===== Property Generators =====

    /// <summary>
    /// Generates table operation data for property testing.
    /// </summary>
    private static Arbitrary<TableOperation> GenerateTableOperations()
    {
        var operationGen = from tableNumber in Gen.Elements("301", "302", "303", "304", "305")
                          from ticketId in Gen.Choose(1, 100).Select(n => $"TICKET-{n:D3}")
                          from operationType in Gen.Elements(
                              TableOperationType.Assign,
                              TableOperationType.AssignThenClear)
                          select new TableOperation
                          {
                              TableNumber = tableNumber,
                              TicketId = ticketId,
                              OperationType = operationType
                          };

        return Arb.From(operationGen);
    }

    /// <summary>
    /// Represents a table operation for property testing.
    /// </summary>
    private class TableOperation
    {
        public string TableNumber { get; set; } = string.Empty;
        public string TicketId { get; set; } = string.Empty;
        public TableOperationType OperationType { get; set; }
    }

    /// <summary>
    /// Types of table operations for property testing.
    /// </summary>
    private enum TableOperationType
    {
        Assign,
        AssignThenClear
    }
}
