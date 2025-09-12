# Guía de Colores y Temas — Red Nacho ToolBox

Esta guía explica cómo funciona el sistema de colores/temas en la app, qué significa cada clave (token) y cómo cambiarlas de manera segura.

Funciona con dos fuentes:
- Defaults en XAML para el tema claro: `Resources/Styles/Colors.xaml`.
- Aplicación dinámica de colores en tiempo de ejecución para Claro/Oscuro: `SettingsPage.xaml.cs` (`ApplyThemeColors()` y `ApplyThemeColorsStatic()`).

Importante: En .NET MAUI no se puede cambiar `ResourceDictionary.Source` en tiempo de ejecución. Por eso la app aplica colores directamente sobre `Application.Current.Resources` (ver `SettingsPage.xaml.cs`).

---

## Dónde están definidos los colores

1) Defaults (tema claro por defecto)
- Archivo: `RedNachoToolbox/RedNachoToolbox/Resources/Styles/Colors.xaml`
- Contiene la lista base de `Color x:Key="..."` usada por `DynamicResource` en XAML.
- Estos valores se sobrescriben dinámicamente al cambiar de tema.

2) Aplicación dinámica de tema (Claro/Oscuro)
- Archivo: `RedNachoToolbox/RedNachoToolbox/SettingsPage.xaml.cs`
- Métodos:
  - `ApplyThemeColors(bool isDarkTheme)`: Aplica colores cuando el usuario cambia de tema en runtime.
  - `ApplyThemeColorsStatic(bool isDarkTheme, ResourceDictionary resources)`: Aplica colores en el arranque.
  - `PropagateThemeKeys(...)`: Garantiza que los tokens importantes queden sincronizados entre diccionarios de recursos.

3) Uso en XAML
- Se consumen con `DynamicResource` para ser tema-sensibles:
```xml
<TextColor="{DynamicResource TextColor}" />
<BackgroundColor="{DynamicResource CardBackgroundColor}" />
<CancelButtonColor="{DynamicResource InteractivePrimaryColor}" />
```

---

## Cómo cambiar colores

### A) Cambiar para el tema claro (por defecto)
- Edita directamente `Resources/Styles/Colors.xaml`.
- Ejemplo: cambiar el fondo de página (claro)
```xml
<Color x:Key="PageBackgroundColor">#F5F5F5</Color>
```

### B) Cambiar para el tema oscuro
- Actualiza la rama `isDarkTheme == true` en `ApplyThemeColors()` y `ApplyThemeColorsStatic()` dentro de `SettingsPage.xaml.cs`.
- Busca la clave a modificar y actualiza su valor para oscuro.

### C) Cambiar en ambos temas
- Actualiza el valor en `Colors.xaml` (para el default claro) y en ambas ramas de `ApplyThemeColors(...)` y `ApplyThemeColorsStatic(...)`.

### D) Añadir un nuevo token (clave de color)
1. Agrega la clave en `Resources/Styles/Colors.xaml` con un valor por defecto (claro).
2. En `SettingsPage.xaml.cs`, añade la asignación para la clave en las ramas de Claro y Oscuro en ambos métodos (`ApplyThemeColors` y `ApplyThemeColorsStatic`).
3. Si quieres propagar inmediatamente ese recurso a todos los diccionarios, añade el nombre de la clave en las llamadas a `PropagateThemeKeys(...)`.
4. Úsalo en XAML con `DynamicResource`.

### E) Buenas prácticas
- Evita hex "hardcoded" en XAML. Usa siempre `DynamicResource` (por ejemplo, `{DynamicResource PrimaryRed}`).
- Mantén las mismas claves en Claro/Oscuro para que la UI reaccione de forma consistente.
- Para cambios de foco/hover, usa los tokens interactivos, no colores arbitrarios.

---

## Significado de cada valor (tokens)

