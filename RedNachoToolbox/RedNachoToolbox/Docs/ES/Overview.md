# Red Nacho ToolBox — Documentación General (Overview, Arquitectura, Build/Publish, Troubleshooting)

Esta es la guía principal del proyecto. Cubre la visión general del producto, la arquitectura técnica, lineamientos de UI/UX, rendimiento, cómo construir y publicar (Windows/Android/iOS/macOS), y una sección extensa de troubleshooting.


## 1) Overview del Producto

- Nombre: Red Nacho ToolBox (Toolkit)
- Plataforma: .NET MAUI (multiplataforma)
- Arquitectura: MVVM con páginas XAML, ViewModels, y servicios/converters.
- Objetivo: Colección de herramientas de productividad con una experiencia moderna, tema claro/oscuro, navegación lateral colapsable y un Dashboard con “Recently used”.

### Características Clave
- Sidebar colapsable con logo e iconos conmutables por tema.
- Dashboard con “Recently used” (últimos 3 tools) y buscador con debounce.
- Cambios de tema en runtime (Light/Dark) con persistencia en `Preferences`.
- Tokens semánticos de color y estilos aplicados por `DynamicResource`.
- Indicadores de página activa (cápsula roja y punto en modo colapsado).
- Tool de ejemplo: Markdown → PDF, con vista previa HTML/CSS (WebView2 en Windows).


## 2) Arquitectura Técnica

La estructura de carpetas y los patrones están descritos a detalle en `FOLDER_STRUCTURE.md`. Resumen:

- `Models/` Modelos de dominio (`ToolInfo`, `ToolCategory`, etc.).
- `ViewModels/` Lógica de presentación (por ejemplo `MainViewModel`).
- `Views/` Páginas y vistas XAML (por ejemplo `MainPage.xaml`, `DashboardView.xaml`).
- `Tools/` Cada Tool en su subcarpeta con su View y ViewModel.
- `Converters/` Convertidores (por ejemplo `InvertedBoolConverter`, `ThemeIconConverter`).
- `Resources/Styles/` Colores y estilos XAML. Tokens consumidos con `DynamicResource`.
- `Docs/` Documentación del proyecto (esta guía y otras).

### Piezas principales
- `MainPage.xaml`/`.cs`: Contenedor principal con el sidebar (expandido/colapsado) y host del contenido.
- `MainViewModel.cs`: Estado de navegación (Dashboard/Productivity), buscador, lista de tools, “Recently used”, tema, preferencia de sidebar, etc.
- `SettingsPage.xaml`/`.cs`: Cambia tema (claro/oscuro) y preferencias (p. ej., sidebar colapsado). Implementa la aplicación directa de colores en runtime.
- `Tools/MarkdownToPdf/*`: Ejemplo de tool completo. Usa WebView2 en Windows y genera PDF.

### Patrón MVVM
- Vistas XAML enlazadas a ViewModels con bindings.
- Navegación por Shell o swaps de `ContentView` en el host principal.
- Converters y recursos (colores/estilos) desacoplados de la lógica.


## 3) UI/UX y Sistema de Temas

- Tokens de color: definidos por clave y aplicados con `DynamicResource` en XAML.
- Tema Claro/Oscuro: la app no carga diccionarios XAML por `Source` en runtime (limitación MAUI). En su lugar, aplica colores programáticamente vía `SettingsPage.ApplyThemeColors()` y `ApplyThemeColorsStatic()` sobre `Application.Current.Resources`.
- Iconografía: PNG con variantes claro/oscuro. Se puede usar `AppThemeBinding` inline o `ThemeIconConverter` (MultiBinding con base + `IsDarkTheme`).
- Indicadores activos: cápsula/borde rojo (`PrimaryRed`) y punto (modo colapsado) con animación suave (fade, no cambia medidas para evitar jank).
- Botones del sidebar: eventos de hover/pressed ligeros (no bloquean UI). Recomendado `Border` para máximo rendimiento; actualmente se usa `Frame` con sombras deshabilitadas.
- Markdown Preview: fondo blanco forzado para legibilidad en modo oscuro.

Ver también: `Docs/GuiaColores.md` y `Docs/GuiaDeColoresYEstilos.md`.


## 4) Rendimiento (Performance)

- Evitar bloquear el hilo de UI en feedback visual (no usar `await Task.Delay` encadenado en handlers; usar helpers no bloqueantes).
- Animaciones con `FadeTo` (Opacity) en lugar de modificar `WidthRequest/HeightRequest` (evita re-medidas y “layout thrash”).
- Cache de vistas: `_dashboardView`, `_productivityView`, y `_markdownToPdfView` para no re-crear vistas pesadas.
- Debounce en búsqueda (`OnSearchTextChanged` con `CancellationTokenSource`) para evitar filtrar en cada tecla.
- Evitar trazas excesivas en Release (`#if DEBUG`).
- Considerar `Border` en vez de `Frame` cuando sea posible.


