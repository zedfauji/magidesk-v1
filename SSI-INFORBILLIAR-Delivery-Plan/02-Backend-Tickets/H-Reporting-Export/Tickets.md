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

## BE-H.2-01: Create Shift Summary Report

**Ticket ID:** BE-H.2-01  
**Feature ID:** H.2  
**Type:** Backend  
**Title:** Create Shift Summary Report  
**Priority:** P1

### Outcome
Shift report showing server performance and sales.

### Scope
- Create GetShiftSummaryQuery
- Include cash drawer reconciliation
- Server sales breakdown
- Payment method totals

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | CashSession entity | Exists |

### Acceptance Criteria
- [ ] Shift summary generated
- [ ] Cash reconciliation accurate
- [ ] Server breakdown included
- [ ] Payment totals correct

---

## BE-H.3-01: Create Server Performance Report

**Ticket ID:** BE-H.3-01  
**Feature ID:** H.3  
**Type:** Backend  
**Title:** Create Server Performance Report  
**Priority:** P2

### Outcome
Analytics on individual server sales and performance.

### Scope
- Create GetServerPerformanceQuery
- Track sales per server
- Track tips per server
- Compare server metrics

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Server assignment | BE-A.13-01 |

### Acceptance Criteria
- [ ] Server stats calculated
- [ ] Ranking functional
- [ ] Date range filtering works
- [ ] Tip tracking accurate

---

## BE-H.7-01: Complete Inventory Report

**Ticket ID:** BE-H.7-01  
**Feature ID:** H.7  
**Type:** Backend  
**Title:** Complete Inventory Report  
**Priority:** P2

### Outcome
Comprehensive inventory status report.

### Scope
- Create GetInventoryReportQuery
- Show current stock levels
- Include low stock items
- Calculate inventory value

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Stock tracking | BE-G.2-01 |

### Acceptance Criteria
- [ ] Stock levels accurate
- [ ] Value calculation correct
- [ ] Low stock highlighted
- [ ] Export capability works

---

## BE-H.8-01: Complete Tax Report

**Ticket ID:** BE-H.8-01  
**Feature ID:** H.8  
**Type:** Backend  
**Title:** Complete Tax Report  
**Priority:** P2

### Outcome
Tax collection report for compliance.

### Scope
- Create GetTaxReportQuery
- Breakdown by tax rate
- Total tax collected
- Export for filing

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Tax calculation | Exists |

### Acceptance Criteria
- [ ] Tax breakdown accurate
- [ ] All rates included
- [ ] Date range works
- [ ] Export format compliant

---

## BE-H.10-01: Implement PDF Export

**Ticket ID:** BE-H.10-01  
**Feature ID:** H.10  
**Type:** Backend  
**Title:** Implement PDF Export  
**Priority:** P2

### Outcome
Export reports to PDF format.

### Scope
- Create PDF generation service
- Template-based generation
- Logo and branding support
- Print-ready formatting

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Report queries | Various |

### Acceptance Criteria
- [ ] PDF generation works
- [ ] Templates customizable
- [ ] Branding included
- [ ] Print quality acceptable

---

## BE-H.11-01: Implement Excel Export

**Ticket ID:** BE-H.11-01  
**Feature ID:** H.11  
**Type:** Backend  
**Title:** Implement Excel Export  
**Priority:** P2

### Outcome
Export reports to Excel format.

### Scope
- Create Excel export service
- Support multiple sheets
- Formatting preserved
- Formulas included

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Report queries | Various |

### Acceptance Criteria
- [ ] Excel generation works
- [ ] Multiple sheets supported
- [ ] Formatting correct
- [ ] Opens in Excel/LibreOffice

---

## BE-H.12-01: Payment Method Breakdown Report

**Ticket ID:** BE-H.12-01  
**Feature ID:** H.12  
**Type:** Backend  
**Title:** Payment Method Breakdown Report  
**Priority:** P2

### Outcome
Report showing sales by payment method.

### Scope
- Create GetPaymentMethodReportQuery
- Breakdown by method (cash, card, etc.)
- Include transaction counts
- Show fees/costs

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Payment entity | Exists |

### Acceptance Criteria
- [ ] All payment methods listed
- [ ] Amounts accurate
- [ ] Transaction counts correct
- [ ] Fee tracking works

---

## BE-H.13-01: Discount Usage Report

**Ticket ID:** BE-H.13-01  
**Feature ID:** H.13  
**Type:** Backend  
**Title:** Discount Usage Report  
**Priority:** P2

### Outcome
Track discount and promotion effectiveness.

### Scope
- Create GetDiscountUsageQuery
- Show usage by discount type
- Calculate revenue impact
- Identify popular promotions

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Discount application | BE-C.7-01 |

### Acceptance Criteria
- [ ] All discounts tracked
- [ ] Impact calculated
- [ ] Usage frequency shown
- [ ] ROI analysis included

---

## BE-H.14-01: Hourly Sales Trend Report

**Ticket ID:** BE-H.14-01  
**Feature ID:** H.14  
**Type:** Backend  
**Title:** Hourly Sales Trend Report  
**Priority:** P2

### Outcome
Sales patterns by hour of day.

### Scope
- Create GetHourlySalesTrendQuery
- Group sales by hour
- Show peak hours
- Compare weekday vs weekend

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Sales data | Exists |

### Acceptance Criteria
- [ ] Hourly breakdown accurate
- [ ] Peak hours identified
- [ ] Day type comparison works
- [ ] Trends visualizable

---

## BE-H.15-01: Product Popularity Report

**Ticket ID:** BE-H.15-01  
**Feature ID:** H.15  
**Type:** Backend  
**Title:** Product Popularity Report  
**Priority:** P2

### Outcome
Top selling products and trends.

### Scope
- Create GetProductPopularityQuery
- Rank by quantity sold
- Rank by revenue
- Track trends over time

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | OrderLine entity | Exists |

### Acceptance Criteria
- [ ] Rankings accurate
- [ ] Multiple sort options
- [ ] Trend calculation works
- [ ] Category filtering works

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P1 | 6 | NOT_STARTED |
| P2 | 9 | NOT_STARTED |
| **Total** | **15** | **NOT_STARTED** |

---

*Last Updated: 2026-01-10*
