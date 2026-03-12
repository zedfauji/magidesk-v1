using System;
using System.IO;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Manual test scenarios for FileCopyVerifier.
/// These will be moved to a proper xUnit test project in task 25.1.
/// </summary>
internal static class FileCopyVerifierTests
{
    /// <summary>
    /// Test scenario: All critical files present
    /// Expected: Success = true
    /// </summary>
    public static void TestAllFilesPresent()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "MagideskTest_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Create critical files
            File.WriteAllText(Path.Combine(tempDir, "Magidesk.exe"), "test");
            File.WriteAllText(Path.Combine(tempDir, "efbundle.exe"), "test");
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), "test");

            var verifier = new FileCopyVerifier();
            var result = verifier.VerifyCriticalFiles(tempDir);

            if (!result.Success)
            {
                throw new Exception($"Test failed: Expected success but got: {result.ErrorMessage}");
            }

            Console.WriteLine("✓ TestAllFilesPresent passed");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    /// Test scenario: Missing critical file
    /// Expected: Success = false, MissingFiles contains the missing file
    /// </summary>
    public static void TestMissingCriticalFile()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "MagideskTest_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Create only some files
            File.WriteAllText(Path.Combine(tempDir, "Magidesk.exe"), "test");
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), "test");
            // Missing: efbundle.exe

            var verifier = new FileCopyVerifier();
            var result = verifier.VerifyCriticalFiles(tempDir);

            if (result.Success)
            {
                throw new Exception("Test failed: Expected failure but got success");
            }

            if (result.MissingFiles.Length != 1)
            {
                throw new Exception($"Test failed: Expected 1 missing file but got {result.MissingFiles.Length}");
            }

            if (!result.MissingFiles[0].Contains("efbundle.exe"))
            {
                throw new Exception($"Test failed: Expected efbundle.exe to be missing but got {result.MissingFiles[0]}");
            }

            Console.WriteLine("✓ TestMissingCriticalFile passed");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    /// Test scenario: Directory does not exist
    /// Expected: Success = false, appropriate error message
    /// </summary>
    public static void TestDirectoryNotExists()
    {
        var nonExistentDir = Path.Combine(Path.GetTempPath(), "NonExistent_" + Guid.NewGuid());

        var verifier = new FileCopyVerifier();
        var result = verifier.VerifyCriticalFiles(nonExistentDir);

        if (result.Success)
        {
            throw new Exception("Test failed: Expected failure for non-existent directory");
        }

        if (!result.ErrorMessage.Contains("does not exist"))
        {
            throw new Exception($"Test failed: Expected 'does not exist' in error message but got: {result.ErrorMessage}");
        }

        Console.WriteLine("✓ TestDirectoryNotExists passed");
    }

    /// <summary>
    /// Test scenario: Null or empty directory path
    /// Expected: Success = false, appropriate error message
    /// </summary>
    public static void TestNullOrEmptyPath()
    {
        var verifier = new FileCopyVerifier();
        
        var result1 = verifier.VerifyCriticalFiles(null!);
        if (result1.Success)
        {
            throw new Exception("Test failed: Expected failure for null path");
        }

        var result2 = verifier.VerifyCriticalFiles(string.Empty);
        if (result2.Success)
        {
            throw new Exception("Test failed: Expected failure for empty path");
        }

        Console.WriteLine("✓ TestNullOrEmptyPath passed");
    }
}
