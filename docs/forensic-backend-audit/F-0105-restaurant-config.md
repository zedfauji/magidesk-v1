# Backend Forensic Audit: F-0105 Restaurant Configuration View

## Feature Context
- **Feature**: Restaurant Configuration View
- **Trace from**: `F-0105-restaurant-configuration-view.md`
- **Reference**: `RestaurantConfiguration.java`

## Backend Invariants
1.  **Singleton**: Typically one global config row/file.
2.  **Fields**: Name, Address, Phone, Default Currency.

## Forbidden States
-   **Null Config**: System cannot boot without valid config.

## Audit Requirements
-   **Event**: `CONFIG_UPDATED`.

## Concurrency Semantics
-   **Cache**: On update, broadcast "Config Changed" to flush caches on all terminals.

## MagiDesk Backend Parity
-   **Service**: ✅ `ConfigurationService`.

## Alignment Strategy
1.  **None**.
