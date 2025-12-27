# UI Wave Execution Plan

This document outlines the phased approach for implementing the UI based on the feature parity analysis.

## Wave 1: Core Daily Operations (P0)

This wave focuses on establishing the absolute minimum functionality for a server to take an order and process a payment.

*   **F-0003**: Login Screen
*   **F-0005, F-0021, F-0022, F-0031**: Functional Order Entry (Ticket View, Menu Buttons)
*   **F-0008**: Settle Ticket Dialog (as a proper dialog)
*   **F-0044, F-0045**: Basic Cash & Card Payments

## Wave 2: Control & Safety (P1)

This wave adds critical cash management, order correction, and manager oversight features.

*   **F-0013**: Void Ticket Dialog
*   **F-0073**: Refund Action
*   **F-0009**: Manager Functions Dialog
*   **F-0062, F-0063, F-0064**: Payout, No Sale, Cash Drop
*   **F-0060, F-0061**: Shift Start/End Dialogs

## Wave 3: Admin & Reports (P1/P2)

This wave focuses on the back-office configuration and reporting necessary for store management.

*   **F-0111**: Back Office Window
*   **F-0112 - F-0121**: All Data Explorers (Menu, Users, etc.)
*   **F-0092 - F-0104**: All Back Office Reports
