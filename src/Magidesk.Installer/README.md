# Magidesk Installer Project

This directory contains the WiX Toolset v5 installer project for Magidesk POS.

## Project Structure

```
Magidesk.Installer/
├── Bundle.wxs                  # Burn bootstrapper definition (entry point)
├── Product.wxs                 # Main MSI product definition
├── Components/                 # WiX component definitions
│   ├── ApplicationFiles.wxs    # Application file harvesting
│   ├── PostgreSQL.wxs          # PostgreSQL installation
│   ├── Shortcuts.wxs           # Desktop and Start Menu shortcuts
│   └── Registry.wxs            # ARP registration
├── UI/                         # Custom UI dialogs
│   ├── InstallDialog.wxs       # Installation dialog
│   └── ProgressDialog.wxs      # Progress display
└── Prerequisites/              # Prerequisite installer references
    └── README.md               # Documentation for prerequisites

Magidesk.Installer.CustomActions/
├── PostgreSQLInstaller.cs      # PostgreSQL setup custom action
├── DatabaseCreator.cs          # Database creation custom action
├── MigrationRunner.cs          # EF Core migration custom action
├── SmokeTestRunner.cs          # Connectivity test custom action
└── ConfigurationWriter.cs      # Config file writer
```

## Build Output

The installer build produces:
- **MagideskSetup.exe**: Burn bootstrapper bundle (entry point for users)
- **Magidesk.msi**: Main application MSI (embedded in bundle)

## Requirements

- WiX Toolset v5.0.0 (installed via `dotnet tool install --global wix`)
- .NET 8 SDK
- Visual Studio 2022 or later (for development)

## Building

```powershell
# Build the installer
dotnet build src/Magidesk.Installer/Magidesk.Installer.wixproj --configuration Release

# Or use the build script (to be created in task 21)
.\build-installer.ps1
```

## Implementation Status

This project structure was created in Task 1 of the installer implementation plan.
Custom actions and WiX components will be implemented in subsequent tasks.
