# Technical Design: Complex Menu Models

## 1. Overview
Enhances the product model to support "Pizza" (fractional modifiers) and "Combos" (fixed price bundles).

**Related Features:**
- F-0037: Pizza Modifiers (Half/Whole)
- F-0040: Combo Section

## 2. Domain Model

### 2.1. Entities

#### `FractionalModifier` (For Pizza)
- Extends `MenuModifier`
- `portion`: Enum (WHOLE, LEFT_HALF, RIGHT_HALF, QUARTER_1...)
- `priceStrategy`: Enum (AVG_OF_HALVES, HIGHEST_HALF, SUM)

#### `ComboDefinition`
- `id`: UUID
- `name`: String ("Lunch Deal")
- `price`: Decimal ($10.99)
- `groups`: List<ComboGroup> (e.g., "Main", "Side", "Drink")

#### `ComboGroup`
- `name`: String
- `items`: List<MenuItem>
- `upcharge`: Map<MenuItem, Decimal> (e.g., "Steak +$2.00")

### 2.2. Services

#### `PriceCalculator` (Update)
- **Pizza Logic**:
    - If `TicketItem` has fractional modifiers:
    - BasePrice = PizzaBase.
    - ModPrice = (ModLeft.Price * 0.5) + (ModRight.Price * 0.5).
- **Combo Logic**:
    - If `TicketItem` is a Combo:
    - BasePrice = Combo.FixedPrice.
    - Add Upsells from `ComboGroup` selections.
    - Sub-items priced at $0.00 on the receipt.

## 3. UI Implications
- **Pizza View**: Needs a visual "Pizza Builder" (Circle) to select Left/Right.
- **Combo View**: Wizard-style (Step 1: Choose Main, Step 2: Choose Side).

## 4. Migration
- Database Schema update to `menu_modifiers` (add `portion` column).
- New tables `combo_definitions`, `combo_groups`.

## 5. API Extensions
- `GET /api/menu/combos`: List active combos.
- `POST /api/tickets/item/combo`: Add fully configured combo to ticket.
