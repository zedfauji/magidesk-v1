# Troubleshooting Runbook

## Overview

This runbook provides step-by-step procedures for diagnosing and resolving common issues with the Magidesk POS system. It covers system failures, performance problems, hardware issues, and user-reported problems with clear diagnostic paths and resolution procedures.

## Emergency Response Procedures

### System Down - Critical Impact

#### Immediate Actions
1. **Assess Impact Scope**
   ```
   Check affected systems:
   - POS Terminals: All or some?
   - Database: Accessible?
   - Network: Connectivity?
   - Kitchen Display: Operational?
   ```

2. **Communicate Status**
   ```
   Notify stakeholders:
   - Restaurant Manager
   - IT Support Team
   - Regional Manager (if multi-location)
   - Customers (if service disruption)
   ```

3. **Start Recovery Timer**
   ```
   Record incident start time
   Target recovery: < 15 minutes for critical systems
   Escalate if > 30 minutes
   ```

#### Diagnostic Steps

**Step 1: Network Connectivity Check**
```powershell
# Test network connectivity
Test-NetConnection -ComputerName 192.168.1.10 -Port 5432  # Database
Test-NetConnection -ComputerName 192.168.1.60 -Port 5020  # KDS
Test-NetConnection -ComputerName 8.8.8.8 -Port 53        # Internet

# Check local network
ipconfig /all
ping 192.168.1.1  # Gateway
ping 192.168.1.10 # Database server
```

**Step 2: Database Service Check**
```bash
# Check PostgreSQL service
systemctl status postgresql-15

# Check database connectivity
psql -h 192.168.1.10 -U magidesk_user -d magidesk -c "SELECT 1;"

# Check database logs
tail -f /var/log/postgresql/postgresql-15-main.log
```

**Step 3: Application Service Check**
```powershell
# Check POS application service
Get-Service -Name "MagideskPOS"

# Check application logs
Get-Content "C:\Magidesk\Logs\magidesk-$(Get-Date -Format 'yyyyMMdd').log" -Tail 50

# Check port availability
netstat -an | findstr :5000
```

#### Recovery Procedures

**Database Recovery**
```bash
# If database service is down
sudo systemctl start postgresql-15
sudo systemctl enable postgresql-15

# If database connection fails
sudo -u postgres psql -c "SELECT pg_reload_conf();"

# If database corruption suspected
pg_dump magidesk > /tmp/emergency_backup.sql
# Restore from backup if necessary
```

**Application Recovery**
```powershell
# Restart POS application
Stop-Service -Name "MagideskPOS" -Force
Start-Service -Name "MagideskPOS"

# Clear application cache
Remove-Item -Path "C:\Magidesk\Cache\*" -Recurse -Force

# Restart terminal if needed
Restart-Computer -Force
```

### Payment Processing Failure

#### Immediate Actions
1. **Isolate Payment Method**
   ```
   Identify affected payment types:
   - Credit/Debit Cards
   - Gift Certificates
   - Cash
   - Mobile Payments
   ```

2. **Switch to Manual Mode**
   ```
   Enable manual processing:
   - Record payments manually
   - Use backup payment terminal
   - Document all transactions
   ```

3. **Notify Payment Provider**
   ```
   Contact payment processor:
   - Check service status
   - Report outage
   - Get ETA for resolution
   ```

#### Diagnostic Steps

**Step 1: Payment Gateway Check**
```powershell
# Test payment gateway connectivity
Invoke-WebRequest -Uri "https://api.stripe.com/v1/charges" -Method GET

# Check API credentials
$apiKey = Get-SecureSetting "Payment:Providers:CreditCard:ApiKey"
$webhookSecret = Get-SecureSetting "Payment:Providers:CreditCard:WebhookSecret"
```

**Step 2: Hardware Check**
```powershell
# Test card reader connection
Get-PnpDevice -Class "SmartCardReader"

# Test receipt printer
Get-Printer | Where-Object { $_.Name -like "*Receipt*" }

# Test cash drawer
# Use manufacturer's test utility
```

**Step 3: Transaction Log Review**
```sql
-- Check recent failed transactions
SELECT * FROM magidesk.Payments 
WHERE Status = 'Declined' 
AND CreatedAt > NOW() - INTERVAL '1 hour'
ORDER BY CreatedAt DESC;

-- Check error patterns
SELECT ProcessorResponse, COUNT(*) as Count
FROM magidesk.Payments 
WHERE Status = 'Declined'
AND CreatedAt > NOW() - INTERVAL '24 hours'
GROUP BY ProcessorResponse
ORDER BY Count DESC;
```

