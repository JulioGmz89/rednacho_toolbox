# Tools and Categories Guide (Architecture, Steps, Examples, Troubleshooting)

This guide explains how to create new tools and categories in Red Nacho ToolBox. It includes the recommended `ContentView` hosting pattern (to keep the sidebar and main layout), the alternative Shell page approach, how to register the tool in the catalog, and how to integrate it with the Dashboard, search, and the "Recently used" section.

> Before starting, review:
> - `Docs/FOLDER_STRUCTURE.md` (project structure)
> - `Docs/GuiaColores.md` (color tokens and runtime application)
> - `Docs/ColorsAndStylesGuide.en.md` (styles and states best practices)

---

## 1) Prerequisites

- .NET 8 SDK + MAUI workloads installed (`dotnet workload install maui`).
- Basic MVVM understanding (View + ViewModel + Model).
- Understanding of the resource system with `DynamicResource` for colors/styles.

---

## 2) Tool Patterns in the App

Two options:

1. `ContentView` hosted inside `MainPage` (RECOMMENDED):
   - Pros: Keeps the sidebar, navigation red dots, and main layout coherent.
   - Implementation: Swap the `ContentView` in `MainContentHost`.

2. Shell `ContentPage` with navigation (`Shell.Current.GoToAsync`):
   - Pros: Isolated full page, convenient for complex flows.
   - Cons: The `MainPage` layout (sidebar) is hidden while navigating.
   - Current use: `SettingsPage`.

The app includes a complete example of a `ContentView` tool: `Tools/MarkdownToPdf/MarkdownToPdfView`.

---

## 3) Recommended Folder and Naming

- Tool base folder: `Tools/YourToolName/`
  - `YourToolNameView.xaml` (or `Page.xaml` if you choose Shell)
  - `YourToolNameView.xaml.cs`
  - `YourToolNameViewModel.cs`
- PNG icons in `Resources/Images/` (see iconography section below).

Conventions:
- PascalCase for classes and files (`ImageConverterView`, `ImageConverterViewModel`).
- Namespace aligned with folder structure (`RedNachoToolbox.Tools.ImageConverter`).

---

## 4) Create a New Tool (ContentView)

### 4.1 Create the View and ViewModel

1) Create folder: `Tools/MyNewTool/`
2) Add a `ContentView`:
```xml
<!-- Tools/MyNewTool/MyNewToolView.xaml -->
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="RedNachoToolbox.Tools.MyNewTool.MyNewToolView"
             BackgroundColor="{DynamicResource PageBackgroundColor}">
    <Grid Padding="16" RowDefinitions="Auto,*">
        <Label Text="My New Tool" FontSize="20"
               TextColor="{DynamicResource TextColor}"/>
        <!-- main content here -->
    </Grid>
</ContentView>
```

```csharp
// Tools/MyNewTool/MyNewToolView.xaml.cs
namespace RedNachoToolbox.Tools.MyNewTool;

public partial class MyNewToolView : ContentView
{
    public MyNewToolView()
    {
        InitializeComponent();
        BindingContext = new MyNewToolViewModel();
    }
}
```

```csharp
// Tools/MyNewTool/MyNewToolViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RedNachoToolbox.Tools.MyNewTool;

public partial class MyNewToolViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [RelayCommand]
    private async Task RunAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            // tool logic
            await Task.Delay(100);
        }
        finally { IsBusy = false; }
    }
}
```

### 4.2 Register the Tool in the Catalog

The catalog is built in `MainViewModel` (e.g., `LoadSampleTools()` or similar). Add a `ToolInfo`:

```csharp
// ViewModels/MainViewModel.cs (snippet)
using RedNachoToolbox.Models;
using RedNachoToolbox.Tools.MyNewTool;

_tools.Add(new ToolInfo(
    name: "My New Tool",
    description: "Does X quickly.",
    iconPath: "document_outline_black.png", // see icon section below
    category: ToolCategory.Productivity,
    targetType: null // For ContentView hosting, leave null and map in ShowTool
));
```

### 4.3 Map the Tool in `MainPage` for Hosting

In `MainPage.xaml.cs` there is a `ShowTool(ToolInfo tool)` method that hosts tools inside `MainContentHost`. Add your mapping:

```csharp
// MainPage.xaml.cs (inside ShowTool)
if (tool.Name.Equals("My New Tool", StringComparison.OrdinalIgnoreCase))
{
    view = new RedNachoToolbox.Tools.MyNewTool.MyNewToolView();
}
```

Once selected from the Dashboard/collections, the `ContentView` will display while keeping the sidebar.

### 4.4 "Recently used"

