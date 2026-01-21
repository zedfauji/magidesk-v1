# Release Notes - Structural Baseline v1.0

**Date:** 2026-01-21
**Type:** Infrastructure / Non-Breaking Change

## Summary
This release establishes the Canonical Directory Structure for the Magidesk POS system. It eliminates root repository clutter and enforces a standard Clean Architecture layout without altering application logic or external dependencies.

## Changes

### 1. Source Code Reorganization
*   **Canonical Source Root**: All source code moved to `src/`.
*   **Library Projects**: `Magidesk.Domain`, `Magidesk.Application`, `Magidesk.Infrastructure`, `Magidesk.Migrations`, `Magidesk.Api` moved to `src/`.
*   **Presentation Layer**: Root-level WinUI artifacts (`App.xaml`, `ViewModels`, `Views`) consolidated into `src/Magidesk.Presentation/`.
*   **Solution File**: `Magidesk.sln` moved to `src/` and updated.

### 2. Artifact Cleanup
*   **Documentation**: All `.md` files moved to `docs/`.
*   **Scripts**: Database and utility scripts (`*.sql`, `*.ps1`, `*.sh`) moved to `scripts/`.
*   **Diagnostics**: Logs and trace files moved to `diagnostics/`.
*   **Archival**: Legacy build artifacts (`bin`/`obj`) and deprecated folders (`WPA`) moved to `archive/`.

### 3. Build & Stability
*   **Ignored Files**: Updated `.gitignore` to exclude `logs/`, `artifacts/`, `*.zip`, `*.bak`, `*.tmp`.
*   **Build Status**: The solution builds successfully with existing warnings (no new regressions introduced).

## Risk Assessment
*   **Low Risk**: No C# logic was modified. References were preserved via relative path strategies or explicit project file updates.
*   **Safe for Production**: This structure is a purely organizational change to support future development and does not impact runtime behavior.
