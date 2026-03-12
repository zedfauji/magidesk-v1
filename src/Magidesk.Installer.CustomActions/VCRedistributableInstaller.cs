using Microsoft.Win32;
using System.Diagnostics;
using WixToolset.Dtf.WindowsInstaller;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Custom action for Visual C++ Redistributable x64 detection and installation.
/// Checks registry for existing installation and installs silently if missing.
/// </summary>
public static class VCRedistributableInstaller
{
    // VC++ 2015-2022 Redistributable uses the same registry key
    // Version 14.0 covers Visual Studio 2015, 2017, 2019, and 2022
    private const string VCRedistRegistryKey = @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64";
    private const string VCRedistInstalledKey = "Installed";
    private const string VCRedistVersionKey = "Version";
    private const int RequiredMajorVersion = 14;

    /// <summary>
    /// Custom action entry point for VC++ Redistributable detection.
    /// Checks if Visual C++ Redistributable x64 is already installed.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [CustomAction]
    public static ActionResult DetectVCRedist(Session session)
    {
        session.Log("Begin DetectVCRedist");

        try
        {
            bool isInstalled = IsVCRedistInstalled(session);

            if (isInstalled)
            {
                session.Log("SUCCESS: Visual C++ Redistributable x64 is already installed");
                session["VCREDIST_INSTALLED"] = "1";
                return ActionResult.Success;
            }
            else
            {
                session.Log("INFO: Visual C++ Redistributable x64 is not installed");
                session["VCREDIST_INSTALLED"] = "0";
                return ActionResult.Success;
            }
        }
        catch (Exception ex)
        {
            session.Log($"ERROR: Exception during VC++ Redistributable detection: {ex.Message}");
            session.Log($"Stack trace: {ex.StackTrace}");
            // Don't fail on detection error - assume not installed
            session["VCREDIST_INSTALLED"] = "0";
            return ActionResult.Success;
        }
    }

    /// <summary>
    /// Custom action entry point for VC++ Redistributable installation.
    /// Installs Visual C++ Redistributable x64 silently from the bundle.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [CustomAction]
    public static ActionResult InstallVCRedist(Session session)
    {
        session.Log("Begin InstallVCRedist");

        try
        {
            // Check if already installed (skip installation)
            string installedFlag = session["VCREDIST_INSTALLED"];
            if (installedFlag == "1")
            {
                session.Log("INFO: Visual C++ Redistributable x64 is already installed, skipping installation");
                return ActionResult.Success;
            }

            // Get the path to the bundled VC++ Redistributable installer
            string installerPath = GetVCRedistInstallerPath(session);
            if (string.IsNullOrEmpty(installerPath) || !File.Exists(installerPath))
            {
                session.Log($"ERROR: VC++ Redistributable installer not found at path: {installerPath}");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = "The Visual C++ Redistributable x64 installer could not be found in the installation bundle. " +
                                     "The installation cannot continue."
                    });
                return ActionResult.Failure;
            }

