# Table Layout Designer - Layout Name vs Floor

## Quick Answer

**Layout Name** and **Floor** are two different concepts in the Table Designer:

- **Floor** = Physical dining area (e.g., "Main Dining Room", "Patio", "Second Floor")
- **Layout Name** = A specific table arrangement for that floor (e.g., "Lunch Setup", "Dinner Setup", "Weekend Configuration")

---

## Detailed Explanation

### Floor (Physical Space)
A **Floor** represents a physical dining area in your restaurant.

**Properties**:
- Name (e.g., "Main Dining Room", "Patio", "Bar Area")
- Physical dimensions (width x height in pixels)
- Background color
- Can have **multiple layouts**

**Example Floors**:
- Main Dining Room (2000x2000px)
- Outdoor Patio (1500x1500px)
- Private Dining Room (1000x1000px)
- Bar Area (800x1200px)

### Layout Name (Table Arrangement)
A **Layout Name** represents a specific table arrangement/configuration for a floor.

**Properties**:
- Name (e.g., "Lunch Setup", "Dinner Setup")
- Belongs to ONE floor
- Contains the actual table positions and properties
- Can be active or draft
- Can be versioned

**Example Layouts for "Main Dining Room" Floor**:
- "Weekday Lunch" - 20 tables, optimized for quick turnover
- "Weekend Dinner" - 15 tables, more spacing for comfort
- "Private Event" - 8 large tables for groups
- "Holiday Setup" - Special arrangement with extra tables

---

## Relationship

```
Floor (Physical Space)
  └── Layout 1: "Lunch Setup"
        └── Table 1, Table 2, Table 3...
  └── Layout 2: "Dinner Setup"
        └── Table 1, Table 2, Table 3...
  └── Layout 3: "Weekend Setup"
        └── Table 1, Table 2, Table 3...
```

**One Floor can have MULTIPLE Layouts**

---

## Real-World Use Case

### Scenario: Restaurant with Different Service Times

**Floor**: "Main Dining Room" (2000x2000px)

**Layouts**:

1. **"Breakfast Layout"**
   - 25 small 2-person tables
   - Quick turnover configuration
   - Active: Mon-Fri 6am-11am

2. **"Lunch Layout"**
   - 20 mixed tables (2-person and 4-person)
   - Efficient spacing
   - Active: Mon-Fri 11am-3pm

3. **"Dinner Layout"**
   - 15 larger tables (4-person and 6-person)
   - More spacing, romantic atmosphere
   - Active: Daily 5pm-10pm

4. **"Weekend Brunch"**
   - 18 tables with bar seating
   - Special configuration
   - Active: Sat-Sun 9am-3pm

**Why Multiple Layouts?**
- Different table arrangements for different times of day
- Optimize for different customer volumes
- Different service styles (quick vs. leisurely)
- Special events or seasons

---

## In the Designer UI

When you open the Table Designer:

1. **Floor Dropdown**: Select which physical area you're designing
   - Example: "Main Dining Room", "Patio", "Bar Area"

2. **Layout Name Field**: Name this specific arrangement
   - Example: "Lunch Setup", "Dinner Setup", "Holiday Configuration"

3. **Canvas**: Shows the floor dimensions and where you place tables
   - Size determined by the selected Floor's width/height
   - Background color from the selected Floor

---

## Database Structure

```sql
Floors
  - Id
  - Name (e.g., "Main Dining Room")
  - Width, Height
  - BackgroundColor

TableLayouts
  - Id
  - Name (e.g., "Lunch Setup")
  - FloorId (references Floors)
  - IsActive, IsDraft
  - Version

Tables
  - Id
  - TableLayoutId (references TableLayouts)
  - TableNumber
  - X, Y (position on canvas)
  - Capacity
  - Shape
```

---

## Key Differences Summary

| Aspect | Floor | Layout Name |
|--------|-------|-------------|
| **Represents** | Physical dining area | Table arrangement |
| **Quantity** | One per physical space | Multiple per floor |
| **Contains** | Dimensions, background | Table positions |
| **Example** | "Main Dining Room" | "Lunch Setup" |
| **Changes** | Rarely (only if renovating) | Frequently (different times/events) |
| **Active** | Always active if in use | One active layout per floor at a time |

---

## Best Practices

### Naming Floors
- Use descriptive names for physical areas
- Examples: "Main Dining Room", "Patio", "Private Room A", "Bar Area"

### Naming Layouts
- Use time-based or event-based names
- Examples: "Weekday Lunch", "Weekend Dinner", "Holiday Party", "Summer Patio"
- Include version numbers if needed: "Dinner Setup v2"

### Managing Multiple Layouts
- Keep one layout active per floor at a time
- Use draft mode when designing new layouts
- Version layouts when making significant changes
- Archive old layouts instead of deleting them

---

## Common Questions

**Q: Can I have the same layout name on different floors?**
A: Yes! "Lunch Setup" on "Main Dining Room" is different from "Lunch Setup" on "Patio"

**Q: How many layouts can one floor have?**
A: Unlimited, but typically 2-5 layouts per floor is common

**Q: Can tables move between floors?**
A: No, tables belong to a specific layout, which belongs to a specific floor

**Q: What happens when I switch layouts?**
A: The active layout changes, and the table map shows the new arrangement. Active tickets remain with their tables.

---

**Summary**: Think of **Floor** as the room and **Layout Name** as the furniture arrangement in that room. You can rearrange the furniture (create new layouts) without changing the room itself (floor).
