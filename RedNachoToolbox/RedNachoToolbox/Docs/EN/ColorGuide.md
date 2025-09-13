# Color and Theme Guide — Red Nacho ToolBox

This guide explains how the color/theme system in the app works, what each key (token) means, and how to change them safely.

It relies on two sources:
- XAML defaults for the light theme: `Resources/Styles/Colors.xaml`.
- Runtime dynamic color application for Light/Dark: `SettingsPage.xaml.cs` (`ApplyThemeColors()` and `ApplyThemeColorsStatic()`).

Important: In .NET MAUI you cannot change `ResourceDictionary.Source` at runtime from C#. For this reason, the app applies colors directly on `Application.Current.Resources` (see `SettingsPage.xaml.cs`).

---

## Where colors are defined

1) Defaults (light theme by default)
- File: `RedNachoToolbox/RedNachoToolbox/Resources/Styles/Colors.xaml`
- Contains the base list of `Color x:Key="..."` used by `DynamicResource` in XAML.
- These values are overridden dynamically when changing theme.

2) Runtime theme application (Light/Dark)
- File: `RedNachoToolbox/RedNachoToolbox/SettingsPage.xaml.cs`
- Methods:
  - `ApplyThemeColors(bool isDarkTheme)`: Applies colors when the user switches theme at runtime.
  - `ApplyThemeColorsStatic(bool isDarkTheme, ResourceDictionary resources)`: Applies colors at startup.
  - `PropagateThemeKeys(...)`: Ensures important tokens are synchronized across resource dictionaries.

3) Usage in XAML
- Consumed with `DynamicResource` to be theme‑aware:
```xml
<TextColor="{DynamicResource TextColor}" />
<BackgroundColor="{DynamicResource CardBackgroundColor}" />
<CancelButtonColor="{DynamicResource InteractivePrimaryColor}" />
```

---

## How to change colors

### A) Change for light theme (default)
- Edit `Resources/Styles/Colors.xaml` directly.
- Example: change page background (light)
```xml
<Color x:Key="PageBackgroundColor">#F5F5F5</Color>
```

### B) Change for dark theme
- Update the `isDarkTheme == true` branch in `ApplyThemeColors()` and `ApplyThemeColorsStatic()` within `SettingsPage.xaml.cs`.
- Find the key to modify and update its value for dark.

### C) Change for both themes
- Update the value in `Colors.xaml` (for light defaults) and in both branches of `ApplyThemeColors(...)` and `ApplyThemeColorsStatic(...)`.

### D) Add a new token (color key)
1. Add the key in `Resources/Styles/Colors.xaml` with a default (light) value.
2. In `SettingsPage.xaml.cs`, add assignments for both Light and Dark branches in both methods (`ApplyThemeColors` and `ApplyThemeColorsStatic`).
3. If you want to immediately propagate the resource to all dictionaries, add the key name in the calls to `PropagateThemeKeys(...)`.
4. Use it in XAML with `DynamicResource`.

### E) Best practices
- Avoid hardcoded hex values in XAML. Always use `DynamicResource` (e.g., `{DynamicResource PrimaryRed}`).
- Keep the same keys in Light/Dark so the UI reacts consistently.
- For focus/hover changes, use interactive tokens, not arbitrary colors.

---

## Meaning of each value (tokens)

### 1) Brand
- `PrimaryRed`: Brand red (indicator dots/capsules, occasional accents).
- `BrandPrimaryColor`: Semantic alias for brand color.

### 2) Surfaces (backgrounds and containers)
- `PageBackgroundColor`: Background of the main area (outside cards/sidebars).
- `SidebarBackgroundColor`: Sidebar background.
- `CardBackgroundColor`: Card and primary container background.
- `CardAccentBackgroundColor`: Card accent (very subtle sub‑surface).
- `ContentBackgroundColor`: Background for inner content areas.
- `CardShadowColor`: Shadow color to create elevation.
- `BorderColorLight` / `BorderColorMedium` / `BorderColorDark`: Border colors to separate sections or containers.

### 3) Typography
- `TextColor`: Primary text (max contrast).
- `TextColorSecondary`: Secondary text (subtitles, subtle metadata).
- `TextColorTertiary`: Tertiary / placeholder text.
- `TextColorDisabled`: Disabled text.

### 4) Buttons (secondary)
- `ButtonBackgroundColor`: Secondary/neutral button background.
- `ButtonTextColor`: Secondary button text.
- `ButtonBorderColor`: Secondary button border.
- `ButtonHoverBackgroundColor`: Background on hover.
- `ButtonPressedBackgroundColor`: Background on pressed.

### 5) Primary Buttons (main action)
- `PrimaryButtonBackgroundColor`: Primary button background (interactive blue in light; desaturated blue in dark).
- `PrimaryButtonTextColor`: Primary button text.
- `PrimaryButtonHoverBackgroundColor`: Primary hover background.
- `PrimaryButtonPressedBackgroundColor`: Primary pressed background.

