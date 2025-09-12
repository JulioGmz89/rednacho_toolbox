# Guía para crear nuevos Tools en Red Nacho ToolBox

Esta guía documenta el proceso para agregar nuevas herramientas ("tools") a la aplicación, tomando como referencia el primer tool implementado: "Markdown → PDF". El objetivo es que el proceso sea rápido, consistente y replicable.

---

## Estructura recomendada de carpetas

- `Tools/NombreDelTool/`
  - `NombreDelToolPage.xaml`
  - `NombreDelToolPage.xaml.cs`
  - `NombreDelToolViewModel.cs`
- `Resources/Images/`
  - Icono(s) PNG del tool

Ejemplo (este repo):
- `Tools/MarkdownToPdf/MarkdownToPdfPage.xaml`
- `Tools/MarkdownToPdf/MarkdownToPdfPage.xaml.cs`
- `Tools/MarkdownToPdf/MarkdownToPdfViewModel.cs`

---

## Pasos para crear un nuevo Tool

1) Crear carpeta del tool
- Ruta: `Tools/NombreDelTool/`

2) Crear la página (UI)
- Archivo: `Tools/NombreDelTool/NombreDelToolPage.xaml`
- Code-behind: `Tools/NombreDelTool/NombreDelToolPage.xaml.cs`
- Recomendaciones de XAML:
  - Usa `Grid` + `Frame` con `DynamicResource` para colores (ver `Resources/Styles/Colors.xaml`).
  - Mantén controles accesibles y responsivos (ScrollView cuando aplique).
  - Expone controles a través de bindings al ViewModel.

3) Crear el ViewModel
- Archivo: `Tools/NombreDelTool/NombreDelToolViewModel.cs`
- Hereda de `BaseViewModel` para contar con `INotifyPropertyChanged`, `IsBusy`, `Title`, etc.
- Expone las propiedades que necesita la UI (texto de entrada, opciones, resultados, etc.).
- Implementa la lógica principal del tool y métodos públicos que el code-behind pueda invocar (por ejemplo: `GenerateAsync()`).

4) Registrar la ruta para navegación
- Archivo: `AppShell.xaml.cs`
- Agrega:
  ```csharp
  using RedNachoToolbox.Tools.NombreDelTool;
  Routing.RegisterRoute(nameof(NombreDelToolPage), typeof(NombreDelToolPage));
  ```
- Esto permite navegar con `await Shell.Current.GoToAsync(nameof(NombreDelToolPage));`

5) Agregar el tool al catálogo
- Archivo: `ViewModels/MainViewModel.cs`, método `LoadSampleTools()`.
- Agrega un `new ToolInfo(...)` con:
  - `Name`: Nombre visible del tool.
  - `Description`: Descripción corta.
  - `IconPath`: PNG existente en `Resources/Images/`.
  - `Category`: una del enum `ToolCategory`.
  - `TargetType`: `typeof(NombreDelToolPage)` para habilitar navegación.

  Ejemplo:
  ```csharp
  new ToolInfo(
      "Markdown → PDF",
      "Convierte Markdown a PDF con estilos personalizables y vista previa.",
      "document_outline_black.png",
      ToolCategory.Productivity,
      typeof(MarkdownToPdfPage)
  ),
  ```

6) Iconos
- Coloca los PNGs en `Resources/Images/`.
- Usa nombres coherentes y existentes en la app. Si no hay icono propio, usa temporales como `document_outline_black.png`.

7) Navegación desde el Dashboard / All Tools
- Ya está habilitada genéricamente: si un `ToolInfo` tiene `TargetType`, al seleccionarse se navega a esa página.
- Código relevante:
  - `Views/AllToolsView.xaml.cs` → `OnToolSelectionChanged(...)`
  - `Views/DashboardView.xaml.cs` → `OnToolSelectionChanged(...)`

8) Estilos y temas
- Usa `DynamicResource` con claves definidas en `Resources/Styles/Colors.xaml` y `Resources/Styles/Styles.xaml`.
- Evita hardcodear colores; así soportas el cambio de tema en tiempo real.

9) Validación rápida
- Compila y ejecuta.
- Verifica que el tool aparece en el Dashboard/All Tools.
- Selecciónalo y confirma que navega a su página.
- Prueba el flujo principal del tool.

---

## Buenas prácticas

- Mantén el ViewModel sin referencias a tipos de UI (solo lógica y estado).
- Usa `IsBusy` y deshabilita acciones largas mientras se ejecutan.
- Registra todas las rutas de páginas de tools en `AppShell.xaml.cs`.
- Para assets/recursos, usa los existentes de la app y `DynamicResource`.
- Para compartir/guardar archivos (si aplica), usa APIs de `Microsoft.Maui.Storage` y `Share`.

---

## Ejemplo: Tool "Markdown → PDF"

- UI: `Tools/MarkdownToPdf/MarkdownToPdfPage.xaml`
  - Panel izquierdo: opciones de estilo (fuente, tamaño, color, márgenes), Editor de Markdown, y ejemplo pre-generado (solo lectura).
  - Panel derecho: `WebView` con vista previa en vivo.
  - Botones: "Refrescar" (reconstruye el HTML) y "Generar PDF".

- VM: `Tools/MarkdownToPdf/MarkdownToPdfViewModel.cs`
  - Convierte Markdown a HTML con Markdig.
  - Aplica CSS dinámico según las opciones elegidas.
  - Genera PDF usando HtmlRendererCore + PdfSharpCore.
  - Devuelve `byte[]` del PDF para compartir/guardar.

- Paquetes NuGet agregados (en `.csproj`):
  - `Markdig`
  - `HtmlRendererCore.PdfSharp`
  - `PdfSharpCore`

---

## Checklist para cada nuevo Tool

- [ ] Carpeta en `Tools/NombreDelTool/`
- [ ] Página XAML + code-behind
- [ ] ViewModel que hereda de `BaseViewModel`
- [ ] Ruta registrada en `AppShell.xaml.cs`
- [ ] Entrada en `LoadSampleTools()` con `TargetType`
- [ ] Icono en `Resources/Images/`
- [ ] Estilos con `DynamicResource`
- [ ] Flujo probado (navegación + función principal)

---

## Preguntas frecuentes

- ¿Cómo agrego más opciones visuales? 
  - Expón nuevas propiedades en el ViewModel y úsalo en el CSS/HTML de vista previa. En XAML agrega los controles (Picker, Slider, etc.).

- ¿Cómo persisto preferencias del tool? 
  - Usa `Preferences` (de `Microsoft.Maui.Storage`) o un servicio dedicado. Lee/guarda en el ViewModel.

- ¿Cómo agrego documentación propia del tool? 
  - Crea un `.md` en `Docs/` con ejemplos y notas de diseño.
