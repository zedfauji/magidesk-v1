# Extended Forensic Failure Audit - Complete Inventory
## Remaining Files to Analyze

**Date**: 2026-01-06  
**Status**: Discovery Phase Continuing

---

## SERVICES LAYER (93 files remaining)

### Command Handlers (57 total)
**Analyzed**: 4 (ProcessPayment, RefundPayment, CloseCashSession, CreateTicket)  
**Remaining**: 53

#### Financial Operations (HIGH PRIORITY - 15 files)
1. AddCashDropCommandHandler.cs
2. AddDrawerBleedCommandHandler.cs
3. AddPayoutCommandHandler.cs
4. AddTipsToCardPaymentCommandHandler.cs
5. ApplyCouponCommandHandler.cs
6. ApplyDiscountCommandHandler.cs
7. AuthorizeCardPaymentCommandHandler.cs
8. CalculateServiceChargeCommandHandler.cs
9. CaptureCardPaymentCommandHandler.cs
10. PayNowCommandHandler.cs
11. RefundTicketCommandHandler.cs
12. SetAdjustmentCommandHandler.cs
13. SetAdvancePaymentCommandHandler.cs
14. SetDeliveryChargeCommandHandler.cs
15. SetServiceChargeCommandHandler.cs

#### Ticket Operations (MEDIUM PRIORITY - 12 files)
1. AssignTableToTicketCommandHandler.cs
2. CloseTicketCommandHandler.cs
3. MergeTicketsCommandHandler.cs
4. SettleTicketCommandHandler.cs
5. SplitTicketCommandHandler.cs
6. TransferTicketCommandHandler.cs
7. TransferTicketToTableCommandHandler.cs
8. SetCustomerCommandHandler.cs
9. SetTaxExemptCommandHandler.cs
10. AddOrderLineCommandHandler.cs
11. AddOrderLineComboCommandHandler.cs
12. ModifyOrderLineCommandHandler.cs

#### Table & Session Operations (MEDIUM PRIORITY - 8 files)
1. ChangeTableCommandHandler.cs
2. ChangeSeatCommandHandler.cs
3. ReleaseTableCommandHandler.cs
4. CreateTableCommandHandler.cs
5. CreateTableLayoutCommandHandler.cs
6. OpenCashSessionCommandHandler.cs
7. CreateShiftCommandHandler.cs
8. LogoutCommandHandler.cs

#### Printing & Kitchen (LOW PRIORITY - 3 files)
1. PrintReceiptCommandHandler.cs
2. PrintToKitchenCommandHandler.cs
3. RemoveOrderLineCommandHandler.cs

#### System Configuration (LOW PRIORITY - 8 files)
1. System/CreateSystemBackupCommandHandler.cs
2. System/RestoreSystemBackupCommandHandler.cs
3. System/UpdateRestaurantConfigCommandHandler.cs
4. System/UpdateTerminalConfigCommandHandler.cs
5. SystemConfig/UpdateCardConfigCommandHandler.cs
6. SystemConfig/UpdatePrinterGroupsCommandHandler.cs
7. SystemConfig/UpdatePrinterMappingsCommandHandler.cs
8. CreateOrderTypeCommandHandler.cs

#### Other (7 files - not shown in truncated list)

---

### Query Handlers (36 total)
**Analyzed**: 0  
**Remaining**: 36

#### Core Queries (HIGH PRIORITY - 8 files)
1. GetTicketQueryHandler.cs
2. GetTicketByNumberQueryHandler.cs
3. GetOpenTicketsQueryHandler.cs
4. GetCashSessionQueryHandler.cs
5. GetCurrentCashSessionQueryHandler.cs
6. GetCurrentShiftQueryHandler.cs
7. GetShiftQueryHandler.cs
8. GetUsersQueryHandler.cs

#### Table & Configuration Queries (MEDIUM PRIORITY - 6 files)
1. GetTableMapQueryHandler.cs
2. GetTableQueryHandler.cs
3. GetAvailableTablesQueryHandler.cs
4. GetOrderTypeQueryHandler.cs
5. System/GetRestaurantConfigQueryHandler.cs
6. System/GetTerminalConfigQueryHandler.cs

#### Report Queries (LOW PRIORITY - 19 files)
1. GetDrawerPullReportQueryHandler.cs
2. GetShiftReportQueryHandler.cs
3. Reports/GetAttendanceReportQueryHandler.cs
4. Reports/GetCashOutReportQueryHandler.cs
5. Reports/GetCreditCardReportQueryHandler.cs
6. Reports/GetDeliveryReportQueryHandler.cs
7. Reports/GetExceptionsReportQueryHandler.cs
8. Reports/GetHourlyLaborReportQueryHandler.cs
9. Reports/GetJournalReportQueryHandler.cs
10. Reports/GetLaborReportQueryHandler.cs
11. Reports/GetMenuUsageReportQueryHandler.cs
12. Reports/GetPaymentReportQueryHandler.cs
13. Reports/GetProductivityReportQueryHandler.cs
14. Reports/GetSalesBalanceQueryHandler.cs
15. Reports/GetSalesDetailQueryHandler.cs
16. Reports/GetSalesSummaryQueryHandler.cs
17. Reports/GetServerProductivityReportQueryHandler.cs
18. Reports/GetTipReportQueryHandler.cs
19. SystemConfig/GetCardConfigQueryHandler.cs

#### Other (3 files)
1. System/GetSystemBackupsQueryHandler.cs
2. SystemConfig/GetPrinterGroupsQueryHandler.cs
3. SystemConfig/GetPrinterMappingsQueryHandler.cs

