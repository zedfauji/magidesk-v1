using Microsoft.Win32;
using System.Diagnostics;
using WixToolset.Dtf.WindowsInstaller;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Custom action for .NET 8 Desktop Runtime detection and installation.
/// Checks registry for existing installation and installs silently if missing.
/// </summary>
public static class DotNetRuntimeInstaller
{
    private const string DotNetRegistryKey = @"SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost";
    private const string DotNetVersionKey = "Version";
    private const int RequiredMajorVersion = 8;

    /// <summary>
    /// Custom action entry point for .NET 8 Runtime detection.
    /// Checks if .NET 8 Desktop Runtime x64 is already installed.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [CustomAction]
    public static ActionResult DetectDotNetRuntime(Session session)
    {
        session.Log("Begin DetectDotNetRuntime");

        try
        {
            bool isInstalled = IsDotNet8Installed(session);

            if (isInstalled)
            {
                session.Log("SUCCESS: .NET 8 Desktop Runtime x64 is already installed");
                session["DOTNET8_INSTALLED"] = "1";
                return ActionResult.Success;
            }
            else
            {
                session.Log("INFO: .NET 8 Desktop Runtime x64 is not installed");
                session["DOTNET8_INSTALLED"] = "0";
                return ActionResult.Success;
            }
        }
        catch (Exception ex)
        {
            session.Log($"ERROR: Exception during .NET Runtime detection: {ex.Message}");
            session.Log($"Stack trace: {ex.StackTrace}");
            // Don't fail on detection error - assume not installed
            session["DOTNET8_INSTALLED"] = "0";
            return ActionResult.Success;
        }
    }

    /// <summary>
    /// Custom action entry point for .NET 8 Runtime installation.
    /// Installs .NET 8 Desktop Runtime x64 silently from the bundle.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [CustomAction]
    public static ActionResult InstallDotNetRuntime(Session session)
    {
        session.Log("Begin InstallDotNetRuntime");

        try
        {
            // Check if already installed (skip installation)
            string installedFlag = session["DOTNET8_INSTALLED"];
            if (installedFlag == "1")
            {
                session.Log("INFO: .NET 8 Desktop Runtime x64 is already installed, skipping installation");
                return ActionResult.Success;
            }

            // Get the path to the bundled .NET Runtime installer
            string installerPath = GetDotNetInstallerPath(session);
            if (string.IsNullOrEmpty(installerPath) || !File.Exists(installerPath))
            {
                session.Log($"ERROR: .NET Runtime installer not found at path: {installerPath}");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = "The .NET 8 Desktop Runtime installer could not be found in the installation bundle. " +
                                     "The installation cannot continue."
                    });
                return ActionResult.Failure;
            }

            session.Log($"INFO: Installing .NET 8 Desktop Runtime from: {installerPath}");

