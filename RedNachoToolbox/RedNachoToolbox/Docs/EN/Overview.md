# Red Nacho ToolBox — General Documentation (Overview, Architecture, Build/Publish, Troubleshooting)

This is the main guide for the project. It covers the product overview, technical architecture, UI/UX guidelines, performance, how to build and publish for each platform (Windows/Android/iOS/macOS), and an extensive troubleshooting section.


## 1) Product Overview

- Name: Red Nacho ToolBox (Toolkit)
- Platform: .NET MAUI (cross‑platform)
- Architecture: MVVM with XAML pages, ViewModels, services and converters.
- Goal: A collection of productivity tools with a modern experience, light/dark themes, a collapsible sidebar, and a Dashboard with "Recently used" tools.

### Key Features
- Collapsible sidebar with logo and theme-aware icons.
- Dashboard with “Recently used” (last 3 tools) and a debounced search.
- Runtime theme switching (Light/Dark) persisted with `Preferences`.
- Semantic color tokens and styles applied via `DynamicResource`.
- Active page indicators (brand‑red capsule and collapsed-state dot).
- Sample tool: Markdown → PDF with HTML/CSS preview (WebView2 on Windows).


## 2) Technical Architecture

The folder structure and patterns are detailed in `Docs/FOLDER_STRUCTURE.md`. Summary:

- `Models/` Domain models (`ToolInfo`, `ToolCategory`, etc.).
- `ViewModels/` Presentation logic (e.g., `MainViewModel`).
- `Views/` XAML pages and views (`MainPage.xaml`, `DashboardView.xaml`).
- `Tools/` Each tool in its own folder with View and ViewModel.
- `Converters/` Value converters (e.g., `InvertedBoolConverter`, `ThemeIconConverter`).
- `Resources/Styles/` XAML colors and styles. Tokens consumed via `DynamicResource`.
- `Docs/` Project documentation (this guide and others).

### Main Building Blocks
- `MainPage.xaml`/`.cs`: Main container with the sidebar (expanded/collapsed) and the content host.
- `MainViewModel.cs`: Navigation state (Dashboard/Productivity), search text, tools list, “Recently used”, theme, sidebar preference, etc.
- `SettingsPage.xaml`/`.cs`: Theme switching (Light/Dark) + preferences (e.g., collapsed sidebar). Implements direct runtime color application.
- `Tools/MarkdownToPdf/*`: Complete example tool. Uses WebView2 on Windows and generates PDF.

### MVVM Pattern
- XAML views bound to ViewModels via bindings.
- Navigation via Shell or by swapping `ContentView` inside the main host.
- Converters and resources (colors/styles) decoupled from logic.


## 3) UI/UX and Theme System

- Color tokens: defined by key and applied with `DynamicResource` in XAML.
- Light/Dark themes: MAUI does not allow changing `ResourceDictionary.Source` at runtime from C#. The app instead applies colors programmatically via `SettingsPage.ApplyThemeColors()` and `ApplyThemeColorsStatic()` on `Application.Current.Resources`.
- Icons: PNG with light/dark variants. You can use an inline `AppThemeBinding` or the `ThemeIconConverter` (MultiBinding with a base name + `IsDarkTheme`).
- Active indicators: red capsule/dot (`PrimaryRed`) with soft fade animations (avoid size animations to reduce jank).
- Sidebar buttons: lightweight hover/pressed feedback (do not block the UI). Prefer `Border` for performance; currently `Frame` is used without shadows.
- Markdown Preview: enforced white background for readability in dark mode.

See also: `Docs/GuiaColores.md` (Spanish), `Docs/GuiaDeColoresYEstilos.md` (Spanish), and `Docs/ColorsAndStylesGuide.en.md` (English).


## 4) Performance Guidelines

- Do not block the UI thread in click feedback (avoid chaining `await Task.Delay` in handlers; use non‑blocking helpers).
- Prefer `FadeTo` (Opacity) over changing `WidthRequest/HeightRequest` (avoids re‑measuring and layout thrash).
- View caching: `_dashboardView`, `_productivityView`, and `_markdownToPdfView` to avoid recreating heavy views.
- Debounced search (`OnSearchTextChanged` with a `CancellationTokenSource`) to avoid filtering on every keystroke.
- Avoid excessive logging in Release builds (`#if DEBUG`).
- Consider `Border` instead of `Frame` when feasible.


## 5) Build and Publish

### Requirements
- .NET 8 SDK
- MAUI workloads:
  ```bash
  dotnet workload install maui
  ```
