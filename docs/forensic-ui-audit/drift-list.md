# UI Implementation Drift List

This document identifies key areas where the current UI implementation deviates from the forensic documentation.

## 1. UI Component Mismatch

Features specified as modal dialogs have been implemented as full pages, creating a different user experience and workflow.

*   **F-0008 Settle Ticket Dialog**: Implemented as `SettlePage.xaml`, a full navigation page, instead of a modal dialog.
*   **F-0012 Drawer Pull Report Dialog**: Implemented as `DrawerPullReportPage.xaml`, not a modal dialog.

## 2. Incomplete Workflows

Several core features are only partially implemented, with commands or initial UI elements existing but failing to complete the full, required user workflow.

*   **F-0019 New Ticket Action**: The `NewTicketCommand` correctly shows the `OrderTypeSelectionDialog`, but then navigates to the non-functional, developer-focused `TicketPage`.
*   **F-0080 Change Table Action**: The `MoveTableUiCommand` exists but the workflow for selecting a new table and confirming the move is not implemented.
*   **F-0082 Table Map View**: The view displays tables, but selecting a table does not lead to a functional order entry screen.

## 3. Developer Test Harnesses vs. Functional UI

A significant portion of the existing UI consists of developer-focused test pages that require manual entry of GUIDs and other data. These are not user-facing features.

*   **`TicketPage.xaml`**: Affects `F-0005`, `F-0006`, `F-0021`, `F-0022`, `F-0024`, `F-0026`, `F-0027`, `F-0035`. This page is a test harness, not a functional order entry screen.
*   **`PaymentPage.xaml`**: A test page for payment commands, not a user-facing payment view.
*   **`TicketManagementPage.xaml`**: A test page for ticket actions like Void, Refund, and Split, lacking the proper dialogs and user workflows.

## 4. Missing Core Security/Operational Features

Critical features for security and basic operations are entirely missing.

*   **F-0003 Login Screen**: No authentication is present; the application starts directly into a functional state.
*   **F-0009 Manager Functions**: The entry point exists but leads to a placeholder, and no actual manager functions are implemented.
*   **F-0062, F-0063, F-0064 (Cash Management)**: No UI exists for Payouts, No Sale, or Cash Drops.
