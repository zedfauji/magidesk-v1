using Microsoft.Win32;
using System.Diagnostics;
using WixToolset.Dtf.WindowsInstaller;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Custom action for Windows App SDK 1.6 detection and installation.
/// Checks for existing installation and installs MSIX silently if missing.
/// </summary>
public static class WindowsAppSdkInstaller
{
    private const string WindowsAppSdkRegistryKey = @"SOFTWARE\Microsoft\WindowsAppRuntime";
    private const string WindowsAppSdkPackageFamilyName = "Microsoft.WindowsAppRuntime.1.6";
    private const int RequiredMajorVersion = 1;
    private const int RequiredMinorVersion = 6;

    /// <summary>
    /// Custom action entry point for Windows App SDK detection.
    /// Checks if Windows App SDK 1.6 or later is already installed.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [CustomAction]
    public static ActionResult DetectWindowsAppSdk(Session session)
    {
        session.Log("Begin DetectWindowsAppSdk");

        try
        {
            bool isInstalled = IsWindowsAppSdk16Installed(session);

            if (isInstalled)
            {
                session.Log("SUCCESS: Windows App SDK 1.6 or later is already installed");
                session["WINDOWSAPPSDK_INSTALLED"] = "1";
                return ActionResult.Success;
            }
            else
            {
                session.Log("INFO: Windows App SDK 1.6 or later is not installed");
                session["WINDOWSAPPSDK_INSTALLED"] = "0";
                return ActionResult.Success;
            }
        }
        catch (Exception ex)
        {
            session.Log($"ERROR: Exception during Windows App SDK detection: {ex.Message}");
            session.Log($"Stack trace: {ex.StackTrace}");
            // Don't fail on detection error - assume not installed
            session["WINDOWSAPPSDK_INSTALLED"] = "0";
            return ActionResult.Success;
        }
    }

    /// <summary>
    /// Custom action entry point for Windows App SDK installation.
    /// Installs Windows App SDK 1.6 MSIX silently from the bundle.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [CustomAction]
    public static ActionResult InstallWindowsAppSdk(Session session)
    {
        session.Log("Begin InstallWindowsAppSdk");

        try
        {
            // Check if already installed (skip installation)
            string installedFlag = session["WINDOWSAPPSDK_INSTALLED"];
            if (installedFlag == "1")
            {
                session.Log("INFO: Windows App SDK 1.6 or later is already installed, skipping installation");
                return ActionResult.Success;
            }

            // Get the path to the bundled Windows App SDK MSIX
            string msixPath = GetWindowsAppSdkMsixPath(session);
            if (string.IsNullOrEmpty(msixPath) || !File.Exists(msixPath))
            {
                session.Log($"ERROR: Windows App SDK MSIX not found at path: {msixPath}");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = "The Windows App SDK 1.6 installer could not be found in the installation bundle. " +
                                     "The installation cannot continue."
                    });
                return ActionResult.Failure;
            }

            session.Log($"INFO: Installing Windows App SDK 1.6 from: {msixPath}");

            // Install MSIX using PowerShell Add-AppxPackage
            bool installSuccess = InstallMsixPackage(session, msixPath);

