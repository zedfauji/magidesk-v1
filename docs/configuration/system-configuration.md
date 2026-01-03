# System Configuration

## Overview

The Magidesk POS system uses a comprehensive configuration management approach that supports multiple deployment environments, flexible business rules, and runtime customization. This document details all configuration aspects from application settings to business rule parameters.

## Configuration Architecture

### Configuration Hierarchy

```
1. Base Configuration (appsettings.json)
2. Environment Configuration (appsettings.{Environment}.json)
3. User Configuration (user-specific settings)
4. Database Configuration (runtime settings stored in database)
5. Environment Variables (deployment overrides)
6. Command Line Arguments (runtime overrides)
```

### Configuration Providers

The system uses .NET configuration providers in the following order of precedence:

```csharp
// Configuration builder setup
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .AddDatabaseConfiguration() // Custom provider for database-stored settings
    .Build();
```

## Base Configuration

### Application Settings (appsettings.json)

```json
{
  "Application": {
    "Name": "Magidesk POS",
    "Version": "1.0.0",
    "Environment": "Production",
    "InstanceName": "POS-001",
    "LocationName": "Main Restaurant",
    "TimeZone": "America/New_York",
    "Culture": "en-US",
    "Currency": "USD",
    "DateFormat": "MM/dd/yyyy",
    "TimeFormat": "hh:mm tt",
    "NumberFormat": {
      "DecimalDigits": 2,
      "DecimalSeparator": ".",
      "GroupSeparator": ","
    }
  },
  "Database": {
    "Provider": "PostgreSQL",
    "ConnectionString": "Host=localhost;Database=magidesk;Username=magidesk_user;Password=secure_password;",
    "Schema": "magidesk",
    "CommandTimeout": 30,
    "EnableRetryOnFailure": true,
    "MaxRetryCount": 3,
    "EnableSensitiveDataLogging": false,
    "EnableDetailedErrors": false,
    "MigrationsAssembly": "Magidesk.Infrastructure"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "Microsoft.EntityFrameworkCore": "Warning",
      "System": "Warning"
    },
    "Console": {
      "IncludeScopes": true,
      "TimestampFormat": "yyyy-MM-dd HH:mm:ss "
    },
    "File": {
      "Path": "logs/magidesk-{Date}.log",
      "RollingInterval": "Day",
      "RetainedFileCountLimit": 30,
      "OutputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
    },
    "Database": {
      "EnableDatabaseLogging": true,
      "LogLevelThreshold": "Warning"
    }
  },
  "Security": {
    "JwtSettings": {
      "SecretKey": "your-256-bit-secret-key-here",
      "Issuer": "MagideskPOS",
      "Audience": "MagideskUsers",
      "ExpirationMinutes": 480,
      "RefreshTokenExpirationDays": 30
    },
    "PasswordPolicy": {
      "MinLength": 8,
      "RequireUppercase": true,
      "RequireLowercase": true,
      "RequireDigit": true,
      "RequireSpecialChar": true,
      "MaxFailedAttempts": 5,
      "LockoutDurationMinutes": 30
    },
    "SessionSettings": {
      "TimeoutMinutes": 60,
      "SlidingExpiration": true,
      "AbsoluteExpirationHours": 8
    }
  },
  "Payment": {
    "Providers": {
      "CreditCard": {
        "Provider": "Stripe",
        "ApiKey": "sk_test_...",
        "WebhookSecret": "whsec_...",
        "Enable3DSecure": true,
        "RequireCVC": true
      },
      "GiftCertificate": {
        "Provider": "Internal",
        "CertificatePrefix": "GC",
        "CertificateLength": 12,
        "DefaultExpiryYears": 2
      }
    },
    "ReceiptSettings": {
      "HeaderLines": [
        "Restaurant Name",
        "123 Main Street",
        "City, State 12345",
        "Phone: (555) 123-4567"
      ],
      "FooterLines": [
        "Thank you for dining with us!",
        "Please come again soon"
      ],
      "IncludeQrCode": true,
      "IncludeBarcode": true,
      "PrinterWidth": 42,
      "LineEnding": "CRLF"
    }
  },
  "Hardware": {
    "Printers": {
      "ReceiptPrinter": {
        "Name": "EPSON TM-T88V",
        "Connection": "USB",
        "Port": "USB001",
        "BaudRate": 9600,
        "DataBits": 8,
        "Parity": "None",
        "StopBits": "One"
      },
      "KitchenPrinter": {
        "Name": "Star TSP650II",
        "Connection": "Ethernet",
        "IpAddress": "192.168.1.100",
        "Port": 9100
      }
    },
    "CashDrawer": {
        "Name": "APG Cash Drawer",
        "Connection": "USB",
        "Port": "USB002",
        "OpenCode": "27,112,0,25,250"
    },
    "CardReader": {
        "Name": "MagTek",
        "Connection": "USB",
        "Port": "USB003",
        "EnableEncryption": true
    },
    "Display": {
        "CustomerDisplay": {
          "Enabled": true,
          "Type": "LineDisplay",
          "Connection": "USB",
          "Port": "USB004",
          "Lines": 2,
          "CharactersPerLine": 20
        }
      }
    },
  "Kitchen": {
    "DisplaySystem": {
      "Enabled": true,
      "Type": "KDS",
      "RefreshInterval": 5,
      "AutoRefresh": true,
      "ShowPreparationTime": true,
      "ShowPriority": true,
      "ColorCoding": {
        "New": "#FF0000",
        "InProgress": "#FFA500",
        "Ready": "#00FF00",
        "Delayed": "#800080"
      }
    },
    "Routing": {
      "DefaultStation": "Preparation",
      "AutoRoute": true,
      "SplitByStation": true,
      "ConsolidateSimilarItems": true
    }
  },
  "Business": {
    "OperatingHours": {
      "Monday": { "Open": "10:00", "Close": "22:00" },
      "Tuesday": { "Open": "10:00", "Close": "22:00" },
      "Wednesday": { "Open": "10:00", "Close": "22:00" },
      "Thursday": { "Open": "10:00", "Close": "22:00" },
      "Friday": { "Open": "10:00", "Close": "23:00" },
      "Saturday": { "Open": "09:00", "Close": "23:00" },
      "Sunday": { "Open": "09:00", "Close": "21:00" }
    },
    "Tax": {
      "DefaultRate": 0.0825,
      "TaxIncluded": false,
      "RoundingMethod": "RoundHalfUp",
      "TaxExemptEnabled": true
    },
    "Service": {
      "AutoGratuity": {
        "Enabled": true,
        "GuestThreshold": 6,
        "Percentage": 0.18,
        "CanBeRemoved": false
      },
      "Delivery": {
        "Enabled": true,
        "MinimumOrder": 15.00,
        "DeliveryFee": 2.99,
        "FreeDeliveryThreshold": 50.00
      }
    },
    "Inventory": {
      "LowStockAlert": true,
      "LowStockThreshold": 10,
      "AutoReorder": false,
      "TrackWaste": true,
      "WasteReasons": [
        "Spoilage",
        "Preparation Error",
        "Customer Complaint",
        "Expired",
        "Other"
      ]
    }
  },
  "Reporting": {
    "DefaultReports": [
      "DailySales",
      "HourlySales",
      "MenuItemSales",
      "ServerPerformance",
      "PaymentSummary"
    ],
    "Retention": {
      "DailyReports": 365,
      "WeeklyReports": 104,
      "MonthlyReports": 120,
      "YearlyReports": 10
    },
    "Export": {
      "Formats": ["PDF", "Excel", "CSV"],
      "DefaultFormat": "PDF",
      "IncludeCharts": true,
      "IncludeDetails": true
    }
  },
  "Performance": {
    "Caching": {
      "Enabled": true,
      "DefaultExpiration": "00:15:00",
      "SizeLimit": 1000,
      "CompactionPercentage": 0.05
    },
    "Database": {
      "ConnectionPoolSize": 100,
      "CommandTimeout": 30,
      "EnableBatching": true,
      "MaxBatchSize": 100
    },
    "UI": {
      "AnimationEnabled": true,
      "TransitionDuration": 200,
      "AutoSaveInterval": 30,
      "RefreshInterval": 5
    }
  }
}
```

