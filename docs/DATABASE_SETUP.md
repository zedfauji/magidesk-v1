# Database Setup Guide

When you launch Magidesk for the first time, you will see the **Database Setup** screen. This is a one-time process to connect the register to your data.

## The Setup Screen

You will need the following information. If you are unsure, ask your system administrator.

| Field | Description | Default / Example |
|-------|-------------|-------------------|
| **Host** | The IP address or name of the computer running the database. | `localhost` (if on same PC) or `192.168.1.100` |
| **Port** | The network port for the database. | `5432` |
| **Database Name** | The name of your data file. | `magidesk_db` |
| **Username** | The database user account. | `postgres` |
| **Password** | The secure password for the database. | *(Your secure password)* |

## Step-by-Step

1. **Enter Details**: Fill in the fields described above.
2. **Test Connection**: Click the **Test Connection** button.
   - ✅ **Success**: You will see a green checkmark.
   - ❌ **Failed**: You will see an error message explaining what is wrong (e.g., "Wrong Password" or "Cannot Find Server"). 
3. **Run Database Setup**:
   - If this is a **New Install**, click **Run Database Setup**.
   - The system will create all necessary tables and standard data for you.
   - *Wait for the progress bar to reach 100%.*
4. **Continue**:
   - Once success is confirmed, clicking **Continue** will take you to the Login screen.

> **Security Note:** Your database password is encrypted and stored securely on this computer. You won't need to enter it again unless you reinstall the application.

## Troubleshooting Connections

- **"Connection Refused"**: Check that the Host IP is correct and the PostgreSQL service is running on the server.
- **"Password Authentication Failed"**: Double-check your username and password. Case sensitivity matters!
- **"Database does not exist"**: Ensure you typed the Database Name correctly. Most setups use `magidesk_db`.

👉 **Next Step: [First Run Checklist](FIRST_RUN_CHECKLIST.md)**
