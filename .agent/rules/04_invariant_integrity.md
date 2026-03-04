# Invariant Integrity

Core invariants must always hold after any change.

## Examples
- ViewModels should not allow null state where the code expects non-null.
- Data caches must be invalidated consistently.
- Single-source data models must not diverge.

## Practical Enforcement
Agents should assume invariants are required compile- and runtime guarantees.