## Environment-Specific Configuration

### Development Environment (appsettings.Development.json)

```json
{
  "Application": {
    "Environment": "Development"
  },
  "Database": {
    "ConnectionString": "Host=localhost;Database=magidesk_dev;Username=dev_user;Password=dev_password;",
    "EnableSensitiveDataLogging": true,
    "EnableDetailedErrors": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "Security": {
    "JwtSettings": {
      "ExpirationMinutes": 1440
    }
  },
  "Payment": {
    "Providers": {
      "CreditCard": {
        "Provider": "Test",
        "ApiKey": "test_key",
        "EnableTestMode": true
      }
    }
  }
}
```

### Production Environment (appsettings.Production.json)

```json
{
  "Application": {
    "Environment": "Production"
  },
  "Database": {
    "ConnectionString": "${DATABASE_CONNECTION_STRING}",
    "EnableSensitiveDataLogging": false,
    "EnableDetailedErrors": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Error"
    }
  },
  "Security": {
    "JwtSettings": {
      "SecretKey": "${JWT_SECRET_KEY}"
    }
  },
  "Payment": {
    "Providers": {
      "CreditCard": {
        "ApiKey": "${STRIPE_API_KEY}",
        "WebhookSecret": "${STRIPE_WEBHOOK_SECRET}"
      }
    }
  }
}
```

