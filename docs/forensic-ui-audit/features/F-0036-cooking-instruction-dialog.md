# Feature: Cooking Instruction Dialog (F-0036)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Pre-defined cooking instructions (Medium Rare, No Ice, Extra Hot). Faster than free-text notes.
- **Evidence**: `CookingInstructionSelectionView.java` - button grid of cooking instructions; prints on kitchen ticket.

## User-facing surfaces
- **Surface type**: Modal dialog or inline view
- **UI entry points**: Item selected → Cooking button; auto-open for food items
- **Exit paths**: Instructions selected / Done

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Item selected that supports cooking instructions
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User adds food item (steak)
2. CookingInstructionDialog opens
3. Shows available instructions:
   - Rare, Medium Rare, Medium, Well Done
   - Other item-specific options
4. User taps selection(s)
5. On done:
   - Instructions attached to ticket item
   - Displayed in ticket view
   - Printed on kitchen ticket
6. No additional charge typically

## Edge cases & failure paths
- **No instructions available**: Dialog not shown
- **Conflicting instructions**: May allow or prevent
- **Custom text**: May have free-text option

## Data / audit / financial impact
- **Writes/updates**: TicketItem.cookingInstructions
- **Audit events**: Part of order
- **Financial risk**: None (informational)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `CookingInstructionSelectionView` → `ui/views/order/CookingInstructionSelectionView.java`
- **Entry action(s)**: Item-specific trigger
- **Workflow/service enforcement**: Ticket item update
- **Messages/labels**: Instruction names

## MagiDesk parity notes
- **What exists today**: Notes field
- **What differs / missing**: Pre-defined cooking instruction dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: CookingInstruction entity; item-instruction mapping
- **API/DTO requirements**: Instructions in order item
- **UI requirements**: CookingInstructionDialog with button grid
- **Constraints for implementers**: Item-specific instructions; prints on kitchen ticket
