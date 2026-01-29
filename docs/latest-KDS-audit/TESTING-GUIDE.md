# KDS Real-Time Notifications - Testing Guide

**Quick Reference for Manual Integration Testing**

---

## Prerequisites

- [ ] Code changes deployed/built
- [ ] Database accessible
- [ ] Network connectivity between components
- [ ] 3 terminal windows available

---

## Quick Start

### Step 1: Launch Applications

**Terminal 1 - API Server (SignalR Hub)**:
```bash
cd Magidesk/src/Magidesk.Api
dotnet run
```
Wait for: `Now listening on: http://localhost:5000`

**Terminal 2 - POS Application**:
```bash
cd Magidesk/src/Magidesk.Presentation
dotnet run
```
Wait for: Application window opens

**Terminal 3 - KDS Application**:
```bash
cd Magidesk/src/Magidesk.Presentation
dotnet run --launch-profile KDS
```
Wait for: KDS window opens

---

## Test Scenarios

### ✅ Test 1: Basic Real-Time Notification (2 minutes)

**Goal**: Verify orders appear on KDS immediately

**Steps**:
1. In POS: Click "New Ticket"
2. In POS: Add a food item (e.g., "Cheeseburger")
3. In POS: Set table number (e.g., "Table 5")
4. In POS: Click "Send to Kitchen"
5. **START TIMER** ⏱️
6. Watch KDS screen
7. **STOP TIMER** when order appears ⏱️

**Expected Results**:
- ✅ Order appears on KDS within 2 seconds
- ✅ Order shows correct table number ("Table 5")
- ✅ Order shows correct item ("Cheeseburger")
- ✅ No manual refresh required

**If Failed**:
- Check API server logs for errors
- Check POS logs for notification calls
- Check KDS logs for SignalR connection status
- Verify network connectivity

---

### ✅ Test 2: Multiple Orders (3 minutes)

**Goal**: Verify system handles multiple orders quickly

**Steps**:
1. Create Ticket 1: Table 1, add "Pizza"
2. Create Ticket 2: Table 2, add "Burger"
3. Create Ticket 3: Table 3, add "Salad"
4. Send all 3 to kitchen within 5 seconds
5. Watch KDS screen

**Expected Results**:
- ✅ All 3 orders appear on KDS
- ✅ Each appears within 2 seconds of sending
- ✅ Orders appear in chronological order
- ✅ No orders are missed

---

### ✅ Test 3: Status Change Regression (2 minutes)

**Goal**: Verify existing functionality still works

**Steps**:
1. Send an order to kitchen (from Test 1 or 2)
2. On KDS: Click "Bump" to change status
3. Watch for status change

**Expected Results**:
- ✅ Status changes immediately
- ✅ No errors or crashes
- ✅ Existing functionality unaffected

---

### ✅ Test 4: SignalR Failure Resilience (5 minutes)

**Goal**: Verify system handles network failures gracefully

**Steps**:
1. **Stop API server** (Ctrl+C in Terminal 1)
2. In POS: Send an order to kitchen
3. Check POS logs for errors
4. Check database for order (should be saved)
5. **Restart API server** (Terminal 1: `dotnet run`)
6. Wait 60 seconds (polling interval)
7. Check KDS - order should appear via polling
8. Send another order
9. Verify real-time notification resumes

**Expected Results**:
- ✅ Order persists to database even when SignalR is down
- ✅ POS doesn't crash
- ✅ Error is logged (not thrown)
- ✅ KDS falls back to polling
- ✅ Real-time resumes after reconnection

---

## Performance Measurement

### Latency Test (10 minutes)

**Goal**: Measure average notification latency

**Steps**:
1. Prepare 10 tickets with food items
2. For each ticket:
   - Click "Send to Kitchen"
   - Start timer ⏱️
   - Stop timer when order appears on KDS ⏱️
   - Record time
3. Calculate:
   - Average latency
   - Maximum latency
   - 99th percentile

**Expected Results**:
- ✅ Average latency < 2 seconds
- ✅ Maximum latency < 2 seconds (excluding network failures)

**Sample Recording Sheet**:
```
Order 1: ___ seconds
Order 2: ___ seconds
Order 3: ___ seconds
Order 4: ___ seconds
Order 5: ___ seconds
Order 6: ___ seconds
Order 7: ___ seconds
Order 8: ___ seconds
Order 9: ___ seconds
Order 10: ___ seconds

Average: ___ seconds
Maximum: ___ seconds
```

---

## Log Verification

### What to Look For

**✅ Successful Notification** (POS/API logs):
```
[Information] New order notification: Kitchen Order {guid}, Table {number}
[Information] Successfully notified KDS about kitchen order {guid}
```

**✅ KDS Connection** (KDS logs):
```
[Debug] KDS Connected to SignalR Hub. Stopping Polling.
```

**⚠️ Notification Failure** (when API is down):
```
[Error] Failed to notify KDS about kitchen order {guid}. Order was still saved to database.
```

**❌ Red Flags** (should NOT see):
```
[Error] Unhandled exception
[Error] NullReferenceException
[Error] Order failed to persist
```

---

## Troubleshooting

### Issue: Orders Not Appearing on KDS

**Check**:
1. Is API server running? (Terminal 1)
2. Is KDS connected to SignalR? (Check KDS logs)
3. Are orders being sent? (Check POS logs)
4. Is database accessible? (Check connection string)

**Quick Fix**:
- Restart API server
- Restart KDS application
- Check network/firewall settings

---

### Issue: Slow Performance (> 2 seconds)

**Check**:
1. Network latency between POS and API
2. Database query performance
3. CPU/memory usage on server
4. Number of concurrent connections

**Quick Fix**:
- Check network connectivity
- Restart applications
- Monitor system resources

---

### Issue: Application Crashes

**Check**:
1. Exception logs in POS/KDS/API
2. Database connection errors
3. SignalR connection errors

**Quick Fix**:
- Review error logs
- Check database connectivity
- Verify configuration settings

---

## Success Checklist

After completing all tests:

- [ ] Test 1: Basic notification works (< 2 seconds)
- [ ] Test 2: Multiple orders handled correctly
- [ ] Test 3: Status changes still work (no regression)
- [ ] Test 4: System resilient to SignalR failures
- [ ] Performance: Average latency < 2 seconds
- [ ] Logs: No unexpected errors
- [ ] Logs: Successful notifications logged
- [ ] Logs: Failures handled gracefully

---

## Reporting Results

### If All Tests Pass ✅

Report back with:
- ✅ All tests passed
- Average latency: ___ seconds
- Any observations or notes

**Next Step**: Update documentation and mark as production-ready

---

### If Tests Fail ❌

Report back with:
- ❌ Which test(s) failed
- Error messages from logs
- Steps to reproduce
- Screenshots (if applicable)

**Next Step**: Debug and fix issues

---

## Quick Commands Reference

### Start API Server
```bash
cd Magidesk/src/Magidesk.Api
dotnet run
```

### Start POS
```bash
cd Magidesk/src/Magidesk.Presentation
dotnet run
```

### Start KDS
```bash
cd Magidesk/src/Magidesk.Presentation
dotnet run --launch-profile KDS
```

### View Logs (PowerShell)
```powershell
# View last 50 lines of logs
Get-Content -Path "logs/app.log" -Tail 50 -Wait
```

### Stop All (Ctrl+C in each terminal)

---

**Estimated Testing Time**: 15-20 minutes  
**Required**: 3 terminal windows, database access  
**Next**: Report results for documentation update

