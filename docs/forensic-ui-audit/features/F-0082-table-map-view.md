# Feature: Table Map View (F-0082)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: EXISTS (TableWorkspacePage implemented)

## Problem / Why this exists (grounded)
- **Operational need**: Dine-in restaurants need visual floor layout showing table status (open, occupied, needs attention). Servers need to quickly identify their tables and start orders.
- **Evidence**: `TableMapView.java` - visual table floor layout; `ShopTableButton` for each table; color-coded status; click to start/resume order.

## User-facing surfaces
- **Surface type**: Screen/Panel
- **UI entry points**: LoginView → Dine-In order type; Switchboard → Table Map
- **Exit paths**: Select table (opens order); back to switchboard

## Preconditions & protections
- **User/role/permission checks**: Dine-in order permission
- **State checks**: Tables must be configured in system
- **Manager override**: Not required for table selection

## Step-by-step behavior (forensic)
1. User selects Dine-In order type or navigates to table map
2. TableMapView loads configured floor layout
3. Each table shown as ShopTableButton with:
   - Table number
   - Capacity
   - Current status (color-coded)
   - Server assignment (if applicable)
4. User clicks table to:
   - Start new order (if open)
   - Resume existing order (if occupied)
   - View order details
5. Order opened in OrderView for selected table

## Edge cases & failure paths
- **Table already has order**: Shows existing order
- **Table not configured**: Empty floor plan
- **No permissions for table**: Button disabled or hidden
- **Server section restrictions**: Only show assigned tables (if configured)

## Data / audit / financial impact
- **Writes/updates**: Ticket.tableNumbers association
- **Audit events**: Table assignment logged
- **Financial risk**: Low - operational workflow

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `TableMapView` → `ui/views/TableMapView.java`
- **Entry action(s)**: Order type selection; navigation
- **Workflow/service enforcement**: ShopTable entity; TableDAO
- **Messages/labels**: Table number, status labels

## Uncertainties (STOP; do not guess)
- Multiple floor support (may use FloorLayoutPlugin)
- Table merge/combine functionality

## MagiDesk parity notes
- **What exists today**: TableWorkspacePage with table grid
- **What differs / missing**: Visual floor layout designer; color-coded status may need enhancement

## Porting strategy (PLAN ONLY)
- **Backend requirements**: ShopTable entity with position, capacity; table status query
- **API/DTO requirements**: GET /tables (with status); GET /tables/{id}/tickets
- **UI requirements**: Visual table grid; status indicators; click to open order
- **Constraints for implementers**: Real-time status updates desirable; support multiple servers
