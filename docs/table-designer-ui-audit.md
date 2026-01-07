# Table Layout Designer - FORENSIC UI AUDIT
## Phase 1: Findings Report

**Audit Date**: 2026-01-06  
**Auditor**: System Architecture Review  
**Scope**: Complete UI/UX analysis of Table Designer  
**Objective**: Identify mental model failures, interaction issues, and UX risks

---

## EXECUTIVE SUMMARY

**VERDICT**: The current Table Layout Designer UI has **CRITICAL ARCHITECTURAL FLAWS** that violate domain boundaries, create ambiguous mental models, and expose operators to data loss risks.

**Severity**: HIGH  
**Production Risk**: MEDIUM-HIGH  
**Recommendation**: **FULL REDESIGN REQUIRED**

---

## A. MENTAL MODEL FAILURES

### 🔴 CRITICAL: Floor vs Layout Confusion

**Finding**: The UI **DOES NOT** clearly separate Floor from Layout.

**Evidence**:
1. **No Layout Selector** - Only a "Layout Name" text field exists
2. **No Layout List** - Operator cannot see available layouts
3. **Ambiguous Save Behavior** - Unclear if saving creates or updates
4. **Floor Auto-Creation** - Lines 152-164 in ViewModel auto-create "Main Floor" if none exist

```csharp
// VIOLATION: Silent entity creation
if (!floors.Any())
{
    var defaultFloor = Floor.Create("Main Floor", ...);
    await _floorRepository.AddAsync(defaultFloor);
}
```

**Impact**:
- Operator cannot answer: "Which layout am I editing?"
- Operator cannot answer: "Which floor is this layout for?"
- Layouts appear as "versions" rather than distinct configurations
- No way to switch between layouts without reloading

**Mental Model Violation**: 
- **Expected**: Floor → Layout → Tables (hierarchical)
- **Actual**: Layout Name + Tables (flat, ambiguous)

---

### 🔴 CRITICAL: Layout Lifecycle Ambiguity

**Finding**: The UI treats layouts as **implicit snapshots** rather than **explicit entities**.

**Evidence**:
1. **No "New Layout" button** - Operator must clear LayoutName manually
2. **No "Clone Layout" option** - Cannot duplicate existing layouts
3. **Ambiguous Save** - Line 395-420 checks `_currentLayoutId` to decide CREATE vs UPDATE

```csharp
if (_currentLayoutId.HasValue)
{
    // UPDATE existing layout
}
else
{
    // CREATE new layout
}
```

