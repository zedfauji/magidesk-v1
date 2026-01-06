# Table Designer UX Workflow (Admin-Only)

This document defines the operator-proof workflow for administrators to design and publish floor plans.

## 1. The Workflow

| Step | Action | Allowed Actions | Blocked Actions |
|------|--------|-----------------|-----------------|
| **1** | **Select Floor** | Change Floor selection, View current layout. | Add Table, Drag Table, Edit Properties. |
| **2** | **Enter Design Mode** | Click "Edit Layout" button. | Switching floors (must confirm exit design mode). |
| **3** | **Add Table** | Select shape from palette, Click on canvas to drop. | Saving (if layout is invalid). |
| **4** | **Configure Properties**| Set Table Number, Capacity, Shape properties. | Dragging *other* tables while one is being configured. |
| **5** | **Place Table** | Drag & drop to final position. | Overlapping existing tables (Visual warning required). |
| **6** | **Save Layout** | Click "Save Changes". Validates all constraints. | Exiting without saving (Must prompt). |
| **7** | **Exit Design Mode**| Return to read-only state. | Any mutations. |

## 2. Dynamic State Enforcement

- **Idle (Read-Only)**: Table buttons are just visual indicators. Commands like `SelectTable` are disabled or toggle an "Inspector" view only.
- **Design (Write)**:
    - Tables become grabbable.
    - Right-click or Side-panel opens for configuration.
    - `Delete` key support enabled for selected items.

## 3. The "Seat" Safety Lock

- **Rule**: A table that is **Occupied** (Status: Seated, Dirty) in the database MUST NOT be deletable or movable in Design Mode without an explicit confirmation.
- **Visual**: Occupied tables should be visually dimmed or locked with a "Lock" icon in the designer to indicate they are active in the live environment.
