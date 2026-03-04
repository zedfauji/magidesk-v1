using System.IO;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Xunit;

namespace Magidesk.Domain.Tests.Compliance;

public class TicketPartialClassTests
{
    [Fact]
    public void Ticket_IsPartialClass_FilesSplitCorrectly()
    {
        // Arrange
        var projectRoot = FindProjectRoot();
        var entitiesPath = Path.Combine(projectRoot, "src", "Magidesk.Domain", "Entities");

        // Act
        var ticketFiles = Directory.GetFiles(entitiesPath, "Ticket*.cs");

        // Assert
        // Primary file should exist
        var primaryFile = Path.Combine(entitiesPath, "Ticket.cs");
        Assert.True(File.Exists(primaryFile), $"Primary Ticket.cs file not found at {primaryFile}");

        // At least 5 partial files should exist (including the primary)
        Assert.True(ticketFiles.Length >= 5,
            $"Expected at least 5 Ticket*.cs files, but found {ticketFiles.Length}. Files: {string.Join(", ", ticketFiles.Select(Path.GetFileName))}");

        // Verify that the Ticket class can be instantiated (proof that partial class is well-formed)
        var ticket = Ticket.Create(1001, new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        Assert.NotNull(ticket);
        Assert.NotEqual(Guid.Empty, ticket.Id);
    }

    private static string FindProjectRoot()
    {
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDir != null)
        {
            if (File.Exists(Path.Combine(currentDir.FullName, "Magidesk.sln")))
            {
                return currentDir.FullName;
            }

            currentDir = currentDir.Parent;
        }

        throw new InvalidOperationException("Could not find project root (Magidesk.sln)");
    }
}