            session.Log($"INFO: Installing Visual C++ Redistributable x64 from: {installerPath}");

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
                session.Log("ERROR: Failed to start VC++ Redistributable installer process");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = "Failed to start the Visual C++ Redistributable x64 installer. " +
                                     "The installation cannot continue."
                    });
                return ActionResult.Failure;
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            int exitCode = process.ExitCode;
            session.Log($"INFO: VC++ Redistributable installer exit code: {exitCode}");

            if (!string.IsNullOrEmpty(output))
            {
                session.Log($"INFO: Installer output: {output}");
            }

            if (!string.IsNullOrEmpty(error))
            {
                session.Log($"WARNING: Installer stderr: {error}");
            }

            // Check exit code
            // 0 = success
            // 3010 = success with reboot required
            // 1638 = another version already installed (treat as success)
            if (exitCode == 0 || exitCode == 3010 || exitCode == 1638)
            {
                session.Log($"SUCCESS: Visual C++ Redistributable x64 installed successfully (exit code: {exitCode})");
                
                // Verify installation succeeded
                if (VerifyInstallation(session))
                {
                    session.Log("SUCCESS: Visual C++ Redistributable x64 installation verified");
                    return ActionResult.Success;
                }
                else
                {
                    session.Log("ERROR: Visual C++ Redistributable x64 installation verification failed");
                    session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                        new Record
                        {
                            FormatString = "The Visual C++ Redistributable x64 installer completed, but the runtime could not be detected. " +
                                         "Please check the installation log for details."
                        });
                    return ActionResult.Failure;
                }
            }
            else
            {
                session.Log($"ERROR: VC++ Redistributable installer failed with exit code: {exitCode}");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = $"The Visual C++ Redistributable x64 installation failed with error code {exitCode}. " +
                                     "Please check the installation log for details."
                    });
                return ActionResult.Failure;
            }
        }
        catch (Exception ex)
        {
            session.Log($"ERROR: Exception during VC++ Redistributable installation: {ex.Message}");
            session.Log($"Stack trace: {ex.StackTrace}");
            session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                new Record
                {
                    FormatString = $"An error occurred while installing Visual C++ Redistributable x64: {ex.Message}"
                });
            return ActionResult.Failure;
        }
    }

    /// <summary>
    /// Checks if Visual C++ Redistributable x64 is installed by querying the registry.
    /// </summary>
    /// <param name="session">WiX installer session for logging</param>
    /// <returns>True if VC++ Redistributable is installed, false otherwise</returns>
    private static bool IsVCRedistInstalled(Session session)
    {
        try
        {
            // Check the VC++ Runtimes registry key
            using var key = Registry.LocalMachine.OpenSubKey(VCRedistRegistryKey);
            if (key == null)
            {
                session.Log("INFO: VC++ Redistributable registry key not found");
                return false;
            }

            // Check if the "Installed" value is set to 1
            var installedValue = key.GetValue(VCRedistInstalledKey);
            if (installedValue == null)
            {
                session.Log("INFO: VC++ Redistributable 'Installed' value not found in registry");
                return false;
            }

            int installed = Convert.ToInt32(installedValue);
            if (installed != 1)
            {
                session.Log($"INFO: VC++ Redistributable 'Installed' value is {installed} (expected 1)");
                return false;
            }

            // Check version to ensure it's version 14.0 or later
            var versionValue = key.GetValue(VCRedistVersionKey);
            if (versionValue == null)
            {
                session.Log("WARNING: VC++ Redistributable version value not found, but 'Installed' is set");
                // If installed flag is set, assume it's installed even without version
                return true;
            }

            string versionString = versionValue.ToString() ?? string.Empty;
            session.Log($"INFO: Found VC++ Redistributable version in registry: {versionString}");

            // Version format is typically "v14.xx.xxxxx.xx" - extract the major version
            if (versionString.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                versionString = versionString.Substring(1);
            }

            // Parse version string
            if (Version.TryParse(versionString, out var version))
            {
                if (version.Major >= RequiredMajorVersion)
                {
                    session.Log($"INFO: VC++ Redistributable version {version.Major}.{version.Minor} meets requirement (>= {RequiredMajorVersion}.0)");
                    return true;
                }
                else
                {
                    session.Log($"INFO: VC++ Redistributable version {version.Major}.{version.Minor} is older than required version {RequiredMajorVersion}.0");
                    return false;
                }
            }
            else
            {
                session.Log($"WARNING: Could not parse VC++ Redistributable version string: {versionString}");
                // If we can't parse but installed flag is set, assume it's installed
                return true;
            }
        }
        catch (Exception ex)
        {
            session.Log($"WARNING: Exception while checking VC++ Redistributable installation: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Verifies that VC++ Redistributable installation succeeded by checking the registry again.
    /// </summary>
    /// <param name="session">WiX installer session for logging</param>
    /// <returns>True if verification succeeded, false otherwise</returns>
    private static bool VerifyInstallation(Session session)
    {
        session.Log("INFO: Verifying Visual C++ Redistributable x64 installation");
        
        // Wait a moment for registry to update
        Thread.Sleep(2000);
        
        return IsVCRedistInstalled(session);
    }

    /// <summary>
    /// Gets the path to the bundled VC++ Redistributable installer.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>Path to the installer executable</returns>
    private static string GetVCRedistInstallerPath(Session session)
    {
        // Try to get from session property (set by WiX)
        string? installerPath = session["VCREDIST_INSTALLER_PATH"];
        
        if (!string.IsNullOrEmpty(installerPath) && File.Exists(installerPath))
        {
            return installerPath;
        }

        // Fallback: look in common locations relative to installer
        string? installerDir = Path.GetDirectoryName(session["OriginalDatabase"]);
        if (!string.IsNullOrEmpty(installerDir))
        {
            // Check in Prerequisites subfolder
            string prerequisitesPath = Path.Combine(installerDir, "Prerequisites", "VC_redist.x64.exe");
            if (File.Exists(prerequisitesPath))
            {
                return prerequisitesPath;
            }

            // Check in same directory as installer
            string sameDirPath = Path.Combine(installerDir, "VC_redist.x64.exe");
            if (File.Exists(sameDirPath))
            {
                return sameDirPath;
            }
        }

        session.Log("WARNING: Could not locate VC++ Redistributable installer");
        return string.Empty;
    }
}