**Impact**:
- Operator cannot tell if they're creating or updating
- No visual indicator of "new" vs "editing existing"
- Accidental overwrites possible (operator thinks they're creating, but updating)
- No layout versioning or history

**Mental Model Violation**:
- **Expected**: Explicit "Create New", "Edit Existing", "Clone"
- **Actual**: Implicit state machine based on hidden `_currentLayoutId`

---

### 🟡 MEDIUM: Draft vs Published Confusion

**Finding**: `IsDraftMode` property exists but has **NO UI REPRESENTATION**.

**Evidence**:
- Line 53: `private bool _isDraftMode = true;` (defaults to draft)
- Line 402, 415: Used in save commands
- **XAML**: No toggle, checkbox, or indicator for draft status

**Impact**:
- Operator cannot tell if layout is draft or published
- No way to promote draft to published
- Accidental publication of incomplete layouts

---

### 🟡 MEDIUM: Floor Selection Side Effects

**Finding**: Changing floor **silently discards unsaved layout changes**.

**Evidence**:
- Lines 676-682: `OnSelectedFloorChanged` calls `LoadTablesAsync()`
- `LoadTablesAsync()` clears `Tables` collection
- No dirty-state check or confirmation dialog

```csharp
partial void OnSelectedFloorChanged(FloorDto? value)
{
    if (value != null && !IsBusy)
    {
        _ = LoadTablesAsync(); // SILENT DATA LOSS
    }
}
```

**Impact**:
- Operator loses work by accidentally changing floor dropdown
- No "unsaved changes" warning
- No undo capability

---

## B. INTERACTION MODEL AUDIT

### 🔴 CRITICAL: Drag & Drop Reliability

**Finding**: Drag & drop has **DUAL IMPLEMENTATIONS** that conflict.

**Evidence**:
1. **DragStarting/Drop** (Lines 58-90 in XAML.cs)
2. **ManipulationStarted/Delta/Completed** (Lines 100-125 in XAML.cs)

**Both are active simultaneously!**

```csharp
// XAML binds BOTH:
DragStarting="Table_DragStarting"
ManipulationStarted="Table_ManipulationStarted"
```

**Impact**:
- Unpredictable behavior (which handler wins?)
- Position updates may conflict
- Potential for lost position data

**Interaction Violation**: One table, two drag systems.

---

### 🔴 CRITICAL: No Undo/Redo

**Finding**: **ZERO undo/redo capability**.

**Evidence**:
- No command history tracking
- No undo stack
- Drag operations commit immediately
- Delete operations are permanent (after confirmation)

**Impact**:
- Operator cannot recover from mistakes
- No experimentation safety
- Professional UX standard violated

---

### 🟡 MEDIUM: Resize Affordances Missing

**Finding**: Tables have **NO RESIZE HANDLES**.

**Evidence**:
- XAML shows fixed 100x100 Grid (Line 131 in XAML)
- No resize UI elements
- Width/Height properties exist in DTO but unused in UI

**Impact**:
- Cannot create different table sizes visually
- Must edit properties manually (no properties panel exists)

---

### 🟡 MEDIUM: Multi-Select Not Supported

**Finding**: **NO MULTI-SELECT** capability.

**Evidence**:
- `SelectedTable` is singular (Line 56 in ViewModel)
- No Ctrl+Click or Shift+Click handling
- No group move operations

**Impact**:
- Cannot move multiple tables at once
- Tedious for large layouts
- No bulk operations (delete, align, distribute)

---

### 🟡 MEDIUM: No Snap-to-Grid

**Finding**: **NO SNAP-TO-GRID** functionality.

**Evidence**:
- Free-form positioning only
- No grid overlay
- No alignment guides
- Manual pixel-perfect alignment required

**Impact**:
- Layouts look unprofessional
- Difficult to align tables
- No consistency enforcement

---

### 🟡 MEDIUM: No Zoom/Pan

**Finding**: **NO ZOOM OR PAN** controls.

**Evidence**:
- Fixed canvas size (2000x2000)
- ScrollViewer exists (Line 103 in XAML) but no zoom
- No minimap or overview

**Impact**:
- Large layouts difficult to navigate
- Cannot see full layout at once
- Cannot zoom in for precision

---

### 🟡 MEDIUM: Read-Only vs Edit Mode Weak

**Finding**: `IsDesignMode` toggle exists but **WEAK ENFORCEMENT**.

**Evidence**:
- Line 29-31 in XAML: Toggle button exists
- BUT: No visual distinction between modes
- No lock icons or disabled states
- Operator can still click tables in read-only mode

**Impact**:
- Unclear which mode is active
- Accidental edits possible
- No visual feedback

---

### 🟡 MEDIUM: Selection Clarity Poor

**Finding**: Selected table has **WEAK VISUAL INDICATOR**.

**Evidence**:
- Line 163-166 in XAML: Blue border on selection
- BUT: Converter `TableToSelectionVisibilityConverter` not found in codebase
- Likely broken or missing

**Impact**:
- Cannot tell which table is selected
- Confusion during editing

---

## C. UX RISK ASSESSMENT

### 🔴 BLOCKER: Silent Data Loss on Floor Change

**Risk**: Changing floor dropdown **DESTROYS UNSAVED LAYOUT** without warning.

**Code Path**:
```
User changes Floor dropdown
  → OnSelectedFloorChanged fires
    → LoadTablesAsync() called
      → Tables.Clear() executed
        → UNSAVED WORK LOST
```

**Probability**: HIGH (common user action)  
**Impact**: CRITICAL (data loss)  
**Mitigation**: NONE (no dirty check, no confirmation)

---

### 🔴 BLOCKER: Ambiguous Save Creates Duplicates

**Risk**: Operator thinks they're creating new layout, but **UPDATES EXISTING**.

**Scenario**:
1. Load "Lunch Setup" layout
2. Make changes
3. Change LayoutName to "Dinner Setup"
4. Click Save
5. **OVERWRITES "Lunch Setup"** instead of creating "Dinner Setup"

**Reason**: `_currentLayoutId` still set from step 1.

**Code**: Lines 395-406 in ViewModel

**Probability**: MEDIUM-HIGH  
**Impact**: CRITICAL (data corruption)  
**Mitigation**: WEAK (name uniqueness check doesn't prevent overwrite)

---

### 🔴 BLOCKER: No Dirty State Indicator

**Risk**: Operator closes page or navigates away, **LOSING UNSAVED CHANGES**.

**Evidence**:
- `IsDirty` property exists (Line 50) but **NEVER SET TO TRUE**
- No visual indicator (no asterisk, no banner)
- No navigation guard

**Probability**: HIGH  
**Impact**: HIGH (data loss)  
**Mitigation**: NONE

---

### 🟡 HIGH: Floor Auto-Creation Violates Governance

**Risk**: System **SILENTLY CREATES "Main Floor"** if none exist.

**Code**: Lines 152-164 in ViewModel

**Violation**: 
- **no-silent-failure.md**: Silent entity creation forbidden
- **audit-convergence.md**: Implicit actions are violations

**Impact**:
- Operator unaware floor was created
- Unexpected database state
- Violates explicit action principle

---

### 🟡 HIGH: Overlap Detection Fails Silently

**Risk**: Overlap check (Lines 304-313) shows error but **ALLOWS INVALID STATE**.

**Code**:
```csharp
if (overlappingTable != null)
{
    await ShowErrorAsync(...);
    return false; // BUT TABLE POSITION ALREADY UPDATED IN UI
}
```

**Impact**:
- UI shows overlapping tables
- Database may reject on save
- Inconsistent state

---

### 🟡 HIGH: Delete Without Undo

**Risk**: Table deletion is **PERMANENT** after confirmation.

**Evidence**:
- Line 266: `Tables.Remove(table);`
- Line 269: Repository delete commented out (not implemented)
- No undo capability

**Impact**:
- Accidental deletions unrecoverable
- Operator must recreate from memory

---

### 🟡 MEDIUM: Boundary Validation Weak

**Risk**: Tables can be placed **PARTIALLY OFF-CANVAS**.

**Code**: Lines 295-301 allow tables at `maxX - 100`, `maxY - 100`

**Impact**:
- Tables cut off visually
- Confusing for operators

---

### 🟡 MEDIUM: No Confirmation on Destructive Reload

**Risk**: "Reload" button (Line 42 in XAML) **DISCARDS ALL CHANGES** without warning.

**Evidence**:
- `LoadLayoutCommand` calls `LoadLayoutAsync()`
- No dirty check
- No confirmation dialog

**Impact**:
- Accidental data loss
- Operator frustration

---

## D. ADDITIONAL FINDINGS

### Missing Features (Expected in Professional Designer)

1. **No Properties Panel** - Cannot edit table properties (capacity, number, shape)
2. **No Keyboard Shortcuts** - No arrow key nudging, no Ctrl+Z, no Delete key
3. **No Alignment Tools** - No "align left", "distribute evenly", "center"
4. **No Copy/Paste** - Cannot duplicate tables
5. **No Export/Import** - Cannot share layouts between systems
6. **No Preview Mode** - Cannot see layout as operator would see it
7. **No Search/Filter** - Cannot find table by number
8. **No Validation Feedback** - No real-time validation (e.g., duplicate numbers)

### Code Quality Issues

1. **Commented Code** - Line 269: `// await _tableRepository.DeleteAsync(table.Id);`
2. **Test Methods in Production** - Lines 486-645: Test methods in ViewModel
3. **Magic Numbers** - Hardcoded 100x100 table size, 2000x2000 canvas
4. **Inconsistent Naming** - `_currentLayoutId` vs `LayoutName` (ID vs Name confusion)

---

## SUMMARY OF CRITICAL FLAWS

| # | Finding | Severity | Risk |
|---|---------|----------|------|
| 1 | Floor vs Layout confusion | CRITICAL | Data loss |
| 2 | Layout lifecycle ambiguity | CRITICAL | Data corruption |
| 3 | Silent data loss on floor change | BLOCKER | Data loss |
| 4 | Ambiguous save behavior | BLOCKER | Data corruption |
| 5 | No dirty state indicator | BLOCKER | Data loss |
| 6 | Dual drag implementations | CRITICAL | Unpredictable behavior |
| 7 | No undo/redo | CRITICAL | Poor UX |
| 8 | Floor auto-creation | HIGH | Governance violation |

---

## OPERATOR IMPACT ASSESSMENT

**Can operator answer these questions?**

| Question | Current UI | Expected |
|----------|-----------|----------|
| Which floor am I editing? | ❌ Unclear | ✅ Explicit selector |
| Which layout is active? | ❌ No indicator | ✅ Layout list with active marker |
| Am I creating or updating? | ❌ Hidden state | ✅ Explicit mode |
| Are my changes saved? | ❌ No indicator | ✅ Dirty state banner |
| Can I undo my last action? | ❌ No | ✅ Undo/redo |
| Can I switch layouts without losing work? | ❌ No | ✅ Dirty check + save prompt |

**Verdict**: Current UI **FAILS** all critical operator questions.

---

## ARCHITECTURAL VIOLATIONS

### Domain Model Violations
1. **Floor treated as implicit** - Auto-created, not explicitly managed
2. **Layout treated as snapshot** - Not first-class entity
3. **Tables loaded globally** - Not scoped to layout

### UX Principles Violated
1. **No explicit actions** - Silent saves, implicit creates
2. **No safety nets** - No undo, no dirty checks
3. **No feedback** - Weak selection, no validation
4. **No discoverability** - Hidden features, unclear state

### Governance Violations
1. **Silent entity creation** - Violates `no-silent-failure.md`
2. **Implicit state changes** - Violates `audit-convergence.md`
3. **No error surfacing** - Weak error handling

---

## CONCLUSION

The current Table Layout Designer UI is **ARCHITECTURALLY UNSOUND** and **OPERATIONALLY UNSAFE**.

**Key Problems**:
1. Mental model does not match domain model
2. Interaction patterns are incomplete and conflicting
3. UX risks include data loss, corruption, and confusion
4. Missing professional-grade features

**Recommendation**: **FULL UI REDESIGN REQUIRED**

The UI must be rebuilt from first principles to:
- Clearly separate Floor from Layout
- Make layout lifecycle explicit
- Provide safety nets (undo, dirty checks, confirmations)
- Implement professional designer features
- Eliminate silent failures and ambiguous states

---

**Next Phase**: Target UI Architecture Design

**Status**: AUDIT COMPLETE ✅  
**Findings**: 8 CRITICAL, 12 HIGH/MEDIUM  
**Verdict**: REDESIGN REQUIRED