            // Execute the installer silently
            var startInfo = new ProcessStartInfo
            {
                FileName = installerPath,
                Arguments = "/install /quiet /norestart",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                session.Log("ERROR: Failed to start .NET Runtime installer process");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = "Failed to start the .NET 8 Desktop Runtime installer. " +
                                     "The installation cannot continue."
                    });
                return ActionResult.Failure;
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            int exitCode = process.ExitCode;
            session.Log($"INFO: .NET Runtime installer exit code: {exitCode}");

            if (!string.IsNullOrEmpty(output))
            {
                session.Log($"INFO: Installer output: {output}");
            }

            if (!string.IsNullOrEmpty(error))
            {
                session.Log($"WARNING: Installer stderr: {error}");
            }

            // Check exit code (0 = success, 3010 = success with reboot required)
            if (exitCode == 0 || exitCode == 3010)
            {
                session.Log($"SUCCESS: .NET 8 Desktop Runtime installed successfully (exit code: {exitCode})");
                
                // Verify installation succeeded
                if (VerifyInstallation(session))
                {
                    session.Log("SUCCESS: .NET 8 Desktop Runtime installation verified");
                    return ActionResult.Success;
                }
                else
                {
                    session.Log("ERROR: .NET 8 Desktop Runtime installation verification failed");
                    session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                        new Record
                        {
                            FormatString = "The .NET 8 Desktop Runtime installer completed, but the runtime could not be detected. " +
                                         "Please check the installation log for details."
                        });
                    return ActionResult.Failure;
                }
            }
            else
            {
                session.Log($"ERROR: .NET Runtime installer failed with exit code: {exitCode}");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = $"The .NET 8 Desktop Runtime installation failed with error code {exitCode}. " +
                                     "Please check the installation log for details."
                    });
                return ActionResult.Failure;
            }
        }
        catch (Exception ex)
        {
            session.Log($"ERROR: Exception during .NET Runtime installation: {ex.Message}");
            session.Log($"Stack trace: {ex.StackTrace}");
            session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                new Record
                {
                    FormatString = $"An error occurred while installing .NET 8 Desktop Runtime: {ex.Message}"
                });
            return ActionResult.Failure;
        }
    }

    /// <summary>
    /// Checks if .NET 8 or later is installed by querying the registry.
    /// </summary>
    /// <param name="session">WiX installer session for logging</param>
    /// <returns>True if .NET 8+ is installed, false otherwise</returns>
    private static bool IsDotNet8Installed(Session session)
    {
        try
        {
            // Check the shared host registry key
            using var key = Registry.LocalMachine.OpenSubKey(DotNetRegistryKey);
            if (key == null)
            {
                session.Log("INFO: .NET shared host registry key not found");
                return false;
            }

            var versionValue = key.GetValue(DotNetVersionKey);
            if (versionValue == null)
            {
                session.Log("INFO: .NET version value not found in registry");
                return false;
            }

            string versionString = versionValue.ToString() ?? string.Empty;
            session.Log($"INFO: Found .NET version in registry: {versionString}");

            // Parse version string (format: "8.0.x")
            if (Version.TryParse(versionString, out var version))
            {
                if (version.Major >= RequiredMajorVersion)
                {
                    session.Log($"INFO: .NET version {version.Major}.{version.Minor} meets requirement (>= {RequiredMajorVersion}.0)");
                    return true;
                }
                else
                {
                    session.Log($"INFO: .NET version {version.Major}.{version.Minor} is older than required version {RequiredMajorVersion}.0");
                    return false;
                }
            }
            else
            {
                session.Log($"WARNING: Could not parse .NET version string: {versionString}");
                return false;
            }
        }
        catch (Exception ex)
        {
            session.Log($"WARNING: Exception while checking .NET installation: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Verifies that .NET 8 installation succeeded by checking the registry again.
    /// </summary>
    /// <param name="session">WiX installer session for logging</param>
    /// <returns>True if verification succeeded, false otherwise</returns>
    private static bool VerifyInstallation(Session session)
    {
        session.Log("INFO: Verifying .NET 8 Desktop Runtime installation");
        
        // Wait a moment for registry to update
        System.Threading.Thread.Sleep(2000);
        
        return IsDotNet8Installed(session);
    }

    /// <summary>
    /// Gets the path to the bundled .NET Runtime installer.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>Path to the installer executable</returns>
    private static string GetDotNetInstallerPath(Session session)
    {
        // Try to get from session property (set by WiX)
        string? installerPath = session["DOTNET_INSTALLER_PATH"];
        
        if (!string.IsNullOrEmpty(installerPath) && File.Exists(installerPath))
        {
            return installerPath;
        }

        // Fallback: look in common locations relative to installer
        string? installerDir = Path.GetDirectoryName(session["OriginalDatabase"]);
        if (!string.IsNullOrEmpty(installerDir))
        {
            // Check in Prerequisites subfolder
            string prerequisitesPath = Path.Combine(installerDir, "Prerequisites", "windowsdesktop-runtime-8.0-win-x64.exe");
            if (File.Exists(prerequisitesPath))
            {
                return prerequisitesPath;
            }

            // Check in same directory as installer
            string sameDirPath = Path.Combine(installerDir, "windowsdesktop-runtime-8.0-win-x64.exe");
            if (File.Exists(sameDirPath))
            {
                return sameDirPath;
            }
        }

        session.Log("WARNING: Could not locate .NET Runtime installer");
        return string.Empty;
    }
}
