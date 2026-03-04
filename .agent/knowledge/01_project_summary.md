# Project Knowledge Summary

## Project Overview
**Name:** Magidesk POS (v1)
**Purpose:** Windows-based Point of Sale (POS) application.
**Repository:** `zedfauji/magidesk-v1`
**Location:** `c:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk`

## Architecture
The project follows a **Clean Architecture** (Onion Architecture) pattern with strict layering:

1.  **Presentation Layer** (`Magidesk.Presentation`)
    *   **Framework:** WinUI 3 (Windows App SDK 1.6+)
    *   **Pattern:** MVVM (Model-View-ViewModel) using `CommunityToolkit.Mvvm`
    *   **Responsibility:** UI rendering, user interaction, routing. No business logic allowed.
2.  **Application Layer** (`Magidesk.Application`)
    *   **Responsibility:** Use cases, service interfaces, DTOs, orchestration.
    *   **Dependencies:** Domain layer only.
3.  **Domain Layer** (`Magidesk.Domain`)
    *   **Responsibility:** Core business entities, value objects, domain logic, repository interfaces.
    *   **Dependencies:** None (Pure C#).
4.  **Infrastructure Layer** (`Magidesk.Infrastructure`)
    *   **Responsibility:** Implementation of services, data access (EF Core), external integrations.
    *   **Technology:** Entity Framework Core 8, SQLite (inferred from typical POS/local usage, verify if otherwise).

## Main Technologies
-   **Language:** C# 12 / .NET 8
-   **UI Framework:** WinUI 3
-   **MVVM Toolkit:** CommunityToolkit.Mvvm (v8.4.0)
-   **Data Access:** Entity Framework Core 8.0.0
-   **Messaging:** MediatR (implied by "Commands/Queries" mention in history, verify if needed)
-   **Dependency Injection:** Microsoft.Extensions.DependencyInjection

## Key Context
-   **Domain:** Point of Sale (Order Entry, Table Management, Switchboard).
-   **Features:**
    -   Switchboard (Dashboard)
    -   Table Sessions (Time-based vs Orders)
    -   Menu Management
    -   Stock/Inventory (Low stock alerts)
    -   Reports (Sales, Labor)
