# WiX Installer Components

This directory contains WiX component definitions for the Magidesk POS installer.

## ApplicationFiles.wxs

The `ApplicationFiles.wxs` component is **auto-generated** by Heat.exe during the build process. It harvests all application files from the `build/installer/staging/app` directory and creates WiX component definitions that preserve the directory structure.

### How It Works

1. **Build Script**: The build script (to be created in task 21) publishes the Magidesk.Presentation application to `build/installer/staging/app`
2. **Heat.exe**: During the WiX project build, a custom MSBuild target runs Heat.exe to harvest files
3. **Component Generation**: Heat.exe generates component definitions for all files and subdirectories
4. **Installation**: The components are installed to `C:\Program Files\Magidesk` (INSTALLFOLDER)

### Heat.exe Command

```powershell
heat dir "build\installer\staging\app" `
     -cg ApplicationFiles `
     -dr INSTALLFOLDER `
     -gg `
     -sfrag `
     -srd `
     -var var.AppSourceDir `
     -out "src\Magidesk.Installer\Components\ApplicationFiles.wxs"
```

**Parameters:**
- `-cg ApplicationFiles`: ComponentGroup ID referenced in Product.wxs
- `-dr INSTALLFOLDER`: Directory reference (resolves to C:\Program Files\Magidesk)
- `-gg`: Generate stable GUIDs for components
- `-sfrag`: Suppress Fragment element (we provide it in the template)
- `-srd`: Suppress root directory element (we use INSTALLFOLDER)
- `-var var.AppSourceDir`: Use preprocessor variable for source paths
- `-out`: Output file path

### Manual Regeneration

If you need to manually regenerate the ApplicationFiles.wxs (for testing):

```powershell
# 1. Ensure the application is published
dotnet publish src\Magidesk.Presentation\Magidesk.Presentation.csproj `
    --configuration Release `
    --runtime win-x64 `
    --self-contained false `
    --output build\installer\staging\app

# 2. Run Heat.exe
heat dir "build\installer\staging\app" `
     -cg ApplicationFiles `
     -dr INSTALLFOLDER `
     -gg `
     -sfrag `
     -srd `
     -var var.AppSourceDir `
     -out "src\Magidesk.Installer\Components\ApplicationFiles.wxs"
```

### Expected Output Structure

After Heat.exe runs, the ApplicationFiles.wxs will contain:

```xml
<ComponentGroup Id="ApplicationFiles" Directory="INSTALLFOLDER">
  <Component Id="Magidesk.exe" Guid="...">
    <File Id="Magidesk.exe" Source="$(var.AppSourceDir)\Magidesk.exe" KeyPath="yes" />
  </Component>
  <Component Id="Magidesk.dll" Guid="...">
    <File Id="Magidesk.dll" Source="$(var.AppSourceDir)\Magidesk.dll" KeyPath="yes" />
  </Component>
  <!-- ... all other application files ... -->
  
  <!-- Subdirectories are preserved -->
  <Directory Id="SubDir1" Name="SubDirectory">
    <Component Id="SubFile.dll" Guid="...">
      <File Id="SubFile.dll" Source="$(var.AppSourceDir)\SubDirectory\SubFile.dll" KeyPath="yes" />
    </Component>
  </Directory>
</ComponentGroup>
```

## Other Components

- **PostgreSQL.wxs**: PostgreSQL installation component (task 5)
- **Shortcuts.wxs**: Desktop and Start Menu shortcuts (task 10)
- **Registry.wxs**: Add/Remove Programs registration (task 10)

## Requirements Validation

This component implementation satisfies:
- **Requirement 10.1**: Installation directory at C:\Program Files\Magidesk
- **Requirement 10.2**: Copy all application files from Bundle to installation directory
- **Requirement 10.3**: Preserve directory structure of published application
