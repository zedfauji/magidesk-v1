# Changelog

All notable changes to Magidesk POS will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0-beta] - 2026-01-07

### Added
- **Table Designer Feature**
  - Create and manage table layouts for restaurant floors
  - Support for multiple table shapes (Rectangle, Square, Round, Oval)
  - Drag & drop interface for table positioning
  - Multi-selection support for bulk operations
  - Floor management with customizable dimensions
  - Draft and published layout states
  - Auto-save before adding tables to prevent FK violations
  
- **Error Handling System**
  - Comprehensive try-catch blocks in all UI event handlers
  - User-friendly error dialogs with actionable messages
  - Dialog guard to prevent multiple ContentDialogs
  - Database constraint violation handling with specific messages
  - Debug logging for all errors
  
- **UI Components**
  - Layout name input TextBox
  - Interactive shape palette with visual feedback
  - Real-time properties panel with validation
  - Status badges for draft/active states
  
- **Code Organization**
  - Split TableDesignerViewModel into partial classes
  - InverseBoolToVisibilityConverter
  - SelectionToBorderColorConverter
  
- **Documentation**
  - Table Designer architecture documentation
  - Interaction specifications
  - UI audit documentation
  - Redesign summary

### Fixed
- NullReferenceException in Floor ComboBox TwoWay binding
- NullReferenceException in Layout ComboBox TwoWay binding
- NullReferenceException in IsDesignMode ToggleSwitch binding
- NullReferenceException in Properties Panel bindings (7 instances)
- InvalidCastException in TableShapePalette RadioButtons
- FK violation when adding tables to unsaved layouts
- "Only a single ContentDialog can be open at any time" crash
- Silent failures in table deletion operations
- Stuck drag state when manipulation errors occur
- Properties panel not updating on selection change

### Changed
- All XAML TwoWay bindings converted to OneWay with safe event handlers
- Error messages now show user-friendly descriptions instead of technical details
- Table addition now auto-saves layout first if not already saved

### Known Issues
- Only 1 table renders instead of all 17 when loading LaCalma1 layout
- Tables collection may not populate correctly on initial load
- 125+ MVVM Toolkit AOT warnings (non-blocking)

---

## Release Types

- **Major** (x.0.0): Breaking changes, major features, database schema changes
- **Minor** (0.x.0): New features, enhancements (backwards compatible)
- **Patch** (0.0.x): Bug fixes, security patches, minor improvements

## Version Suffixes

- `-alpha`: Internal testing only
- `-beta`: External testers
- `-rc`: Release candidate (bar testing)
- (none): Production release
