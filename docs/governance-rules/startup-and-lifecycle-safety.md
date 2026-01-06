# Startup and Lifecycle Safety Rule

**Category**: MANDATORY  
**Enforcement**: Architectural Pattern + Code Review  
**Violations**: BLOCKING

---

## RULE STATEMENT

**Partial startup is FORBIDDEN.**  
**Startup failures MUST block app.**  
**Background init MUST be guarded.**

---

## RULE 1: Partial Startup is FORBIDDEN

### Definition
**Partial startup** occurs when:
1. App initializes with missing/broken services
2. App continues despite critical initialization failure
3. App enters zombie state (UI visible but non-functional)

### Why Partial Startup is Dangerous
1. Operators see UI but features don't work
2. Silent failures cascade
3. Debugging is difficult
4. Data corruption risk

### Examples

#### ❌ VIOLATION: Partial Startup
```csharp
public App()
{
    _host = Host.CreateDefaultBuilder().Build();
    // NO SERVICE VALIDATION - App continues even if services broken
}
```

#### ✅ COMPLIANT: Fail-Fast Validation
```csharp
public App()
{
    _host = Host.CreateDefaultBuilder().Build();
    
    // VALIDATE CRITICAL SERVICES
    var requiredServices = new[] { 
        typeof(INavigationService), 
        typeof(IUserService),
        // ... all critical services
    };
    
    foreach (var serviceType in requiredServices) {
        var service = _host.Services.GetService(serviceType);
        if (service == null) {
            ShowFatalErrorAndExit($"Critical service {serviceType.Name} failed to resolve.");
            return; // BLOCKS APP START
        }
    }
}
```

---

## RULE 2: Startup Failures MUST Block App

### Requirement
**If startup fails, the app MUST NOT continue.**

### Enforcement Mechanisms

#### Mechanism 1: Service Validation + Exit
```csharp
if (service == null) {
    ShowFatalErrorAndExit("Service resolution failed.");
    return; // App terminates
}
```

#### Mechanism 2: OnLaunched Fail-Fast
```csharp
protected override async void OnLaunched(...)
{
    try {
        await InitializeSystemAsync();
    } catch (Exception ex) {
        ShowFatalDialog("Startup Failed", ex.Message);
        Environment.Exit(1); // TERMINATES APP
    }
}
```

#### Mechanism 3: API Database Validation
```csharp
for (int i = 0; i < 3; i++) {
    if (await dbContext.Database.CanConnectAsync()) break;
    if (i == 2) {
        logger.LogCritical("FATAL: Database unreachable");
        Environment.Exit(1); // BLOCKS API START
    }
}
```

### Anti-Pattern: Continue Despite Failure
```csharp
// ❌ VIOLATION
try {
    await InitializeSystemAsync();
} catch (Exception ex) {
    _logger.LogError(ex, "Initialization failed");
    // App continues - VIOLATION
}
```

---

## RULE 3: Background Init MUST Be Guarded

### Definition
**Background initialization** is any async operation during startup.

### Requirements
1. ✅ MUST have exception handling
2. ✅ MUST surface errors to operator
3. ✅ MUST fail-fast if critical
4. ✅ MUST NOT leave app in partial state

### Examples

#### ❌ VIOLATION: Unguarded Background Init
```csharp
protected override async void OnLaunched(...)
{
    // No try-catch - exceptions crash app silently
    await LoadConfigurationAsync();
    await ConnectToDatabaseAsync();
}
```

#### ✅ COMPLIANT: Guarded Background Init
```csharp
protected override async void OnLaunched(...)
{
    try {
        await LoadConfigurationAsync();
        await ConnectToDatabaseAsync();
    } catch (Exception ex) {
        ShowFatalDialog("Startup Failed", ex.Message);
        Environment.Exit(1); // FAIL-FAST
    }
}
```

#### ✅ COMPLIANT: Non-Critical Background Init
```csharp
protected override async void OnLaunched(...)
{
    try {
        await LoadConfigurationAsync(); // Critical
    } catch (Exception ex) {
        ShowFatalDialog("Startup Failed", ex.Message);
        Environment.Exit(1);
    }
    
    try {
        await LoadUserPreferencesAsync(); // Non-critical
    } catch (Exception ex) {
        _logger.LogWarning(ex, "Failed to load user preferences");
        MainWindow.ShowWarningBanner("Preferences Unavailable", 
            "Using default settings.");
        // App continues - acceptable for non-critical
    }
}
```

