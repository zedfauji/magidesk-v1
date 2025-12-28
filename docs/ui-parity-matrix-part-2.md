# UI Parity Audit: Part 2 (F-0051 - F-0100)

| ID | Feature | FloreantPOS UI Behavior | Forensic UI Expectation | Current Magidesk Status | Parity Level |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **F-0051** | Refund Btn | Refund Workflow Trigger. | Action Button. | **PARTIAL** (Dev button, no workflow) | **HIGH RISK** |
| **F-0052** | Check Pay | Dialog: Check # + Amount. | Payment Method. | **NOT IMPLEMENTED** | **LOW** |
| **F-0053** | House Acct | Dialog: Customer Search. | Credit Charge. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0054** | Gift Cert | Dialog: Cert # + Amount. | Voucher Redeemer. | **PARTIAL** (Button only) | **MEDIUM** |
| **F-0055** | Tip Input | Numpad or % Selection. | Gratuity Adder. | **PARTIAL** (Field in Settle, no UI) | **MEDIUM** |
| **F-0056** | Tip Adjust | Post-Auth Edit Screen. | Adjustment Workflow. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0057** | Batch Cap | *Duplicate F-0018*. | *Duplicate F-0018*. | **NOT IMPLEMENTED** | **MISSING** |
| **F-0058** | Multi-Curr | Dialog: Currency Select. | FX Calculator. | **NOT IMPLEMENTED** | **LOW** |
| **F-0059** | Signature | Touch Pad / Print. | Capture Surface. | **NOT IMPLEMENTED** | **LOW** |
| **F-0060** | Shift Start | Dialog: Count + Assign. | Opening Workflow. | **PARTIAL** (Command, no Dialog) | **BLOCKER** |
| **F-0061** | Shift End | Dialog: Count + Variance. | Closing Workflow. | **PARTIAL** (Command, no Dialog) | **BLOCKER** |
| **F-0062** | Payout | Dialog: Reason + Amount. | Cash Expense. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0063** | No Sale | One-click Action. | Logged Event. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0064** | Cash Drop | Dialog: Amount. | Safe Deposit. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0065** | Drawer Assign | Dialog: User -> Drawer. | Assignment. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0066** | Tip Declare | Dialog: Amount. | Tax Compliance. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0067** | Count Dialog | Denominations Grid. | Money Counter. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0068** | Ticket Exp | Grid: All Tickets (Hist). | Audit Browser. | **PARTIAL** (Open Tickets only) | **MEDIUM** |
| **F-0069** | Edit Ticket | Action -> Order View. | Resume Workflow. | **PARTIAL** (Action exists, Nav broken) | **BLOCKER** |
| **F-0070** | View Receipt | Preview Dialog. | Historical View. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0071** | Hold Ticket | *Implied Save*. | Explicit Action. | **NOT IMPLEMENTED** | **LOW** |
| **F-0072** | Reopen Tkt | Action: Reverse Close. | Correction Flow. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0073** | Refund Act | *Duplicate F-0051*. | *Duplicate F-0051*. | **PARTIAL** | **HIGH RISK** |
| **F-0074** | Transfer Usr | Dialog: Select User. | Ticket Handoff. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0075** | Merge Tkt | Dialog: Select Tickets. | Ticket Combine. | **NOT IMPLEMENTED** | **LOW** |
| **F-0076** | Multi-Pay | Dialog: Select N Tickets. | Batch Settle. | **NOT IMPLEMENTED** | **LOW** |
| **F-0077** | Cust Select | Search Dialog / List. | CRM Link. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0078** | Cust Form | Edit Details / History. | CRM Editor. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0079** | Cust Exp | Grid: All Customers. | CRM Browser. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0080** | Chg Table | Dialog: Floor Map picker. | Table Swap. | **PARTIAL** (Navigates to Map, no swap) | **HIGH RISK** |
| **F-0081** | Floor Exp | Designer Canvas. | Layout Editor. | **NOT IMPLEMENTED** | **LOW** |
| **F-0082** | Table Map | Interactive Canvas. | Visual Status. | **PARTIAL** (Canvas exists, no interaction) | **BLOCKER** |
| **F-0083** | Delivery | Kanban Board. | Driver Logistics. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0084** | Pickup | List with Timers. | Queue Monitor. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0085** | Bar Tab | List of Names. | Tab Manager. | **NOT IMPLEMENTED** | **LOW** |
| **F-0086** | Seat Select | Grid / Visualizer. | Assignment UI. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0087** | Table List | Admin Grid. | CRUD Tables. | **NOT IMPLEMENTED** | **LOW** |
| **F-0088** | Kitchen Win | Fullscreen KDS. | Digital expeditor. | **NOT IMPLEMENTED** | **CRITICAL** |
| **F-0089** | KDS Ticket | Card View. | Order Display. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0090** | KDS Status | Filters (Done/Cooking). | Workflow View. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0091** | KDS Grid | Masonry Layout. | Ticket Wall. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0092** | Sales Rpt | Summary Grid. | Financial Snapshot. | **PARTIAL** (Basic View) | **HIGH RISK** |
| **F-0093** | Detail Rpt | Big Data Grid. | Transaction Log. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0094** | Balance Rpt | Reconciliation View. | Cash Control. | **NOT IMPLEMENTED** | **BLOCKER** |
| **F-0095** | Except Rpt | Filtered Grid. | Fraud Watch. | **NOT IMPLEMENTED** | **LOW** |
| **F-0096** | CC Report | Batch Summary. | Gateway Audit. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0097** | Pay Report | Tender Breakdown. | Bank Deposit. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0098** | Menu Usage | Velocity Grid. | Product Mix. | **NOT IMPLEMENTED** | **LOW** |
| **F-0099** | Server Prod | Efficiency Stats. | Staff Ranking. | **NOT IMPLEMENTED** | **LOW** |
| **F-0100** | Labor Rpt | Cost vs Sales Graph. | KPI Dashboard. | **PARTIAL** | **MEDIUM** |
