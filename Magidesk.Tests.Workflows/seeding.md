# Test Data Seeding

To ensure deterministic tests, we use a specific seeding strategy.

## Strategy
-   **No Shared State**: Each test (or test class) works with a clean state or a known seeded state.
-   **Factories**: Use Builder pattern to create entities (e.g., `TicketBuilder`, `UserBuilder`).

## Standard Seeds

### 1. Minimal Seed
-   1 Admin User
-   1 Terminal
-   Empty Menu

### 2. Standard POS Seed
-   Includes minimal seed.
-   Menu Categories: Pizza, Drinks, Sides.
-   Modifiers: Sizes, Toppings.
-   Tax Rates configured.
-   Shift created but not open.

## Implementation
Helper classes will be located in `Magidesk.Tests.Workflows/Infrastructure/Seeding`.
