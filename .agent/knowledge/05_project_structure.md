# Project Structure Knowledge Item

## Directory Map

-   `src/` - Source Code Root
    -   `Magidesk.Presentation/` - **WinUI 3 App**. Contains `Views`, `ViewModels`, `Assets`. Entry point.
    -   `Magidesk.Application/` - **Core Logic**. Contains `Services` (Interfaces), `DTOs`, `Commands`, `Queries`, `DomainEventHandlers`.
    -   `Magidesk.Domain/` - **Domain Entities**. Contains `Entities` (e.g., `Ticket`, `Table`), `ValueObjects`, `RepositoryInterfaces`.
    -   `Magidesk.Infrastructure/` - **Implementation**. Contains `Persistence` (EF Core `DbContext`, `Migrations`), `Services` (Concrete impl).
    -   `Magidesk.Api/` - Likely a backend API if applicable, or server component.
    -   `Magidesk.Tests.*/` - Test projects mirroring the layers.

-   `.agent/` - Agent Configuration
    -   `rules/` - Authoritative project rules (`.md` files).
    -   `skills/` - Specialized agent capabilities.
        -   `winui-mvvm-validator/`, `coding-standards-enforcer/`, etc.
    -   `knowledge/` - **(New)** This knowledge base.

## Mapping Rules to Codebase

| Scope | Relevant Rules/Skills | Expected Pattern |
| :--- | :--- | :--- |
| **UI (`*.xaml`, `*.xaml.cs`)** | `ui-architecture-enforcer`, `winui-mvvm-validator` | No logic in .cs. `x:Bind` in XAML. |
| **ViewModels (`*ViewModel.cs`)** | `observable-state-validator`, `file-size-and-structure-enforcer` | < 300 lines. `ObservableObject`. Commands for actions. |
| **Application Services** | `coding-standards-enforcer` | Single responsibility. Interface-based. |
| **Infrastructure** | `architecture-and-boundaries` | Implement interfaces from Application/Domain. |
| **All Files** | `file-size-limits`, `coding-standards` | Max 300 lines. 1 class/file. |

## Critical Paths
-   **Feature Implementation:** Define Entity (Domain) -> Define Repo Interface (Domain) -> Implement Repo (Infra) -> Define/Impl Service/Command (App) -> Create VM & View (Presentation).
