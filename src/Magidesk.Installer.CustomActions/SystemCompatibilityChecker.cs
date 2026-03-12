using System.Runtime.InteropServices;
using WixToolset.Dtf.WindowsInstaller;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Custom action for system compatibility checks.
/// Validates OS version, architecture, disk space, and RAM requirements.
/// </summary>
public static class SystemCompatibilityChecker
{
    private const int MinimumWindows10Build = 18362; // Windows 10 1903
    private const long MinimumDiskSpaceBytes = 10L * 1024 * 1024 * 1024; // 10 GB
    private const long WarningRamBytes = 4L * 1024 * 1024 * 1024; // 4 GB

    /// <summary>
    /// Custom action entry point for OS version detection.
    /// Validates Windows version, build number, and architecture.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [CustomAction]
    public static ActionResult CheckOSVersion(Session session)
    {
        session.Log("Begin CheckOSVersion");

        try
        {
            // Check if running on Windows
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                session.Log("ERROR: Operating system is not Windows");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = "Magidesk POS requires Windows 10 (version 1903 or later) or Windows 11. " +
                                     "This operating system is not supported."
                    });
                return ActionResult.Failure;
            }

            // Check architecture
            if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
            {
                session.Log($"ERROR: Architecture is {RuntimeInformation.ProcessArchitecture}, but x64 is required");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = $"Magidesk POS requires x64 (64-bit) architecture. " +
                                     $"This system is running {RuntimeInformation.ProcessArchitecture} architecture."
                    });
                return ActionResult.Failure;
            }

            // Get OS version information
            var osVersion = Environment.OSVersion;
            session.Log($"OS Version: {osVersion.VersionString}");
            session.Log($"Platform: {osVersion.Platform}");
            session.Log($"Version: {osVersion.Version.Major}.{osVersion.Version.Minor}.{osVersion.Version.Build}");

            // Check if Windows 10 or 11 (NT version 10.0)
            if (osVersion.Platform != PlatformID.Win32NT || osVersion.Version.Major < 10)
            {
                session.Log($"ERROR: OS version {osVersion.Version.Major}.{osVersion.Version.Minor} is not Windows 10 or 11");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = "Magidesk POS requires Windows 10 (version 1903 or later) or Windows 11. " +
                                     $"This system is running an unsupported version: {osVersion.VersionString}"
                    });
                return ActionResult.Failure;
            }

            // Check Windows 10 build number (must be >= 18362 for version 1903)
            int buildNumber = osVersion.Version.Build;
            if (buildNumber < MinimumWindows10Build)
            {
                session.Log($"ERROR: Windows 10 build {buildNumber} is earlier than minimum required build {MinimumWindows10Build} (version 1903)");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = "Magidesk POS requires Windows 10 version 1903 (build 18362) or later. " +
                                     $"This system is running Windows 10 build {buildNumber}, which is not supported. " +
                                     "Please update Windows to version 1903 or later."
                    });
                return ActionResult.Failure;
            }

            session.Log($"SUCCESS: OS version check passed - Windows {osVersion.Version.Major}.{osVersion.Version.Minor} build {buildNumber}, x64 architecture");
            return ActionResult.Success;
        }
        catch (Exception ex)
        {
            session.Log($"ERROR: Exception during OS version check: {ex.Message}");
            session.Log($"Stack trace: {ex.StackTrace}");
            session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                new Record
                {
                    FormatString = $"An error occurred while checking system compatibility: {ex.Message}"
                });
            return ActionResult.Failure;
        }
    }

    /// <summary>
    /// Custom action entry point for disk space and RAM checks.
    /// Validates available disk space and system RAM.
    /// </summary>
    /// <param name="session">WiX installer session</param>
    /// <returns>ActionResult indicating success or failure</returns>
    [CustomAction]
    public static ActionResult CheckSystemResources(Session session)
    {
        session.Log("Begin CheckSystemResources");

        try
        {
            // Get installation drive (default to C:)
            string installDrive = session["INSTALLFOLDER"];
            if (string.IsNullOrEmpty(installDrive) || installDrive.Length < 2)
            {
                installDrive = "C:\\";
            }
            else
            {
                // Extract drive letter from install path
                installDrive = Path.GetPathRoot(installDrive) ?? "C:\\";
            }

            session.Log($"Checking disk space on drive: {installDrive}");

            // Check available disk space
            var driveInfo = new DriveInfo(installDrive);
            long availableSpace = driveInfo.AvailableFreeSpace;
            double availableSpaceGB = availableSpace / (1024.0 * 1024.0 * 1024.0);

            session.Log($"Available disk space: {availableSpaceGB:F2} GB ({availableSpace} bytes)");

            if (availableSpace < MinimumDiskSpaceBytes)
            {
                double requiredGB = MinimumDiskSpaceBytes / (1024.0 * 1024.0 * 1024.0);
                session.Log($"ERROR: Insufficient disk space. Required: {requiredGB:F2} GB, Available: {availableSpaceGB:F2} GB");
                session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                    new Record
                    {
                        FormatString = $"Magidesk POS requires at least 10 GB of available disk space. " +
                                     $"Drive {installDrive} has only {availableSpaceGB:F2} GB available. " +
                                     "Please free up disk space and try again."
                    });
                return ActionResult.Failure;
            }

            session.Log($"SUCCESS: Disk space check passed - {availableSpaceGB:F2} GB available");

            // Check system RAM
            // Note: This requires P/Invoke as .NET doesn't provide a direct API
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                long totalRam = (long)memStatus.ullTotalPhys;
                double totalRamGB = totalRam / (1024.0 * 1024.0 * 1024.0);

                session.Log($"Total system RAM: {totalRamGB:F2} GB ({totalRam} bytes)");

                if (totalRam < WarningRamBytes)
                {
                    double warningGB = WarningRamBytes / (1024.0 * 1024.0 * 1024.0);
                    session.Log($"WARNING: System has less than {warningGB:F2} GB RAM ({totalRamGB:F2} GB detected)");
                    
                    // Display warning but don't fail installation
                    var result = session.Message(InstallMessage.Warning | (InstallMessage)MessageButtons.OKCancel,
                        new Record
                        {
                            FormatString = $"Your system has {totalRamGB:F2} GB of RAM. " +
                                         $"Magidesk POS recommends at least 4 GB of RAM for optimal performance. " +
                                         "You may experience slower performance with less RAM. " +
                                         "Do you want to continue with the installation?"
                        });

                    if (result == MessageResult.Cancel)
                    {
                        session.Log("User cancelled installation due to low RAM warning");
                        return ActionResult.UserExit;
                    }

                    session.Log("User chose to continue despite low RAM warning");
                }
                else
                {
                    session.Log($"SUCCESS: RAM check passed - {totalRamGB:F2} GB available");
                }
            }
            else
            {
                session.Log("WARNING: Could not retrieve system memory information");
            }

            return ActionResult.Success;
        }
        catch (Exception ex)
        {
            session.Log($"ERROR: Exception during system resources check: {ex.Message}");
            session.Log($"Stack trace: {ex.StackTrace}");
            session.Message(InstallMessage.Error | (InstallMessage)MessageButtons.OK,
                new Record
                {
                    FormatString = $"An error occurred while checking system resources: {ex.Message}"
                });
            return ActionResult.Failure;
        }
    }

    #region P/Invoke for Memory Status

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;

        public MEMORYSTATUSEX()
        {
            dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

    #endregion
}