#### Recovery Procedures

**Payment Gateway Recovery**
```powershell
# Reset payment gateway connection
Restart-Service -Name "PaymentGatewayService"

# Update API credentials if expired
Set-SecureSetting "Payment:Providers:CreditCard:ApiKey" $newApiKey

# Test with small transaction
# Use test credit card number
```

**Hardware Recovery**
```powershell
# Reinstall card reader driver
pnputil /add-driver "MagTek_CardReader.inf" /install

# Reset printer spooler
Stop-Service -Name "Spooler" -Force
Remove-Item -Path "C:\Windows\System32\spool\PRINTERS\*" -Force
Start-Service -Name "Spooler"
```

## Performance Issues

### Slow System Response

#### Diagnostic Steps

**Step 1: System Resource Check**
```powershell
# Check CPU usage
Get-Counter '\Processor(_Total)\% Processor Time' -SampleInterval 1 -MaxSamples 10

# Check memory usage
Get-Counter '\Memory\Available MBytes'

# Check disk usage
Get-Counter '\PhysicalDisk(_Total)\% Disk Time'

# Check network usage
Get-Counter '\Network Interface(*)\Bytes Total/sec'
```

**Step 2: Database Performance Check**
```sql
-- Check slow queries
SELECT query, mean_time, calls, total_time
FROM pg_stat_statements
WHERE mean_time > 1000
ORDER BY mean_time DESC
LIMIT 10;

-- Check database connections
SELECT count(*) as active_connections
FROM pg_stat_activity
WHERE state = 'active';

-- Check table sizes
SELECT schemaname, tablename, 
       pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) as size
FROM pg_tables
WHERE schemaname = 'magidesk'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
```

**Step 3: Application Performance Check**
```powershell
# Check application logs for errors
Get-Content "C:\Magidesk\Logs\*.log" | Select-String "ERROR|WARN" | Select-Object -Last 20

# Check database connection pool
# Use application monitoring tools

# Check for memory leaks
# Use performance monitoring tools
```

#### Resolution Procedures

**Database Optimization**
```sql
-- Rebuild indexes
REINDEX DATABASE magidesk;

-- Update statistics
ANALYZE magidesk;

-- Clear cache
SELECT pg_reload_conf();

-- Kill long-running queries
SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE state = 'active'
AND query_start < NOW() - INTERVAL '5 minutes'
AND query NOT LIKE '%pg_stat_activity%';
```

**Application Optimization**
```powershell
# Clear application cache
Remove-Item -Path "C:\Magidesk\Cache\*" -Recurse -Force

# Restart application pool
Restart-WebAppPool -Name "MagideskAppPool"

# Optimize configuration
Set-ConfigValue "Performance:Caching:Enabled" $true
Set-ConfigValue "Performance:Database:ConnectionPoolSize" 100
```

## Hardware Issues

### Printer Problems

#### Receipt Printer Not Working

**Diagnostic Steps**
```powershell
# Check printer status
Get-Printer | Where-Object { $_.Name -like "*Receipt*" }

# Check printer queue
Get-PrintJob -PrinterName "EPSON TM-T88V"

# Test printer connection
# Use manufacturer's test utility

# Check USB connection
Get-PnpDevice | Where-Object { $_.FriendlyName -like "*EPSON*" }
```

**Resolution Procedures**
```powershell
# Restart printer spooler
Stop-Service -Name "Spooler" -Force
Start-Service -Name "Spooler"

# Clear printer queue
Get-PrintJob -PrinterName "EPSON TM-T88V" | Remove-PrintJob

# Reinstall printer driver
pnputil /add-driver "EPSON_TM88V.inf" /install

# Test print
Add-Printer -Name "TestPrinter" -DriverName "EPSON TM-T88V"
Get-Content "test.txt" | Out-Printer -Name "TestPrinter"
```

#### Kitchen Printer Not Working

**Diagnostic Steps**
```bash
# Test network connectivity to kitchen printer
ping 192.168.1.100
telnet 192.168.1.100 9100

# Check printer status
# Use printer's web interface

# Check print queue
lpq -P KitchenPrinter
```

