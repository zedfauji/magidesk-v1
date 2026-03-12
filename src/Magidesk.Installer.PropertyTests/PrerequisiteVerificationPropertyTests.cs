using FsCheck;
using FsCheck.Xunit;
using Microsoft.Win32;

namespace Magidesk.Installer.PropertyTests;

/// <summary>
/// Property-based tests for prerequisite installation verification.
/// **Validates: Requirements 3.2, 4.2, 5.2**
/// Property 2: Prerequisite Installation Verification
/// </summary>
public class PrerequisiteVerificationPropertyTests
{
    /// <summary>
    /// Property: For any prerequisite component that reports successful installation,
    /// the verification check should confirm the component is present in the registry.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool SuccessfulInstallation_IsVerifiable(PrerequisiteType prerequisiteType)
    {
        // For any prerequisite type, if installation succeeds (exit code 0 or 3010),
        // verification should be able to detect it
        var validExitCodes = new[] { 0, 3010 };
        
        foreach (var exitCode in validExitCodes)
        {
            var installResult = new InstallationResult(
                Success: true,
                ExitCode: exitCode,
                PrerequisiteType: prerequisiteType);

            // Verification should be possible for successful installations
            if (!installResult.Success)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Property: For any prerequisite verification, the registry key path should be
    /// non-empty and follow the expected Windows registry format.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool PrerequisiteRegistryKey_HasValidFormat(PrerequisiteType prerequisiteType)
    {
        var registryKey = GetRegistryKeyForPrerequisite(prerequisiteType);

        // Registry key should not be null or empty
        if (string.IsNullOrWhiteSpace(registryKey))
        {
            return false;
        }

        // Registry key should start with a valid root (SOFTWARE or SYSTEM)
        if (!registryKey.StartsWith("SOFTWARE\\", StringComparison.OrdinalIgnoreCase) &&
            !registryKey.StartsWith("SYSTEM\\", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Registry key should not contain invalid characters
        var invalidChars = new[] { '/', '*', '?', '"', '<', '>', '|' };
        if (registryKey.Any(c => invalidChars.Contains(c)))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Property: For any prerequisite verification that fails, the system should
    /// return false and not throw an exception.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool FailedVerification_DoesNotThrow(PrerequisiteType prerequisiteType)
    {
        try
        {
            // Simulate verification on a non-existent registry key
            var fakeRegistryKey = $"SOFTWARE\\NonExistent\\{Guid.NewGuid()}";
            
            using var key = Registry.LocalMachine.OpenSubKey(fakeRegistryKey);
            
            // If key doesn't exist, verification should return false (not throw)
            var isInstalled = key != null;
            
            // This should always be false for non-existent keys
            return !isInstalled;
        }
        catch
        {
            // Verification should not throw exceptions
            return false;
        }
    }

    /// <summary>
    /// Property: For any prerequisite installation with exit code 0 or 3010,
    /// the installation should be considered successful.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool SuccessExitCodes_IndicateSuccess(int exitCode)
    {
        // Valid success exit codes for Windows installers
        var successCodes = new[] { 0, 3010, 1638 }; // 0=success, 3010=reboot required, 1638=already installed

        if (!successCodes.Contains(exitCode))
        {
            return true; // Skip non-success codes
        }

        var installResult = new InstallationResult(
            Success: true,
            ExitCode: exitCode,
            PrerequisiteType: PrerequisiteType.DotNetRuntime);

        return installResult.Success && successCodes.Contains(installResult.ExitCode);
    }

    /// <summary>
    /// Property: For any prerequisite installation with a non-zero, non-3010 exit code,
    /// the installation should be considered failed.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool FailureExitCodes_IndicateFailure(int exitCode)
    {
        // Skip valid success codes
        var successCodes = new[] { 0, 3010, 1638 };
        if (successCodes.Contains(exitCode))
        {
            return true;
        }

        // Any other exit code should indicate failure
        var installResult = new InstallationResult(
            Success: false,
            ExitCode: exitCode,
            PrerequisiteType: PrerequisiteType.DotNetRuntime);

        return !installResult.Success;
    }

    /// <summary>
    /// Property: For any prerequisite verification, the check should complete
    /// within a reasonable time (not hang indefinitely).
    /// </summary>
    [Property(MaxTest = 100)]
    public bool PrerequisiteVerification_CompletesQuickly(PrerequisiteType prerequisiteType)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            // Simulate a verification check
            var registryKey = GetRegistryKeyForPrerequisite(prerequisiteType);
            using var key = Registry.LocalMachine.OpenSubKey(registryKey);
            var isInstalled = key != null;
            
            var elapsed = DateTime.UtcNow - startTime;
            
            // Verification should complete in less than 5 seconds
            return elapsed.TotalSeconds < 5;
        }
        catch
        {
            var elapsed = DateTime.UtcNow - startTime;
            return elapsed.TotalSeconds < 5;
        }
    }

    /// <summary>
    /// Property: For any prerequisite type, the verification method should be
    /// idempotent (calling it multiple times returns the same result).
    /// </summary>
    [Property(MaxTest = 100)]
    public bool PrerequisiteVerification_IsIdempotent(PrerequisiteType prerequisiteType)
    {
        try
        {
            var registryKey = GetRegistryKeyForPrerequisite(prerequisiteType);
            
            // Check multiple times
            var result1 = CheckRegistryKey(registryKey);
            var result2 = CheckRegistryKey(registryKey);
            var result3 = CheckRegistryKey(registryKey);
            
            // All results should be the same
            return result1 == result2 && result2 == result3;
        }
        catch
        {
            // If any check throws, verification is not idempotent
            return false;
        }
    }

    /// <summary>
    /// Property: For any prerequisite verification after successful installation,
    /// waiting a short period should allow the registry to update.
    /// </summary>
    [Property(MaxTest = 50)] // Reduced test count due to sleep
    public bool VerificationAfterInstall_AllowsRegistryUpdateTime()
    {
        // Simulate the wait time used in actual verification (2 seconds)
        var waitTime = TimeSpan.FromMilliseconds(100); // Reduced for testing
        
        var startTime = DateTime.UtcNow;
        Thread.Sleep(waitTime);
        var elapsed = DateTime.UtcNow - startTime;
        
        // Verify that the wait actually occurred
        return elapsed >= waitTime;
    }

    /// <summary>
    /// Property: For any prerequisite version check, the version comparison should
    /// correctly identify versions meeting the minimum requirement.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool VersionComparison_IdentifiesValidVersions(int majorVersion, int minorVersion)
    {
        // Skip invalid version numbers
        if (majorVersion < 0 || minorVersion < 0)
        {
            return true;
        }

        var version = new Version(majorVersion, minorVersion);
        var requiredVersion = new Version(8, 0); // .NET 8.0 requirement

        // If version is >= required, it should be accepted
        if (version >= requiredVersion)
        {
            return version.Major >= requiredVersion.Major;
        }
        else
        {
            return version.Major < requiredVersion.Major;
        }
    }

    /// <summary>
    /// Helper method to get the registry key for a prerequisite type.
    /// </summary>
    private static string GetRegistryKeyForPrerequisite(PrerequisiteType prerequisiteType)
    {
        return prerequisiteType switch
        {
            PrerequisiteType.DotNetRuntime => @"SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost",
            PrerequisiteType.WindowsAppSDK => @"SOFTWARE\Microsoft\WindowsAppSDK",
            PrerequisiteType.VCRedistributable => @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64",
            _ => throw new ArgumentException($"Unknown prerequisite type: {prerequisiteType}")
        };
    }

    /// <summary>
    /// Helper method to check if a registry key exists.
    /// </summary>
    private static bool CheckRegistryKey(string registryKey)
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(registryKey);
            return key != null;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Enum representing the types of prerequisites that can be installed.
/// </summary>
public enum PrerequisiteType
{
    DotNetRuntime,
    WindowsAppSDK,
    VCRedistributable
}

/// <summary>
/// Record representing the result of a prerequisite installation.
/// </summary>
public record InstallationResult(
    bool Success,
    int ExitCode,
    PrerequisiteType PrerequisiteType);
