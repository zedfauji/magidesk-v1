# Ship Decision: Table Designer (v1)

## 1. Decision: NO

The Table Designer SHOULD NOT be## Updated Assessment (Post-Reset)

The **Forensic Reset (Phases 1-3)** has successfully mitigated the primary architectural risks:
1. **Precision**: Jagged movements and truncation errors are eliminated via `double` unification.
2. **Safety**: "Live Table Locks" prevent operational accidents during layout edits.
3. **Drafting**: Operators can now safely design in `IsDraft` mode without corrupting the active floor.

### Current Recommendation: BETA READY
The Table Designer is now safe for administrative use in a controlled beta/audit environment.
3. **UX Resilience**: The optimistic concurrency implemented previously is a patch, not a solution. A full redesign is required to support robust layout management.
4. **Mission Priority**: The Table Map (Runtime) is now stable and correct. Shipping with a functional Map but a hidden Designer is safer than shipping a fragile Designer.

## 3. Deployment Action

- **Status**: **LOCKED**.
- **Action**: All entry points to the Table Designer have been commented out or restricted. 
- **Requirement for Unlock**: Completion of tickets [T-DR-001] through [T-DR-006] as defined in the Rebuild Tickets document.

## 4. Workaround for v1

- Floor layouts for v1 should be initialized via Database Seeding or a one-time Admin SQL script until the Designer is recovered in a post-v1 update.
