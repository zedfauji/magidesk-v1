# UI Parity Audit: Part 1 (F-0001 - F-0050)

| ID | Feature | FloreantPOS UI Behavior | Forensic UI Expectation | Current Magidesk Status | Parity Level |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **F-0001** | App Bootstrap | Splash screen -> Check DB -> Login. | Blocking "Initializing" overlay. | **PARTIAL** (App loads, immediate nav to Switchboard, no splash/checks) | **DIVERGENT** |
| **F-0002** | Main Window | Fullscreen, Status Bar (Time, User). | Shell with Status/Clock. | **PARTIAL** (Window exists, Status Bar missing) | **MISSING** |
| **F-0003** | Login Screen | Numpad for PIN / User List. | Gatekeeper for all access. | **NOT IMPLEMENTED** (Bypassed) | **CRITICAL** |
| **F-0004** | Switchboard | Grid of functions + "My Tickets". | Central Hub + Dashboard. | **PARTIAL** (Nav Grid only, no Dashboard data) | **HIGH RISK** |
| **F-0005** | Order Entry | Split Pane: Ticket (Left), Menu (Right). | Unified Order Screen. | **PARTIAL** (`OrderEntryPage` exists but is empty/stub) | **BLOCKER** |
| **F-0006** | Ticket Panel | List of items, modifiers, totals. | Interactive Line Item List. | **PARTIAL** (Basic ListView, poor formatting) | **HIGH RISK** |
| **F-0007** | Payment Keypad | Large Numpad + Tender Types. | Embedded in Settle Dialog. | **PARTIAL** (Exists in `SettlePage`, logic ok) | **MEDIUM** |
| **F-0008** | Settle Dialog | Modal. Split/Partial payment support. | Modal Transaction Focus. | **DIVERGENT** (Full Page `SettlePage`, loss of context) | **HIGH RISK** |
| **F-0009** | Manager Func | Dialog with Admin buttons. | Secure Admin Menu. | **NOT IMPLEMENTED** | **MISSING** |
| **F-0010** | Cash Ops | Dialogs for Drop/Payout. | Drawer Operations Menu. | **NOT IMPLEMENTED** | **MISSING** |
| **F-0011** | Open Tickets | List with Filters (Table/Server). | Searchable Ticket Grid. | **PARTIAL** (`TicketManagementPage` is basic list) | **MEDIUM** |
| **F-0012** | Drawer Report | HTML Preview of Shift. | Read-Only Report View. | **PARTIAL** (`DrawerPullReportPage` exists, no Print/Finish) | **MEDIUM** |
| **F-0013** | Void Ticket | Dialog: Reason Selection. | Modal with Reason Code. | **NOT IMPLEMENTED** (Command exists, UI missing) | **BLOCKER** |
| **F-0014** | Split Ticket | Dual-List drag-and-drop. | Interactive Splitter. | **PARTIAL** (`SplitTicketDialog` stub exists) | **HIGH RISK** |
| **F-0015** | Pay Wait | Blocking "Processing..." modal. | Feedback during async. | **PARTIAL** (`ProgressRing` used, non-blocking?) | **LOW** |
| **F-0016** | Swipe Card | Listening for MSR input. | Global KeyListener/Event. | **NOT IMPLEMENTED** | **CRITICAL** |
| **F-0017** | Auth Code | Manual Entry Dialog. | Fallback for Voice Auth. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0018** | Batch Cap | EOD Credit Card Batch. | Admin Process. | **NOT IMPLEMENTED** | **MISSING** |
| **F-0019** | New Ticket | Action -> Order Type -> Screen. | Workflow Trigger. | **PARTIAL** (Command exists, Nav broken) | **HIGH RISK** |
| **F-0020** | Order Type | Dialog: DineIn/ToGo/etc. | Selection Dialog. | **PARTIAL** (`OrderTypeSelectionDialog` exists, basic) | **MEDIUM** |
| **F-0021** | Ticket View | *See F-0006*. | *See F-0006*. | **PARTIAL** | **HIGH RISK** |
| **F-0022** | Modifiers | Auto-popup on Item Add. | Mandatory Selection Flow. | **PARTIAL** (`ModifierSelectionDialog` exists, logic weak) | **BLOCKER** |
| **F-0023** | Guest Count | Numpad prompt. | Headcount capture. | **NOT IMPLEMENTED** | **LOW** |
| **F-0024** | Qty Entry | Numpad for X * Item. | Quantity Multiplier. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0025** | Print Ticket | Action (Guest Check). | Print Preview/Action. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0026** | +/- Qty | Buttons on line item. | Quick Adjust. | **NOT IMPLEMENTED** | **LOW** |
| **F-0027** | Send Kitchen | Commit Button. | Status Update -> KDS. | **PARTIAL** (Button exists, no visual state change) | **BLOCKER** |
| **F-0028** | Delete Item | Trash Icon / Swipe. | Removal Action. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0029** | Misc Item | Name/Price Input Dialog. | Ad-Hoc Item Creator. | **NOT IMPLEMENTED** | **LOW** |
| **F-0030** | Ticket Fee | Selection Dialog. | Service Charge Adder. | **NOT IMPLEMENTED** | **LOW** |
| **F-0031** | Menu Grid | Categories/Groups/Items. | Touch-friendly Grid. | **PARTIAL** (`OrderEntryPage` placeholder) | **BLOCKER** |
| **F-0032** | Size Select | Dialog (Sm/Med/Lg). | Variant Picker. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0033** | Quick Bev | Sidebar Shortcuts. | Fast Access Panel. | **NOT IMPLEMENTED** | **LOW** |
| **F-0034** | Item Search | Text Input/Keyboard. | Search Bar. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0035** | Price Entry | Numpad for Open Price. | Variable Price Handler. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0036** | Cooking Inst | Text/Predefined List. | Note Editor. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0037** | Pizza Mods | Left/Right/Whole. | Visual Pizza Builder. | **NOT IMPLEMENTED** | **MISSING** |
| **F-0038** | Mod Select | Group Constraints (Min/Max). | *See F-0022*. | **PARTIAL** | **BLOCKER** |
| **F-0039** | Add-Ons | Upsell Prompt. | Suggested Items. | **NOT IMPLEMENTED** | **LOW** |
| **F-0040** | Combo | Wizard Flow. | Meal Builder. | **NOT IMPLEMENTED** | **MISSING** |
| **F-0041** | Quick Pay | One-tap Cash. | Shortcut. | **NOT IMPLEMENTED** | **LOW** |
| **F-0042** | Exact Due | Button. | Tender Filler. | **PARTIAL** (Auto-fills, no button) | **LOW** |
| **F-0043** | Quick Cash | $10/$20 Buttons. | Denomination Shortcuts. | **NOT IMPLEMENTED** | **LOW** |
| **F-0044** | Cash Btn | Tender Type. | Payment Trigger. | **IMPLEMENTED** | **FULL** |
| **F-0045** | Card Btn | Tender Type. | Payment Trigger. | **IMPLEMENTED** | **FULL** |
| **F-0046** | Group Settle | Multi-Ticket Select. | Batch Payment. | **NOT IMPLEMENTED** | **MISSING** |
| **F-0047** | Tax Exempt | Toggle Button. | Tax Remover. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0048** | Coupon | Code Entry/Scan. | Discount Engine. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0049** | Discount | Selection List. | Manual Adjustment. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0050** | Void Btn | Trigger F-0013. | Action. | **NOT IMPLEMENTED** | **BLOCKER** |
