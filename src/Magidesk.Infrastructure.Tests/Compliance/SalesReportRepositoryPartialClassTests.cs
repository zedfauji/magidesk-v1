using System.IO;
using Magidesk.Infrastructure.Repositories;
using Xunit;

namespace Magidesk.Infrastructure.Tests.Compliance;

public class SalesReportRepositoryPartialClassTests
{
    [Fact]
    public void SalesReportRepository_IsPartialClass_FilesSplitCorrectly()
    {
        // Arrange
        var projectRoot = FindProjectRoot();
        var repositoriesPath = Path.Combine(projectRoot, "src", "Magidesk.Infrastructure", "Repositories");

        // Act
        var repositoryFiles = Directory.GetFiles(repositoriesPath, "SalesReportRepository*.cs");

        // Assert
        // Primary file should exist
        var primaryFile = Path.Combine(repositoriesPath, "SalesReportRepository.cs");
        Assert.True(File.Exists(primaryFile), $"Primary SalesReportRepository.cs file not found at {primaryFile}");

        // At least 5 partial files should exist (including the primary)
        Assert.True(repositoryFiles.Length >= 5,
            $"Expected at least 5 SalesReportRepository*.cs files, but found {repositoryFiles.Length}. Files: {string.Join(", ", repositoryFiles.Select(Path.GetFileName))}");

        // Verify that the SalesReportRepository type can be resolved
        var repositoryType = typeof(SalesReportRepository);
        Assert.NotNull(repositoryType);
        Assert.Equal("SalesReportRepository", repositoryType.Name);
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
