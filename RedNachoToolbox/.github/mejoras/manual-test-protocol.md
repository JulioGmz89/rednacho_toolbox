# ?? Red Nacho Toolbox - Manual Testing Protocol

**Version**: 1.0  
**Date**: January 17, 2025  
**Branch**: `feature/mejoras-modernizacion`  
**Tester**: _____________  
**Platform**: _____________  

---

## ?? Test Environment

**Application Version**: .NET 9 MAUI  
**Test Date**: _____________  
**OS Version**: _____________  
**Device**: _____________  

---

## ?? Test Execution Instructions

1. **For each test case**, follow the steps exactly as written
2. **Record actual results** in the provided space
3. **Mark status** as PASS ? / FAIL ? / BLOCKED ??
4. **Add notes** for any observations or issues
5. **Take screenshots** for visual issues or successes

---

## ?? Test Summary

| Category | Total | Passed | Failed | Blocked |
|----------|-------|--------|--------|---------|
| Functional | 15 | ___ | ___ | ___ |
| UI/UX | 10 | ___ | ___ | ___ |
| Accessibility | 8 | ___ | ___ | ___ |
| Performance | 5 | ___ | ___ | ___ |
| **TOTAL** | **38** | **___** | **___** | **___** |

---

# ?? FUNCTIONAL TESTING

## TC-F-001: Application Startup

**Priority**: ????? (Critical)

**Preconditions**: App is not running

**Steps**:
1. Launch Red Nacho Toolbox application
2. Wait for app to fully load
3. Observe startup time and initial screen

**Expected Result**:
- App launches without errors
- Dashboard view loads
- All UI elements visible
- Startup time < 3 seconds
- No crash or freeze

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-002: Dashboard View Loading

**Priority**: ????? (Critical)

**Preconditions**: App is running

**Steps**:
1. Verify Dashboard is the default view
2. Check that tool cards are displayed
3. Verify Recently Used section (if applicable)
4. Check All Applications section

**Expected Result**:
- Dashboard loads correctly
- Tool cards displayed in 3-column grid
- Icons load properly
- Text is readable
- No layout issues

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-003: Navigation - Sidebar Buttons

**Priority**: ???? (High)

**Preconditions**: App is on Dashboard

**Steps**:
1. Click "Dashboard" button in sidebar
2. Click "Productivity" button in sidebar
3. Click "Settings" button in sidebar
4. Navigate back to Dashboard

**Expected Result**:
- Each button navigates to correct view
- Active indicator shows on current page
- Navigation is smooth (< 500ms)
- No errors or crashes

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-004: Sidebar Collapse/Expand

**Priority**: ??? (Medium)

**Preconditions**: App is running, sidebar expanded

**Steps**:
1. Go to Settings page
2. Toggle "Collapsed Sidebar" switch ON
3. Return to Dashboard
4. Verify sidebar is collapsed (shows only icons)
5. Go back to Settings
6. Toggle "Collapsed Sidebar" switch OFF
7. Return to Dashboard
8. Verify sidebar is expanded (shows icons + text)

**Expected Result**:
- Sidebar collapses to show only icons
- Sidebar expands to show icons + text
- Setting persists across navigation
- Active indicators visible in both modes
- No layout breaks

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-005: Theme Switching - Light Mode

**Priority**: ????? (Critical)

**Preconditions**: App is running, theme is not Light

**Steps**:
1. Navigate to Settings page
2. Open Theme picker
3. Select "Light"
4. Observe theme change across all UI elements

