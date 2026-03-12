# Shortcuts Component

## Overview

This component creates desktop and start menu shortcuts for Magidesk POS during installation.

## Implementation Details

### Desktop Shortcut
- **Name**: "Magidesk POS"
- **Location**: User's Desktop folder
- **Target**: `[INSTALLFOLDER]Magidesk.exe`
- **Icon**: Extracted from Magidesk.exe

### Start Menu Shortcut
- **Name**: "Magidesk POS"
- **Location**: Start Menu → Programs → Magidesk
- **Target**: `[INSTALLFOLDER]Magidesk.exe`
- **Icon**: Extracted from Magidesk.exe

## Technical Notes

### Icon Extraction
The icon is defined in `Product.wxs` using:
```xml
<Icon Id="MagideskIcon.exe" SourceFile="$(var.StagingDir)\app\Magidesk.exe" />
```

WiX automatically extracts the icon from the executable at build time.

### Registry Keys
Each shortcut component uses a registry key as its KeyPath:
- Desktop: `HKCU\Software\Magidesk\POS\DesktopShortcut`
- Start Menu: `HKCU\Software\Magidesk\POS\StartMenuShortcut`

This is required by WiX for per-user components and enables proper uninstallation.

### Uninstallation
The `RemoveFolder` elements ensure shortcuts are removed during uninstall:
- Desktop shortcut is removed from DesktopFolder
- Start Menu folder and shortcut are removed from ApplicationProgramsFolder

## Requirements Satisfied

- **Requirement 11.1**: Desktop shortcut named "Magidesk POS" pointing to Magidesk.exe
- **Requirement 11.2**: Start Menu entry under "Magidesk" pointing to Magidesk.exe

## Error Handling

Per Requirement 11.5, shortcut creation failures are non-fatal. If shortcut creation fails:
- The error is logged to the installation log
- Installation continues with remaining steps
- A warning is included in the final installation summary

This is handled by WiX's default behavior for per-user components.
