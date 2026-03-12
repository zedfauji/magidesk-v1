# Heat.exe Integration for Application File Harvesting

## Overview

The Magidesk installer uses WiX Heat.exe to automatically harvest application files from the publish output directory. This ensures all application files are included in the installer without manual maintenance.

## How It Works

### 1. Build Process Flow

```
dotnet publish Magidesk.Presentation
    ↓
Files copied to build/installer/staging/app
    ↓
WiX build starts
    ↓
HarvestApplicationFiles target runs (BeforeTargets="Compile")
    ↓
Heat.exe harvests files from staging/app
    ↓
ApplicationFiles.wxs is generated/updated
    ↓
WiX compiles all .wxs files including ApplicationFiles.wxs
    ↓
Installer bundle created
```

### 2. MSBuild Integration

The `Magidesk.Installer.wixproj` file contains a custom MSBuild target that runs before compilation:

```xml
<Target Name="HarvestApplicationFiles" BeforeTargets="Compile">
  <Exec Command="heat dir &quot;$(AppSourceDir)&quot; -cg ApplicationFiles -dr INSTALLFOLDER -gg -sfrag -srd -var var.AppSourceDir -out &quot;$(MSBuildProjectDirectory)\Components\ApplicationFiles.wxs&quot;" />
</Target>
```

This target:
- Runs automatically before WiX compilation
- Checks if the staging directory exists (fails with clear error if missing)
- Executes Heat.exe to harvest files
- Generates/overwrites `Components/ApplicationFiles.wxs`

### 3. Heat.exe Parameters Explained

| Parameter | Value | Purpose |
|-----------|-------|---------|
| `dir` | `build/installer/staging/app` | Source directory to harvest |
| `-cg` | `ApplicationFiles` | ComponentGroup ID (referenced in Product.wxs) |
| `-dr` | `INSTALLFOLDER` | Directory reference (C:\Program Files\Magidesk) |
| `-gg` | (flag) | Generate stable GUIDs for components |
| `-sfrag` | (flag) | Suppress Fragment element (we provide it) |
| `-srd` | (flag) | Suppress root directory element |
| `-var` | `var.AppSourceDir` | Use preprocessor variable for paths |
| `-out` | `Components/ApplicationFiles.wxs` | Output file path |

### 4. Directory Structure Preservation

Heat.exe preserves the complete directory structure from the publish output:

**Source (staging/app):**
```
staging/app/
├── Magidesk.exe
├── Magidesk.dll
├── appsettings.json
├── Npgsql.dll
├── Microsoft.EntityFrameworkCore.dll
└── assets/
    ├── logo.png
    └── styles/
        └── app.css
```

**Target (C:\Program Files\Magidesk):**
```
C:\Program Files\Magidesk/
├── Magidesk.exe
├── Magidesk.dll
├── appsettings.json
├── Npgsql.dll
├── Microsoft.EntityFrameworkCore.dll
└── assets/
    ├── logo.png
    └── styles/
        └── app.css
```

### 5. Generated Component Structure

Heat.exe generates WiX components like this:

```xml
<ComponentGroup Id="ApplicationFiles" Directory="INSTALLFOLDER">
  <Component Id="Magidesk.exe" Guid="GENERATED-GUID">
    <File Id="Magidesk.exe" Source="$(var.AppSourceDir)\Magidesk.exe" KeyPath="yes" />
  </Component>
  
  <Component Id="Magidesk.dll" Guid="GENERATED-GUID">
    <File Id="Magidesk.dll" Source="$(var.AppSourceDir)\Magidesk.dll" KeyPath="yes" />
  </Component>
  
  <!-- Subdirectories -->
  <Directory Id="assets" Name="assets">
    <Component Id="logo.png" Guid="GENERATED-GUID">
      <File Id="logo.png" Source="$(var.AppSourceDir)\assets\logo.png" KeyPath="yes" />
    </Component>
    
    <Directory Id="styles" Name="styles">
      <Component Id="app.css" Guid="GENERATED-GUID">
        <File Id="app.css" Source="$(var.AppSourceDir)\assets\styles\app.css" KeyPath="yes" />
      </Component>
    </Directory>
  </Directory>
</ComponentGroup>
```

## Prerequisites

Before building the installer, you must:

1. **Publish the application:**
   ```powershell
   dotnet publish src\Magidesk.Presentation\Magidesk.Presentation.csproj `
       --configuration Release `
       --runtime win-x64 `
       --self-contained false `
       --output build\installer\staging\app
   ```

2. **Verify staging directory exists:**
   ```powershell
   Test-Path build\installer\staging\app
   # Should return: True
   ```

3. **Build the installer:**
   ```powershell
   dotnet build src\Magidesk.Installer\Magidesk.Installer.wixproj `
       --configuration Release
   ```

## Troubleshooting

### Error: "Application staging directory not found"

**Cause:** The `build/installer/staging/app` directory doesn't exist.

**Solution:** Run the publish command first (see Prerequisites above).

### Error: "heat.exe is not recognized"

**Cause:** WiX Toolset is not installed or not in PATH.

**Solution:** 
```powershell
dotnet tool install --global wix
```

### Files Missing from Installer

**Cause:** Files were added to the project after Heat.exe ran.

**Solution:** Clean and rebuild the installer project:
```powershell
dotnet clean src\Magidesk.Installer\Magidesk.Installer.wixproj
dotnet build src\Magidesk.Installer\Magidesk.Installer.wixproj
```

### GUIDs Change on Every Build

**Cause:** Using `-g` instead of `-gg` parameter.

**Solution:** The project is configured correctly with `-gg` (generate stable GUIDs). If GUIDs still change, ensure you're not manually editing the generated file.

## Manual Heat.exe Execution

For testing or debugging, you can run Heat.exe manually:

```powershell
# From project root
heat dir "build\installer\staging\app" `
     -cg ApplicationFiles `
     -dr INSTALLFOLDER `
     -gg `
     -sfrag `
     -srd `
     -var var.AppSourceDir `
     -out "src\Magidesk.Installer\Components\ApplicationFiles.wxs"
```

## Requirements Satisfied

This implementation satisfies the following requirements from the spec:

- **Requirement 10.1**: Installation directory at C:\Program Files\Magidesk
- **Requirement 10.2**: Copy all application files from Bundle to installation directory
- **Requirement 10.3**: Preserve directory structure of published application

## Next Steps

After this task (9.1), the following tasks will build on this foundation:

- **Task 9.2**: Configure file permissions for installation directory
- **Task 9.3**: Implement file copy error handling
- **Task 9.4**: Write property test for file structure preservation