**Expected Result**:
- Theme switches to light mode immediately
- All colors update correctly:
  - Page background: Light gray (#F5F5F5)
  - Sidebar: White (#FFFFFF)
  - Text: Dark (#212121)
  - Cards: White with light shadows
- No visual glitches or flicker
- Theme persists after app restart

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-006: Theme Switching - Dark Mode

**Priority**: ????? (Critical)

**Preconditions**: App is running, theme is not Dark

**Steps**:
1. Navigate to Settings page
2. Open Theme picker
3. Select "Dark"
4. Observe theme change across all UI elements

**Expected Result**:
- Theme switches to dark mode immediately
- All colors update correctly:
  - Page background: Dark gray (#121212)
  - Sidebar: Darker gray (#1E1E1E)
  - Text: Light (#E0E0E0)
  - Cards: Dark with subtle shadows
- No visual glitches or flicker
- Theme persists after app restart

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-007: Theme Switching - System Mode

**Priority**: ???? (High)

**Preconditions**: App is running

**Steps**:
1. Navigate to Settings page
2. Select "System" theme
3. Observe app follows OS theme
4. Change OS theme (if possible)
5. Verify app theme updates automatically

**Expected Result**:
- App adopts system theme
- Theme updates when OS theme changes (if testable)
- Smooth transition
- No errors

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-008: Productivity View

**Priority**: ???? (High)

**Preconditions**: App is running

**Steps**:
1. Click "Productivity" in sidebar
2. Verify Productivity view loads
3. Check tool listing
4. Verify search functionality (if present)

**Expected Result**:
- Productivity view loads
- Only productivity tools shown
- Layout is correct
- Icons and text visible

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-009: Tool Selection - Markdown to PDF

**Priority**: ????? (Critical)

**Preconditions**: App is on Dashboard or Productivity view

**Steps**:
1. Click on "Markdown to PDF" tool card
2. Wait for tool view to load
3. Verify tool UI is displayed correctly

**Expected Result**:
- Tool view loads within 1 second
- Left panel shows style options
- Right panel shows preview/editor
- All controls are visible and functional

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-010: Color Picker - Visual Selection

**Priority**: ????? (Critical)

**Preconditions**: Markdown to PDF tool is open

**Steps**:
1. Locate "Text color" field
2. Click to expand color picker
3. Click on different areas of color canvas
4. Adjust hue slider
5. Observe color preview swatch updates

**Expected Result**:
- Color picker expands smoothly
- Canvas is interactive (click/drag)
- Selected color shows in preview swatch
- Hex value updates in real-time
- Color picker renders at 60 FPS (smooth)
- HSV color wheel displays correctly

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-011: Color Picker - Hex Input

**Priority**: ???? (High)

**Preconditions**: Color picker is expanded

**Steps**:
1. Click hex input field
2. Type "#FF0000" (red)
3. Press Enter or click outside
4. Observe color canvas and preview update
5. Try invalid hex "#GGGGGG"
6. Verify graceful handling

**Expected Result**:
- Valid hex updates color canvas position
- Preview swatch shows correct color
- Invalid hex either:
  - Reverts to previous color, OR
  - Shows validation message
- No crash or error

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-012: Color Picker - Preset Colors

**Priority**: ??? (Medium)

**Preconditions**: Color picker is expanded

**Steps**:
1. Click on Black preset
2. Verify color updates to black
3. Click on Red preset
4. Verify color updates to red
5. Test 2-3 more presets

**Expected Result**:
- Preset colors apply immediately
- Hex input updates
- Canvas position updates
- Preview shows correct color

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-013: Markdown Editor Toggle

**Priority**: ???? (High)

**Preconditions**: Markdown to PDF tool is open

**Steps**:
1. Click "Preview" tab (if not already selected)
2. Verify preview is shown
3. Click "Editor" tab
4. Verify editor is shown
5. Toggle back to Preview

**Expected Result**:
- Tabs switch smoothly
- Preview shows rendered markdown
- Editor shows markdown source
- No lag or stutter

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-014: Settings - Close Button

**Priority**: ??? (Medium)

**Preconditions**: Settings page is open

**Steps**:
1. Click X (close) button on Settings page
2. Verify navigation back to previous page

**Expected Result**:
- Closes Settings
- Returns to previous view (Dashboard)
- X button has hover/active states
- Smooth transition

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-F-015: Preferences Persistence

**Priority**: ???? (High)

**Preconditions**: App is running

**Steps**:
1. Change theme to Dark
2. Collapse sidebar
3. Close and restart app
4. Verify theme is still Dark
5. Verify sidebar is still collapsed

**Expected Result**:
- All preferences persist across restarts
- App loads with saved settings
- No reset to defaults

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

# ?? UI/UX TESTING

## TC-UI-001: Visual Hierarchy

**Priority**: ??? (Medium)

**Preconditions**: App is running on Dashboard

**Steps**:
1. Observe overall layout
2. Check visual hierarchy (headings, body text, buttons)
3. Verify important elements stand out
4. Check spacing and alignment

**Expected Result**:
- Clear visual hierarchy
- Headings are prominent
- Interactive elements are obvious
- Consistent spacing
- Professional appearance

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-UI-002: Button Hover States

**Priority**: ??? (Medium)

**Preconditions**: App is running

**Steps**:
1. Hover over Dashboard button
2. Hover over tool card
3. Hover over Settings button
4. Hover over Generate PDF button
5. Observe hover effects

**Expected Result**:
- Buttons show hover state (background change)
- Hover is subtle but noticeable
- Transition is smooth (<200ms)
- Cursor changes to pointer
- No lag

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-UI-003: Active Page Indicator

**Priority**: ??? (Medium)

**Preconditions**: App is running

**Steps**:
1. Go to Dashboard - observe indicator
2. Go to Productivity - observe indicator
3. Go to Settings - observe indicator
4. Verify indicator follows navigation

**Expected Result**:
- Active indicator shows on current page
- Capsule shape for expanded sidebar
- Dot for collapsed sidebar
- Red color (#CC333B)
- Always visible

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-UI-004: Card Shadows & Elevation

**Priority**: ?? (Low)

**Preconditions**: App is running, theme is Light

**Steps**:
1. Observe tool cards on Dashboard
2. Check shadow visibility
3. Look for depth/elevation effect
4. Compare to Design specs

**Expected Result**:
- Cards have subtle shadows
- Elevation is visible but not excessive
- Consistent across all cards
- Professional appearance

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-UI-005: Font Rendering

**Priority**: ??? (Medium)

**Preconditions**: App is running

**Steps**:
1. Check text across different views
2. Verify font family (Inter)
3. Check font weights (Regular, Semibold, Bold)
4. Verify text is crisp and readable

**Expected Result**:
- Inter font loads correctly
- Text is sharp and clear
- Font weights are distinct
- No font fallback issues
- Consistent across app

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-UI-006: Icon Display

**Priority**: ??? (Medium)

**Preconditions**: App is running

**Steps**:
1. Check Dashboard icons
2. Check Productivity icons
3. Check Markdown to PDF icon
4. Check Settings icon
5. Verify icons change with theme

**Expected Result**:
- All icons load correctly
- Icons are crisp (not pixelated)
- Theme-specific icons load (light/dark variants)
- Proper size and alignment
- No missing icons

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-UI-007: Color Contrast (Light Theme)

**Priority**: ???? (High)

**Preconditions**: App is in Light theme

**Steps**:
1. Check text on backgrounds
2. Verify primary text (#212121) on white
3. Check secondary text (#616161)
4. Check tertiary text (#757575)
5. Use contrast checker if available

**Expected Result**:
- Primary text: ?7:1 ratio (AAA)
- Secondary text: ?4.5:1 ratio (AA)
- Tertiary text: ?4.5:1 ratio (AA)
- All text is easily readable
- No eye strain

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-UI-008: Color Contrast (Dark Theme)

**Priority**: ???? (High)

**Preconditions**: App is in Dark theme

**Steps**:
1. Check text on backgrounds
2. Verify primary text (#E0E0E0) on dark
3. Check secondary text (#BDBDBD)
4. Check tertiary text (#9E9E9E)
5. Use contrast checker if available

**Expected Result**:
- Primary text: ?7:1 ratio (AAA)
- Secondary text: ?4.5:1 ratio (AA)
- Tertiary text: ?4.5:1 ratio (AA)
- All text is easily readable
- No eye strain

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-UI-009: Animation Smoothness

**Priority**: ??? (Medium)

**Preconditions**: App is running

**Steps**:
1. Switch between views (Dashboard ? Productivity)
2. Expand/collapse color picker
3. Switch themes
4. Toggle sidebar
5. Observe all animations

**Expected Result**:
- All animations are smooth (no jank)
- Duration feels appropriate (150-400ms)
- No stuttering or freezing
- Animations enhance UX
- 60 FPS target

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-UI-010: Responsive Layout

**Priority**: ??? (Medium)

**Preconditions**: App is running

**Steps**:
1. Resize app window (if applicable)
2. Test different window sizes
3. Verify layout adapts
4. Check no content is cut off

**Expected Result**:
- Layout adapts to window size
- No horizontal scrolling
- Content remains accessible
- Tool cards reflow appropriately
- No overlap or cutoff

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

# ? ACCESSIBILITY TESTING

## TC-A-001: Screen Reader - Navigation

**Priority**: ???? (High)

**Preconditions**: Screen reader enabled (Windows Narrator / VoiceOver / TalkBack)

**Steps**:
1. Start screen reader
2. Tab through sidebar buttons
3. Tab through tool cards
4. Tab through Settings controls
5. Listen to announcements

**Expected Result**:
- All interactive elements are reachable
- Elements announce their purpose
- Tab order is logical (top to bottom, left to right)
- Current focus is clear
- No tab traps

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-A-002: Screen Reader - Buttons

**Priority**: ???? (High)

**Preconditions**: Screen reader enabled

**Steps**:
1. Navigate to Dashboard button with Tab
2. Listen to screen reader announcement
3. Navigate to tool card
4. Navigate to Settings button
5. Verify all announce as buttons

**Expected Result**:
- Buttons announce as "Button"
- Button name/label is clear
- Action is described (e.g., "Dashboard button, activate to navigate")
- Consistent announcements

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-A-003: Screen Reader - Form Controls

**Priority**: ???? (High)

**Preconditions**: Screen reader enabled, Settings page open

**Steps**:
1. Tab to Theme picker
2. Tab to Sidebar toggle switch
3. Listen to announcements
4. Verify control types announced

**Expected Result**:
- Picker announces as "Combobox" or "Picker"
- Switch announces as "Switch" or "Toggle"
- Current value is announced
- Labels are clear

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-A-004: Keyboard Navigation - Full App

**Priority**: ????? (Critical)

**Preconditions**: App is running, mouse disabled (if possible)

**Steps**:
1. Use Tab to navigate entire app
2. Use Shift+Tab to navigate backwards
3. Use Enter/Space to activate buttons
4. Navigate through all views
5. Verify all functions accessible via keyboard

**Expected Result**:
- All interactive elements reachable via Tab
- Tab order is logical
- Enter/Space activates buttons
- Arrow keys work in pickers/lists
- No keyboard traps
- Focus indicator always visible

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-A-005: Focus Indicator Visibility

**Priority**: ???? (High)

**Preconditions**: App is running

**Steps**:
1. Tab through all interactive elements
2. Observe focus indicator
3. Test in both Light and Dark themes
4. Verify visibility on all backgrounds

**Expected Result**:
- Focus indicator is always visible
- High contrast border/outline
- Clear which element has focus
- Visible in both themes
- Not obscured by other elements

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-A-006: Semantic HTML/XAML

**Priority**: ??? (Medium)

**Preconditions**: Screen reader enabled

**Steps**:
1. Navigate with screen reader
2. Listen for heading announcements
3. Verify proper element types
4. Check semantic structure

**Expected Result**:
- Headings announced as "Heading level X"
- Buttons announced as "Button"
- Links announced as "Link" (if any)
- Proper semantic hierarchy

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-A-007: Alternative Text

**Priority**: ??? (Medium)

**Preconditions**: Screen reader enabled

**Steps**:
1. Navigate to tool cards
2. Listen for icon descriptions
3. Navigate to sidebar icons
4. Verify all images have alt text

**Expected Result**:
- All icons/images have alt text
- Alt text is descriptive
- Decorative images are marked
- No "image" without description

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-A-008: High Contrast Mode (Windows)

**Priority**: ?? (Low)

**Preconditions**: Windows High Contrast mode enabled

**Steps**:
1. Enable Windows High Contrast theme
2. Launch app
3. Verify app respects high contrast
4. Check text visibility
5. Check interactive elements

**Expected Result**:
- App respects high contrast colors
- All text is visible
- Interactive elements have borders
- No low-contrast combinations
- Usable in high contrast mode

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

# ? PERFORMANCE TESTING

## TC-P-001: Application Startup Time

**Priority**: ???? (High)

**Preconditions**: App is not running

**Steps**:
1. Close app completely
2. Start timer
3. Launch app
4. Stop timer when Dashboard is fully loaded and interactive

**Expected Result**:
- Startup time < 3 seconds
- No splash screen delays
- Smooth loading
- No janky UI rendering

**Actual Result**:
```
Startup Time: _______ seconds

_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-P-002: Theme Switch Performance

**Priority**: ??? (Medium)

**Preconditions**: App is running on Settings page

**Steps**:
1. Start timer
2. Switch theme (Light ? Dark or Dark ? Light)
3. Stop timer when theme fully applied
4. Repeat 3 times for average

**Expected Result**:
- Theme switch < 500ms
- Smooth transition
- No flicker or flash
- Immediate response

**Actual Result**:
```
Attempt 1: _______ ms
Attempt 2: _______ ms
Attempt 3: _______ ms
Average: _______ ms

_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-P-003: Color Picker Rendering

**Priority**: ???? (High)

**Preconditions**: Markdown to PDF tool open

**Steps**:
1. Expand color picker
2. Drag cursor across color canvas
3. Observe smoothness and responsiveness
4. Check for lag or stutter

**Expected Result**:
- Rendering at 60 FPS (smooth)
- No lag when dragging
- Immediate color updates
- Canvas remains responsive
- No performance degradation

**Actual Result**:
```
_________________________________________________________________________
_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-P-004: Navigation Speed

**Priority**: ??? (Medium)

**Preconditions**: App is running

**Steps**:
1. Navigate: Dashboard ? Productivity
2. Navigate: Productivity ? Settings
3. Navigate: Settings ? Dashboard
4. Measure time for each navigation

**Expected Result**:
- Each navigation < 300ms
- Smooth transitions
- No delay or freeze
- Responsive immediately

**Actual Result**:
```
Dashboard ? Productivity: _______ ms
Productivity ? Settings: _______ ms
Settings ? Dashboard: _______ ms

_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

## TC-P-005: Memory Usage

**Priority**: ?? (Low)

**Preconditions**: App is running

**Steps**:
1. Open Task Manager / Activity Monitor
2. Find Red Nacho Toolbox process
3. Note initial memory usage
4. Navigate through all views
5. Open/close color picker multiple times
6. Switch themes multiple times
7. Note final memory usage

**Expected Result**:
- Memory usage < 200 MB (reasonable for MAUI app)
- No significant memory leaks
- Memory returns close to initial after operations
- No continuous growth

**Actual Result**:
```
Initial Memory: _______ MB
After Operations: _______ MB
Memory Growth: _______ MB

_________________________________________________________________________
```

**Status**: ? PASS ? FAIL ? BLOCKED

**Notes**:
```
_________________________________________________________________________
```

---

# ?? TEST COMPLETION CHECKLIST

## Overall Testing

- [ ] All test cases executed
- [ ] All failures documented with screenshots
- [ ] All blocked tests have reasons noted
- [ ] Regression testing complete (no old features broken)
- [ ] Platform-specific testing done (Windows/Android/iOS)

## Documentation

- [ ] Test results recorded in test-results.md
- [ ] Screenshots attached for visual issues
- [ ] Performance metrics documented
- [ ] Accessibility issues logged

## Issues Found

```
List all issues discovered during testing:

1. ___________________________________________________________________
2. ___________________________________________________________________
3. ___________________________________________________________________
4. ___________________________________________________________________
5. ___________________________________________________________________
```

## Recommendations

```
Recommendations for improvements or next steps:

1. ___________________________________________________________________
2. ___________________________________________________________________
3. ___________________________________________________________________
```

---

## ?? Final Sign-Off

**Tester Name**: _______________________________  
**Date**: _______________________________  
**Overall Status**: ? PASS ? FAIL ? CONDITIONAL PASS

**Comments**:
```
_________________________________________________________________________
_________________________________________________________________________
_________________________________________________________________________
```

---

*Red Nacho Toolbox - Manual Testing Protocol v1.0*  
*Phase 4: Testing & Quality Assurance*  
*January 17, 2025*