### 6) Input controls
- `InputBackgroundColor`: Inputs background (Entry, SearchBar, etc.).
- `InputBorderColor`: Normal input border.
- `InputTextColor`: Input text color.
- `InputPlaceholderColor`: Input placeholder (tertiary).
- `InputFocusBorderColor`: Focus border (interactive blue).

### 7) Status
- `SuccessColor`: Confirmation/success.
- `WarningColor`: Warning.
- `ErrorColor`: Error (uses material red in light and material/brand in dark depending on theme).
- `InfoColor`: Information.

### 8) Selection & Hover
- `SelectionColor`: Highlight/selection color (interactive blue).
- `SelectionBackgroundColor`: Selected item background (very light blue in light theme).
- `HighlightColor`: Hover overlay (low alpha for subtlety).
- `HoverOverlayColor`: Semantic alias for hover overlays.

### 9) Navigation (sidebar and active states)
- `NavigationBackgroundColor`: Navigation background.
- `NavigationTextColor`: Navigation text.
- `NavigationSelectedBackgroundColor`: Active item background (light: very light blue).
- `NavigationSelectedTextColor`: Active item text.
- `NavigationHoverBackgroundColor`: Navigation hover background.

### 10) Settings
- `SettingsBackgroundColor`, `SettingsBorderColor`, `SettingsTextColor`, `SettingsSecondaryTextColor`: Tokens specific to the Settings view.

### 11) Additional/interactive semantic tokens
- `InteractivePrimaryColor`: Primary blue for interactive actions (cancel button, active tag highlight, etc.).
- `InteractiveSecondaryColor`: Secondary blue variant.
- `BorderInteractiveColor`: Interactive border (input focus).
- `TextInteractiveColor`: Text over interactive backgrounds (adequate contrast).
- `IconPrimaryColor`: Base icon color.
- `IconInteractiveColor`: Icon color in interactive state.
- `DisabledBackgroundColor`: Background for disabled items.

---

## Common change examples

### Change brand color (PrimaryRed)
1) In `Resources/Styles/Colors.xaml` adjust:
```xml
<Color x:Key="PrimaryRed">#D32F2F</Color>
```
2) In `SettingsPage.xaml.cs` change the `resources["PrimaryRed"] = ...` values for Light/Dark in:
- `ApplyThemeColors(bool isDarkTheme)`
- `ApplyThemeColorsStatic(bool isDarkTheme, ResourceDictionary resources)`

### Change global interactive blue
1) In `Colors.xaml` (light):
```xml
<Color x:Key="InteractivePrimaryColor">#1976D2</Color>
<Color x:Key="InteractiveSecondaryColor">#0288D1</Color>
<Color x:Key="BorderInteractiveColor">#2196F3</Color>
```
2) In `SettingsPage.xaml.cs` define equivalents for Light/Dark in both methods.
3) In XAML, use `{DynamicResource InteractivePrimaryColor}` where appropriate (e.g., `SearchBar.CancelButtonColor`).

### Add a new token
1) Add to `Colors.xaml`:
```xml
<Color x:Key="MyNewAccentBackground">#FFEEDD</Color>
```
2) Add to Light/Dark sets in `ApplyThemeColors(...)` and `ApplyThemeColorsStatic(...)`:
```csharp
resources["MyNewAccentBackground"] = Color.FromArgb("#FFEEDD"); // Light
resources["MyNewAccentBackground"] = Color.FromArgb("#3A2A25"); // Dark (example)
```
3) Add the key to `PropagateThemeKeys(...)` if immediate propagation is needed after theme change.
4) Consume in XAML:
```xml
<Border BackgroundColor="{DynamicResource MyNewAccentBackground}" />
```

---

## How to test changes

- Build: from repo root run `dotnet build` or use your IDE.
- Switch theme: `Settings` → "Dark Mode" (Switch) to see component reactions.
- Verify:
  - Page, sidebar and card backgrounds/borders.
  - Typography (proper contrast).
  - Hover/pressed/selected states.
  - Inputs (interactive blue focus border).
  - Navigation: active item (interactive blue text and brand red dots/capsules, per design).

---

## Pitfalls (things to avoid)
- Do not use `ResourceDictionary.Source` at runtime (MAUI limitation). Always use the functions in `SettingsPage.xaml.cs` to apply colors.
- Do not leave hardcoded hex values in views. Replace with `{DynamicResource ...}`.
- Do not forget dark theme: any new key must have its Light and Dark equivalent.

---

## Quick references
- Light theme defaults: `Resources/Styles/Colors.xaml`.
- Runtime color application: `SettingsPage.xaml.cs` → `ApplyThemeColors()` and `ApplyThemeColorsStatic()`.
- Key propagation: `SettingsPage.xaml.cs` → `PropagateThemeKeys(...)`.
- Usage examples in views: `MainPage.xaml`, `Views/DashboardView.xaml`, `Views/AllToolsView.xaml`.

If you need a variable set for a new component (e.g., banners, chips, badges), specify the component and we will add appropriate Light/Dark tokens.
