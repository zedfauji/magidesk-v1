# Deployment Guide

## Overview

This document provides comprehensive guidance for deploying the Magidesk POS system in various environments, from development setups to production deployments. It covers infrastructure requirements, installation procedures, configuration management, and operational considerations.

## System Requirements

### Hardware Requirements

#### Minimum Requirements
- **CPU**: Intel Core i5 or AMD Ryzen 5 (2.5 GHz or higher)
- **RAM**: 8 GB DDR4
- **Storage**: 256 GB SSD
- **Network**: Gigabit Ethernet
- **USB Ports**: Minimum 4 USB 3.0 ports
- **Display**: 1920x1080 resolution

#### Recommended Requirements
- **CPU**: Intel Core i7 or AMD Ryzen 7 (3.0 GHz or higher)
- **RAM**: 16 GB DDR4
- **Storage**: 512 GB NVMe SSD
- **Network**: Gigabit Ethernet with redundant connection
- **USB Ports**: 6+ USB 3.0 ports
- **Display**: 1920x1080 or higher, dual monitor support

#### Production Requirements
- **CPU**: Intel Core i7 or AMD Ryzen 7 (3.5 GHz or higher)
- **RAM**: 32 GB DDR4
- **Storage**: 1 TB NVMe SSD + 2 TB HDD for backups
- **Network**: Dual Gigabit Ethernet with failover
- **USB Ports**: 8+ USB 3.0 ports
- **Display**: 1920x1080 or higher, triple monitor support
- **UPS**: Uninterruptible Power Supply with 30+ minute runtime

### Software Requirements

#### Operating System
- **Primary**: Windows 11 Pro (64-bit)
- **Alternative**: Windows 10 Pro (64-bit) with latest updates
- **Server**: Windows Server 2022 Standard (for database server)

#### Database Server
- **PostgreSQL**: Version 15.x or higher
- **Memory**: Minimum 4 GB dedicated RAM
- **Storage**: Minimum 100 GB dedicated SSD
- **Backup**: Automated daily backups with point-in-time recovery

#### .NET Runtime
- **.NET 8.0 Runtime**: Latest version with security updates
- **ASP.NET Core Runtime**: For API components
- **Desktop Runtime**: For WinUI 3 application

#### Additional Software
- **Microsoft Visual C++ Redistributable**: Latest version
- **Microsoft Edge WebView2**: Latest version
- **Hardware Drivers**: Latest drivers for all POS hardware
- **Antivirus**: Compatible with POS systems (exclude POS directories)

## Network Architecture

### Network Topology

```
Internet
    |
    |-- Firewall
    |
    |-- Core Switch
        |
        |-- Database Server
        |-- POS Terminal 1
        |-- POS Terminal 2
        |-- POS Terminal N
        |-- Kitchen Display System
        |-- Manager Station
        |-- Backup Server
        |-- Network Printer
```

### Network Configuration

#### IP Addressing
- **Database Server**: 192.168.1.10/24
- **POS Terminals**: 192.168.1.20-50/24
- **KDS**: 192.168.1.60/24
- **Manager Station**: 192.168.1.70/24
- **Backup Server**: 192.168.1.80/24
- **Network Printer**: 192.168.1.90/24

#### Port Configuration
- **PostgreSQL**: 5432/TCP
- **POS Application**: 5000-5010/TCP
- **Kitchen Display**: 5020/TCP
- **Backup Service**: 5030/TCP
- **Remote Desktop**: 3389/TCP (management only)

#### DNS Configuration
```
magidesk-db.local     -> 192.168.1.10
magidesk-kds.local    -> 192.168.1.60
magidesk-backup.local -> 192.168.1.80
```

### Security Configuration

#### Firewall Rules
```bash
# Database server
iptables -A INPUT -p tcp --dport 5432 -s 192.168.1.0/24 -j ACCEPT
iptables -A INPUT -p tcp --dport 5432 -j DROP

# POS application
iptables -A INPUT -p tcp --dport 5000:5010 -s 192.168.1.0/24 -j ACCEPT
iptables -A INPUT -p tcp --dport 5000:5010 -j DROP

# Management access
iptables -A INPUT -p tcp --dport 3389 -s 192.168.1.70 -j ACCEPT
iptables -A INPUT -p tcp --dport 3389 -j DROP
```