### 1) Marca
- `PrimaryRed`: Rojo de marca (puntos/cápsulas de indicador, acentos puntuales).
- `BrandPrimaryColor`: Alias semántico del color de marca.

### 2) Superficies (fondos y contenedores)
- `PageBackgroundColor`: Fondo del área principal (fuera de tarjetas/sidebars).
- `SidebarBackgroundColor`: Fondo del sidebar.
- `CardBackgroundColor`: Fondo de tarjetas y contenedores principales.
- `CardAccentBackgroundColor`: Acento de tarjeta (sub-superficie muy sutil).
- `ContentBackgroundColor`: Fondo para áreas de contenido interno.
- `CardShadowColor`: Color de sombra para dar elevación.
- `BorderColorLight` / `BorderColorMedium` / `BorderColorDark`: Colores de borde para separar secciones o contenedores.

### 3) Tipografía
- `TextColor`: Texto primario (máximo contraste).
- `TextColorSecondary`: Texto secundario (subtítulos, metadatos suaves).
- `TextColorTertiary`: Texto terciario / placeholder.
- `TextColorDisabled`: Texto deshabilitado.

### 4) Botones (secundarios)
- `ButtonBackgroundColor`: Fondo de botón secundario / neutrales.
- `ButtonTextColor`: Texto del botón secundario.
- `ButtonBorderColor`: Borde del botón secundario.
- `ButtonHoverBackgroundColor`: Fondo al pasar el mouse.
- `ButtonPressedBackgroundColor`: Fondo al presionar.

### 5) Botones Primarios (acción principal)
- `PrimaryButtonBackgroundColor`: Fondo del botón primario (azul interactivo en claro; azul desaturado en oscuro).
- `PrimaryButtonTextColor`: Texto del botón primario.
- `PrimaryButtonHoverBackgroundColor`: Fondo en hover del primario.
- `PrimaryButtonPressedBackgroundColor`: Fondo en pressed del primario.

### 6) Controles de entrada (inputs)
- `InputBackgroundColor`: Fondo de entradas (Entry, SearchBar, etc.).
- `InputBorderColor`: Borde normal de inputs.
- `InputTextColor`: Color del texto en inputs.
- `InputPlaceholderColor`: Placeholder de inputs (terciario).
- `InputFocusBorderColor`: Borde en foco (interactivo azul).

### 7) Estados (status)
- `SuccessColor`: Confirmación/éxito.
- `WarningColor`: Advertencia.
- `ErrorColor`: Error (usa rojo de material en claro y material/brand en oscuro según tema).
- `InfoColor`: Información.

### 8) Selección & Hover
- `SelectionColor`: Color de realce/selección (azul interactivo).
- `SelectionBackgroundColor`: Fondo del elemento seleccionado (azul muy claro en claro).
- `HighlightColor`: Overlay de hover (bajo alfa para sutilidad).
- `HoverOverlayColor`: Alias semántico para overlays de hover.

### 9) Navegación (sidebar y estados activos)
- `NavigationBackgroundColor`: Fondo de navegación.
- `NavigationTextColor`: Texto de navegación.
- `NavigationSelectedBackgroundColor`: Fondo del ítem activo (claro: azul muy claro).
- `NavigationSelectedTextColor`: Texto del ítem activo.
- `NavigationHoverBackgroundColor`: Fondo al pasar el mouse en navegación.

### 10) Ajustes (Settings)
- `SettingsBackgroundColor`, `SettingsBorderColor`, `SettingsTextColor`, `SettingsSecondaryTextColor`: Tokens específicos para la vista de Ajustes.

### 11) Tokens semánticos interactivos/adicionales
- `InteractivePrimaryColor`: Azul principal para acciones interactivas (cancel button, resaltado activo en etiquetas, etc.).
- `InteractiveSecondaryColor`: Variante secundaria del azul.
- `BorderInteractiveColor`: Borde interactivo (focus de inputs).
- `TextInteractiveColor`: Texto sobre fondo interactivo (contraste adecuado).
- `IconPrimaryColor`: Color base de iconos.
- `IconInteractiveColor`: Color de icono en estado interactivo.
- `DisabledBackgroundColor`: Fondo para elementos deshabilitados.

