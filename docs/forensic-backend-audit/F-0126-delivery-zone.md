# Backend Forensic Audit: F-0126 Delivery Zone Configuration

## Feature Context
- **Feature**: Delivery Zone Configuration
- **Trace from**: `F-0126-delivery-zone-configuration.md`
- **Reference**: `DeliveryConfiguration.java`

## Backend Invariants
1.  **Logic**: Zip Code list or Polygon.
2.  **Fee**: Specific Delivery Charge per Zone.
3.  **Minimum**: Min Order Amount per Zone.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `DELIVERY_CONFIG_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ⚠️ `DriveZone` needed.

## Alignment Strategy
1.  **Implement** Zone Model.
