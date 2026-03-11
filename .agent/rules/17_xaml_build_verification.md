# XAML Build Verification Protocol
# File: .agent/rules/17_xaml_build_verification.md
# Last Updated: 2026-03-10

## Why This Rule Exists

`dotnet build` uses the .NET SDK's Roslyn pipeline but **does NOT run the full
WinUI 3 XAML compiler** (Microsoft.UI.Xaml.Markup.Compiler). This means:
- MC*, WMC*, XBF* errors are silently omitted
- Binding errors, missing resource errors, and type errors in XAML are hidden
- A passing `dotnet build` does NOT mean XAML compiles successfully

`build-xaml.ps1` at the project root uses the VS Insider MSBuild pipeline,
which runs the complete compiler and surfaces all hidden XAML failures.

---

## Mandatory Trigger Conditions

Run `.\build-xaml.ps1` (not `dotnet build`) after ANY of the following:

- Editing or creating a `.xaml` file
- Editing or creating a `.xaml.cs` code-behind file
- Adding, removing, or renaming a resource dictionary
- Adding a new `x:Bind`, `{Binding}`, converter reference, or TemplateBinding
- Adding a new control type or namespace import to XAML
- Changing a DataTemplate or ControlTemplate
- Renaming a ViewModel property that is referenced in XAML

---

## How to Run

```powershell
# From project root — standard incremental build:
.\build-xaml.ps1

# Full clean rebuild (use when errors seem stale):
.\build-xaml.ps1 -Target Rebuild

# Build only the Presentation project (faster, XAML only):
.\build-xaml.ps1 -Project src\Magidesk.Presentation\Magidesk.Presentation.csproj

# Release config:
.\build-xaml.ps1 -Configuration Release
```

---

## Reading the Output

The script always writes two output files:

| File | Purpose |
|---|---|
| `diagnostics/build-logs/build_summary_LATEST.txt` | **Always read this.** Overwritten every run. Structured, noise-free. |
| `diagnostics/build-logs/build_errors_<timestamp>.log` | Errors + warnings only for this run |
| `diagnostics/build-logs/build_full_<timestamp>.log` | Full verbose log (for deep investigation) |

Parse `build_summary_LATEST.txt` — it contains:
- BUILD RESULT (SUCCESS / FAILED)
- Error count, warning count, XAML-specific error count
- All error/warning lines
- A dedicated XAML-SPECIFIC ERRORS section for MC*/WMC*/XBF* codes

---

## Agent Decision Loop

```
AFTER ANY XAML CHANGE:
│
├─► Run: .\build-xaml.ps1
│
├─► Read: diagnostics\build-logs\build_summary_LATEST.txt
│
├─► IF "BUILD RESULT : SUCCESS"
│     └─► Proceed. Mark task done.
│
└─► IF "BUILD RESULT : FAILED"
      ├─► Parse errors from summary
      ├─► Fix the specific errors (XAML-specific first: MC*, WMC*, XBF*)
      ├─► Run: .\build-xaml.ps1  (repeat)
      │
      └─► IF same error appears 3 consecutive times:
            STOP. Do NOT keep guessing.
            Report: "STUCK ON XAML ERROR — requires manual inspection"
            Output the exact error text for the owner.
```

---

## XAML Error Code Reference

| Prefix | Compiler | Meaning |
|---|---|---|
| `MC****` | XAML Markup Compiler | Markup/binding/type errors in .xaml |
| `WMC****` | WinUI XAML Compiler | WinUI 3 specific XAML errors |
| `XBF****` | XAML Binary Format | Pre-compiled XAML binary issues |
| `XAML` | General | XAML parse / schema errors |

---

## What NOT To Do

- **Do NOT** use `dotnet build` as XAML verification — it will miss errors
- **Do NOT** run 4+ consecutive build attempts on the same unchanged error
- **Do NOT** modify FROZEN.md or domain layer to resolve a XAML error
- **Do NOT** mark a XAML-touching task complete without a passing `build-xaml.ps1`
