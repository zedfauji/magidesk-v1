# UI Implementation Risk Matrix

This document categorizes the risks associated with the identified gaps in the UI implementation.

## High-Risk Gaps (Financial & Core Operations)

These gaps represent a direct risk to financial integrity or block core business operations.

| Feature ID | Risk Description | Impact |
| :--- | :--- | :--- |
| **F-0003** | **No Login/Authentication**: Anyone can access the system, perform sales, and handle cash without accountability. | **CRITICAL** - Prevents secure deployment. |
| **F-0013** | **Incomplete Void Workflow**: No UI for void reasons or manager approval. High risk of un-audited, fraudulent voids. | **HIGH** - Financial loss, inventory discrepancy. |
| **F-0073** | **Incomplete Refund Workflow**: No UI for processing refunds correctly. Prevents basic customer service resolution. | **HIGH** - Customer dissatisfaction, financial reconciliation issues. |
| **F-0062/63/64** | **Missing Cash Management**: No UI for Payouts, No Sale, or Cash Drops. Makes drawer reconciliation impossible. | **HIGH** - Financial loss, theft risk. |
| **F-0005/22/31**| **No Functional Order Entry**: The core function of a POS—taking an order—is not possible through a user-facing UI. | **CRITICAL** - Blocks all other development and testing. |

## Medium-Risk Gaps (Operational & Workflow)

These gaps disrupt standard operational workflows but may have workarounds.

| Feature ID | Risk Description | Impact |
| :--- | :--- | :--- |
| **F-0014** | **No Split Ticket UI**: Inability to split tickets is a major operational bottleneck in restaurant environments. | **MEDIUM** - Degraded customer service, billing friction. |
| **F-0009** | **No Manager Functions**: Managers have no access to back-office functions from the main UI. | **MEDIUM** - Prevents on-the-fly administrative actions. |
| **F-0111-0121**| **Missing Back-Office Explorers**: Inability to manage the menu, users, or system configuration prevents setup and maintenance. | **MEDIUM** - Blocks store setup and ongoing management. |

## Low-Risk Gaps (UX & Polish)

These gaps represent deviations from the specified UI pattern but do not block core functionality.

| Feature ID | Risk Description | Impact |
| :--- | :--- | :--- |
| **F-0008** | **Settle Dialog as Page**: The settle workflow is implemented as a full page, not a modal dialog. Functionally similar but a UX inconsistency. | **LOW** - Minor UI drift. |
| **F-0012** | **Drawer Pull as Page**: The report is a full page, not a modal dialog. | **LOW** - Minor UI drift. |
| **F-0042/43** | **Missing Quick Pay Buttons**: Lack of "Exact Due" and denomination buttons slightly slows down cash transactions. | **LOW** - Minor UX friction. |