## Database Configuration

### Configuration Tables

```sql
-- System settings stored in database
CREATE TABLE magidesk.SystemSettings (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Category VARCHAR(50) NOT NULL,
    Key VARCHAR(100) NOT NULL,
    Value TEXT,
    DataType VARCHAR(20) NOT NULL DEFAULT 'String' CHECK (DataType IN ('String', 'Integer', 'Decimal', 'Boolean', 'DateTime', 'Json')),
    Description TEXT,
    IsEncrypted BOOLEAN NOT NULL DEFAULT FALSE,
    RequiresRestart BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UNIQUE(Category, Key)
);

CREATE INDEX IX_SystemSettings_Category ON magidesk.SystemSettings(Category);
CREATE INDEX IX_SystemSettings_Key ON magidesk.SystemSettings(Key);

-- User-specific settings
CREATE TABLE magidesk.UserSettings (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID NOT NULL REFERENCES magidesk.Users(Id) ON DELETE CASCADE,
    Category VARCHAR(50) NOT NULL,
    Key VARCHAR(100) NOT NULL,
    Value TEXT,
    DataType VARCHAR(20) NOT NULL DEFAULT 'String',
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UNIQUE(UserId, Category, Key)
);

CREATE INDEX IX_UserSettings_UserId ON magidesk.UserSettings(UserId);
CREATE INDEX IX_UserSettings_Category ON magidesk.UserSettings(Category);
```

### Configuration Service

