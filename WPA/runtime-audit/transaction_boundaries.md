# Transaction Boundary Analysis

| Endpoint / Operation | Participating Entities | Transaction Scope | Isolation Level | Risk Assessment |
| :--- | :--- | :--- | :--- | :--- |
| **POST /api/auth/login** | `User` (Read) | Read-Only | ReadCommitted | ✅ Safe. |
| **GET /api/tables** | `Table`, `TableSession`, `Ticket` | Read-Only | ReadCommitted | ✅ Safe (Dirty reads unlikely to harm). |
| **POST /api/tables/start** | `Table`, `TableSession`, `Ticket` | **Atomic** | Serializable (EF Opt. Concurrency) | ✅ Safe. `Table.Version` protects against double-booking. |
| **POST /api/tables/pause** | `TableSession` | **Atomic** | ReadCommitted | ✅ Safe. |
| **POST /api/orders/lines** | `Ticket`, `OrderLine`, `StockMovement`, `AuditEvent` | **PER ITEM** (Loop) | ReadCommitted | ❌ **CRITICAL**. Non-Atomic Batch. Loop commits after each item. Failure on Item N leaves Items 1..N-1 committed. |
| **GET /api/menu/items** | `MenuCategory`, `MenuItem` | Read-Only | ReadCommitted | ✅ Safe. |
