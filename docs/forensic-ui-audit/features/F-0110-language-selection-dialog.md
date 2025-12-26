# Feature: Language Selection Dialog (F-0110)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Multi-lingual support for diverse staff. Allow users to select their preferred language.
- **Evidence**: `LanguageSelectionDialog.java` + i18n infrastructure - language selection; message bundles.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: Login screen; Back Office → Settings; user preferences
- **Exit paths**: Select / Cancel

## Preconditions & protections
- **User/role/permission checks**: None
- **State checks**: Languages must be configured
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User opens language selection
2. Dialog shows available languages:
   - English
   - Spanish
   - French
   - (others as configured)
3. Icon/flag for each language
4. User selects language
5. On select:
   - User preference saved
   - UI reloads in new language
   - All labels, messages translated
6. Persists for user session/profile

## Edge cases & failure paths
- **Missing translations**: Fall back to English
- **Partial translation**: Some strings may not translate
- **Language file missing**: Error, use default

## Data / audit / financial impact
- **Writes/updates**: User.preferredLanguage or session setting
- **Audit events**: Not typically logged
- **Financial risk**: None

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `LanguageSelectionDialog` → `ui/dialog/LanguageSelectionDialog.java`
- **Entry action(s)**: `SelectLanguageAction` → (path)
- **Workflow/service enforcement**: Messages class; ResourceBundle; i18n framework
- **Messages/labels**: Language names

## MagiDesk parity notes
- **What exists today**: English only (presumably)
- **What differs / missing**: Multi-language support

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Language preferences; localized strings
- **API/DTO requirements**: User preference for language
- **UI requirements**: LanguageSelectionDialog; resource dictionaries
- **Constraints for implementers**: Complete translation coverage desirable
