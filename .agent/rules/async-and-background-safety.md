# Async and Background Safety Rule

**Category**: MANDATORY  
**Enforcement**: Code Review + Static Analysis  
**Violations**: BLOCKING

---

## RULE STATEMENT

**Async void is BANNED (except UI event handlers).**  
**Fire-and-forget is BANNED without SafeRun wrapper.**  
**All background tasks MUST be observed.**

---

## RULE 1: Async Void is BANNED

### Why Async Void is Dangerous
1. Exceptions cannot be caught by caller
2. No way to await completion
3. No way to track task status
4. Unhandled exceptions crash app

### Examples

#### ❌ VIOLATION: Async Void in ViewModel
```csharp
public async void LoadDataAsync()
{
    // Exceptions cannot be caught - VIOLATION
    var data = await _repository.GetDataAsync();
}
```

#### ✅ COMPLIANT: AsyncRelayCommand
```csharp
public ICommand LoadDataCommand { get; }

public ViewModel()
{
    LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
}

private async Task LoadDataAsync()
{
    // Exceptions can be caught - COMPLIANT
    var data = await _repository.GetDataAsync();
}
```

### Exception: UI Event Handlers
**Async void is ALLOWED in UI event handlers (framework requirement).**

#### ✅ COMPLIANT: UI Event Handler
```csharp
protected override async void OnNavigatedTo(NavigationEventArgs e)
{
    // Framework requirement - COMPLIANT
    await LoadDataAsync();
}

private async void Button_Click(object sender, RoutedEventArgs e)
{
    // Framework requirement - COMPLIANT
    await ProcessClickAsync();
}
```

**Requirement**: Global exception handlers MUST catch exceptions from UI event handlers.

---

## RULE 2: Fire-and-Forget is BANNED

### Why Fire-and-Forget is Dangerous
1. Unobserved exceptions
2. No way to track completion
3. No way to cancel
4. Resource leaks

### Examples

#### ❌ VIOLATION: Fire-and-Forget
```csharp
Task.Run(() => BackgroundWork()); // No await, no observation - VIOLATION
```

#### ❌ VIOLATION: Discarded Task
```csharp
_ = DoWorkAsync(); // Intentionally discarded - VIOLATION
```

#### ✅ COMPLIANT: Awaited Task
```csharp
await Task.Run(() => BackgroundWork()); // Awaited - COMPLIANT
```

#### ✅ COMPLIANT: Tracked Background Task
```csharp
var task = Task.Run(() => BackgroundWork());
_backgroundTasks.Add(task); // Tracked - COMPLIANT

// Later: await all background tasks
await Task.WhenAll(_backgroundTasks);
```

#### ✅ COMPLIANT: SafeRun Wrapper (Future)
```csharp
SafeRun.FireAndForget(async () => {
    await BackgroundWork();
}, ex => {
    _logger.LogError(ex, "Background work failed");
    MainWindow.ShowErrorBanner(ex);
});
```

---

## RULE 3: All Background Tasks MUST Be Observed

### What is an Observed Task?
A task is observed if:
1. It is awaited, OR
2. It is tracked in a collection, OR
3. It has a continuation that handles exceptions

### Examples

#### ❌ VIOLATION: Unobserved Task
```csharp
public void StartBackgroundWork()
{
    Task.Run(() => DoWork()); // Unobserved - VIOLATION
}
```

#### ✅ COMPLIANT: Awaited Task
```csharp
public async Task StartBackgroundWorkAsync()
{
    await Task.Run(() => DoWork()); // Observed - COMPLIANT
}
```

#### ✅ COMPLIANT: Tracked Task
```csharp
private List<Task> _backgroundTasks = new();

public void StartBackgroundWork()
{
    var task = Task.Run(() => DoWork());
    _backgroundTasks.Add(task); // Observed - COMPLIANT
}

public async Task ShutdownAsync()
{
    await Task.WhenAll(_backgroundTasks); // All tasks observed
}
```

#### ✅ COMPLIANT: Continuation
```csharp
Task.Run(() => DoWork()).ContinueWith(task => {
    if (task.IsFaulted) {
        _logger.LogError(task.Exception, "Background work failed");
        MainWindow.ShowErrorBanner(task.Exception);
    }
}, TaskScheduler.FromCurrentSynchronizationContext());
```

---

## ENFORCEMENT MECHANISMS

### 1. Code Review
**EVERY pull request MUST be reviewed for async safety.**

**Checklist**:
- [ ] No async void (except UI event handlers)?
- [ ] No fire-and-forget tasks?
- [ ] All background tasks observed?
- [ ] Exception handling in place?

### 2. Static Analysis
**Automated detection of async violations.**

**Rules**:
- Detect async void methods (except UI event handlers)
- Detect Task.Run without await
- Detect discarded tasks (`_ = ...`)
- Detect unobserved task exceptions

### 3. Runtime Verification
**Global exception handlers catch unobserved task exceptions.**

**Installed**:
- ✅ TaskScheduler.UnobservedTaskException

---

## PATTERNS

### Pattern 1: AsyncRelayCommand (ViewModels)
**USE**: AsyncRelayCommand for all async ViewModel operations

```csharp
public ICommand SaveCommand { get; }

public ViewModel()
{
    SaveCommand = new AsyncRelayCommand(SaveAsync);
}

private async Task SaveAsync()
{
    try {
        await _repository.SaveAsync();
    } catch (Exception ex) {
        await ShowErrorAsync("Save Failed", ex.Message);
    }
}
```

### Pattern 2: Awaited Background Work
**USE**: Await all background tasks

```csharp
private async Task LoadDataAsync()
{
    var task1 = _repository.GetDataAsync();
    var task2 = _service.GetConfigAsync();
    
    await Task.WhenAll(task1, task2); // All tasks awaited
}
```

### Pattern 3: Tracked Background Tasks
**USE**: Track long-running background tasks

```csharp
private List<Task> _backgroundTasks = new();

public void StartMonitoring()
{
    var task = Task.Run(async () => {
        while (!_cancellationToken.IsCancellationRequested) {
            await MonitorAsync();
            await Task.Delay(1000);
        }
    });
    
    _backgroundTasks.Add(task);
}

public async Task ShutdownAsync()
{
    _cancellationTokenSource.Cancel();
    await Task.WhenAll(_backgroundTasks);
}
```

---

## VIOLATION SEVERITY

| Violation Type | Severity | Response |
|----------------|----------|----------|
| Async void in ViewModel | HIGH | Same day fix |
| Fire-and-forget in critical path | HIGH | Same day fix |
| Unobserved background task | MEDIUM | Next sprint |
| Missing exception handling | HIGH | Same day fix |

---

## COMPLIANCE VERIFICATION

### Question 1: Is this async void?
- ✅ NO → COMPLIANT
- ⚠️ YES, UI event handler → COMPLIANT (exception)
- ❌ YES, ViewModel method → VIOLATION

### Question 2: Is this task awaited or tracked?
- ✅ YES → COMPLIANT
- ❌ NO → VIOLATION

### Question 3: Are exceptions handled?
- ✅ YES → COMPLIANT
- ❌ NO → VIOLATION

---

## SUMMARY

**GOLDEN RULE**: Async void is banned. Fire-and-forget is banned. All tasks must be observed.

**ENFORCEMENT**: Code review + Static analysis + Runtime verification

**VIOLATIONS**: BLOCKING (must be fixed before merge)

---

**Status**: MANDATORY  
**Enforcement**: ACTIVE  
**Violations**: FORBIDDEN