## Installation Procedures

### Database Server Setup

#### PostgreSQL Installation

1. **Download PostgreSQL**
   ```bash
   # Download PostgreSQL 15.x
   wget https://get.enterprisedb.com/postgresql/postgresql-15.x-windows-x64.exe
   ```

2. **Install PostgreSQL**
   ```powershell
   # Run installer with custom configuration
   .\postgresql-15.x-windows-x64.exe --mode unattended --superpassword "SecurePassword123!"
   ```

3. **Create Database and User**
   ```sql
   -- Connect to PostgreSQL
   psql -U postgres

   -- Create database
   CREATE DATABASE magidesk WITH ENCODING 'UTF8';

   -- Create user
   CREATE USER magidesk_user WITH PASSWORD 'SecurePassword123!';
   GRANT ALL PRIVILEGES ON DATABASE magidesk TO magidesk_user;

   -- Create schema
   \c magidesk;
   CREATE SCHEMA magidesk;
   GRANT ALL ON SCHEMA magidesk TO magidesk_user;
   ```

4. **Configure PostgreSQL**
   ```ini
   # postgresql.conf
   listen_addresses = '*'
   port = 5432
   max_connections = 200
   shared_buffers = 256MB
   effective_cache_size = 1GB
   maintenance_work_mem = 64MB
   checkpoint_completion_target = 0.9
   wal_buffers = 16MB
   default_statistics_target = 100
   random_page_cost = 1.1
   effective_io_concurrency = 200
   ```

5. **Configure pg_hba.conf**
   ```
   # TYPE  DATABASE        USER            ADDRESS                 METHOD
   local   all             postgres                                peer
   host    magidesk        magidesk_user    192.168.1.0/24          md5
   host    all             all             127.0.0.1/32             md5
   ```

#### Database Migration

1. **Run Database Migrations**
   ```bash
   # Navigate to application directory
   cd C:\Magidesk\Infrastructure

   # Run migrations
   dotnet ef database update --connection "Host=192.168.1.10;Database=magidesk;Username=magidesk_user;Password=SecurePassword123!"
   ```

2. **Seed Initial Data**
   ```bash
   # Run seed data script
   dotnet run --project Magidesk.Infrastructure -- --seed-data
   ```

### POS Terminal Installation

#### Application Installation

1. **Create Installation Directory**
   ```powershell
   # Create main directory
   New-Item -ItemType Directory -Path "C:\Magidesk" -Force
   New-Item -ItemType Directory -Path "C:\Magidesk\Logs" -Force
   New-Item -ItemType Directory -Path "C:\Magidesk\Config" -Force
   New-Item -ItemType Directory -Path "C:\Magidesk\Backups" -Force
   ```

2. **Copy Application Files**
   ```powershell
   # Copy from installation media
   Copy-Item -Path "D:\Install\Magidesk\*" -Destination "C:\Magidesk\" -Recurse -Force
   ```

3. **Install .NET Runtime**
   ```powershell
   # Install .NET 8.0 Runtime
   .\dotnet-runtime-8.0.x-win-x64.exe /quiet /norestart

   # Install ASP.NET Core Runtime
   .\aspnetcore-runtime-8.0.x-win-x64.exe /quiet /norestart

   # Install Desktop Runtime
   .\windowsdesktop-runtime-8.0.x-win-x64.exe /quiet /norestart
   ```

4. **Install WebView2**
   ```powershell
   # Install Microsoft Edge WebView2
   .\MicrosoftEdgeWebView2RuntimeInstallerX64.exe /silent /install
   ```

#### Configuration Setup

1. **Create Configuration Files**
   ```json
   // appsettings.json
   {
     "Application": {
       "InstanceName": "POS-001",
       "LocationName": "Main Restaurant"
     },
     "Database": {
       "ConnectionString": "Host=192.168.1.10;Database=magidesk;Username=magidesk_user;Password=SecurePassword123!;"
     },
     "Hardware": {
       "Printers": {
         "ReceiptPrinter": {
           "Name": "EPSON TM-T88V",
           "Connection": "USB",
           "Port": "USB001"
         }
       }
     }
   }
   ```

