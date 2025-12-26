# Forensic UI & Workflow Audit (FloreantPOS → MagiDesk)

**Mode:** forensic reconstruction + porting plan (no implementation).

## Non-negotiables
- FloreantPOS code under `/projects/Code/Redesign-POS/floreantpos` is the only behavioral source of truth.
- Every documented behavior must cite specific FloreantPOS classes/files (UI vs workflow vs service enforcement).
- If behavior cannot be proven from code, we explicitly mark it as **UNCERTAIN** and link the exact code location that is ambiguous.

## Output structure
- `feature-index.md`: complete index of UI surfaces/features with traceability.
- `parity-matrix.md`: per-feature EXISTS/PARTIAL/MISSING vs MagiDesk v1.
- `risk-register.md`: operational/financial/audit risks and mitigations.
- `roadmap.md`: low-risk porting plan (plan only) with sequencing and constraints.
- `features/`: one markdown document per feature using the template.

## Forensic template
See `template.md`.
