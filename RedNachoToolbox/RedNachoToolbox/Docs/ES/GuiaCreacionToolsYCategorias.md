# Guía de Creación de Tools y Categorías (Arquitectura, Pasos, Ejemplos, Troubleshooting)

Esta guía explica, de forma práctica y detallada, cómo crear nuevos tools (herramientas) y categorías en Red Nacho ToolBox. Incluye el flujo recomendado con `ContentView` (para mantener el sidebar y el layout principal), la alternativa con páginas Shell, cómo registrar el tool en el catálogo y cómo integrarlo con el Dashboard, el buscador y la sección de "Recently used".

> Antes de empezar, revisa:
> - `Docs/FOLDER_STRUCTURE.md` (estructura del proyecto)
> - `Docs/GuiaColores.md` (tokens de color y aplicación en runtime)
> - `Docs/GuiaDeColoresYEstilos.md` (buenas prácticas de estilos y estados)

---

## 1) Prerrequisitos

- .NET 8 SDK + workloads de MAUI instalados (`dotnet workload install maui`).
- Conocer el patrón MVVM básico (View + ViewModel + Model).
- Entender el sistema de recursos con `DynamicResource` para colores/estilos.

---

## 2) Patrones de Tool en la app

Hay dos patrones posibles:

1. `ContentView` hospedado dentro de `MainPage` (RECOMENDADO):
   - Pro: Mantiene el sidebar, red dots de navegación, y coherencia del layout.
   - Implementación: Cambiar el `ContentView` en `MainContentHost`.

2. Página Shell (`ContentPage`) con navegación (`Shell.Current.GoToAsync`):
   - Pro: Página aislada, conveniente para flujos complejos.
   - Contra: Se pierde el layout de `MainPage` mientras navegas a pantalla completa.
   - Uso actual: `SettingsPage`.

La app ya incluye un ejemplo de tool con `ContentView`: `Tools/MarkdownToPdf/MarkdownToPdfView`.

---

## 3) Estructura recomendada de carpetas y nombres

- Carpeta base del tool: `Tools/NombreDelTool/`
  - `NombreDelToolView.xaml` (o `Page.xaml` si usas la ruta Shell)
  - `NombreDelToolView.xaml.cs`
  - `NombreDelToolViewModel.cs`
- Iconos PNG en `Resources/Images/` (ver sección de iconografía más abajo).

Convenciones sugeridas:
- Clases y archivos en PascalCase (`ImageConverterView`, `ImageConverterViewModel`).
- Namespace acorde al árbol de carpetas (`RedNachoToolbox.Tools.ImageConverter`).

---

## 4) Crear un nuevo Tool (ContentView)

### 4.1 Crear la vista y el ViewModel

1) Crea la carpeta: `Tools/MiNuevoTool/`
2) Agrega un `ContentView`:
```xml
<!-- Tools/MiNuevoTool/MiNuevoToolView.xaml -->
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="RedNachoToolbox.Tools.MiNuevoTool.MiNuevoToolView"
             BackgroundColor="{DynamicResource PageBackgroundColor}">
    <Grid Padding="16" RowDefinitions="Auto,*">
        <Label Text="Mi Nuevo Tool" FontSize="20"
               TextColor="{DynamicResource TextColor}"/>
        <!-- contenido principal -->
    </Grid>
</ContentView>
```

```csharp
// Tools/MiNuevoTool/MiNuevoToolView.xaml.cs
namespace RedNachoToolbox.Tools.MiNuevoTool;

public partial class MiNuevoToolView : ContentView
{
    public MiNuevoToolView()
    {
        InitializeComponent();
        BindingContext = new MiNuevoToolViewModel();
    }
}
```

```csharp
// Tools/MiNuevoTool/MiNuevoToolViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RedNachoToolbox.Tools.MiNuevoTool;

public partial class MiNuevoToolViewModel : ObservableObject
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
            // lógica principal del tool
            await Task.Delay(100);
        }
        finally { IsBusy = false; }
    }
}
```

### 4.2 Registrar el tool en el catálogo

El catálogo se construye en el `MainViewModel` (p. ej., `LoadSampleTools()` o el método que uses para llenar la colección). Añade un `ToolInfo`:

```csharp
// ViewModels/MainViewModel.cs (fragmento)
using RedNachoToolbox.Models;
using RedNachoToolbox.Tools.MiNuevoTool;

_tools.Add(new ToolInfo(
    name: "Mi Nuevo Tool",
    description: "Hace X de forma rápida.",
    iconPath: "document_outline_black.png", // ver sección iconografía
    category: ToolCategory.Productivity,
    targetType: null // Para ContentView hospedado, deja null y usa mapeo en ShowTool
));
```

### 4.3 Mapear el tool para hospedarlo en `MainPage`

En `MainPage.xaml.cs` existe un método `ShowTool(ToolInfo tool)` que hospeda tools dentro de `MainContentHost`. Añade tu mapeo:

```csharp
// MainPage.xaml.cs (dentro de ShowTool)
if (tool.Name.Equals("Mi Nuevo Tool", StringComparison.OrdinalIgnoreCase))
{
    view = new RedNachoToolbox.Tools.MiNuevoTool.MiNuevoToolView();
}
```