```csharp
public interface IConfigurationService
{
    Task<T> GetValueAsync<T>(string category, string key, T defaultValue = default);
    Task SetValueAsync<T>(string category, string key, T value);
    Task<T> GetUserSettingAsync<T>(Guid userId, string category, string key, T defaultValue = default);
    Task SetUserSettingAsync<T>(Guid userId, string category, string key, T value);
    Task<IEnumerable<SystemSetting>> GetSettingsByCategoryAsync(string category);
    Task ReloadConfigurationAsync();
}

public class ConfigurationService : IConfigurationService
{
    private readonly IMagideskDbContext _context;
    private readonly ILogger<ConfigurationService> _logger;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public ConfigurationService(
        IMagideskDbContext context,
        ILogger<ConfigurationService> logger,
        IMemoryCache cache,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _configuration = configuration;
    }

    public async Task<T> GetValueAsync<T>(string category, string key, T defaultValue = default)
    {
        var cacheKey = $"config:{category}:{key}";
        
        if (_cache.TryGetValue(cacheKey, out T cachedValue))
        {
            return cachedValue;
        }

        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Category == category && s.Key == key);

        if (setting == null)
        {
            // Fallback to appsettings
            var configValue = _configuration[$"{category}:{key}"];
            if (configValue != null)
            {
                return ConvertValue<T>(configValue);
            }
            return defaultValue;
        }

        var value = ConvertValue<T>(setting.Value, setting.DataType);
        _cache.Set(cacheKey, value, TimeSpan.FromMinutes(15));
        return value;
    }

    public async Task SetValueAsync<T>(string category, string key, T value)
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Category == category && s.Key == key);

        if (setting == null)
        {
            setting = new SystemSetting
            {
                Category = category,
                Key = key,
                DataType = typeof(T).Name,
                CreatedAt = DateTime.UtcNow
            };
            _context.SystemSettings.Add(setting);
        }

        setting.Value = value?.ToString();
        setting.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Clear cache
        var cacheKey = $"config:{category}:{key}";
        _cache.Remove(cacheKey);

        _logger.LogInformation("Configuration updated: {Category}:{Key} = {Value}", category, key, value);
    }

    private T ConvertValue<T>(string value, string dataType = null)
    {
        if (string.IsNullOrEmpty(value))
            return default(T);

        try
        {
            var targetType = dataType != null ? Type.GetType($"System.{dataType}") : typeof(T);
            return (T)Convert.ChangeType(value, targetType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert configuration value: {Value} to {Type}", value, typeof(T).Name);
            return default(T);
        }
    }
}
```

## Business Rules Configuration

### Menu Configuration

```sql
-- Menu settings
INSERT INTO magidesk.SystemSettings (Category, Key, Value, DataType, Description) VALUES
('Menu', 'AutoSaveInterval', '30', 'Integer', 'Auto-save interval for menu items in seconds'),
('Menu', 'ShowImages', 'true', 'Boolean', 'Show menu item images in selection'),
('Menu', 'EnableSearch', 'true', 'Boolean', 'Enable menu item search functionality'),
('Menu', 'MaxSearchResults', '50', 'Integer', 'Maximum number of search results to display'),
('Menu', 'HighlightNewItems', 'true', 'Boolean', 'Highlight newly added menu items'),
('Menu', 'NewItemDays', '7', 'Integer', 'Number of days to consider item as new');
```

### Order Configuration

```sql
-- Order settings
INSERT INTO magidesk.SystemSettings (Category, Key, Value, DataType, Description) VALUES
('Order', 'AutoSendToKitchen', 'false', 'Boolean', 'Automatically send orders to kitchen when added'),
('Order', 'RequireTableAssignment', 'true', 'Boolean', 'Require table assignment for dine-in orders'),
('Order', 'AllowNegativeBalance', 'false', 'Boolean', 'Allow tickets with negative balances'),
('Order', 'MaxOrderValue', '9999.99', 'Decimal', 'Maximum order total amount'),
('Order', 'DefaultGuestCount', '1', 'Integer', 'Default guest count for new tickets'),
('Order', 'AutoCalculateTax', 'true', 'Boolean', 'Automatically calculate tax on order changes'),
('Order', 'ShowRunningTotal', 'true', 'Boolean', 'Show running total during order entry');
```

### Payment Configuration

