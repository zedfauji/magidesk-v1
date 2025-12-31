# Documentation Alignment (Docs vs Code)

## Scope
- Docs are treated as **intent**, not truth.
- Alignment is assessed for a limited, high-signal subset of docs that directly describe runtime behavior.

## Alignment table

| Doc | Claimed / intended behavior | Observed code behavior | Classification |
|---|---|---|---|
| `docs/parity-audit-master-report.md` | Claims UI readiness is low; highlights missing session context, cash accounting, hardware abstraction | Code shows multiple hardcoded IDs and simulated flows; dialogs/pages exist but several TODOs remain | Accurate (high-level) |
| `docs/ui-audit-summary-report.md` | Claims UI is prototype state; core loop broken; dialog-first mismatch | Code includes `LoginPage`, `SwitchboardPage`, `OrderEntryPage`, `SettlePage`; settle is a page, modifiers are dialogs | Mostly accurate |
| `docs/forensic-ui-audit/drift-list.md` | Lists specific mismatches (Settle as page, DrawerPull as page, ticket management as harness) | Code includes `SettlePage.xaml`; drawer pull currently appears as `DrawerPullReportDialog.xaml` (dialog) | Partially outdated / Mixed |
| `docs/forensic-ui-audit/feature-index.md` | Enumerates Floreant UI surfaces and expected Magidesk features by ID | Current codebase includes many similarly named dialogs/pages and commands but parity varies | Aspirational (as a parity target) |

## Notes
- Many feature-specific docs under `docs/forensic-*-audit/features/` were not individually validated line-by-line in this pass.
- Where doc-to-code contradictions exist, they should be revalidated against the current commit state.
