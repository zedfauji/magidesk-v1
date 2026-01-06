# Table Designer Parity Gaps Audit

## 1. Model Parity (Domain vs Application)

| Field | Table Entity (Domain) | TableDto (Application) | Gap / Risk |
|-------|-----------------------|------------------------|------------|
| **X / Y** | `double` | `double` | **Resolved.** Domain upgraded to double for precise tracking. |
| **Width / Height** | `double` | `double` | **Resolved.** Synchronized as double. |
| **FloorId** | `Guid?` | `Guid?` | No gap. |
| **LayoutId** | `Guid?` | `Guid?` | **Resolved.** Added to TableDto. |
| **Status** | `TableStatus` | `TableStatus` | No gap. |

## 2. Shape Enum Parity

- **Enum**: `TableShapeType`
- **Values**: Rectangle, Square, Round, Oval.
- **Status**: Enums are shared across both views. No parity gap found in the enum definition.

## 3. Coordinate System & Branding

- **Designer**: Uses absolute positioning on a Canvas. 
- **Map**: Uses absolute positioning on a Canvas.
- **Problem**: `TableMapPage.xaml` was hardcoded to 2000x2000 (partially fixed in previous task), but the Designer permits placement outside bounds.

## 4. State Synchronization

- **Designer Query**: `GetActiveAsync` (Modified in previous task to show occupied tables).
- **Map Query**: `GetActiveAsync`.
- **Gap**: **Resolved.** Drafting separation implemented via `IsDraft` flag in `TableLayout` and filtering in `Floor.GetActiveLayout()`. |

## 5. Metadata Blindspots

- `Seats` count: Not explicitly defined in `TableDto` but present in `Table` entity as `Capacity`.
- `DisplayNumber`: Maps to `TableNumber` (int). No gap.
- `IsActive`: Synchronized.
