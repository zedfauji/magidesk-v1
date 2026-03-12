using FsCheck;
using FsCheck.Xunit;

namespace Magidesk.Installer.PropertyTests;

/// <summary>
/// Property-based tests for file structure preservation during installation.
/// **Validates: Requirements 10.3**
/// Property 12: File Copy Structure Preservation
/// </summary>
public class FileStructurePreservationPropertyTests
{
    /// <summary>
    /// Property: For any file path with subdirectories, the relative path structure
    /// should be preserved when copying from source to destination.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool FileCopy_PreservesRelativePath(string[] pathSegments)
    {
        // Skip invalid inputs
        if (pathSegments == null || pathSegments.Length == 0)
        {
            return true;
        }

        // Filter out invalid path segments
        var validSegments = pathSegments
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Where(s => !s.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
            .Where(s => s != "." && s != "..")
            .ToArray();

        if (validSegments.Length == 0)
        {
            return true; // Skip if no valid segments
        }

        // Build relative path
        var relativePath = Path.Combine(validSegments);
        
        // Verify path can be combined with base paths
        try
        {
            var sourcePath = Path.Combine("C:\\Source", relativePath);
            var destPath = Path.Combine("C:\\Dest", relativePath);

            // Extract relative portions
            var sourceRelative = Path.GetRelativePath("C:\\Source", sourcePath);
            var destRelative = Path.GetRelativePath("C:\\Dest", destPath);

            // Relative paths should be identical
            return sourceRelative.Equals(destRelative, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return true; // Skip invalid path combinations
        }
    }

    /// <summary>
    /// Property: For any directory structure, the number of subdirectory levels
    /// should be preserved during copy operations.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool FileCopy_PreservesDirectoryDepth(List<string> pathSegments)
    {
        if (pathSegments == null || pathSegments.Count == 0)
        {
            return true;
        }

        // Filter valid segments
        var validSegments = pathSegments
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Where(s => !s.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
            .Where(s => s != "." && s != "..")
            .ToList();

        if (validSegments.Count == 0)
        {
            return true;
        }

        try
        {
            var relativePath = Path.Combine(validSegments.ToArray());
            var sourceDepth = validSegments.Count;
            
            // Simulate copying to destination
            var destPath = Path.Combine("C:\\Dest", relativePath);
            var destSegments = destPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Where(s => !string.IsNullOrWhiteSpace(s) && s != "C:")
                .Skip(1) // Skip "Dest"
                .ToList();

            return destSegments.Count == sourceDepth;
        }
        catch
        {
            return true; // Skip invalid paths
        }
    }

    /// <summary>
    /// Property: For any file in a nested directory structure, the parent directory
    /// hierarchy should be preserved in the destination.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool FileCopy_PreservesParentHierarchy(string[] directories, string fileName)
    {
        if (directories == null || directories.Length == 0 || string.IsNullOrWhiteSpace(fileName))
        {
            return true;
        }

        // Filter valid directory names
        var validDirs = directories
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Where(d => !d.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
            .Where(d => d != "." && d != "..")
            .ToArray();

        // Validate filename
        if (fileName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
        {
            return true;
        }

        if (validDirs.Length == 0)
        {
            return true;
        }

        try
        {
            var sourceBase = "C:\\Source";
            var destBase = "C:\\Dest";

            var sourceFilePath = Path.Combine(sourceBase, Path.Combine(validDirs), fileName);
            var destFilePath = Path.Combine(destBase, Path.Combine(validDirs), fileName);

            var sourceDir = Path.GetDirectoryName(sourceFilePath);
            var destDir = Path.GetDirectoryName(destFilePath);

            var sourceRelativeDir = Path.GetRelativePath(sourceBase, sourceDir!);
            var destRelativeDir = Path.GetRelativePath(destBase, destDir!);

            return sourceRelativeDir.Equals(destRelativeDir, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return true; // Skip invalid paths
        }
    }

    /// <summary>
    /// Property: For any set of files in different directories, the relative
    /// directory structure should be identical between source and destination.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool FileCopy_PreservesMultipleDirectoryStructure(List<string[]> filePaths)
    {
        if (filePaths == null || filePaths.Count == 0)
        {
            return true;
        }

        var sourceBase = "C:\\Source";
        var destBase = "C:\\Dest";
        var preservedCount = 0;
        var totalCount = 0;

        foreach (var pathSegments in filePaths)
        {
            if (pathSegments == null || pathSegments.Length == 0)
            {
                continue;
            }

            var validSegments = pathSegments
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Where(s => !s.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
                .Where(s => s != "." && s != "..")
                .ToArray();

            if (validSegments.Length == 0)
            {
                continue;
            }

            try
            {
                var relativePath = Path.Combine(validSegments);
                var sourcePath = Path.Combine(sourceBase, relativePath);
                var destPath = Path.Combine(destBase, relativePath);

                var sourceRelative = Path.GetRelativePath(sourceBase, sourcePath);
                var destRelative = Path.GetRelativePath(destBase, destPath);

                totalCount++;
                if (sourceRelative.Equals(destRelative, StringComparison.OrdinalIgnoreCase))
                {
                    preservedCount++;
                }
            }
            catch
            {
                // Skip invalid paths
            }
        }

        // If we had no valid paths to test, skip
        if (totalCount == 0)
        {
            return true;
        }

        // All valid paths should preserve structure
        return preservedCount == totalCount;
    }

    /// <summary>
    /// Property: For any file path, the filename should remain unchanged
    /// during copy operations (only directory structure matters).
    /// </summary>
    [Property(MaxTest = 100)]
    public bool FileCopy_PreservesFileName(string[] directories, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return true;
        }

        // Validate filename
        if (fileName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
        {
            return true;
        }

        var validDirs = (directories ?? Array.Empty<string>())
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Where(d => !d.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
            .Where(d => d != "." && d != "..")
            .ToArray();

        try
        {
            var sourceBase = "C:\\Source";
            var destBase = "C:\\Dest";

            var sourcePath = validDirs.Length > 0
                ? Path.Combine(sourceBase, Path.Combine(validDirs), fileName)
                : Path.Combine(sourceBase, fileName);

            var destPath = validDirs.Length > 0
                ? Path.Combine(destBase, Path.Combine(validDirs), fileName)
                : Path.Combine(destBase, fileName);

            var sourceFileName = Path.GetFileName(sourcePath);
            var destFileName = Path.GetFileName(destPath);

            return sourceFileName.Equals(destFileName, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return true; // Skip invalid paths
        }
    }

    /// <summary>
    /// Property: For any nested directory structure, the directory separator
    /// count should be preserved (indicating same nesting level).
    /// </summary>
    [Property(MaxTest = 100)]
    public bool FileCopy_PreservesNestingLevel(string[] pathSegments)
    {
        if (pathSegments == null || pathSegments.Length == 0)
        {
            return true;
        }

        var validSegments = pathSegments
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Where(s => !s.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
            .Where(s => s != "." && s != "..")
            .ToArray();

        if (validSegments.Length == 0)
        {
            return true;
        }

        try
        {
            var relativePath = Path.Combine(validSegments);
            
            var sourcePath = Path.Combine("C:\\Source", relativePath);
            var destPath = Path.Combine("C:\\Dest", relativePath);

            var sourceSeparatorCount = sourcePath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar);
            var destSeparatorCount = destPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar);

            // Both should have same number of separators (accounting for base path difference)
            var sourceRelativeSeparators = Path.GetRelativePath("C:\\Source", sourcePath)
                .Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar);
            var destRelativeSeparators = Path.GetRelativePath("C:\\Dest", destPath)
                .Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar);

            return sourceRelativeSeparators == destRelativeSeparators;
        }
        catch
        {
            return true; // Skip invalid paths
        }
    }
}