2. **Configure Windows Firewall**
   ```powershell
   # Allow POS application
   New-NetFirewallRule -DisplayName "Magidesk POS" -Direction Inbound -Port 5000-5010 -Protocol TCP -Action Allow

   # Allow database access
   New-NetFirewallRule -DisplayName "PostgreSQL" -Direction Outbound -Port 5432 -Protocol TCP -Action Allow
   ```

3. **Create Windows Service (Optional)**
   ```powershell
   # Install as Windows service for auto-start
   sc create "MagideskPOS" binPath= "C:\Magidesk\Magidesk.Presentation.exe" start= auto
   sc description "MagideskPOS" "Magidesk Point of Sale System"
   sc start "MagideskPOS"
   ```

#### Hardware Installation

1. **Install Printer Drivers**
   ```powershell
   # Install receipt printer driver
   .\EPSON_TM-T88V_Driver.exe /quiet

   # Install kitchen printer driver
   .\Star_TSP650II_Driver.exe /quiet
   ```

2. **Configure Cash Drawer**
   ```powershell
   # Install cash drawer driver
   .\APG_CashDrawer_Driver.exe /quiet

   # Test cash drawer connection
   # Use manufacturer's test utility
   ```

3. **Install Card Reader**
   ```powershell
   # Install card reader driver
   .\MagTek_CardReader_Driver.exe /quiet

   # Test card reader
   # Use manufacturer's test utility
   ```

### Kitchen Display System Setup

#### KDS Installation

1. **Install KDS Application**
   ```bash
   # Copy KDS files
   cp -r /mnt/install/kds/* /opt/magidesk-kds/

   # Install dependencies
   apt-get install -y nodejs npm

   # Install application
   npm install
   npm run build
   ```

2. **Configure KDS**
   ```json
   {
     "Database": {
       "ConnectionString": "Host=192.168.1.10;Database=magidesk;Username=magidesk_user;Password=SecurePassword123!"
     },
     "Display": {
       "RefreshInterval": 5,
       "ShowPreparationTime": true,
       "ColorCoding": {
         "New": "#FF0000",
         "InProgress": "#FFA500",
         "Ready": "#00FF00"
       }
     }
   }
   ```

3. **Setup Auto-Start**
   ```bash
   # Create systemd service
   cat > /etc/systemd/system/magidesk-kds.service << EOF
   [Unit]
   Description=Magidesk Kitchen Display System
   After=network.target

   [Service]
   Type=simple
   User=kds
   WorkingDirectory=/opt/magidesk-kds
   ExecStart=/usr/bin/npm start
   Restart=always
   RestartSec=10

   [Install]
   WantedBy=multi-user.target
   EOF

   # Enable and start service
   systemctl enable magidesk-kds
   systemctl start magidesk-kds
   ```

## Configuration Management

### Environment-Specific Configuration