```sql
-- Payment settings
INSERT INTO magidesk.SystemSettings (Category, Key, Value, DataType, Description) VALUES
('Payment', 'RequireTipEntry', 'false', 'Boolean', 'Require tip entry for card payments'),
('Payment', 'DefaultTipPercentage', '15', 'Decimal', 'Default tip percentage suggestion'),
('Payment', 'TipPercentages', '[15,18,20,25]', 'Json', 'Tip percentage suggestions'),
('Payment', 'EnableSplitPayment', 'true', 'Boolean', 'Enable split payment functionality'),
('Payment', 'MaxPaymentMethods', '5', 'Integer', 'Maximum number of payment methods per ticket'),
('Payment', 'RequireManagerForLargeCash', 'true', 'Boolean', 'Require manager approval for large cash payments'),
('Payment', 'LargeCashThreshold', '500.00', 'Decimal', 'Cash payment amount requiring manager approval');
```

## User Interface Configuration

### Display Settings

```sql
-- UI settings
INSERT INTO magidesk.SystemSettings (Category, Key, Value, DataType, Description) VALUES
('UI', 'Theme', 'Light', 'String', 'Application theme (Light, Dark, Auto)'),
('UI', 'FontSize', 'Medium', 'String', 'Font size (Small, Medium, Large, ExtraLarge)'),
('UI', 'ShowKeyboardShortcuts', 'true', 'Boolean', 'Show keyboard shortcuts in menus'),
('UI', 'AnimationEnabled', 'true', 'Boolean', 'Enable UI animations'),
('UI', 'SoundEnabled', 'true', 'Boolean', 'Enable system sounds'),
('UI', 'AutoLogout', 'true', 'Boolean', 'Enable automatic user logout'),
('UI', 'AutoLogoutMinutes', '60', 'Integer', 'Minutes of inactivity before auto-logout');
```

### Layout Configuration

```sql
-- Layout settings
INSERT INTO magidesk.SystemSettings (Category, Key, Value, DataType, Description) VALUES
('Layout', 'MainMenuColumns', '4', 'Integer', 'Number of columns in main menu'),
('Layout', 'ShowTableMap', 'true', 'Boolean', 'Show table map on main screen'),
('Layout', 'CompactMode', 'false', 'Boolean', 'Use compact layout for smaller screens'),
('Layout', 'ShowOrderSummary', 'true', 'Boolean', 'Show order summary panel'),
('Layout', 'PanelWidth', '300', 'Integer', 'Width of side panels in pixels');
```

## Hardware Configuration

### Printer Configuration

```sql
-- Printer settings
INSERT INTO magidesk.SystemSettings (Category, Key, Value, DataType, Description) VALUES
('Printer', 'ReceiptPrinter', 'EPSON_TM88V', 'String', 'Default receipt printer model'),
('Printer', 'KitchenPrinter', 'Star_TSP650', 'String', 'Default kitchen printer model'),
('Printer', 'AutoPrintReceipt', 'true', 'Boolean', 'Automatically print receipts'),
('Printer', 'PrintKitchenCopies', '2', 'Integer', 'Number of kitchen ticket copies'),
('Printer', 'ReceiptWidth', '42', 'Integer', 'Receipt printer width in characters'),
('Printer', 'KitchenWidth', '40', 'Integer', 'Kitchen printer width in characters');
```

### Cash Drawer Configuration

```sql
-- Cash drawer settings
INSERT INTO magidesk.SystemSettings (Category, Key, Value, DataType, Description) VALUES
('CashDrawer', 'AutoOpen', 'true', 'Boolean', 'Automatically open cash drawer for cash payments'),
('CashDrawer', 'RequireCashCount', 'true', 'Boolean', 'Require cash count on shift end'),
('CashDrawer', 'AllowOverShort', 'true', 'Boolean', 'Allow cash over/short on shift end'),
('CashDrawer', 'MaxOverShort', '25.00', 'Decimal', 'Maximum allowed over/short amount');
```

## Integration Configuration

### Third-Party Services

```sql
-- Integration settings
INSERT INTO magidesk.SystemSettings (Category, Key, Value, DataType, Description) VALUES
('Integration', 'AccountingSoftware', 'QuickBooks', 'String', 'Accounting software integration'),
('Integration', 'InventorySystem', 'None', 'String', 'Inventory management system'),
('Integration', 'LoyaltyProgram', 'Internal', 'String', 'Customer loyalty program'),
('Integration', 'OnlineOrdering', 'None', 'String', 'Online ordering platform'),
('Integration', 'DeliveryService', 'None', 'String', 'Delivery service integration');
```