            if (installSuccess)
            {
                session.Log("SUCCESS: Windows App SDK 1.6 installed successfully");
                
                // Verify installation succeeded
                if (VerifyInstallation(session))
                {
                    session.Log("SUCCESS: Windows App SDK 1.6 installation verified");
                    return ActionResult.Success;
                }
                else
                {
                    session.Log("ERROR: Windows App SDK 1.6 installation verification failed");
                    session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                        new Record
                        {
                            FormatString = "The Windows App SDK 1.6 installer completed, but the runtime could not be detected. " +
                                         "Please check the installation log for details."
                        });
                    return ActionResult.Failure;
                }
            }
            else
            {
                session.Log("ERROR: Windows App SDK MSIX installation failed");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = "The Windows App SDK 1.6 installation failed. " +
                                     "Please check the installation log for details."
                    });
                return ActionResult.Failure;
            }
        }
        catch (Exception ex)
        {
            session.Log($"ERROR: Exception during Windows App SDK installation: {ex.Message}");
            session.Log($"Stack trace: {ex.StackTrace}");
            session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                new Record
                {
                    FormatString = $"An error occurred while installing Windows App SDK 1.6: {ex.Message}"
                });
            return ActionResult.Failure;
        }
    }

    /// <summary>
    /// Checks if Windows App SDK 1.6 or later is installed.
    /// Uses PowerShell to query installed AppX packages.
    /// </summary>
    /// <param name="session">WiX installer session for logging</param>
    /// <returns>True if Windows App SDK 1.6+ is installed, false otherwise</returns>
    private static bool IsWindowsAppSdk16Installed(Session session)
    {
        try
        {
            // Use PowerShell to check for installed AppX package
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -NonInteractive -Command \"Get-AppxPackage -Name '{WindowsAppSdkPackageFamilyName}*' | Select-Object -ExpandProperty Version\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                session.Log("WARNING: Failed to start PowerShell process for Windows App SDK detection");
                return false;
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                session.Log($"WARNING: PowerShell stderr during detection: {error}");
            }

            if (string.IsNullOrWhiteSpace(output))
            {
                session.Log("INFO: Windows App SDK package not found");
                return false;
            }

            // Parse version string (format: "1.6.240829007.0")
            string versionString = output.Trim();
            session.Log($"INFO: Found Windows App SDK version: {versionString}");

            if (Version.TryParse(versionString, out var version))
            {
                if (version.Major > RequiredMajorVersion || 
                    (version.Major == RequiredMajorVersion && version.Minor >= RequiredMinorVersion))
                {
                    session.Log($"INFO: Windows App SDK version {version.Major}.{version.Minor} meets requirement (>= {RequiredMajorVersion}.{RequiredMinorVersion})");
                    return true;
                }
                else
                {
                    session.Log($"INFO: Windows App SDK version {version.Major}.{version.Minor} is older than required version {RequiredMajorVersion}.{RequiredMinorVersion}");
                    return false;
                }
            }
            else
            {
                session.Log($"WARNING: Could not parse Windows App SDK version string: {versionString}");
                return false;
            }
        }
        catch (Exception ex)
        {
            session.Log($"WARNING: Exception while checking Windows App SDK installation: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Installs an MSIX package using PowerShell Add-AppxPackage.
    /// </summary>
    /// <param name="session">WiX installer session for logging</param>
    /// <param name="msixPath">Path to the MSIX package file</param>
    /// <returns>True if installation succeeded, false otherwise</returns>
    private static bool InstallMsixPackage(Session session, string msixPath)
    {
        try
        {
            session.Log($"INFO: Installing MSIX package: {msixPath}");

            // Use PowerShell Add-AppxPackage to install silently
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -NonInteractive -Command \"Add-AppxPackage -Path '{msixPath}' -ErrorAction Stop\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                Verb = "runas" // Run with elevated privileges
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                session.Log("ERROR: Failed to start PowerShell process for MSIX installation");
                return false;
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            int exitCode = process.ExitCode;
            session.Log($"INFO: PowerShell Add-AppxPackage exit code: {exitCode}");

            if (!string.IsNullOrEmpty(output))
            {
                session.Log($"INFO: PowerShell output: {output}");
            }

            if (!string.IsNullOrEmpty(error))
            {
                session.Log($"WARNING: PowerShell stderr: {error}");
            }

            // Exit code 0 indicates success
            if (exitCode == 0)
            {
                session.Log("SUCCESS: MSIX package installed successfully");
                return true;
            }
            else
            {
                session.Log($"ERROR: MSIX installation failed with exit code: {exitCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            session.Log($"ERROR: Exception during MSIX installation: {ex.Message}");
            session.Log($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// Verifies that Windows App SDK 1.6 installation succeeded.
    /// </summary>
    /// <param name="session">WiX installer session for logging</param>
    /// <returns>True if verification succeeded, false otherwise</returns>
    private static bool VerifyInstallation(Session session)
    {
        session.Log("INFO: Verifying Windows App SDK 1.6 installation");
        
        // Wait a moment for package registration to complete
        System.Threading.Thread.Sleep(2000);
        
        return IsWindowsAppSdk16Installed(session);
    }

    /// <summary>
    /// Gets the path to the bundled Windows App SDK MSIX.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>Path to the MSIX file</returns>
    private static string GetWindowsAppSdkMsixPath(Session session)
    {
        // Try to get from session property (set by WiX)
        string? msixPath = session["WINDOWSAPPSDK_INSTALLER_PATH"];
        
        if (!string.IsNullOrEmpty(msixPath) && File.Exists(msixPath))
        {
            return msixPath;
        }

        // Fallback: look in common locations relative to installer
        string? installerDir = Path.GetDirectoryName(session["OriginalDatabase"]);
        if (!string.IsNullOrEmpty(installerDir))
        {
            // Check in Prerequisites subfolder
            string prerequisitesPath = Path.Combine(installerDir, "Prerequisites", "Microsoft.WindowsAppRuntime.1.6.msix");
            if (File.Exists(prerequisitesPath))
            {
                return prerequisitesPath;
            }

            // Check in same directory as installer
            string sameDirPath = Path.Combine(installerDir, "Microsoft.WindowsAppRuntime.1.6.msix");
            if (File.Exists(sameDirPath))
            {
                return sameDirPath;
            }
        }

        session.Log("WARNING: Could not locate Windows App SDK MSIX");
        return string.Empty;
    }
}