---

## Ejemplos de cambios comunes

### Cambiar el color de marca (PrimaryRed)
1) En `Resources/Styles/Colors.xaml` ajusta:
```xml
<Color x:Key="PrimaryRed">#D32F2F</Color>
```
2) En `SettingsPage.xaml.cs` cambia los valores de `resources["PrimaryRed"] = ...` en Claro/Oscuro dentro de:
- `ApplyThemeColors(bool isDarkTheme)`
- `ApplyThemeColorsStatic(bool isDarkTheme, ResourceDictionary resources)`

### Cambiar el azul interactivo global
1) En `Colors.xaml` (claro):
```xml
<Color x:Key="InteractivePrimaryColor">#1976D2</Color>
<Color x:Key="InteractiveSecondaryColor">#0288D1</Color>
<Color x:Key="BorderInteractiveColor">#2196F3</Color>
```
2) En `SettingsPage.xaml.cs` define los equivalentes para Claro/Oscuro en ambos métodos.
3) En XAML, usa `{DynamicResource InteractivePrimaryColor}` donde aplique (por ejemplo, `SearchBar.CancelButtonColor`).

### Añadir un nuevo token
1) Agregar a `Colors.xaml`:
```xml
<Color x:Key="MyNewAccentBackground">#FFEEDD</Color>
```
2) Añadir al set de Claro/Oscuro en `ApplyThemeColors(...)` y `ApplyThemeColorsStatic(...)`:
```csharp
resources["MyNewAccentBackground"] = Color.FromArgb("#FFEEDD"); // Claro
resources["MyNewAccentBackground"] = Color.FromArgb("#3A2A25"); // Oscuro (ejemplo)
```
3) Añadir la clave a `PropagateThemeKeys(...)` si necesitas que se propague inmediatamente tras el cambio de tema.
4) Consumir en XAML:
```xml
<Border BackgroundColor="{DynamicResource MyNewAccentBackground}" />
```

---

## Cómo probar los cambios
- Compila: desde la raíz del repo ejecuta `dotnet build` o usa tu IDE.
- Cambia de tema: `Settings` -> "Dark Mode" (Switch) para ver cómo reaccionan los componentes.
- Revisa:
  - Fondos y bordes de tarjetas, sidebar y página.
  - Tipografía (contraste correcto).
  - Estados de hover/pressed/selected.
  - Entradas (borde en foco azul interactivo).
  - Navegación: ítem activo (texto azul interactivo y cápsulas/puntos rojos de marca, según diseño).

---

## Pitfalls (cosas a evitar)
- No usar `ResourceDictionary.Source` en runtime (limitación MAUI). Usa siempre las funciones de `SettingsPage.xaml.cs` para aplicar colores.
- No dejar hex valores fijos en vistas. Reemplázalos por `{DynamicResource ...}`.
- No olvidar el tema oscuro: cualquier clave nueva debe tener su equivalente para Claro y Oscuro.

---

## Referencias rápidas
- Defaults del tema claro: `Resources/Styles/Colors.xaml`.
- Aplicación de colores (runtime): `SettingsPage.xaml.cs` → `ApplyThemeColors()` y `ApplyThemeColorsStatic()`.
- Propagación de claves: `SettingsPage.xaml.cs` → `PropagateThemeKeys(...)`.
- Ejemplos de uso en vistas: `MainPage.xaml`, `Views/DashboardView.xaml`, `Views/AllToolsView.xaml`.

Si necesitas que cree un set de variables para un nuevo componente (p. ej., banners, chips, badges), indícame el componente y te agrego los tokens adecuados para Claro/Oscuro.