### API Configuration

```sql
-- API settings
INSERT INTO magidesk.SystemSettings (Category, Key, Value, DataType, Description) VALUES
('API', 'EnableExternalAPI', 'false', 'Boolean', 'Enable external API access'),
('API', 'APIKeyRequired', 'true', 'Boolean', 'Require API key for external access'),
('API', 'RateLimitEnabled', 'true', 'Boolean', 'Enable API rate limiting'),
('API', 'RateLimitPerMinute', '100', 'Integer', 'API requests per minute limit'),
('API', 'CORS Origins', '[]', 'Json', 'Allowed CORS origins for API');
```

## Configuration Management

### Configuration Validation

```csharp
public class ConfigurationValidator
{
    private readonly IConfigurationService _configService;
    private readonly ILogger<ConfigurationValidator> _logger;

    public ConfigurationValidator(
        IConfigurationService configService,
        ILogger<ConfigurationValidator> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateConfigurationAsync()
    {
        var result = new ValidationResult();

        // Validate database connection
        await ValidateDatabaseConnection(result);

        // Validate required settings
        await ValidateRequiredSettings(result);

        // Validate business rules
        await ValidateBusinessRules(result);

        // Validate hardware configuration
        await ValidateHardwareConfiguration(result);

        return result;
    }

    private async Task ValidateDatabaseConnection(ValidationResult result)
    {
        try
        {
            // Test database connectivity
            var testQuery = "SELECT 1";
            // Implementation would test actual database connection
        }
        catch (Exception ex)
        {
            result.AddError("Database connection failed: " + ex.Message);
        }
    }

    private async Task ValidateRequiredSettings(ValidationResult result)
    {
        var requiredSettings = new[]
        {
            ("Application", "Name"),
            ("Application", "TimeZone"),
            ("Security", "JwtSettings:SecretKey"),
            ("Payment", "Providers:CreditCard:Provider")
        };

        foreach (var (category, key) in requiredSettings)
        {
            var value = await _configService.GetValueAsync<string>(category, key);
            if (string.IsNullOrEmpty(value))
            {
                result.AddError($"Required setting missing: {category}:{key}");
            }
        }
    }

    private async Task ValidateBusinessRules(ValidationResult result)
    {
        // Validate tax settings
        var taxRate = await _configService.GetValueAsync<decimal>("Business", "Tax:DefaultRate");
        if (taxRate < 0 || taxRate > 1)
        {
            result.AddError("Tax rate must be between 0 and 1");
        }

        // Validate service charges
        var gratuityThreshold = await _configService.GetValueAsync<int>("Business", "Service:AutoGratuity:GuestThreshold");
        if (gratuityThreshold < 1)
        {
            result.AddError("Auto-gratuity guest threshold must be at least 1");
        }
    }
}
```

### Configuration Backup and Restore

```csharp
public class ConfigurationBackupService
{
    private readonly IConfigurationService _configService;
    private readonly IMagideskDbContext _context;
    private readonly ILogger<ConfigurationBackupService> _logger;

    public async Task<string> BackupConfigurationAsync()
    {
        var settings = await _context.SystemSettings.ToListAsync();
        var backup = new ConfigurationBackup
        {
            Timestamp = DateTime.UtcNow,
            Settings = settings.Select(s => new SettingBackup
            {
                Category = s.Category,
                Key = s.Key,
                Value = s.Value,
                DataType = s.DataType
            }).ToList()
        };

        var json = JsonSerializer.Serialize(backup, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return json;
    }

    public async Task RestoreConfigurationAsync(string backupJson)
    {
        var backup = JsonSerializer.Deserialize<ConfigurationBackup>(backupJson);
        
        foreach (var setting in backup.Settings)
        {
            await _configService.SetValueAsync(setting.Category, setting.Key, setting.Value);
        }

        _logger.LogInformation("Configuration restored from backup dated {Timestamp}", backup.Timestamp);
    }
}
```