---

## REPOSITORIES LAYER (31 files)

**Analyzed**: 0 (pattern search only - SaveChangesAsync calls identified)  
**Remaining**: 31 (all require deep analysis)

### Core Repositories (HIGH PRIORITY - 8 files)
1. TicketRepository.cs
2. PaymentRepository.cs
3. CashSessionRepository.cs
4. TableRepository.cs
5. ShiftRepository.cs
6. UserRepository.cs
7. OrderTypeRepository.cs
8. AuditEventRepository.cs

### Menu & Inventory (MEDIUM PRIORITY - 7 files)
1. MenuRepository.cs
2. MenuCategoryRepository.cs
3. MenuGroupRepository.cs
4. MenuModifierRepository.cs
5. ModifierGroupRepository.cs
6. InventoryItemRepository.cs
7. DiscountRepository.cs

### Configuration & System (LOW PRIORITY - 11 files)
1. RestaurantConfigurationRepository.cs
2. TerminalRepository.cs
3. FloorRepository.cs
4. TableLayoutRepository.cs
5. PrinterGroupRepository.cs
6. PrinterMappingRepository.cs
7. PrintTemplateRepository.cs
8. MerchantGatewayConfigurationRepository.cs
9. ServerSectionRepository.cs
10. RoleRepository.cs
11. SalesReportRepository.cs

### Other (5 files)
1. AttendanceRepository.cs
2. InMemoryAttendanceRepository.cs
3. KitchenOrderRepository.cs
4. GroupSettlementRepository.cs
5. PaymentBatchRepository.cs

---

## CONTROLLERS LAYER (6 files - API)

**Analyzed**: 0  
**Remaining**: 6 (all require analysis)

**Location**: `Magidesk.Api/Controllers/`

**Expected Files** (based on typical API structure):
1. TicketsController.cs
2. PaymentsController.cs
3. TablesController.cs
4. MenuController.cs
5. ReportsController.cs
6. SystemController.cs

---

## VIEWS LAYER (73 XAML files)

**Analyzed**: 0  
**Remaining**: 73 (all require analysis for binding errors, event handlers)

**Location**: `Magidesk/Views/`

**Analysis Focus**:
- Event handler exception handling
- Binding error handling
- Navigation error handling
- Dialog error handling

---

## ANALYSIS PRIORITY MATRIX

| Priority | Area | Files | Est. Hours | Rationale |
|----------|------|-------|------------|-----------|
| **CRITICAL** | Financial Command Handlers | 15 | 4-5 | Money operations must never fail silently |
| **HIGH** | Core Query Handlers | 8 | 2-3 | Data retrieval failures affect all operations |
| **HIGH** | Core Repositories | 8 | 2-3 | Database failures must be visible |
| **HIGH** | Ticket Command Handlers | 12 | 3-4 | Order operations are critical |
| **MEDIUM** | Table/Session Handlers | 8 | 2-3 | Operational but not financial |
| **MEDIUM** | Menu/Inventory Repos | 7 | 2 | Configuration data |
| **MEDIUM** | Controllers | 6 | 1-2 | API layer exception handling |
| **LOW** | Report Queries | 19 | 3-4 | Read-only, non-critical |
| **LOW** | System Config | 19 | 3-4 | Infrequent operations |
| **LOW** | Views (XAML) | 73 | 3-4 | UI binding errors (non-critical) |
| **TOTAL** | | **175** | **25-35** | **Remaining discovery work** |

---

## ESTIMATED FINDINGS

Based on current findings rate (18 findings in 98 files = 18.4% finding rate):

| Area | Files | Est. Findings |
|------|-------|---------------|
| Financial Handlers | 15 | 3-5 |
| Ticket Handlers | 12 | 2-4 |
| Core Queries | 8 | 1-2 |
| Core Repositories | 8 | 1-2 |
| Table/Session Handlers | 8 | 1-2 |
| Controllers | 6 | 1-2 |
| Other Services | 45 | 5-10 |
| Other Repositories | 23 | 2-5 |
| Views | 73 | 5-10 |
| **TOTAL** | **175** | **21-42** |

**Current Findings**: 18  
**Estimated Total**: 39-60 findings

---

## RECOMMENDED ANALYSIS SEQUENCE

### Phase 1: Critical Financial Operations (6-8 hours)
1. Analyze all 15 financial command handlers
2. Analyze 8 core repositories
3. Expected findings: 4-7

### Phase 2: Core Operations (5-7 hours)
1. Analyze 12 ticket command handlers
2. Analyze 8 core query handlers
3. Expected findings: 3-6

### Phase 3: Supporting Operations (4-6 hours)
1. Analyze 8 table/session handlers
2. Analyze 6 API controllers
3. Expected findings: 2-4

### Phase 4: Configuration & Reports (6-8 hours)
1. Analyze remaining 45 service handlers
2. Analyze 23 configuration repositories
3. Expected findings: 7-15

### Phase 5: UI Layer (3-4 hours)
1. Analyze 73 XAML views
2. Expected findings: 5-10

**Total Estimated**: 24-33 hours for complete discovery

---

## CURRENT STATUS

**Completed**: 30% (98 of 329 files)  
**Remaining**: 70% (231 files)  
**Findings So Far**: 18 (9 fixed, 9 pending)  
**Estimated Total Findings**: 39-60

---

**Last Updated**: 2026-01-06 13:10 CST
