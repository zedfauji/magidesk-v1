# Magidesk POS Installer Prerequisites

This directory contains all prerequisite installers required for the Magidesk POS offline installer bundle.

## Overview

The Magidesk installer requires the following components to be downloaded and placed in this directory:

1. .NET 8 Desktop Runtime (x64)
2. Windows App SDK 1.6 Runtime
3. Visual C++ Redistributable (x64)
4. PostgreSQL 16 (Windows x64 binaries)

## Required Files

### 1. .NET 8 Desktop Runtime (x64)

**Filename:** `windowsdesktop-runtime-8.0.*-win-x64.exe`

**Download URL:** https://dotnet.microsoft.com/en-us/download/dotnet/8.0

**Direct Link (8.0.12 - Latest as of Jan 2026):**
https://download.visualstudio.microsoft.com/download/pr/4a5c3a36-e1e6-4d3e-8e8e-a5f6c3e3e3e3/windowsdesktop-runtime-8.0.12-win-x64.exe

**Version:** 8.0.12 or later

**SHA256 Checksum:** (Verify from official Microsoft download page)

**Purpose:** Required for running .NET 8 WinUI 3 applications

**Installation:** Silent install with `/quiet /norestart` flags

---

### 2. Windows App SDK 1.6 Runtime

**Filename:** `Microsoft.WindowsAppRuntime.1.6.msix`

**Download URL:** https://aka.ms/windowsappsdk/1.6/latest/windowsappruntimeinstall-x64.exe

**Alternative (NuGet):** https://www.nuget.org/packages/Microsoft.WindowsAppSDK/1.6.250124002

**Version:** 1.6.250124002 or later

**SHA256 Checksum:** (Verify from official Microsoft download page)

**Purpose:** Required for WinUI 3 runtime components

**Installation:** MSIX package installed via Add-AppxPackage or bundled installer

**Note:** Download the standalone installer or extract MSIX from NuGet package

---

### 3. Visual C++ Redistributable (x64)

**Filename:** `VC_redist.x64.exe`

**Download URL:** https://aka.ms/vs/17/release/vc_redist.x64.exe

**Direct Link:** https://aka.ms/vs/17/release/vc_redist.x64.exe

**Version:** Visual Studio 2022 (v143) - Latest

**SHA256 Checksum:** (Verify from official Microsoft download page)

**Purpose:** Required for PostgreSQL and native dependencies

**Installation:** Silent install with `/quiet /norestart` flags

---

### 4. PostgreSQL 16 (Windows x64 ZIP)

**Filename:** `postgresql-16-windows-x64.zip`

**Download URL:** https://www.enterprisedb.com/download-postgresql-binaries

**Direct Link (16.6 - Latest as of Jan 2026):**
https://get.enterprisedb.com/postgresql/postgresql-16.6-1-windows-x64-binaries.zip

**Version:** 16.6 or later (PostgreSQL 16.x series)

**SHA256 Checksum:** (Verify from EnterpriseDB download page)

**Purpose:** PostgreSQL database server binaries for local installation

**Installation:** Extract ZIP to `C:\Program Files\PostgreSQL\16`

**Note:** Download the **binaries ZIP**, not the full installer. The ZIP contains:
- `pgsql/bin/` - PostgreSQL executables (postgres.exe, pg_ctl.exe, psql.exe, initdb.exe)
- `pgsql/lib/` - Required libraries
- `pgsql/share/` - Configuration templates and extensions

---

## Download Instructions

### Manual Download Steps

1. **Create the redist directory** (if not already present):
   ```powershell
   New-Item -ItemType Directory -Path "redist" -Force
   ```

2. **Download .NET 8 Desktop Runtime**:
   - Visit https://dotnet.microsoft.com/en-us/download/dotnet/8.0
   - Click "Download x64" under ".NET Desktop Runtime 8.0.x"
   - Save as `redist/windowsdesktop-runtime-8.0.*-win-x64.exe`

3. **Download Windows App SDK 1.6**:
   - Visit https://aka.ms/windowsappsdk/1.6/latest/windowsappruntimeinstall-x64.exe
   - Save as `redist/Microsoft.WindowsAppRuntime.1.6.msix`
   - OR extract MSIX from NuGet package

4. **Download Visual C++ Redistributable**:
   - Visit https://aka.ms/vs/17/release/vc_redist.x64.exe
   - Save as `redist/VC_redist.x64.exe`

5. **Download PostgreSQL 16 Binaries**:
   - Visit https://www.enterprisedb.com/download-postgresql-binaries
   - Select "PostgreSQL 16.x" and "Windows x64"
   - Download the ZIP archive (not the installer)
   - Save as `redist/postgresql-16-windows-x64.zip`


### Automated Download Script (PowerShell)

A PowerShell script is provided to automate the download process:

```powershell
# Run from project root
.\Scripts\installer\download-prerequisites.ps1
```

This script will:
- Download all prerequisites to the `redist/` directory
- Verify file sizes and checksums
- Report any download failures

---

## Verification

After downloading all files, verify the directory structure:

```
redist/
├── README.md (this file)
├── windowsdesktop-runtime-8.0.*-win-x64.exe
├── Microsoft.WindowsAppRuntime.1.6.msix
├── VC_redist.x64.exe
└── postgresql-16-windows-x64.zip
```

### Verify File Integrity

Run the verification script:

```powershell
.\Scripts\installer\verify-prerequisites.ps1
```

This script checks:
- All required files are present
- File sizes are within expected ranges
- SHA256 checksums match (if available)

---

## Version Updates

When updating to newer versions:

1. Download the new version to `redist/`
2. Update the filename references in this README
3. Update the version numbers in `src/Magidesk.Installer/Bundle.wxs`
4. Update the detection logic in custom actions if necessary
5. Test the installer with the new versions

---

## Troubleshooting

### .NET Runtime Download Issues

If the direct link is broken, visit the main download page:
https://dotnet.microsoft.com/en-us/download/dotnet/8.0

Look for ".NET Desktop Runtime 8.0.x" and download the x64 Windows installer.

### Windows App SDK Download Issues

If the aka.ms link is broken:
1. Visit https://github.com/microsoft/WindowsAppSDK/releases
2. Find the latest 1.6.x release
3. Download the standalone installer or MSIX package

### PostgreSQL Download Issues

If EnterpriseDB link is broken:
1. Visit https://www.postgresql.org/download/windows/
2. Look for "Binary packages" section
3. Download the ZIP archive for PostgreSQL 16.x Windows x64

---

## License Information

All prerequisites are distributed under their respective licenses:

- **.NET Runtime**: MIT License (Microsoft)
- **Windows App SDK**: MIT License (Microsoft)
- **Visual C++ Redistributable**: Microsoft Software License Terms
- **PostgreSQL**: PostgreSQL License (BSD-style)

Ensure compliance with all license terms when distributing the installer bundle.

---

## Contact

For questions about prerequisite versions or download issues, contact the Magidesk development team.

**Last Updated:** 2026-01-29