**Resolution Procedures**
```bash
# Restart printer service
systemctl restart cups

# Clear printer queue
cancel -a -P KitchenPrinter

# Reconfigure printer
lpadmin -p KitchenPrinter -v socket://192.168.1.100:9100 -m raw

# Test print
echo "Test Print" | lp -d KitchenPrinter
```

### Cash Drawer Issues

#### Cash Drawer Not Opening

**Diagnostic Steps**
```powershell
# Check cash drawer connection
Get-PnpDevice | Where-Object { $_.FriendlyName -like "*APG*" }

# Test cash drawer command
# Use manufacturer's test utility

# Check USB port
Get-PnpDevice -Class "USB"
```

**Resolution Procedures**
```powershell
# Reset USB port
Disable-PnpDevice -InstanceId "USB\VID_XXXX&PID_XXXX\..."
Enable-PnpDevice -InstanceId "USB\VID_XXXX&PID_XXXX..."

# Reinstall cash drawer driver
pnputil /add-driver "APG_CashDrawer.inf" /install

# Test open command
# Send open code: 27,112,0,25,250
```

### Card Reader Issues

#### Card Reader Not Responding

**Diagnostic Steps**
```powershell
# Check card reader connection
Get-PnpDevice | Where-Object { $_.FriendlyName -like "*MagTek*" }

# Test card reader
# Use manufacturer's test utility

# Check USB port
Get-PnpDevice -Class "USB"
```

**Resolution Procedures**
```powershell
# Reset card reader
Disable-PnpDevice -InstanceId "USB\VID_XXXX&PID_XXXX..."
Enable-PnpDevice -InstanceId "USB\VID_XXXX&PID_XXXX..."

# Reinstall driver
pnputil /add-driver "MagTek_CardReader.inf" /install

# Test with sample card
# Use test credit card number
```

## Data Issues

### Database Corruption

#### Detection
```sql
-- Check for database corruption
SELECT * FROM pg_database WHERE datname = 'magidesk';

-- Check table integrity
SELECT * FROM pg_class WHERE relname = 'tickets';

-- Run database consistency check
-- Use pg_dump to verify
```

#### Recovery Procedures
```bash
# Create emergency backup
pg_dump magidesk > /tmp/emergency_backup.sql

# Restore from last known good backup
psql -d magidesk < /backup/magidesk_backup_YYYYMMDD.sql

# Run database repair
pg_resetwal /var/lib/postgresql/15/main/
```

### Data Synchronization Issues

#### Detection
```sql
-- Check for orphaned records
SELECT t.Id FROM magidesk.Tickets t
LEFT JOIN magidesk.Payments p ON t.Id = p.TicketId
WHERE t.Status = 'Paid' AND p.TicketId IS NULL;

-- Check for data inconsistencies
SELECT SUM(t.TotalAmount) as TicketTotal, SUM(p.Amount) as PaymentTotal
FROM magidesk.Tickets t
LEFT JOIN magidesk.Payments p ON t.Id = p.TicketId
WHERE t.Status = 'Paid';
```

#### Resolution Procedures
```sql
-- Fix orphaned payments
UPDATE magidesk.Payments 
SET Status = 'Voided'
WHERE TicketId NOT IN (SELECT Id FROM magidesk.Tickets);

-- Reconcile ticket totals
UPDATE magidesk.Tickets
SET PaidAmount = (
    SELECT COALESCE(SUM(Amount), 0)
    FROM magidesk.Payments
    WHERE TicketId = magidesk.Tickets.Id
    AND Status IN ('Completed', 'Captured')
);
```

## User Issues

### Login Problems

#### User Cannot Log In

**Diagnostic Steps**
```sql
-- Check user account
SELECT * FROM magidesk.Users 
WHERE Username = 'problem_user';

-- Check failed login attempts
SELECT * FROM magidesk.SystemLogs 
WHERE Category = 'Authentication'
AND Message LIKE '%problem_user%'
AND CreatedAt > NOW() - INTERVAL '1 hour';
```

**Resolution Procedures**
```sql
-- Reset user password
UPDATE magidesk.Users
SET PasswordHash = crypt('NewPassword123!', gen_salt('bf'))
WHERE Username = 'problem_user';

-- Unlock account
UPDATE magidesk.Users
SET IsLocked = false, FailedLoginAttempts = 0
WHERE Username = 'problem_user';
```

### Permission Issues

#### User Cannot Access Features

