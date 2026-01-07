# Installation & Setup Guide

This guide covers the essential steps to initialize the application, including database setup and language configuration.

## 1. Database Setup

The application uses an SQL Server database. The connection string is configured in `appsettings.json`.

### Prerequisites
- SQL Server (LocalDB or Standard Edition)
- .NET 8 SDK

### Steps
1.  **Configure Connection String**:
    Open `appsettings.json` and ensure the `DefaultConnection` string points to your SQL Server instance.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MagideskDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

2.  **Apply Migrations**:
    Open a terminal in the solution root and run:
    ```powershell
    dotnet ef database update --project Magidesk.Infrastructure --startup-project Magidesk
    ```
    This command creates the database schema.

3.  **Seed Initial Data** (Optional but Recommended):
    The application includes a seeder for initial data (roles, default user, menu items). This is typically run automatically on first startup if the database is empty, or can be triggered via the `Magidesk.Infrastructure.Data.DbInitializer` class.

## 2. App Initialization

Upon first launch, the application performs the following checks:
- **Database Connectivity**: Ensures the database is reachable.
- **Migration Status**: Checks if pending migrations need to be applied.
- **User existence**: Checks if at least one admin user exists. If not, a default admin account is often created (`admin` / `admin` or similar, check `DbInitializer`).

### Configuration
Key application settings are found in `appsettings.json`:
- `AppConfig:TerminalId`: Unique identifier for the POS terminal.
- `output_format`: Logging format.

## 3. Language Switcher

The application supports multiple languages (English, Spanish, French).

### How to Change Language
1.  **Login**: Access the application with a valid user.
2.  **Navigate to Login Screen**: The language selector is primarily available on the PIN pad / Login screen.
3.  **Select Language**: Click the "Globe" icon or the current language flag/text.
4.  **Choose Locale**: A dialog will appear listing available languages:
    - English (en-US)
    - Spanish (es-ES)
    - French (fr-FR)
5.  **Confirm**: Select the desired language. The UI will update immediately without restarting the application.

### Developer Notes
- **Localization Service**: Managed by `LocalizationService.cs`.
- **Resource Files**: Located in `Strings/{culture}/Resources.resw`.
- **Adding a New Language**:
    1.  Create a new folder in `Strings/` (e.g., `de-DE`).
    2.  Add a `Resources.resw` file.
    3.  Copy keys from `en-US/Resources.resw` and translate values.
    4.  Update `LocalizationService` to include the new culture in the supported list.
