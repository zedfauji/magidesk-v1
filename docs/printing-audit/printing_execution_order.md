# Printing Execution Order

## Step 1: Core Backend Infrastructure
1.  **PRT-BE-001**: Implement `WindowsPrintingService` (Low Level).
2.  **PRT-BE-004**: Implement Cash Drawer Logic (needs low level service).

## Step 2: Service Implementation
3.  **PRT-BE-002**: Implement `RealKitchenPrintService`.
4.  **PRT-BE-003**: Implement `RealReceiptPrintService`.

## Step 3: Frontend Configuration
5.  **PRT-CFG-001**: Printer Configuration UI (so we can select *where* to print for testing).

## Step 4: Workflow Wiring
6.  **PRT-FLW-001**: Wire Payment Workflows (Receipts + Drawer).
7.  **PRT-BE-005**: Implement Report Printing (Backend).
8.  **PRT-FLW-002**: Wire Report Workflows (Frontend).

## Step 5: Verification
9.  **VERIFY**: Run full end-to-end verification.