## Configuration Monitoring

### Health Checks

```csharp
public class ConfigurationHealthCheck : IHealthCheck
{
    private readonly IConfigurationService _configService;
    private readonly ILogger<ConfigurationHealthCheck> _logger;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Test configuration service
            var testValue = await _configService.GetValueAsync<string>("Application", "Name");
            
            if (string.IsNullOrEmpty(testValue))
            {
                return HealthCheckResult.Unhealthy("Configuration service not responding");
            }

            // Test database connectivity
            // Implementation would test actual database connection

            return HealthCheckResult.Healthy("Configuration system operational");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Configuration health check failed");
            return HealthCheckResult.Unhealthy("Configuration system error", ex);
        }
    }
}
```

### Configuration Change Notifications

```csharp
public class ConfigurationChangeNotifier
{
    private readonly IConfigurationService _configService;
    private readonly ILogger<ConfigurationChangeNotifier> _logger;

    public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;

    public async Task StartMonitoringAsync()
    {
        // Implementation would monitor database changes and notify subscribers
        // This could use SQL dependency or polling mechanism
    }

    private void OnConfigurationChanged(string category, string key, object oldValue, object newValue)
    {
        var args = new ConfigurationChangedEventArgs
        {
            Category = category,
            Key = key,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedAt = DateTime.UtcNow
        };

        ConfigurationChanged?.Invoke(this, args);
        _logger.LogInformation("Configuration changed: {Category}:{Key} from {OldValue} to {NewValue}", 
            category, key, oldValue, newValue);
    }
}
```

## Security Considerations

### Sensitive Data Protection

```csharp
public class SecureConfigurationService
{
    private readonly IEncryptionService _encryptionService;
    private readonly IConfigurationService _configService;

    public async Task SetSecureValueAsync(string category, string key, string value)
    {
        var encryptedValue = await _encryptionService.EncryptAsync(value);
        await _configService.SetValueAsync(category, key, encryptedValue);
    }

    public async Task<string> GetSecureValueAsync(string category, string key)
    {
        var encryptedValue = await _configService.GetValueAsync<string>(category, key);
        if (string.IsNullOrEmpty(encryptedValue))
            return null;

        return await _encryptionService.DecryptAsync(encryptedValue);
    }
}
```

### Access Control

```csharp
public class ConfigurationAuthorizationService
{
    public bool CanAccessSetting(string category, string key, string userRole)
    {
        var permissions = GetConfigurationPermissions();
        
        return permissions.TryGetValue(category, out var categoryPermissions) &&
               categoryPermissions.Contains(userRole);
    }

    private Dictionary<string, HashSet<string>> GetConfigurationPermissions()
    {
        return new Dictionary<string, HashSet<string>>
        {
            ["Security"] = new HashSet<string> { "Admin", "Manager" },
            ["Payment"] = new HashSet<string> { "Admin", "Manager" },
            ["Database"] = new HashSet<string> { "Admin" },
            ["UI"] = new HashSet<string> { "Admin", "Manager", "Server" },
            ["Business"] = new HashSet<string> { "Admin", "Manager" }
        };
    }
}
```

## Conclusion

The Magidesk POS configuration system provides:

- **Flexibility**: Support for multiple configuration sources and environments
- **Security**: Protection for sensitive configuration data
- **Validation**: Comprehensive validation of configuration values
- **Monitoring**: Health checks and change notifications
- **Backup/Restore**: Configuration backup and recovery capabilities
- **Access Control**: Role-based access to configuration settings

This comprehensive configuration management ensures the system can be easily adapted to different business requirements while maintaining security and reliability.

---

*This configuration documentation serves as the definitive reference for understanding and managing all aspects of Magidesk POS system configuration.*