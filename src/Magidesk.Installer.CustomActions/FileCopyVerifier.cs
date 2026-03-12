using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WixToolset.Dtf.WindowsInstaller;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Verifies that critical application files were copied successfully during installation.
/// WiX handles the actual file copy operations; this custom action validates the results.
/// </summary>
public class FileCopyVerifier
{
    /// <summary>
    /// Critical files that must exist for the application to function.
    /// If any of these files are missing, the installation must fail.
    /// </summary>
    private static readonly string[] CriticalFiles = new[]
    {
        "Magidesk.exe",
        "efbundle.exe",
        "appsettings.json"
    };

    /// <summary>
    /// Verifies that all critical files exist in the installation directory.
    /// </summary>
    /// <param name="installDirectory">The installation directory path.</param>
    /// <returns>Result indicating success or failure with details.</returns>
    public FileCopyVerificationResult VerifyCriticalFiles(string installDirectory)
    {
        if (string.IsNullOrWhiteSpace(installDirectory))
        {
            return new FileCopyVerificationResult(
                false,
                "Installation directory path is null or empty",
                Array.Empty<string>());
        }

        if (!Directory.Exists(installDirectory))
        {
            return new FileCopyVerificationResult(
                false,
                $"Installation directory does not exist: {installDirectory}",
                Array.Empty<string>());
        }

        var missingFiles = new List<string>();

        foreach (var fileName in CriticalFiles)
        {
            var filePath = Path.Combine(installDirectory, fileName);
            if (!File.Exists(filePath))
            {
                missingFiles.Add(filePath);
            }
        }

        if (missingFiles.Any())
        {
            var errorMessage = $"Critical files are missing from installation directory. " +
                             $"Missing files: {string.Join(", ", missingFiles.Select(Path.GetFileName))}";
            
            return new FileCopyVerificationResult(false, errorMessage, missingFiles.ToArray());
        }

        return new FileCopyVerificationResult(true, "All critical files verified", Array.Empty<string>());
    }

    /// <summary>
    /// WiX custom action entry point that verifies critical files after file copy.
    /// This action should be scheduled after InstallFiles in the InstallExecuteSequence.
    /// </summary>
    [CustomAction]
    public static ActionResult VerifyFileCopy(Session session)
    {
        try
        {
            session.Log("Begin VerifyFileCopy custom action");

            var installFolder = session["INSTALLFOLDER"];
            
            if (string.IsNullOrWhiteSpace(installFolder))
            {
                session.Log("ERROR: INSTALLFOLDER property is not set");
                session.Message(InstallMessage.Error, new Record
                {
                    FormatString = "Installation failed: Unable to determine installation directory. " +
                                 "Please check the installation log for details."
                });
                return ActionResult.Failure;
            }

            session.Log($"Verifying critical files in: {installFolder}");

            var verifier = new FileCopyVerifier();
            var result = verifier.VerifyCriticalFiles(installFolder);

            if (!result.Success)
            {
                session.Log($"ERROR: File copy verification failed: {result.ErrorMessage}");
                
                foreach (var missingFile in result.MissingFiles)
                {
                    session.Log($"ERROR: Missing critical file: {missingFile}");
                }

                var errorRecord = new Record
                {
                    FormatString = $"Installation failed: {result.ErrorMessage}\n\n" +
                                 "The installer was unable to copy all required files. " +
                                 "This may be caused by:\n" +
                                 "• Insufficient disk space\n" +
                                 "• Antivirus software blocking file operations\n" +
                                 "• Insufficient permissions\n\n" +
                                 $"Installation log: {session["MsiLogFileLocation"]}"
                };

                session.Message(InstallMessage.Error, errorRecord);
                return ActionResult.Failure;
            }

            session.Log("File copy verification succeeded - all critical files present");
            return ActionResult.Success;
        }
        catch (Exception ex)
        {
            session.Log($"EXCEPTION in VerifyFileCopy: {ex.Message}");
            session.Log($"Stack trace: {ex.StackTrace}");
            
            session.Message(InstallMessage.Error, new Record
            {
                FormatString = $"Installation failed: An error occurred while verifying file copy operations.\n\n" +
                             $"Error: {ex.Message}\n\n" +
                             $"Installation log: {session["MsiLogFileLocation"]}"
            });
            
            return ActionResult.Failure;
        }
    }
}

/// <summary>
/// Result of file copy verification operation.
/// </summary>
public record FileCopyVerificationResult(
    bool Success,
    string ErrorMessage,
    string[] MissingFiles);
