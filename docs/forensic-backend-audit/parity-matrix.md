# Backend Parity Matrix (Consolidated F-0001 to F-0132)

| Feature | Name | Parity Status | Critical Gap / Risk |
| :--- | :--- | :--- | :--- |
| **F-0001** | Bootstrap | ✅ Parity | DB Connection handling |
| **F-0002** | Main Window | ✅ Parity | Terminal Status Broadcast |
| **F-0003** | Login Screen | ✅ Parity | Session Constraints |
| **F-0004** | Switchboard | ⚠️ Partial | Ticket Locking (Concurrency) |
| **F-0005** | Order Entry | ✅ Parity | State Machine enforcement |
| **F-0006** | Ticket Panel | ✅ Parity | Atomic Save+Pay workflow |
| **F-0007** | Payment Keypad | ⚠️ Partial | Non-Cash Overpayment rules |
| **F-0008** | Settle Dialog | ✅ Parity | Drawer attribution |
| **F-0009** | Manager Functions | ⚠️ Review | Granular RBAC Permissions |
| **F-0010** | Switchboard Panel | ✅ Parity | User Context binding |
| **F-0011** | Open Tickets | ✅ Parity | Unrestricted Manager View |
| **F-0012** | Drawer Pull | ⚠️ Missing | Snapshot Calculation Logic |
| **F-0013** | Void Ticket | ✅ Parity | Inventory Reversal |
| **F-0014** | Split Ticket | ✅ Parity | Transaction Atomicity |
| **F-0015** | Pay Wait Dialog | ✅ Parity | Optimistic Locking |
| **F-0016** | Swipe Card | ⚠️ Risk | Secure Data Handling (Logs) |
| **F-0017** | Auth Code | ⚠️ Review | Manual Gateway Bypass logic |
| **F-0018** | Auth Batch | ⚠️ Missing | Batch Capture Service |
| **F-0019** | New Ticket | ✅ Parity | Factory Initialization |
| **F-0020** | Order Type | ✅ Parity | Tax Recalculation Trigger |
| **F-0021** | Ticket View | ✅ Parity | Hierarchy Display |
| **F-0022** | Modifiers | ✅ Parity | Min/Max Constraint Logic |
| **F-0023** | Instructions | ✅ Parity | Persistence |
| **F-0024** | Quantity | ✅ Parity | Non-negative constraint |
| **F-0025** | Print Ticket | ⚠️ Missing | Fiscal/Audit Logging |
| **F-0026** | Inc/Dec Qty | ✅ Parity | Sent Item Blocking |
| **F-0027** | Send Kitchen | ⚠️ Missing | KDS/Print Routing Service |
| **F-0028** | Delete Item | ✅ Parity | Sent Item Restrictions |
| **F-0029** | Misc Item | ⚠️ Partial | Ad-hoc usage tracking |
| **F-0030** | Ticket Fee | ✅ Parity | Taxable functionality |
| **F-0031** | Menu Button | ✅ Parity | Stock Status Display |
| **F-0032** | Size Select | ⚠️ Review | Pricing Model Alignment |
| **F-0033** | Bev Quick Add | ✅ Parity | Macro logic |
| **F-0034** | Item Search | ✅ Parity | Scope/Visibility |
| **F-0035** | Price Entry | ✅ Parity | Validation |
| **F-0036** | Cooking Instr | ✅ Parity | Predefined List |
| **F-0037** | Pizza Modifiers | ⚠️ Missing | Half/Whole Logic |
| **F-0038** | Modifiers | ✅ Parity | Min/Max Constraint Logic |
| **F-0039** | Add-on Select | ✅ Parity | Upsell Logic |
| **F-0040** | Combo Select | ⚠️ Missing | Component Pricing Logic |
| **F-0041** | Quick Pay | ✅ Parity | Workflow Shortcut |
| **F-0042** | Exact Due | ✅ Parity | UI Logic |
| **F-0043** | Quick Cash | ✅ Parity | Configurable Denoms |
| **F-0044** | Cash Button | ✅ Parity | Standard |
| **F-0045** | Card Button | ✅ Parity | Standard |
| **F-0046** | Group Settle | ⚠️ Missing | Batch Payment Domain |
| **F-0047** | Tax Exempt | ✅ Parity | Recalc Trigger |
| **F-0048** | Coupon | ⚠️ Review | Validation Engine |
| **F-0049** | Discount | ✅ Parity | Authorization Check |
| **F-0050** | Void Button | ✅ Parity | UI Trigger |
| **F-0051** | Refund Button | ✅ Parity | Paid Ticket Requirement |
| **F-0052** | Settle Button | ✅ Parity | Standard |
| **F-0053** | Split Button | ✅ Parity | Standard |
| **F-0054** | Manager Button | ✅ Parity | Auth Check |
| **F-0055** | Logout Button | ✅ Parity | Token Revocation |
| **F-0056** | Info Button | ✅ Parity | Read Only |
| **F-0057** | Shutdown Button | ⚠️ Risk | Graceful DB Close |
| **F-0058** | Kitchen Status | ⚠️ Missing | Print Success Tracking |
| **F-0059** | Receipt Print | ✅ Parity | Duplicate Marking |
| **F-0060** | Drawer Assign | ⚠️ Missing | Exclusive Locking |
| **F-0061** | Cust Select | ✅ Parity | Order Type Requirement |
| **F-0062** | Cust Editor | ✅ Parity | Unique Constraint |
| **F-0063** | Deliv Date | ⚠️ Review | Pre-Order Auto-fire |
| **F-0064** | Ticket List | ✅ Parity | Indexing |
| **F-0065** | Ticket Filter | ✅ Parity | Predicates |
| **F-0066** | Ticket Detail | ✅ Parity | Read Only View |
| **F-0067** | Order Info | ✅ Parity | Meta-data |
| **F-0068** | Ticket Type | ⚠️ Review | Validation on Switch |
| **F-0069** | Void Reason | ✅ Parity | Mandatory Capture |
| **F-0070** | Shift Select | ✅ Parity | Login Integration |
| **F-0071** | Mgr Password | ✅ Parity | Privilege Elevation |
| **F-0072** | Cash Drop | ✅ Parity | Transaction Type |
| **F-0073** | Pay Out | ✅ Parity | Transaction Type |
| **F-0074** | Drawer Bleed | ✅ Parity | Transaction Type |
| **F-0075** | Open Drawer | ✅ Parity | Audit Log |
| **F-0076** | Drawer Report | ⚠️ Missing | Balance Query |
| **F-0077** | Table Select | ✅ Parity | Occupancy Lock |
| **F-0078** | Table Layout | ✅ Parity | Static Data |
| **F-0079** | Cust Explorer | ✅ Parity | Search |
| **F-0080** | Change Table | ⚠️ Missing | Atomic Transfer Command |
| **F-0081** | Floor Expl | ✅ Parity | Hierarchy |
| **F-0082** | Table Map | ✅ Parity | Polling |
| **F-0083** | Delivery View | ⚠️ Missing | Dispatch Status |
| **F-0084** | Pickup View | ⚠️ Missing | Ready Time Tracking |
| **F-0085** | Bar Tab | ✅ Parity | Name-based Ticket |
| **F-0086** | Seat Select | ✅ Parity | Seat Attribute |
| **F-0087** | Table Browser | ✅ Parity | List View |
| **F-0088** | Kitchen Disp | ⚠️ Missing | KDS Query |
| **F-0089** | Kitchen Ticket | ✅ Parity | View |
| **F-0090** | Kitchen Stat | ⚠️ Missing | Bump Logic |
| **F-0091** | Kitchen List | ✅ Parity | List View |
| **F-0092** | Sales Summary | ⚠️ Missing | Aggregation Query |
| **F-0093** | Sales Detail | ✅ Parity | Export |
| **F-0094** | Sales Balance | ⚠️ Missing | Recon Logic |
| **F-0095** | Exception Rep | ⚠️ Missing | Fraud Rules |
| **F-0096** | Card Report | ✅ Parity | Masking |
| **F-0097** | Payment Rep | ✅ Parity | Aggregation |
| **F-0098** | Menu Usage | ✅ Parity | Product Mix |
| **F-0099** | Serv Prod | ⚠️ Missing | Performance Stats |
| **F-0100** | Labor Report | ⚠️ Missing | Cost Calculation |
| **F-0101** | Tip Report | ✅ Parity | Gratuity Aggregation |
| **F-0102** | Attend Report | ✅ Parity | Time Tracking |
| **F-0103** | Journal Rep | ⚠️ Review | Complete Coverage |
| **F-0104** | Cash Out | ✅ Parity | Checkout Logic |
| **F-0105** | Rest Config | ✅ Parity | Singleton |
| **F-0106** | Term Config | ✅ Parity | Node ID |
| **F-0107** | Card Config | ⚠️ Risk | Encryption |
| **F-0108** | Print Config | ✅ Parity | Routing |
| **F-0109** | Tax Config | ✅ Parity | Rules |
| **F-0110** | Language | ✅ Parity | I18n |
| **F-0111** | Back Office | ✅ Parity | RBAC |
| **F-0112** | Menu Cat | ✅ Parity | Hierarchy |
| **F-0113** | Menu Group | ✅ Parity | Hierarchy |
| **F-0114** | Menu Item | ✅ Parity | Catalog |
| **F-0115** | Mod Explorer | ✅ Parity | Catalog |
| **F-0116** | Mod Group | ✅ Parity | Catalog |
| **F-0117** | Coupon Expl | ⚠️ Missing | Model |
| **F-0118** | Tax Expl | ✅ Parity | Model |
| **F-0119** | Shift Expl | ✅ Parity | Model |
| **F-0120** | User Expl | ✅ Parity | Model |
| **F-0121** | Order Type | ✅ Parity | Config |
| **F-0122** | Coupon/Disc | ✅ Parity | Combined UI |
| **F-0123** | Discount App | ✅ Parity | Selection |
| **F-0124** | Section Cfg | ✅ Parity | Zone |
| **F-0125** | Notes | ✅ Parity | Free Text |
| **F-0126** | Deliv Zone | ⚠️ Missing | Geofence Model |
| **F-0127** | Print Group | ✅ Parity | Routing |
| **F-0128** | DB Backup | ⚠️ Missing | Service |
| **F-0129** | Banner | ✅ Parity | UI |
| **F-0130** | About | ✅ Parity | Metadata |
| **F-0131** | Confirm | ✅ Parity | Pattern |
| **F-0132** | Progress | ✅ Parity | Pattern |

## Critical Risk Summary
1.  **Financial Integrity**: Drawer Pull (F-0012) and Sales Balance (F-0094) backend logic is missing or undefined.
2.  **Kitchen Operations**: KDS Backend (F-0088, F-0090) and Printing Routing (F-0027) are major gaps.
3.  **Security**: Card Config (F-0107) requires encryption strategy.
4.  **Complex Products**: Pizza (F-0037) and Combos (F-0040) need domain modeling.
