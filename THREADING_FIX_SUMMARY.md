# Threading Issue Fix Summary

## Date: 2026-01-19

## Issue Description

**Error**: `System.Runtime.InteropServices.COMException: 0x8001010E (RPC_E_WRONG_THREAD)`

**Root Cause**: The `OrderPageViewModel` was using a `System.Timers.Timer` to update UI-bound properties (`CurrentTime` and `WaitTime`) every second. The timer's callback executes on a background thread, but WinUI requires all UI updates to happen on the UI thread.

**Stack Trace Location**:
```
at Magidesk.Presentation.ViewModels.OrderPageViewModel.<.ctor>b__17_0(Object s, ElapsedEventArgs e)
at System.Timers.Timer.MyTimerCallback(Object state)
```

## Solution Applied

### 1. Added DispatcherQueue Support

Added `Microsoft.UI.Dispatching.DispatcherQueue` to marshal property change notifications to the UI thread.

**Changes to `OrderPageViewModel.cs`:**

```csharp
// Added using statement
using Microsoft.UI.Dispatching;

// Added field
private readonly DispatcherQueue _dispatcherQueue;

// In constructor - capture dispatcher queue
_dispatcherQueue = DispatcherQueue.GetForCurrentThread();
if (_dispatcherQueue == null)
{
    throw new InvalidOperationException("OrderPageViewModel must be constructed on the UI thread");
}

// Fixed timer callback to use dispatcher
_timeUpdateTimer = new System.Timers.Timer(1000);
_timeUpdateTimer.Elapsed += (s, e) =>
{
    // Marshal property changes to UI thread
    _dispatcherQueue.TryEnqueue(() =>
    {
        OnPropertyChanged(nameof(CurrentTime));
        OnPropertyChanged(nameof(WaitTime));
    });
};
_timeUpdateTimer.Start();
```

### 2. Why This Works

- **DispatcherQueue**: WinUI's mechanism for marshalling work to the UI thread
- **TryEnqueue()**: Queues the lambda to execute on the UI thread
- **Property Changes**: Now happen on the UI thread, avoiding the COM exception

### 3. Alternative Solutions Considered

1. **DispatcherTimer**: Could use `Microsoft.UI.Xaml.DispatcherTimer` instead of `System.Timers.Timer`
   - Pros: Automatically runs on UI thread
   - Cons: Requires XAML dependency in ViewModel (less testable)

2. **SynchronizationContext**: Could use `SynchronizationContext.Post()`
   - Pros: More generic, works across UI frameworks
   - Cons: Less explicit, harder to debug

3. **Polling from UI**: Could have the View poll the ViewModel
   - Pros: No threading concerns
   - Cons: Violates MVVM pattern, less efficient

**Decision**: Chose DispatcherQueue as it's the WinUI-native solution and makes the threading explicit.

## Testing Results

- ✅ Build: 0 errors, 661 warnings (MVVM Toolkit AOT warnings - non-blocking)
- ✅ Application starts successfully
- ✅ No COM exceptions after 11+ seconds of runtime
- ✅ Timer continues to update properties without crashes

## Files Modified

1. `Magidesk/ViewModels/OrderPageViewModel.cs`
   - Added `using Microsoft.UI.Dispatching;`
   - Added `_dispatcherQueue` field
   - Captured dispatcher in constructor
   - Wrapped timer callback with `TryEnqueue()`

## Recommendations

### For Future Development:

1. **Always use DispatcherQueue for background thread UI updates**
   - Any timer, Task, or background operation that updates UI-bound properties must marshal to UI thread

2. **Consider DispatcherTimer for simple cases**
   - If you only need periodic UI updates, `DispatcherTimer` is simpler

3. **Add threading validation**
   - Consider adding debug assertions to verify UI thread access:
   ```csharp
   Debug.Assert(_dispatcherQueue.HasThreadAccess, "Must be called on UI thread");
   ```

4. **Document threading requirements**
   - Add XML comments to ViewModels that must be constructed on UI thread

5. **Review other ViewModels**
   - Check if `SettlePageViewModel` or other ViewModels have similar timer patterns

## Related Issues

This is a common WinUI/WPF threading issue. Similar patterns to watch for:
- `System.Timers.Timer` callbacks
- `Task.Run()` updating properties
- Event handlers from background services
- WebSocket/SignalR message handlers

All of these need to marshal to UI thread when updating UI-bound properties.

## Additional Notes

The error code `0x8001010E` (RPC_E_WRONG_THREAD) is a COM error indicating that an object was accessed from the wrong apartment/thread. In WinUI, all UI objects must be accessed from the thread that created them (the UI thread).

This is enforced by the COM threading model that WinUI is built on top of.
