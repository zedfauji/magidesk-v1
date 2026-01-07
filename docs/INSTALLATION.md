# Installation Guide

**Applies to:** Magidesk POS v0.1.0+
**Audience:** System Administrators / Owners

## System Requirements

Before you begin, ensure your computer meets these requirements:

- **Operating System:** Windows 10 (Build 1809 or newer) or Windows 11.
- **Processor:** Intel Core i3 or equivalent (i5 recommended for busy bars).
- **Memory (RAM):** 4 GB minimum (8 GB recommended).
- **Storage:** At least 500 MB free space.
- **Display:** Touchscreen monitor recommended (1920x1080 resolution preferred).

---

## Installation Steps

### 1. Download the Installer
Download the latest `Magidesk-Setup.exe` from the release page or the link provided by your vendor.

### 2. Run the Installer
1. Double-click `Magidesk-Setup.exe`.
2. Generally, you can accept the default settings by clicking **Next**.
3. If Windows prompts you ("User Account Control"), click **Yes** to allow the installation.
4. The installer will place a shortcut on your Desktop and in the Start Menu.

### 3. Database Requirement
Magidesk requires a **PostgreSQL** database to store your sales and menu data.
- If you are setting up the **Main Server** (the first computer): You must install PostgreSQL separately if it's not already installed.
- If this is a **Secondary Terminal**: You just need to know the IP address of your Main Server.

> **Note:** The Magidesk installer does *not* install PostgreSQL automatically. If you need help installing the database engine, please refer to the specific Database Installation Guide provided by your support team.

---

## Next Steps

Once installed, launch **Magidesk POS** from your desktop. You will be greeted by the **Database Setup Wizard**.

👉 **Proceed to: [Database Setup Guide](DATABASE_SETUP.md)**
