# FileCopyVerifier - File Copy Error Handling

## Overview

The `FileCopyVerifier` custom action verifies that critical application files were successfully copied during installation. WiX handles the actual file copy operations automatically; this custom action validates the results and fails the installation if critical files are missing.

## Critical Files

The following files are considered critical and must exist for the application to function:

- `Magidesk.exe` - Main application executable
- `efbundle.exe` - EF Core migration bundle
- `appsettings.json` - Application configuration template

## Implementation Details

### Custom Action Execution

The `CA_VerifyFileCopy` custom action is scheduled in the `InstallExecuteSequence` immediately after the `InstallFiles` action:

```xml
<CustomAction Id="CA_VerifyFileCopy"
              BinaryKey="CustomActions"
              DllEntry="VerifyFileCopy"
              Execute="immediate"
              Return="check" />

<InstallExecuteSequence>
  <Custom Action="CA_VerifyFileCopy" After="InstallFiles">
    NOT Installed
  </Custom>
</InstallExecuteSequence>
```

### Error Handling

If any critical file is missing, the custom action:

1. **Logs the error** with the full file path to the MSI log
2. **Displays an error message** to the user with:
   - List of missing files
   - Possible causes (disk space, antivirus, permissions)
   - Path to the installation log
3. **Fails the installation** by returning `ActionResult.Failure`
4. **Triggers rollback** through WiX's built-in rollback mechanism

### Example Error Message

```
Installation failed: Critical files are missing from installation directory. 
Missing files: efbundle.exe

The installer was unable to copy all required files. This may be caused by:
• Insufficient disk space
• Antivirus software blocking file operations
• Insufficient permissions

Installation log: C:\Users\[user]\AppData\Local\Temp\MSI*.log
```

## Testing

Manual test scenarios are provided in `FileCopyVerifier.Tests.cs`:

- `TestAllFilesPresent` - Verifies success when all files exist
- `TestMissingCriticalFile` - Verifies failure when a file is missing
- `TestDirectoryNotExists` - Verifies failure when directory doesn't exist
- `TestNullOrEmptyPath` - Verifies failure for invalid paths

These will be converted to proper xUnit tests in task 25.1.

## Requirements Validation

This implementation satisfies **Requirement 10.5**:

> WHEN file copy fails, THE Installer SHALL log the error to Install_Log and display an error message

The implementation:
- ✓ Logs file copy errors with file path
- ✓ Displays error message on failure
- ✓ Fails installation if critical files cannot be copied
- ✓ Provides actionable guidance to the user

## Integration with Other Components

The `FileCopyVerifier` integrates with:

- **WiX InstallFiles action** - Runs after file copy completes
- **MSI logging** - Writes to the standard MSI log file
- **WiX rollback mechanism** - Triggers automatic rollback on failure
- **InstallationLogger** (task 11.1) - Will integrate with structured logging when implemented

## Future Enhancements

When the `InstallationLogger` class is implemented (task 11.1), the `FileCopyVerifier` should be updated to use structured logging instead of direct `session.Log()` calls.
