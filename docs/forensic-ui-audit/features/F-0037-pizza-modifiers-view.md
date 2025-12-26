# Feature: Pizza Modifiers View (F-0037)

## Classification
- **Parity classification**: DEFER (specialty feature)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Pizza items have unique modification needs - half-and-half toppings, extra cheese on one side, crust types.
- **Evidence**: `PizzaModifierView.java` - pizza-specific UI for topping placement (left/right/whole).

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: Add pizza item → auto-open
- **Exit paths**: Pizza configured / Cancel

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Item is pizza type
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User adds pizza item
2. PizzaModifierView opens
3. Shows components:
   - Size selection (if applicable)
   - Crust type
   - Sauce selection
   - Topping grid with sections:
     - Whole pizza
     - Left half
     - Right half
   - Quantity per topping
4. User configures pizza
5. Price calculated based on selections
6. On done: Pizza added with all modifiers

## Edge cases & failure paths
- **No toppings selected**: May be allowed (plain)
- **Too many toppings**: Price calculation continues
- **Complex half-and-half**: Must track per section

## Data / audit / financial impact
- **Writes/updates**: TicketItem with pizza modifiers and section data
- **Audit events**: Part of order
- **Financial risk**: Complex pricing

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `PizzaModifierView` → `ui/views/order/PizzaModifierView.java`
- **Entry action(s)**: Pizza item selection
- **Workflow/service enforcement**: Pizza modifier logic
- **Messages/labels**: Topping names, section labels

## MagiDesk parity notes
- **What exists today**: No pizza-specific modifiers
- **What differs / missing**: Entire pizza modifier system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - PizzaModifier entity with section (WHOLE/LEFT/RIGHT)
  - Complex pricing for fractional count
- **API/DTO requirements**: Pizza modifier structure
- **UI requirements**: PizzaModifierView with section grid
- **Constraints for implementers**: Kitchen ticket must show sections clearly