When a tool opens, `MainViewModel` exposes `AddToRecentlyUsed(tool)`. The selection flow in Dashboard/All Tools already calls it. If you open the tool manually, call it so it appears in the recent list:
```csharp
ViewModel.AddToRecentlyUsed(tool);
```

---

## 5) Alternative: Tool as Shell `ContentPage`

If you prefer full‑page navigation:

1) Create `Tools/MyNewTool/MyNewToolPage.xaml` (ContentPage) + ViewModel.
2) Register the route:
```csharp
// AppShell.xaml.cs
Routing.RegisterRoute(nameof(MyNewToolPage), typeof(MyNewToolPage));
```
3) Navigate:
```csharp
await Shell.Current.GoToAsync(nameof(MyNewToolPage));
```

Be aware this flow hides the main `MainPage` layout and sidebar while active. Use for special cases.

---

## 6) Categories: Create and Use

### 6.1 Add a New Category

1) Edit the enum in `Models/ToolCategory.cs` and add the value (e.g., `AI`):
```csharp
public enum ToolCategory
{
    Utilities,
    Development,
    System,
    Network,
    FileManagement,
    Security,
    Productivity,
    Media,
    AI // new category
}
```

2) Assign the category when creating the new tool’s `ToolInfo`.

3) If you have explicit category buttons/filters in the sidebar, add the corresponding UI (similar to `Productivity` in `MainPage.xaml`).

### 6.2 Filtering and Counts

- Dashboard/All Tools already supports text filtering; if you implement category filters, include the new category in your cases.
- Update labels/texts as needed. Keep using `DynamicResource` for colors and styles.

---

## 7) Iconography and Theme (PNG)

- Put PNGs under `Resources/Images/`.
- Naming: `{base}_black.png` (light) and `{base}_white.png` (dark).
- Two options for theme switching:
  - Inline `AppThemeBinding`:
    ```xml
    <Image Source="{AppThemeBinding Light=document_outline_black.png, Dark=document_outline_white.png}" />
    ```
  - `ThemeIconConverter` (if you manage base name + theme state):
    ```xml
    <Image>
      <Image.Source>
        <MultiBinding Converter="{StaticResource ThemeIconConverter}">
          <Binding Path="IconBase"/>
          <Binding Path="IsDarkTheme"/>
        </MultiBinding>
      </Image.Source>
    </Image>
    ```

Avoid name conflicts with `Resources/Vector/`. The app prefers PNG to avoid Resizetizer conflicts.

---

## 8) Performance Best Practices

- Do not block the UI thread with `Task.Delay` in handlers. Use non‑blocking helpers.
- Animate `Opacity` (`FadeTo`) instead of size to avoid relayout.
- If your tool uses `WebView`, consider pre‑warming/sharing the instance (cache) and waiting for initialization before use.
- For long lists, use `CollectionView` (virtualization) and lightweight `ItemTemplate`s.

---

## 9) Tool Publication Checklist

- [ ] View (`ContentView` or `ContentPage`) created and bound to its ViewModel.
- [ ] PNG icon resources added without conflicts.
- [ ] `ToolInfo` added to catalog (`MainViewModel`).
- [ ] Mapping in `ShowTool(...)` (if using `ContentView`).
- [ ] Consistent visual states (hover/pressed) and colors via `DynamicResource`.
- [ ] Tested in Light/Dark.
- [ ] Verified in Release build.

---

## 10) Troubleshooting

- **Tool not visible in Dashboard**: ensure the `ToolInfo` is in the collection and not filtered by search.
- **Icon missing**: ensure the PNG is in `Resources/Images/` and the name is correct. Avoid duplicates with `Resources/Vector/`.
- **Shell navigation fails**: route registered in `AppShell.xaml.cs` and use `nameof(YourPage)`.
- **Theme does not change**: use tokens and `DynamicResource`. Colors are applied at runtime from `SettingsPage.ApplyThemeColors*`.
- **Animation jank**: do not animate sizes; use `FadeTo` and avoid `Task.Delay` in the UI thread.

---

## 11) Minimal Complete Example (ContentView)

```csharp
// Models/ToolInfo.cs (create the model)
var info = new ToolInfo(
    name: "Image Converter",
    description: "Converts images between formats.",
    iconPath: "document_outline_black.png",
    category: ToolCategory.Utilities,
    targetType: null);

// ViewModels/MainViewModel.cs (add to catalog)
_tools.Add(info);

// MainPage.xaml.cs (mapping in ShowTool)
if (tool.Name == "Image Converter")
{
    view = new RedNachoToolbox.Tools.ImageConverter.ImageConverterView();
}
```

With these steps and recommendations, you can create and integrate new tools and categories quickly and consistently while keeping a great user experience.
