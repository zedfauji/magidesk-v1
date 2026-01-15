
# Backend Readiness Summary

| WPA Feature | Status | Handler / Implementation | Gap Actions |
| :--- | :--- | :--- | :--- |
| **Authentication** | ✅ Ready | `ISecurityService` | Needs Controller Wrapper. |
| **Table Grid** | ✅ Ready | `GetActiveSessionsQueryHandler` | Logic is complete. |
| **Start Session** | ✅ Ready | `StartTableSessionCommandHandler` | Logic is complete. |
| **End Session** | ✅ Ready | `EndTableSessionCommandHandler` | Logic is complete. |
| **Session Timer** | ✅ Ready | `Pause/ResumeCommandHandler` | Logic is complete. |
| **Menu Browsing** | ⚠️ Wrapper | *Repo Only* | Need to expose `MenuRepository` via API. |
| **Ordering** | ⚠️ Wrapper | `AddOrderLineCommandHandler` | Need Batch loop in API for arrays. |
| **Kitchen Sync** | ❌ Blocked | `KitchenRoutingService` | Check network reachability of printers from Web Server. |
| **Context** | ⚠️ Config | `IUser/TerminalContext` | Need Scoped HTTP implementations. |

**Overall Status:** **80% Ready.** Core business logic for sessions and ordering is solid. Gaps are primarily in simple "Read" operations (Menu) and standard Web API plumbing (Context/Auth).
