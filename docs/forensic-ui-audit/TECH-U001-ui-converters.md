# TECH-U001: UI Value Converters Implementation

## Description
Implement the 13+ value converters that currently throw `NotImplementedException`. These are critical for data binding visibility, formatting, and color logic.

## Scope
-   **Directory**: `Magidesk/Converters`
-   **Key Converters**:
    -   `CurrencyConverter`
    -   `DateTimeToTimeConverter`
    -   `StringToBoolConverter`
    -   `NullToVisibilityConverter`
    -   `CollectionEmptyToVisibilityConverter`
    -   `TableStatusToBrushConverter`

## Implementation Tasks
- [ ] Implement `Convert` methods for all identified stubs.
- [ ] Implement `ConvertBack` where two-way binding is required (or return `DependencyProperty.UnsetValue`).
- [ ] Add robust null checking to prevent runtime crashes.

## Acceptance Criteria
-   No `NotImplementedException` thrown during UI rendering.
-   Currency displays correctly (e.g., "$12.50").
-   Null values correctly toggle visibility.