Con esto, al seleccionar el `ToolInfo` desde Dashboard/colecciones, se mostrará tu `ContentView` manteniendo el sidebar.

### 4.4 "Recently used"

Cuando un tool se abre, el `MainViewModel` expone un método `AddToRecentlyUsed(tool)`. El flujo de selección en Dashboard/All Tools ya lo invoca. Si abres el tool por tu cuenta, llama al método para reflejarlo en la sección de recientes:
```csharp
ViewModel.AddToRecentlyUsed(tool);
```

---

## 5) Alternativa: Tool como `ContentPage` + Shell

Si prefieres navegar a una página completa:

1) Crea `Tools/MiNuevoTool/MiNuevoToolPage.xaml` (ContentPage) + ViewModel.
2) Registra la ruta:
```csharp
// AppShell.xaml.cs
Routing.RegisterRoute(nameof(MiNuevoToolPage), typeof(MiNuevoToolPage));
```
3) Navega:
```csharp
await Shell.Current.GoToAsync(nameof(MiNuevoToolPage));
```

Ten en cuenta que este flujo reemplaza la vista principal y oculta el sidebar mientras la página está activa. Úsalo solo si el tool necesita una pantalla completa.

---

## 6) Categorías: crear y usar

### 6.1 Agregar una categoría nueva

1) Edita el enum en `Models/ToolCategory.cs` y añade el nuevo valor (por ejemplo `AI`):
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
    AI // nueva categoría
}
```

2) Asigna la categoría al crear el `ToolInfo` del nuevo tool.

3) Si tienes botones de categorías en el sidebar o filtros específicos, añade la UI correspondiente (ejemplo similar a `Productivity` en `MainPage.xaml`).

### 6.2 Filtrado y conteos

- El Dashboard/All Tools ya soporta filtrado por texto; si hay filtros por categoría, asegúrate de incluir la nueva categoría en el switch/casos.
- Actualiza textos/etiquetas visibles para mantener consistencia (usa `DynamicResource` para colores y estilos).

---

## 7) Iconografía y tema (PNG)

- Coloca los PNG en `Resources/Images/`.
- Usa nombres coherentes: `{base}_black.png` (claro) y `{base}_white.png` (oscuro).
- Dos opciones para tema:
  - `AppThemeBinding` inline:
    ```xml
    <Image Source="{AppThemeBinding Light=document_outline_black.png, Dark=document_outline_white.png}" />
    ```
  - `ThemeIconConverter` (si manejas nombres base + estado de tema):
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

Evita conflictos de nombres con `Resources/Vector/`. La app prioriza PNG para evitar errores de Resizetizer.

---

## 8) Buenas prácticas de rendimiento

- No bloquees el hilo de UI con `Task.Delay` en handlers. Usa helpers no bloqueantes.
- Anima `Opacity` (`FadeTo`) en vez de dimensiones para evitar relayout.
- Si tu tool usa `WebView`, considera precalentar/compartir la instancia (caché) y esperar la inicialización antes de operar.
- Para listas largas, usa `CollectionView` (virtualización) y `ItemTemplate`s ligeros.

---

## 9) Checklist de publicación del tool

- [ ] Vista (`ContentView` o `ContentPage`) creada y enlazada a su ViewModel.
- [ ] Recursos de iconos PNG agregados sin conflictos.
- [ ] `ToolInfo` añadido al catálogo (`MainViewModel`).
- [ ] Mapeo en `ShowTool(...)` (si es `ContentView`).
- [ ] Estados visuales coherentes (hover/pressed) y colores por `DynamicResource`.
- [ ] Pruebas en Light/Dark.
- [ ] Verificado en Release build.

---

## 10) Troubleshooting

- **El tool no aparece en el Dashboard**: valida que el `ToolInfo` esté en la colección y que no esté filtrado por el buscador.
- **Icono no se ve**: revisa que el PNG esté en `Resources/Images/` y que el nombre sea correcto. Evita duplicados con `Resources/Vector/`.
- **Navegación Shell falla**: confirma que la ruta está registrada en `AppShell.xaml.cs` y que usas `nameof(TuPagina)`.
- **Tema no cambia**: usa tokens y `DynamicResource`. La app aplica colores en runtime desde `SettingsPage.ApplyThemeColors*`.
- **Jank en animaciones**: no animes tamaños; usa `FadeTo` y evita `Task.Delay` en el hilo de UI.

---

## 11) Ejemplo mínimo completo (ContentView)

```csharp
// Models/ToolInfo.cs (creación del modelo)
var info = new ToolInfo(
    name: "Image Converter",
    description: "Convierte imágenes entre formatos.",
    iconPath: "document_outline_black.png",
    category: ToolCategory.Utilities,
    targetType: null);

// ViewModels/MainViewModel.cs (agregar al catálogo)
_tools.Add(info);

// MainPage.xaml.cs (mapeo en ShowTool)
if (tool.Name == "Image Converter")
{
    view = new RedNachoToolbox.Tools.ImageConverter.ImageConverterView();
}
```

Con estos pasos y recomendaciones, podrás crear e integrar nuevos tools y categorías de manera rápida, consistente y con una excelente experiencia de usuario.