---

## STARTUP PHASES

### Phase 1: Constructor (Synchronous)
**MUST**:
- Install global exception handlers
- Build DI container
- Validate critical services
- Fail-fast on validation failure

**MUST NOT**:
- Perform async operations
- Access database
- Make network calls

```csharp
public App()
{
    this.InitializeComponent();
    
    // Install handlers
    this.UnhandledException += App_UnhandledException;
    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    
    // Build container
    _host = Host.CreateDefaultBuilder().Build();
    
    // Validate services
    ValidateCriticalServices(); // Synchronous, fail-fast
}
```

### Phase 2: OnLaunched (Asynchronous)
**MUST**:
- Wrap all async operations in try-catch
- Fail-fast on critical failures
- Show fatal dialog + exit on failure

**MUST NOT**:
- Continue despite critical failure
- Leave app in partial state

```csharp
protected override async void OnLaunched(...)
{
    try {
        await InitializeSystemAsync(); // Critical
        Navigate(typeof(LoginPage));
    } catch (Exception ex) {
        ShowFatalDialog("Startup Failed", ex.Message);
        Environment.Exit(1);
    }
}
```

### Phase 3: Background Initialization (Optional)
**MUST**:
- Have exception handling
- Surface errors appropriately
- Distinguish critical vs non-critical

**MUST NOT**:
- Block UI for non-critical operations
- Fail silently

```csharp
private async Task InitializeBackgroundServicesAsync()
{
    try {
        await _cacheService.WarmUpAsync(); // Non-critical
    } catch (Exception ex) {
        _logger.LogWarning(ex, "Cache warm-up failed");
        // Continue - non-critical
    }
}
```

---

## API STARTUP SAFETY

### Requirements
1. ✅ Validate database connection before accepting requests
2. ✅ Install global exception middleware
3. ✅ Fail-fast if critical services unavailable
4. ✅ Provide health check endpoint

### Example
```csharp
var app = builder.Build();

// Install middleware BEFORE MapControllers
app.UseExceptionHandler("/error");

// Validate database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    for (int i = 0; i < 3; i++) {
        if (await dbContext.Database.CanConnectAsync()) break;
        if (i == 2) {
            logger.LogCritical("FATAL: Database unreachable");
            Environment.Exit(1); // BLOCKS API START
        }
        await Task.Delay(1000);
    }
}

// Map controllers AFTER validation
app.MapControllers();
app.MapGet("/health", () => Results.Ok("Healthy"));

await app.RunAsync();
```

---

## ENFORCEMENT MECHANISMS

### 1. Architectural Pattern (Structural)
**ENFORCED**:
- ✅ Service validation in App constructor (TICKET-004)
- ✅ OnLaunched fail-fast (TICKET-002)
- ✅ API database validation (TICKET-005)

### 2. Code Review (Procedural)
**Checklist**:
- [ ] Are critical services validated?
- [ ] Does startup fail-fast on critical failure?
- [ ] Are background operations guarded?
- [ ] Is partial startup prevented?

### 3. Testing
**Scenarios**:
- Database unavailable → App exits with error
- Service resolution fails → App exits with error
- Configuration invalid → App exits with error

---

## VIOLATION SEVERITY

| Violation Type | Severity | Response |
|----------------|----------|----------|
| Partial startup allowed | CRITICAL | IMMEDIATE FIX |
| No fail-fast on critical failure | CRITICAL | IMMEDIATE FIX |
| Unguarded background init | HIGH | Same day fix |
| Missing service validation | HIGH | Same day fix |

---

## COMPLIANCE VERIFICATION

### Question 1: Are critical services validated?
- ✅ YES → COMPLIANT
- ❌ NO → VIOLATION

### Question 2: Does startup fail-fast on critical failure?
- ✅ YES → COMPLIANT
- ❌ NO → VIOLATION

### Question 3: Are background operations guarded?
- ✅ YES → COMPLIANT
- ❌ NO → VIOLATION

---

## SUMMARY

**GOLDEN RULE**: Partial startup is forbidden. Fail-fast on critical failures.

**ENFORCEMENT**: Architectural pattern + Code review + Testing

**VIOLATIONS**: BLOCKING (must be fixed before merge)

---

**Status**: MANDATORY  
**Enforcement**: ACTIVE  
**Violations**: FORBIDDEN