- IDE: Visual Studio 2022 (Windows/macOS) or JetBrains Rider

### Local Build
- Debug:
  ```bash
  dotnet build
  ```
- Release (all):
  ```bash
  dotnet build -c Release
  ```

### Windows (MSIX)
- Publish MSIX:
  ```bash
  dotnet publish -f net8.0-windows10.0.19041.0 -c Release -p:WindowsPackageType=Msix
  ```
- Signing/Certificate: configure a signing certificate (project properties or MSIX signing parameters). For testing, a dev certificate can be used.

### Android (APK/AAB)
- APK (quick debug):
  ```bash
  dotnet publish -f net8.0-android -c Release -p:AndroidPackageFormat=apk
  ```
- AAB (Play Store):
  ```bash
  dotnet publish -f net8.0-android -c Release -p:AndroidPackageFormat=aab \
    -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=your.keystore \
    -p:AndroidSigningKeyAlias=alias -p:AndroidSigningKeyPass=password -p:AndroidSigningStorePass=password
  ```

### iOS (.ipa)
- Requires macOS + Xcode
  ```bash
  dotnet publish -f net8.0-ios -c Release \
    -p:ArchiveOnBuild=true -p:CodesignKey="Apple Distribution: YOUR ORG" -p:CodesignProvision="YOUR PROFILE"
  ```
  Then export `.ipa` from the generated archive (Xcode Organizer or `xcodebuild -exportArchive`).

### macOS (Mac Catalyst)
- App `.app`/`.pkg`:
  ```bash
  dotnet publish -f net8.0-maccatalyst -c Release
  ```
  Sign and notarize with `codesign`/`notarytool`.


## 6) Troubleshooting

### XAML: `AppThemeBinding` accessibility / `BindableLayout` resolution
- Use `AppThemeBinding` inline in `Image.Source`:
  ```xml
  <Image Source="{AppThemeBinding Light=icon_light.png, Dark=icon_dark.png}" />
  ```
- Add namespace alias for `BindableLayout` attached members:
  ```xml
  xmlns:controls="clr-namespace:Microsoft.Maui.Controls;assembly=Microsoft.Maui.Controls"
  ...
  <VerticalStackLayout controls:BindableLayout.ItemsSource="{Binding Items}">
      <controls:BindableLayout.ItemTemplate>
          <DataTemplate>...</DataTemplate>
      </controls:BindableLayout.ItemTemplate>
  </VerticalStackLayout>
  ```

### Error: “Source can only be set from XAML”
- At runtime MAUI does not allow assigning `ResourceDictionary.Source` from C#.
- Implemented solution: apply colors directly to `Application.Current.Resources` (`SettingsPage.ApplyThemeColors*`).

### CA1416 (platform compatibility)
- Wrap platform‑specific APIs with guards or pragmas (e.g., `FileSaver.Default.SaveAsync` for Windows):
  ```csharp
  #pragma warning disable CA1416
  // platform‑specific call
  #pragma warning restore CA1416
  ```
  Or `#if WINDOWS`.

### WebView2 not initialized (Windows)
- Ensure `EnsureCoreWebView2Async()` is called and wait for document readiness before extracting content.
- See `Tools/MarkdownToPdf/MarkdownToPdfView.xaml.cs`.

### Resizetizer: resource conflicts
- Do not duplicate names between `Resources/Images/` and `Resources/Vector/`.
- Prefer PNG (SVG inclusion was removed from `.csproj` to avoid collisions).

### Jank/Twitching in animations
- Do not animate size; use `FadeTo`/`TranslationTo`.
- Avoid `Task.Delay` sequences in UI handlers; use non‑blocking helpers.
- Cache heavy views (WebView).


## 7) Roadmap / Next Steps
- Migrate sidebar buttons from `Frame` → `Border` to lighten the UI.
- Replace CA1416 pragmas with platform guards.
- Convert the Productivity sidebar list to a `CollectionView` for virtualization.
- Accessibility tests (contrast, visible focus, keyboard navigation).


## 8) References
- `Docs/FOLDER_STRUCTURE.md`
- `Docs/GuiaColores.md` (Spanish)
- `Docs/GuiaDeColoresYEstilos.md` (Spanish)
- `Docs/ColorsAndStylesGuide.en.md`
- `Docs/Guia_Creacion_Tools_y_Categorias.md` (Spanish)
- `Docs/ToolsAndCategoriesGuide.en.md`
- Source code (this repository)