## 5) Build y Publish

### Requisitos
- .NET 8 SDK.
- Workloads MAUI: 
  ```bash
  dotnet workload install maui
  ```
- IDE: Visual Studio 2022 (Windows/macOS) o Rider.

### Construcción local
- Debug:
  ```bash
  dotnet build
  ```
- Release general:
  ```bash
  dotnet build -c Release
  ```

### Windows (MSIX)
- Publicar MSIX:
  ```bash
  dotnet publish -f net8.0-windows10.0.19041.0 -c Release -p:WindowsPackageType=Msix
  ```
- Firma/Certificado: usar un certificado de firma (puede configurarse en las propiedades del proyecto o pasar parámetros MSIX de firma). Para pruebas, se puede instalar un certificado de desarrollo.

### Android (APK/AAB)
- APK (debug rápido):
  ```bash
  dotnet publish -f net8.0-android -c Release -p:AndroidPackageFormat=apk
  ```
- AAB (Play Store):
  ```bash
  dotnet publish -f net8.0-android -c Release -p:AndroidPackageFormat=aab \
    -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=tu.keystore \
    -p:AndroidSigningKeyAlias=alias -p:AndroidSigningKeyPass=password -p:AndroidSigningStorePass=password
  ```

### iOS (.ipa)
- Requiere macOS y Xcode.
  ```bash
  dotnet publish -f net8.0-ios -c Release \
    -p:ArchiveOnBuild=true -p:CodesignKey="Apple Distribution: TU ORG" -p:CodesignProvision="TU PERFIL"
  ```
  Luego exportar `.ipa` desde el archivo generado (Xcode Organizer o `xcodebuild -exportArchive`).

### macOS (Mac Catalyst)
- App `.app`/`.pkg`:
  ```bash
  dotnet publish -f net8.0-maccatalyst -c Release
  ```
  Firma y notarización se realizan con `codesign`/`notarytool`.


## 6) Troubleshooting

### XAML: `AppThemeBinding` “inaccesible” / `BindableLayout` no resuelto
- Usa `AppThemeBinding` como extensión inline en `Image.Source`:
  ```xml
  <Image Source="{AppThemeBinding Light=icon_light.png, Dark=icon_dark.png}" />
  ```
- Añade alias de namespace para adjuntos de `BindableLayout`:
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
- En runtime MAUI no permite asignar `ResourceDictionary.Source` desde C#.
- Solución implementada: aplicar colores directamente sobre `Application.Current.Resources` (`SettingsPage.ApplyThemeColors*`).

### CA1416 (API no soportada en todas las plataformas)
- Encapsular llamadas con guards o pragmas alrededor de API específicas de plataforma (por ejemplo, `FileSaver.Default.SaveAsync` para Windows):
  ```csharp
  #pragma warning disable CA1416
  // llamada específica
  #pragma warning restore CA1416
  ```
  O usar `#if WINDOWS`.

### WebView2 no inicializa (Windows)
- Asegurarse de ejecutar `EnsureCoreWebView2Async()` y usar `WaitForDocumentCompleteAsync` antes de extraer contenido.
- Ver `Tools/MarkdownToPdf/MarkdownToPdfView.xaml.cs`.

### Resizetizer: conflictos de recursos
- No dupliques nombres entre `Resources/Images/` y `Resources/Vector/`.
- Preferir PNG (se eliminó la inclusión de SVG en `.csproj` para evitar colisiones).

### Jank/Twitching en animaciones
- Evita animar medidas (`WidthRequest/HeightRequest`). Usa `FadeTo`/`TranslationTo`.
- Evita `await Task.Delay` en handlers de UI; usa helpers no bloqueantes.
- Cachea vistas pesadas (WebView).


## 7) Roadmap / Próximos pasos
- Migrar botones del sidebar de `Frame` → `Border` para aligerar UI.
- Reemplazar pragmas CA1416 por guards de plataforma.
- Convertir contenedor lateral de Productivity a `CollectionView` para virtualización.
- Pruebas de accesibilidad (contraste, focus visible, navegación teclado).


## 8) Referencias
- `Docs/FOLDER_STRUCTURE.md`
- `Docs/GuiaColores.md`
- `Docs/GuiaDeColoresYEstilos.md`
- `Docs/Guia_Creacion_Tools.md`
- `Docs/Guia_Creacion_Tools_y_Categorias.md`
- Código fuente (este repo)
