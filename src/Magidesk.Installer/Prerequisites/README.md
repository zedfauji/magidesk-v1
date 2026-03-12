# Prerequisites Directory

This directory contains references to prerequisite installers that will be bundled with the Magidesk installer.

The actual prerequisite files should be placed in the `redist/` directory at the repository root.

## Required Files (to be downloaded in task 2):

- .NET 8 Desktop Runtime x64: `windowsdesktop-runtime-8.0.*-win-x64.exe`
- Windows App SDK 1.6: `Microsoft.WindowsAppRuntime.1.6.msix`
- VC++ Redistributable x64: `VC_redist.x64.exe`
- PostgreSQL 16 Windows x64: `postgresql-16-windows-x64.zip`

These files will be copied to the staging directory during the build process.
