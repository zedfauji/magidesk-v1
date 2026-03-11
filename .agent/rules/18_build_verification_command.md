---
trigger: always_on
priority: critical
---

# Build Verification Command — AUTHORITATIVE RULE

## THE RULE

**`dotnet build` is BANNED for build verification in this project.**

All agents — Antigravity, Kiro, Claude, Copilot, or any other — MUST use:

```powershell
.\build-xaml.ps1
```

This is non-negotiable. There are no exceptions.

---

## Why `dotnet build` Is Banned

`dotnet build` uses the .NET SDK's Roslyn pipeline only. It does NOT invoke the
WinUI 3 XAML compiler (Microsoft.UI.Xaml.Markup.Compiler). This means:

- MC*, WMC*, XBF* errors are silently hidden
- XAML binding errors pass invisibly
- Missing resource references are not caught
- Type resolution errors in .xaml files are skipped

A passing `dotnet build` is **meaningless** as a correctness signal in this codebase.
It has caused repeated debug loops where agents declared tasks complete while
real XAML compilation failures existed.

---

## Mandatory Build Protocol

### After ANY of these changes — run `.\build-xaml.ps1`:
- Any `.xaml` file edit or creation
- Any `.xaml.cs` code-behind edit
- Any ViewModel property rename referenced in XAML
- Any new `x:Bind`, converter, resource, or namespace import in XAML
- Any new control type added to a View
- Any DataTemplate or ControlTemplate change
- Any resource dictionary add, remove, or rename

### How to run:
```powershell
# Standard (incremental) — use after small changes:
.\build-xaml.ps1

# Full clean rebuild — use when errors seem stale or after large changes:
.\build-xaml.ps1 -Target Rebuild

# Presentation project only (faster for XAML-only changes):
.\build-xaml.ps1 -Project src\Magidesk.Presentation\Magidesk.Presentation.csproj
```

### Where to read results:
```
diagnostics\build-logs\build_summary_LATEST.txt
```
This file is always overwritten with the latest run. It contains:
- BUILD RESULT: SUCCESS or FAILED
- Error count, warning count, XAML-specific error count
- All error and warning lines
- A dedicated XAML error section (MC* / WMC* / XBF* codes)

---

## Agent Loop Rules

```
AFTER ANY XAML-TOUCHING CHANGE:
│
├─► Run: .\build-xaml.ps1
├─► Read: diagnostics\build-logs\build_summary_LATEST.txt
│
├─► BUILD RESULT: SUCCESS → proceed, mark task done
│
└─► BUILD RESULT: FAILED
      ├─► Fix errors found in summary (XAML-specific first)
      ├─► Run: .\build-xaml.ps1 again
      │
      └─► Same error 3 times in a row → STOP
            Do NOT keep guessing.
            Report to owner: "STUCK ON XAML ERROR — manual inspection required"
            Include the exact error text in the report.
```

---

## Violations

If an agent uses `dotnet build` as its verification step and declares a task
complete based on that result, the task is considered **unverified** and must
be re-run with `.\build-xaml.ps1` before it can be marked done.

This rule exists because repeated agent debug loops caused by hidden XAML
errors have been a significant source of wasted cycles in this project.