#### Development Environment
```json
{
  "Application": {
    "Environment": "Development",
    "InstanceName": "DEV-POS-001"
  },
  "Database": {
    "ConnectionString": "Host=localhost;Database=magidesk_dev;Username=dev_user;Password=dev_password;",
    "EnableSensitiveDataLogging": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

#### Staging Environment
```json
{
  "Application": {
    "Environment": "Staging",
    "InstanceName": "STAGE-POS-001"
  },
  "Database": {
    "ConnectionString": "Host=staging-db.local;Database=magidesk_staging;Username=staging_user;Password=staging_password;",
    "EnableSensitiveDataLogging": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

#### Production Environment
```json
{
  "Application": {
    "Environment": "Production",
    "InstanceName": "PROD-POS-001"
  },
  "Database": {
    "ConnectionString": "Host=192.168.1.10;Database=magidesk;Username=magidesk_user;Password=${DB_PASSWORD};",
    "EnableSensitiveDataLogging": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

### Configuration Deployment

#### Automated Configuration Deployment
```powershell
# Deploy configuration script
param(
    [Parameter(Mandatory=$true)]
    [string]$Environment,
    
    [Parameter(Mandatory=$true)]
    [string]$InstanceName
)

# Load configuration template
$configTemplate = Get-Content "config\appsettings.$Environment.json" | ConvertFrom-Json

# Update instance-specific settings
$configTemplate.Application.InstanceName = $InstanceName
$configTemplate.Application.LocationName = Get-LocationName $InstanceName

# Save configuration
$configPath = "C:\Magidesk\appsettings.json"
$configTemplate | ConvertTo-Json -Depth 10 | Set-Content $configPath

Write-Host "Configuration deployed for $InstanceName in $Environment environment"
```

#### Configuration Validation
```powershell
# Validate configuration script
function Test-Configuration {
    param(
        [string]$ConfigPath = "C:\Magidesk\appsettings.json"
    )
    
    try {
        $config = Get-Content $ConfigPath | ConvertFrom-Json
        
        # Test database connection
        $connectionString = $config.Database.ConnectionString
        $connectionTest = Test-DatabaseConnection $connectionString
        
        if (-not $connectionTest.Success) {
            throw "Database connection failed: $($connectionTest.Error)"
        }
        
        # Test required settings
        $requiredSettings = @(
            "Application:InstanceName",
            "Database:ConnectionString",
            "Security:JwtSettings:SecretKey"
        )
        
        foreach ($setting in $requiredSettings) {
            $value = Get-ConfigValue $config $setting
            if ([string]::IsNullOrEmpty($value)) {
                throw "Required setting missing: $setting"
            }
        }
        
        Write-Host "Configuration validation passed"
        return $true
    }
    catch {
        Write-Error "Configuration validation failed: $($_.Exception.Message)"
        return $false
    }
}
```

## Backup and Recovery

### Database Backup Strategy

#### Automated Backup Script
```bash
#!/bin/bash
# backup_database.sh

DB_HOST="192.168.1.10"
DB_NAME="magidesk"
DB_USER="magidesk_user"
BACKUP_DIR="/backup/database"
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="$BACKUP_DIR/magidesk_backup_$DATE.sql"

# Create backup directory
mkdir -p $BACKUP_DIR

# Create backup
pg_dump -h $DB_HOST -U $DB_USER -d $DB_NAME > $BACKUP_FILE

# Compress backup
gzip $BACKUP_FILE

# Remove old backups (keep 30 days)
find $BACKUP_DIR -name "*.sql.gz" -mtime +30 -delete

echo "Database backup completed: $BACKUP_FILE.gz"
```

#### Backup Schedule (Cron)
```bash
# Daily backup at 2:00 AM
0 2 * * * /opt/scripts/backup_database.sh

# Weekly full backup on Sunday at 1:00 AM
0 1 * * 0 /opt/scripts/full_backup.sh

# Hourly transaction log backup
0 * * * * /opt/scripts/backup_transaction_log.sh
```

### Application Backup

#### Application Backup Script
```powershell
# backup_application.ps1
param(
    [string]$BackupPath = "C:\Magidesk\Backups"
)

$Date = Get-Date -Format "yyyyMMdd_HHmmss"
$BackupDir = Join-Path $BackupPath "application_backup_$Date"

# Create backup directory
New-Item -ItemType Directory -Path $BackupDir -Force

# Backup application files
Copy-Item -Path "C:\Magidesk\*" -Destination $BackupDir -Recurse -Exclude "Logs","Backups"

# Backup configuration
Copy-Item -Path "C:\Magidesk\Config\*" -Destination "$BackupDir\Config" -Recurse

# Compress backup
Compress-Archive -Path $BackupDir -DestinationPath "$BackupDir.zip" -Force

# Remove uncompressed backup
Remove-Item -Path $BackupDir -Recurse -Force

# Remove old backups (keep 7 days)
Get-ChildItem -Path $BackupPath -Name "application_backup_*.zip" | 
    Where-Object { $_.CreationTime -lt (Get-Date).AddDays(-7) } | 
    Remove-Item -Force

Write-Host "Application backup completed: $BackupDir.zip"
```

### Recovery Procedures

#### Database Recovery
```bash
#!/bin/bash
# restore_database.sh

BACKUP_FILE=$1
DB_HOST="192.168.1.10"
DB_NAME="magidesk"
DB_USER="magidesk_user"

if [ -z "$BACKUP_FILE" ]; then
    echo "Usage: $0 <backup_file>"
    exit 1
fi

# Stop application
systemctl stop magidesk-pos

# Drop existing database
dropdb -h $DB_HOST -U $DB_USER $DB_NAME

# Create new database
createdb -h $DB_HOST -U $DB_USER $DB_NAME

# Restore backup
if [[ $BACKUP_FILE == *.gz ]]; then
    gunzip -c $BACKUP_FILE | psql -h $DB_HOST -U $DB_USER -d $DB_NAME
else
    psql -h $DB_HOST -U $DB_USER -d $DB_NAME < $BACKUP_FILE
fi

# Run migrations to ensure latest schema
dotnet ef database update --connection "Host=$DB_HOST;Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD;"

# Start application
systemctl start magidesk-pos

echo "Database recovery completed"
```

#### Application Recovery
```powershell
# restore_application.ps1
param(
    [Parameter(Mandatory=$true)]
    [string]$BackupFile
)

# Stop application
Stop-Service -Name "MagideskPOS" -Force

# Extract backup
Expand-Archive -Path $BackupFile -Destination "C:\" -Force

# Restore permissions
icacls "C:\Magidesk" /grant "IIS_IUSRS:(OI)(CI)F"
icacls "C:\Magidesk\Logs" /grant "IIS_IUSRS:(OI)(CI)F"

# Start application
Start-Service -Name "MagideskPOS"

Write-Host "Application recovery completed"
```

## Monitoring and Maintenance

### System Monitoring

#### Health Check Script
```powershell
# health_check.ps1
function Test-MagideskHealth {
    $results = @()
    
    # Test database connection
    try {
        $connection = Test-DatabaseConnection
        $results += [PSCustomObject]@{
            Component = "Database"
            Status = if ($connection.Success) { "Healthy" } else { "Unhealthy" }
            Message = $connection.Message
            Timestamp = Get-Date
        }
    }
    catch {
        $results += [PSCustomObject]@{
            Component = "Database"
            Status = "Unhealthy"
            Message = $_.Exception.Message
            Timestamp = Get-Date
        }
    }
    
    # Test application
    try {
        $appResponse = Invoke-WebRequest -Uri "http://localhost:5000/health" -TimeoutSec 10
        $results += [PSCustomObject]@{
            Component = "Application"
            Status = if ($appResponse.StatusCode -eq 200) { "Healthy" } else { "Unhealthy" }
            Message = "HTTP $($appResponse.StatusCode)"
            Timestamp = Get-Date
        }
    }
    catch {
        $results += [PSCustomObject]@{
            Component = "Application"
            Status = "Unhealthy"
            Message = $_.Exception.Message
            Timestamp = Get-Date
        }
    }
    
    # Test hardware
    $printerStatus = Test-PrinterConnection
    $results += [PSCustomObject]@{
        Component = "Printer"
        Status = $printerStatus.Status
        Message = $printerStatus.Message
        Timestamp = Get-Date
    }
    
    return $results
}
```

#### Automated Monitoring Setup
```bash
# Setup monitoring cron jobs
# Health check every 5 minutes
*/5 * * * * /opt/scripts/health_check.sh

# Log rotation daily
0 0 * * * /opt/scripts/rotate_logs.sh

# Disk space check hourly
0 * * * * /opt/scripts/check_disk_space.sh

# Performance metrics collection every 15 minutes
*/15 * * * * /opt/scripts/collect_metrics.sh
```

### Maintenance Procedures

#### Database Maintenance
```sql
-- Database maintenance script
-- Rebuild indexes
REINDEX DATABASE magidesk;

-- Update statistics
ANALYZE magidesk;

-- Vacuum full (run during maintenance window)
VACUUM FULL magidesk;

-- Clean up old audit logs
DELETE FROM magidesk.AuditLogs 
WHERE CreatedAt < NOW() - INTERVAL '1 year';

-- Clean up old system logs
DELETE FROM magidesk.SystemLogs 
WHERE CreatedAt < NOW() - INTERVAL '6 months';
```

#### Application Maintenance
```powershell
# maintenance.ps1
# Clear application logs older than 30 days
Get-ChildItem -Path "C:\Magidesk\Logs" -Name "*.log" | 
    Where-Object { $_.CreationTime -lt (Get-Date).AddDays(-30) } | 
    Remove-Item -Force

# Clear temporary files
Get-ChildItem -Path "C:\Magidesk\Temp" -Recurse | 
    Where-Object { $_.CreationTime -lt (Get-Date).AddDays(-7) } | 
    Remove-Item -Force -Recurse

# Optimize database
Invoke-Sqlcmd -Query "EXEC magidesk.OptimizeDatabase" -ServerInstance "192.168.1.10"

Write-Host "Maintenance completed"
```

## Security Hardening

### Network Security

#### SSL/TLS Configuration
```bash
# Generate SSL certificate
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /etc/ssl/private/magidesk.key \
    -out /etc/ssl/certs/magidesk.crt \
    -subj "/C=US/ST=State/L=City/O=Restaurant/CN=magidesk.local"

# Configure nginx for SSL
cat > /etc/nginx/sites-available/magidesk << EOF
server {
    listen 443 ssl;
    server_name magidesk.local;
    
    ssl_certificate /etc/ssl/certs/magidesk.crt;
    ssl_certificate_key /etc/ssl/private/magidesk.key;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
EOF
```

#### Firewall Configuration
```powershell
# Configure Windows Firewall
# Block all inbound traffic by default
Set-NetFirewallProfile -Profile Domain,Public,Private -DefaultInboundAction Block

# Allow specific ports
New-NetFirewallRule -DisplayName "POS Application" -Direction Inbound -Port 5000-5010 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "Database Access" -Direction Outbound -Port 5432 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "Remote Desktop" -Direction Inbound -Port 3389 -Protocol TCP -Action Allow -RemoteAddress 192.168.1.70

# Enable logging
Set-NetFirewallProfile -Profile Domain,Public,Private -LogAllowed True -LogBlocked True
```

### Application Security

#### Security Configuration
```json
{
  "Security": {
    "JwtSettings": {
      "SecretKey": "${JWT_SECRET_KEY}",
      "Issuer": "MagideskPOS",
      "Audience": "MagideskUsers",
      "ExpirationMinutes": 480
    },
    "PasswordPolicy": {
      "MinLength": 12,
      "RequireUppercase": true,
      "RequireLowercase": true,
      "RequireDigit": true,
      "RequireSpecialChar": true,
      "MaxFailedAttempts": 3,
      "LockoutDurationMinutes": 30
    },
    "SessionSettings": {
      "TimeoutMinutes": 30,
      "SlidingExpiration": false,
      "AbsoluteExpirationHours": 8
    }
  }
}
```

#### Security Monitoring
```powershell
# security_monitor.ps1
function Test-SecurityCompliance {
    $issues = @()
    
    # Check for weak passwords
    $weakPasswords = Get-WeakPasswords
    if ($weakPasswords.Count -gt 0) {
        $issues += "Found $($weakPasswords.Count) weak passwords"
    }
    
    # Check for expired sessions
    $expiredSessions = Get-ExpiredSessions
    if ($expiredSessions.Count -gt 0) {
        $issues += "Found $($expiredSessions.Count) expired sessions"
    }
    
    # Check for failed login attempts
    $failedLogins = Get-RecentFailedLogins -Hours 24
    if ($failedLogins.Count -gt 100) {
        $issues += "High number of failed login attempts: $($failedLogs.Count)"
    }
    
    return $issues
}
```

## Troubleshooting

### Common Issues

#### Database Connection Issues
```powershell
# Troubleshoot database connection
function Test-DatabaseConnection {
    try {
        $connectionString = "Host=192.168.1.10;Database=magidesk;Username=magidesk_user;Password=SecurePassword123!"
        $connection = New-Object Npgsql.NpgsqlConnection($connectionString)
        $connection.Open()
        $connection.Close()
        return @{ Success = $true; Message = "Connection successful" }
    }
    catch {
        return @{ Success = $false; Message = $_.Exception.Message }
    }
}
```

#### Printer Issues
```powershell
# Troubleshoot printer
function Test-PrinterConnection {
    try {
        $printer = Get-WmiObject -Class Win32_Printer -Filter "Name='EPSON TM-T88V'"
        if ($printer -and $printer.WorkOffline) {
            return @{ Status = "Offline"; Message = "Printer is offline" }
        }
        
        # Test print
        $testPage = $printer.PrintTestPage()
        return @{ Status = "Healthy"; Message = "Printer working correctly" }
    }
    catch {
        return @{ Status = "Error"; Message = $_.Exception.Message }
    }
}
```

#### Performance Issues
```powershell
# Diagnose performance issues
function Get-PerformanceMetrics {
    $metrics = @()
    
    # CPU usage
    $cpuUsage = Get-Counter '\Processor(_Total)\% Processor Time' | Select-Object -ExpandProperty CounterSamples
    $metrics += [PSCustomObject]@{
        Metric = "CPU Usage"
        Value = $cpuUsage.CookedValue
        Unit = "%"
        Status = if ($cpuUsage.CookedValue -lt 80) { "Good" } else { "High" }
    }
    
    # Memory usage
    $memory = Get-Counter '\Memory\Available MBytes'
    $metrics += [PSCustomObject]@{
        Metric = "Available Memory"
        Value = $memory.CounterSamples.CookedValue
        Unit = "MB"
        Status = if ($memory.CounterSamples.CookedValue -gt 1024) { "Good" } else { "Low" }
    }
    
    # Disk usage
    $disk = Get-Counter '\PhysicalDisk(_Total)\% Disk Time'
    $metrics += [PSCustomObject]@{
        Metric = "Disk Usage"
        Value = $disk.CounterSamples.CookedValue
        Unit = "%"
        Status = if ($disk.CounterSamples.CookedValue -lt 90) { "Good" } else { "High" }
    }
    
    return $metrics
}
```

### Emergency Procedures

#### System Recovery
```powershell
# emergency_recovery.ps1
function Start-EmergencyRecovery {
    Write-Host "Starting emergency recovery procedures..."
    
    # Stop all services
    Stop-Service -Name "MagideskPOS" -Force
    Stop-Service -Name "postgresql-x64-15" -Force
    
    # Check database integrity
    $integrityCheck = Test-DatabaseIntegrity
    if (-not $integrityCheck.Success) {
        Write-Host "Database integrity check failed. Restoring from backup..."
        Restore-DatabaseFromBackup
    }
    
    # Start services
    Start-Service -Name "postgresql-x64-15"
    Start-Sleep -Seconds 30
    Start-Service -Name "MagideskPOS"
    
    # Verify system health
    $healthCheck = Test-MagideskHealth
    $unhealthyComponents = $healthCheck | Where-Object { $_.Status -eq "Unhealthy" }
    
    if ($unhealthyComponents.Count -eq 0) {
        Write-Host "Emergency recovery completed successfully"
    } else {
        Write-Host "Emergency recovery completed with issues:"
        $unhealthyComponents | ForEach-Object { Write-Host "- $($_.Component): $($_.Message)" }
    }
}
```

## Conclusion

This deployment guide provides comprehensive procedures for deploying and maintaining the Magidesk POS system in production environments. Key considerations include:

- **Planning**: Thorough hardware and network requirements assessment
- **Security**: Multi-layered security approach with proper hardening
- **Reliability**: Redundant systems and comprehensive backup strategies
- **Monitoring**: Proactive monitoring and automated alerting
- **Maintenance**: Regular maintenance procedures and updates
- **Recovery**: Well-defined emergency procedures and recovery plans

Following these guidelines ensures a robust, secure, and reliable POS deployment that can handle the demands of busy restaurant operations.

---

*This deployment guide serves as the definitive reference for installing, configuring, and maintaining the Magidesk POS system in production environments.*