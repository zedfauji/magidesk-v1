# Project Brief

## Objective
The primary objective is to rebuild the Magidesk POS system with a modern technology stack (.NET 8, WinUI 3, PostgreSQL) while maintaining 100% behavioral constraint parity with the legacy FloreantPOS system. The new system must support high-volume transactions, strict financial integrity, and a responsive touch-first UI.

## Core Requirements
1.  **Behavioral Parity**: The system must behave exactly like FloreantPOS in terms of financial calculations, order workflows, and security constraints, unless explicitly documented as a "Modernization" improvement.
2.  **Forensic Driven**: All development is verified against the Forensic Audit documents (F-0001 to F-0132).
3.  **Modern Architecture**: Use Clean Architecture, CQRS, and Domain-Driven Design principles.
4.  **Financial Accuracy**: Zero tolerance for rounding errors or financial discrepancies.

## Source of Truth
- **Behavior**: `docs/forensic-ui-audit/features/` (F-XXXX files)
- **Architecture**: `memory-bank/systemPatterns.md`
- **Current State**: `memory-bank/activeContext.md`