**Diagnostic Steps**
```sql
-- Check user permissions
SELECT * FROM magidesk.Users 
WHERE Id = 'user_id';

-- Check role permissions
SELECT * FROM magidesk.SystemSettings 
WHERE Category = 'Permissions'
AND Key LIKE '%user_role%';
```

**Resolution Procedures**
```sql
-- Update user permissions
UPDATE magidesk.Users
SET Permissions = ARRAY['order.create', 'order.view', 'payment.process']
WHERE Id = 'user_id';

-- Update role permissions
UPDATE magidesk.SystemSettings
SET Value = '["order.create", "order.view", "payment.process"]'
WHERE Category = 'Permissions'
AND Key = 'Server';
```

## Network Issues

### Connectivity Problems

#### Cannot Connect to Database

**Diagnostic Steps**
```powershell
# Test network connectivity
Test-NetConnection -ComputerName 192.168.1.10 -Port 5432

# Check DNS resolution
nslookup magidesk-db.local

# Check routing
tracert 192.168.1.10
```

**Resolution Procedures**
```powershell
# Reset network adapter
Restart-NetAdapter -Name "Ethernet"

# Update DNS settings
Set-DnsClientServerAddress -InterfaceAlias "Ethernet" -ServerAddresses "192.168.1.1"

# Flush DNS cache
Clear-DnsClientCache
```

### Performance Issues

#### Slow Network Response

**Diagnostic Steps**
```powershell
# Test network speed
Test-NetConnection -ComputerName 192.168.1.10 -Port 5432

# Check network utilization
Get-Counter '\Network Interface(*)\Bytes Total/sec'

# Check for network congestion
# Use network monitoring tools
```

**Resolution Procedures**
```powershell
# Optimize network settings
Set-NetTCPSetting -SettingName InternetCustom -AutoTuningLevel HighlyRestricted

# Update network drivers
# Download and install latest drivers

# Check network hardware
# Replace cables if damaged
```

## Security Issues

### Unauthorized Access

#### Detection
```sql
-- Check for suspicious login attempts
SELECT * FROM magidesk.SystemLogs 
WHERE Category = 'Authentication'
AND Message LIKE '%failed%'
AND CreatedAt > NOW() - INTERVAL '24 hours';

-- Check for unusual activity
SELECT * FROM magidesk.AuditLogs 
WHERE Action IN ('INSERT', 'UPDATE', 'DELETE')
AND CreatedAt > NOW() - INTERVAL '1 hour'
AND UserId NOT IN (SELECT Id FROM magidesk.Users WHERE IsActive = true);
```

**Response Procedures**
```sql
-- Lock suspicious accounts
UPDATE magidesk.Users
SET IsLocked = true
WHERE Username IN ('suspicious_user1', 'suspicious_user2');

-- Review recent changes
SELECT * FROM magidesk.AuditLogs 
WHERE CreatedAt > NOW() - INTERVAL '24 hours'
ORDER BY CreatedAt DESC;
```

### Data Breach

#### Immediate Actions
1. **Isolate Affected Systems**
   ```
   Disconnect from network
   Stop all services
   Preserve evidence
   ```

2. **Assess Impact**
   ```
   Identify compromised data
   Determine affected users
   Calculate potential damage
   ```

3. **Notify Stakeholders**
   ```
   Security team
   Management
   Legal department
   Affected customers
   ```

#### Recovery Procedures
```sql
-- Reset all user passwords
UPDATE magidesk.Users
SET PasswordHash = crypt('TempPassword123!', gen_salt('bf')),
    PasswordChangedAt = NOW()
WHERE IsActive = true;

-- Review and revoke suspicious sessions
DELETE FROM magidesk.UserSessions 
WHERE CreatedAt < NOW() - INTERVAL '1 hour';

-- Update security settings
UPDATE magidesk.SystemSettings
SET Value = '3'
WHERE Category = 'Security'
AND Key = 'PasswordPolicy:MaxFailedAttempts';
```

## Preventive Maintenance

### Daily Checks

#### System Health Check
```powershell
# Automated daily health check
function Daily-HealthCheck {
    $results = @()
    
    # Check database connectivity
    $dbTest = Test-DatabaseConnection
    $results += @{ Component = "Database"; Status = $dbTest.Status }
    
    # Check application services
    $appTest = Test-ApplicationService
    $results += @{ Component = "Application"; Status = $appTest.Status }
    
    # Check hardware
    $printerTest = Test-PrinterConnection
    $results += @{ Component = "Printer"; Status = $printerTest.Status }
    
    # Generate report
    $results | ConvertTo-Json | Out-File "C:\Magidesk\Logs\health_check_$(Get-Date -Format 'yyyyMMdd').json"
}
```

