# WiX Installer Build Fix Summary

## Issues Fixed

### 1. EmbeddedResource Configuration Error
**Problem**: The project file included `<EmbeddedResource Include="..\..\build\installer\staging\**\*" />` which caused WiX to try processing all DLL files as embedded resources.

**Solution**: Removed the EmbeddedResource item group. Application files should be harvested by Heat.exe and included as Components, not embedded resources.

### 2. Multiple Entry Sections Error
**Problem**: WiX SDK was auto-including all .wxs files in the project directory, causing both Product.wxs and Bundle.wxs to be compiled together, which is invalid (can only have one Package or one Bundle per project).

**Solution**: Added `<EnableDefaultCompileItems>false</EnableDefaultCompileItems>` to disable auto-inclusion and explicitly listed all .wxs files to compile in the project file.

### 3. WiX v5 Syntax Updates
**Problem**: Product.wxs used WiX v3/v4 syntax that's incompatible with WiX v5.

**Solutions**:
- Changed `<Product>` element to `<Package>` element
- Changed `BinaryKey` attribute to `BinaryRef` in CustomAction
- Changed `<Directory Id="TARGETDIR">` to `<StandardDirectory>` elements
- Moved condition from inner text to `Condition` attribute in `<Custom>` element
- Fixed custom actions DLL path from `.CA.dll` to `.dll`

## Current Build Status

✅ **Build Successful**
- Output: `Magidesk.msi` (700KB)
- Location: `src/Magidesk.Installer/bin/x64/Release/Magidesk.msi`
- Warnings: 3 (non-critical)
- Errors: 0

## Project Configuration

The installer project now:
- Explicitly controls which .wxs files are compiled
- Properly references custom actions DLL
- Uses WiX v5 syntax throughout
- Has Heat.exe integration ready for application file harvesting
- Keeps Bundle.wxs separate (will be used when switching to Bundle output type)

## Next Steps

1. The installer currently builds an MSI package
2. To create a Bundle (bootstrapper with prerequisites), change `<OutputType>Package</OutputType>` to `<OutputType>Bundle</OutputType>` and update the Compile item group to include Bundle.wxs instead of Product.wxs
3. Continue with remaining installer tasks (custom actions, UI, etc.)
