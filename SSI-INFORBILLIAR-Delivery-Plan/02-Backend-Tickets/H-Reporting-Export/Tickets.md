# Backend Tickets: Category H - Reporting & Export

> [!NOTE]
> This category has 18.2% parity (2 full, 6 partial, 3 not implemented). Major work needed on analytics.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-H.1-01 | H.1 | Complete Daily Sales Report | P1 | NOT_STARTED |
| BE-H.2-01 | H.2 | Create Shift Summary Report | P1 | NOT_STARTED |
| BE-H.3-01 | H.3 | Create Server Performance Report | P2 | NOT_STARTED |
| BE-H.4-01 | H.4 | Complete Table Utilization Report | P1 | NOT_STARTED |
| BE-H.5-01 | H.5 | Create Time-Based Revenue Report | P1 | NOT_STARTED |
| BE-H.6-01 | H.6 | Create Member Activity Report | P1 | NOT_STARTED |
| BE-H.7-01 | H.7 | Complete Inventory Report | P2 | NOT_STARTED |
| BE-H.8-01 | H.8 | Complete Tax Report | P2 | NOT_STARTED |
| BE-H.10-01 | H.10 | Implement PDF Export | P2 | NOT_STARTED |
| BE-H.11-01 | H.11 | Implement Excel Export | P2 | NOT_STARTED |

---

## BE-H.1-01: Complete Daily Sales Report

**Ticket ID:** BE-H.1-01  
**Feature ID:** H.1  
**Type:** Backend  
**Title:** Complete Daily Sales Report  
**Priority:** P1

### Outcome (measurable, testable)
Comprehensive daily sales report with breakdown by hour, table, and category.

### Scope
- Create `GetDailySalesReportQuery`
- Include hourly breakdown
- Include table breakdown for time charges
- Include product category breakdown
- Include payment method breakdown

### Current State (Partial)
- Basic sales data exists
- **Missing:** Full breakdown, time charge analytics

### Implementation Notes
```csharp
public record GetDailySalesReportQuery(DateTime Date);

public record DailySalesReportDto(
    DateTime Date,
    Money TotalSales,
    Money TotalTimeSales,
    Money TotalProductSales,
    Money TotalTax,
    Money TotalGratuity,
    int TotalTransactions,
    int TotalCustomers,
    IEnumerable<HourlySales> HourlyBreakdown,
    IEnumerable<CategorySales> CategoryBreakdown,
    IEnumerable<PaymentMethodSales> PaymentBreakdown,
    IEnumerable<TableSales> TableBreakdown
);
```

### Acceptance Criteria
- [ ] Report generates correctly
- [ ] Time charges included
- [ ] All breakdowns accurate
- [ ] Performance < 2 seconds
- [ ] Tests verify calculations

---

## BE-H.4-01: Complete Table Utilization Report

**Ticket ID:** BE-H.4-01  
**Feature ID:** H.4  
**Type:** Backend  
**Title:** Complete Table Utilization Report  
**Priority:** P1

### Outcome (measurable, testable)
Report showing table usage patterns and efficiency.

### Scope
- Create `GetTableUtilizationReportQuery`
- Calculate occupancy percentages
- Track average session duration
- Identify peak usage times

### Current State (Not Implemented)
- No table session analytics exist

### Implementation Notes
```csharp
public record GetTableUtilizationReportQuery(
    DateTime StartDate,
    DateTime EndDate
);

public record TableUtilizationReportDto(
    IEnumerable<TableUtilizationEntry> Tables,
    decimal OverallOccupancyPercent,
    TimeSpan AverageSessionDuration,
    HourlyOccupancy PeakHours
);

public record TableUtilizationEntry(
    Guid TableId,
    string TableName,
    int TotalSessions,
    TimeSpan TotalUsageTime,
    decimal OccupancyPercent,
    TimeSpan AverageSessionDuration,
    Money TotalRevenue
);
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableSession entity | BE-A.1-01 |

### Acceptance Criteria
- [ ] Utilization calculated correctly
- [ ] Peak hours identified
- [ ] Per-table breakdown works
- [ ] Date range filtering works

---

## BE-H.5-01: Create Time-Based Revenue Report

**Ticket ID:** BE-H.5-01  
**Feature ID:** H.5  
**Type:** Backend  
**Title:** Create Time-Based Revenue Report  
**Priority:** P1

### Outcome (measurable, testable)
Report specifically for time-based charges and billiard revenue.

### Scope
- Create time charge analytics query
- Break down by table type
- Show rate effectiveness
- Compare weekday vs. weekend

### Current State (Not Implemented)
- No time charge analytics exist

### Implementation Notes
```csharp
public record GetTimeRevenueReportQuery(
    DateTime StartDate,
    DateTime EndDate
);

public record TimeRevenueReportDto(
    Money TotalTimeRevenue,
    TimeSpan TotalBilledTime,
    decimal AverageHourlyRevenue,
    IEnumerable<TableTypeRevenue> ByTableType,
    IEnumerable<DayOfWeekRevenue> ByDayOfWeek,
    IEnumerable<HourlyRevenue> ByHourOfDay
);
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableSession entity | BE-A.1-01 |
| HARD | TableType entity | BE-A.5-01 |

### Acceptance Criteria
- [ ] Time revenue calculated accurately
- [ ] Table type breakdown works
- [ ] Day of week analysis works
- [ ] Peak hours identified

---

## BE-H.6-01: Create Member Activity Report

**Ticket ID:** BE-H.6-01  
**Feature ID:** H.6  
**Type:** Backend  
**Title:** Create Member Activity Report  
**Priority:** P1

### Outcome (measurable, testable)
Report showing member engagement, retention, and value.

### Scope
- Create member analytics query
- Track visit frequency
- Calculate member value
- Identify at-risk members (no recent visits)

### Current State (Not Implemented)
- No member analytics exist

### Implementation Notes
```csharp
public record GetMemberActivityReportQuery(
    DateTime StartDate,
    DateTime EndDate
);

public record MemberActivityReportDto(
    int TotalActiveMembers,
    int NewMembersInPeriod,
    int ChurnedMembers,
    Money TotalMemberRevenue,
    decimal MemberRevenuePercent,  // of total
    IEnumerable<TopMember> TopMembers,
    IEnumerable<AtRiskMember> AtRiskMembers,
    IEnumerable<TierBreakdown> ByTier
);
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Member entity | BE-F.3-01 |
| HARD | Customer entity | BE-F.1-01 |

### Acceptance Criteria
- [ ] Active members counted correctly
- [ ] At-risk members identified (no visit in 30 days)
- [ ] Revenue attribution correct
- [ ] Tier breakdown accurate

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P1 | 5 | NOT_STARTED |
| P2 | 5 | NOT_STARTED |
| **Total** | **10** | **NOT_STARTED** |

---

*Last Updated: 2026-01-08*