#### Log Review
```powershell
# Review error logs
Get-Content "C:\Magidesk\Logs\*.log" | 
    Select-String "ERROR|FATAL" | 
    Select-Object -Last 50 | 
    Out-File "C:\Magidesk\Logs\error_summary_$(Get-Date -Format 'yyyyMMdd').txt"
```

### Weekly Maintenance

#### Database Maintenance
```sql
-- Weekly database maintenance
BEGIN;
-- Rebuild indexes
REINDEX DATABASE magidesk;

-- Update statistics
ANALYZE magidesk;

-- Clean up old logs
DELETE FROM magidesk.SystemLogs 
WHERE CreatedAt < NOW() - INTERVAL '30 days';

COMMIT;
```

#### System Updates
```powershell
# Check for system updates
Get-WindowsUpdate

# Apply security patches
Install-WindowsUpdate -AcceptAll -AutoReboot

# Update application
# Deploy latest version
```

### Monthly Maintenance

#### Performance Review
```sql
-- Monthly performance review
SELECT 
    DATE_TRUNC('month', CreatedAt) as month,
    COUNT(*) as ticket_count,
    AVG(TotalAmount) as avg_ticket_value,
    SUM(TotalAmount) as total_sales
FROM magidesk.Tickets
WHERE CreatedAt > NOW() - INTERVAL '12 months'
GROUP BY DATE_TRUNC('month', CreatedAt)
ORDER BY month DESC;
```

#### Security Audit
```sql
-- Monthly security audit
SELECT 
    u.Username,
    u.LastLoginAt,
    COUNT(sl.Id) as failed_logins
FROM magidesk.Users u
LEFT JOIN magidesk.SystemLogs sl ON u.Username = sl.Message
    AND sl.Category = 'Authentication'
    AND sl.Message LIKE '%failed%'
    AND sl.CreatedAt > NOW() - INTERVAL '30 days'
GROUP BY u.Username, u.LastLoginAt
HAVING COUNT(sl.Id) > 5;
```

## Escalation Procedures

### Level 1 Support

#### When to Escalate
- Issue not resolved within 30 minutes
- Multiple users affected
- System-wide failure
- Security incident suspected

#### Escalation Process
1. Document all troubleshooting steps
2. Gather system logs and error messages
3. Contact Level 2 support
4. Provide detailed incident report

### Level 2 Support

#### When to Escalate
- Issue not resolved within 2 hours
- Database corruption suspected
- Hardware failure
- Security breach confirmed

#### Escalation Process
1. Engage database administrator
2. Contact hardware vendor
3. Notify security team
4. Prepare incident report

### Level 3 Support

#### When to Escalate
- Critical system failure
- Data loss
- Security breach with data exposure
- Extended downtime (> 4 hours)

#### Escalation Process
1. Engage senior management
2. Contact legal department
3. Notify regulatory authorities (if required)
4. Prepare comprehensive incident report

## Documentation

### Incident Reporting

#### Incident Report Template
```
Incident ID: [Auto-generated]
Date/Time: [Start and end times]
Impact: [Systems/users affected]
Severity: [Critical/High/Medium/Low]
Description: [Detailed description]
Root Cause: [Analysis of cause]
Resolution: [Steps taken]
Prevention: [Future prevention measures]
Lessons Learned: [Key takeaways]
```

#### Knowledge Base Updates

After resolving incidents:
1. Document symptoms and solutions
2. Update troubleshooting procedures
3. Create new runbooks if needed
4. Share with support team

## Conclusion

This troubleshooting runbook provides comprehensive procedures for handling common and critical issues with the Magidesk POS system. Key success factors include:

- **Systematic Approach**: Follow diagnostic steps methodically
- **Documentation**: Record all actions and findings
- **Communication**: Keep stakeholders informed
- **Prevention**: Learn from incidents to prevent recurrence
- **Continuous Improvement**: Update procedures based on experience

Regular training and practice of these procedures ensures quick and effective response to system issues, minimizing downtime and impact on restaurant operations.

---

*This troubleshooting runbook serves as the definitive reference for diagnosing and resolving issues with the Magidesk POS system.*