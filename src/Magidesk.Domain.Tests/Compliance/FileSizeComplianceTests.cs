using System.IO;
using Xunit;

namespace Magidesk.Domain.Tests.Compliance;

public class FileSizeComplianceTests
{
    private const int MaxLinesPerFile = 300;

    [Fact]
    public void AllProductionCsFiles_AreUnder300Lines()
    {
        // Arrange
        var projectRoot = FindProjectRoot();
        var srcPath = Path.Combine(projectRoot, "src");

        var filesToCheck = new[]
        {
            Path.Combine(srcPath, "Magidesk.Domain", "Entities", "Ticket*.cs"),
            Path.Combine(srcPath, "Magidesk.Infrastructure", "Repositories", "SalesReportRepository*.cs"),
            Path.Combine(srcPath, "Magidesk.Presentation", "ViewModels", "OrderPageViewModel*.cs"),
            Path.Combine(srcPath, "Magidesk.Presentation", "ViewModels", "TableMapViewModel*.cs"),
            Path.Combine(srcPath, "Magidesk.Presentation", "ViewModels", "SettlePageViewModel*.cs"),
            Path.Combine(srcPath, "Magidesk.Presentation", "ViewModels", "TableDesignerViewModel*.cs"),
        };

        var violatingFiles = new List<(string FilePath, int LineCount)>();

        // Act
        foreach (var pattern in filesToCheck)
        {
            var directory = Path.GetDirectoryName(pattern);
            var filePattern = Path.GetFileName(pattern);

            if (directory == null || !Directory.Exists(directory))
                continue;

            var files = Directory.GetFiles(directory, filePattern);

            foreach (var file in files)
            {
                // Exclude generated files
                if (file.EndsWith(".g.cs") || file.EndsWith(".Designer.cs") || file.EndsWith("ModelSnapshot.cs"))
                    continue;

                var lineCount = File.ReadAllLines(file).Length;
                if (lineCount > MaxLinesPerFile)
                {
                    violatingFiles.Add((file, lineCount));
                }
            }
        }

        // Assert
        if (violatingFiles.Count > 0)
        {
            var message = "The following files exceed the 300-line limit:\n";
            foreach (var (filePath, lineCount) in violatingFiles)
            {
                message += $"  - {filePath}: {lineCount} lines\n";
            }
            throw new Xunit.Sdk.XunitException(message);
        }

        Assert.Empty(violatingFiles);
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
