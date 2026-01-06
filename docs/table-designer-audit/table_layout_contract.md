# Table Layout Contract (Single Source of Truth)

This document defines the ONLY valid contract for persisting and exchanging table layouts within Magidesk. No UI-specific fields or WinRT types are permitted in the core data model.

## 1. TableLayout Structure

| Field | Type | Description |
|-------|------|-------------|
| **LayoutId** | `Guid` | Unique identifier for the layout configuration. |
| **FloorId** | `Guid` | Identity of the floor this layout belongs to. |
| **Name** | `string` | Human-readable name (e.g., "Main Lunch", "Event Setup"). |
| **IsActive** | `bool` | Whether this is the currently live layout for the floor. |
| **Tables** | `Array` | Collection of Table definitions. |

## 2. Table Definition

Each entry in the `Tables` array must conform to the following:

| Field | Type | Description |
|-------|------|-------------|
| **TableId** | `Guid` | Permanent identity of the physical table. |
| **DisplayNumber** | `int` | Human-readable table identifier. |
| **ShapeType** | `ENUM` | `Rectangle`, `Square`, `Round`, `Oval`. |
| **X** | `double` | X-coordinate on the floor grid (Upgraded to Double). |
| **Y** | `double` | Y-coordinate on the floor grid (Upgraded to Double). |
| **Width** | `double` | Width of the table hit-box. |
| **Height** | `double` | Height of the table hit-box. |
| **Capacity** | `int` | Max capacity (Mapped to `Seats` in UI). |
| **IsActive** | `bool` | Soft-delete flag. |

## 3. Serialization Rules

1. **Numeric Precision**: All coordinates and dimensions ARE serialized as `double`. This upgrade was made to ensure sub-pixel precision for complex designers and prevents "jagged" animation artifacts.
2. **Enum Handling**: Enums must be serialized as strings or consistent integers defined in `Magidesk.Domain.Enumerations`.
3. **No View State**: Fields like `IsSelected`, `ZIndex`, `ColorOverride`, or `DragOffset` are PROHIBITED from the persistence contract.
