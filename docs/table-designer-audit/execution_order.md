# Execution Order: Table Designer Rebuild

The recovery of the Table Designer must follow this strict sequence to avoid corruption.

| Order | Ticket | Dependency | Risk if Ignored |
|-------|--------|------------|-----------------|
| **1** | **[T-DR-001]** | None | Data truncation and UI jitter. |
| **2** | **[T-DR-006]** | [T-DR-001] | Crash during mapping. |
| **3** | **[T-DR-003]** | None | Security breech (Operators editing layout). |
| **4** | **[T-DR-002]** | [T-DR-001] | Modifying live layouts during design. |
| **5** | **[T-DR-004]** | [T-DR-002] | UX Confusion (Invalid states). |
| **6** | **[T-DR-005]** | None | Corrupting active tickets / ghost tables. |
