# Propuesta de Desarrollo: Red Nacho Toolbox

## Resumen Ejecutivo

Esta propuesta detalla la arquitectura, el diseño de la interfaz de usuario (UI) y el plan de implementación para la Red Nacho Toolbox, una aplicación de escritorio multiplataforma construida con .NET MAUI. El objetivo es crear un contenedor de herramientas robusto y extensible, inspirado en la funcionalidad y el diseño de DevToys. La fase inicial se centrará en establecer la estructura de navegación principal, la gestión de temas (claro/oscuro) y la implementación de una primera herramienta de marcador de posición.

## Índice

* [1. Pila tecnológica principal](#1-pila-tecnologica-principal)
* [2. Arquitectura de la aplicación y diseño de UI/UX](#2-arquitectura-de-la-aplicacion-y-diseno-de-uiux)
	+ [2.1. Estructura de la ventana principal](#21-estructura-de-la-ventana-principal)
	+ [2.2. Diseño de la barra lateral](#22-diseno-de-la-barra-lateral)
	+ [2.3. Vista "All Applications": Diseño de tarjetas](#23-vista-all-applications-diseno-de-tarjetas)
	+ [2.4. Arquitectura lógica (MVVM, DI y Shell)](#24-arquitectura-logica-mvvm-di-y-shell)
* [3. Plan de implementación de características](#3-plan-de-implementacion-de-caracteristicas)
	+ [3.1. Gestión de temas (Light/Dark Mode)](#31-gestion-de-temas-lightdark-mode)
	+ [3.2. Herramientas y modelo de datos](#32-herramientas-y-modelo-de-datos)
	+ [3.3. DI y registro de servicios y ViewModels](#33-di-y-registro-de-servicios-y-viewmodels)
* [4. Diseño visual y branding](#4-diseno-visual-y-branding)
* [5. Estructura del proyecto y próximos pasos](#5-estructura-del-proyecto-y-proximos-pasos)
* [Referencias](#referencias)

## 1. Pila Tecnológica Principal

* Framework: .NET MAUI
* Lenguaje: C# y XAML
* Entorno de Desarrollo: Visual Studio 2022

La elección de .NET MAUI permite un desarrollo eficiente al utilizar una única base de código para compilar aplicaciones nativas para Windows y macOS, garantizando un rendimiento óptimo y una apariencia fiel a la plataforma.

## 2. Arquitectura de la Aplicación y Diseño de UI/UX

La interfaz principal se inspirará en el diseño claro y funcional de DevToys, utilizando un patrón de navegación con barra lateral persistente.

### 2.1. Estructura de la Ventana Principal

La ventana principal se dividirá en dos columnas principales utilizando un `Grid`:

* Barra Lateral de Navegación (Izquierda): Ocupará una porción fija del ancho de la ventana (aprox. 250px) y contendrá los enlaces de navegación principales.
* Área de Contenido (Derecha): Ocupará el espacio restante y mostrará la herramienta seleccionada o la lista de herramientas.

Ejemplo de XAML para la estructura principal (`MainPage.xaml`):

```xaml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="250" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>

    <VerticalStackLayout Grid.Column="0" BackgroundColor="{DynamicResource SidebarBackgroundColor}">
        <!-- Contenido de la barra lateral -->
    </VerticalStackLayout>

    <ContentView Grid.Column="1" Content="{Binding CurrentToolView}" />
</Grid>
```

### 2.2. Diseño de la Barra Lateral

La barra lateral se organizará verticalmente y contendrá los siguientes elementos:

* Barra de Búsqueda: En la parte superior, un control `SearchBar` para filtrar las herramientas mostradas en el área de contenido.
* Categorías de Herramientas:
	+ All Applications: Botón o enlace que mostrará todas las herramientas disponibles.
	+ Documentation: Botón o enlace para herramientas relacionadas con la documentación.
* Configuración: En la parte inferior, un botón de "Settings" que navegará a la página de configuración de la aplicación.

### 2.3. Vista "All Applications": Diseño de Tarjetas

Cuando el usuario seleccione "All Applications", el área de contenido mostrará una cuadrícula de tarjetas, cada una representando una herramienta, utilizando un `CollectionView`.

* Diseño de Tarjeta: Cada tarjeta será un control `Border` o `Frame` para darle un aspecto definido con esquinas redondeadas y una sombra sutil. El contenido se dividirá con un `Grid` de dos columnas:
	+ Columna 1 (25-30%): `Image` con el ícono de la herramienta.
	+ Columna 2 (70-75%): `VerticalStackLayout` con el nombre (Label en negrita) y la descripción.

Ejemplo de XAML para la `CollectionView` y el `DataTemplate` de la tarjeta:

```xaml
<CollectionView ItemsSource="{Binding Tools}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Border Stroke="{DynamicResource CardBorderColor}"
                    StrokeThickness="1"
                    Padding="12"
                    Margin="8"
                    HeightRequest="100">
                <Border.StrokeShape>
                    <RoundRectangle CornerRadius="8"/>
                </Border.StrokeShape>

                <Grid ColumnDefinitions="0.3*, 0.7*">
                    <Image Grid.Column="0" Source="{Binding IconPath}" Aspect="AspectFit" />
                    <VerticalStackLayout Grid.Column="1" VerticalOptions="Center">
                        <Label Text="{Binding Name}" FontAttributes="Bold" />
                        <Label Text="{Binding Description}" FontSize="Small" />
                    </VerticalStackLayout>
                </Grid>
            </Border>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

### 2.4. Arquitectura lógica (MVVM, DI y Shell)

La solución adopta patrones modernos para asegurar mantenibilidad y escalabilidad:

- MVVM: separación de responsabilidades con `Models/`, `Views/` y `ViewModels/`.
  - `ViewModels/BaseViewModel.cs`: estado común (IsBusy, Title) y `INotifyPropertyChanged`.
  - `ViewModels/MainViewModel.cs`: orquesta la `ToolCollection` y la navegación a herramientas.
- AppShell: navegación declarativa en `Views/AppShell.xaml` y `AppShell.xaml.cs`.
- DI (Dependency Injection): servicios, ViewModels y páginas registrados en `MauiProgram.cs` para acoplamiento débil.
- Arquitectura de herramientas tipo plugin:
  - Base común en `Tools/Base/IToolPage.cs` y `Tools/Base/BaseToolPage.cs`.
  - Cada herramienta se encapsula en su carpeta (`Tools/Calculator/` como ejemplo), con su Page y ViewModel.

## 3. Plan de implementación de características

### 3.1. Gestión de Temas (Light/Dark Mode) - ✅ IMPLEMENTADO

**Estado**: Completamente implementado con funcionalidad avanzada de cambio de tema en tiempo real.

**Implementación Final**:

La aplicación utiliza un sistema de gestión de temas robusto que permite cambio instantáneo entre modo claro y oscuro con persistencia de preferencias del usuario. La implementación se basa en aplicación directa de colores a `Application.Current.Resources` en lugar de archivos XAML estáticos.

**Componentes Implementados**:

1. **SettingsPage.xaml**: Página de configuración con control Switch para cambio de tema
2. **SettingsPage.xaml.cs**: Lógica completa de gestión de temas con:
   - `ApplyTheme()`: Aplica colores de tema directamente a recursos de aplicación
   - `ApplyThemeColors()`: Define 40+ colores para cada tema (claro/oscuro)
   - `SaveThemePreference()`: Persiste preferencia del usuario
   - `LoadThemePreference()`: Carga preferencia guardada
   - `ApplySavedTheme()`: Aplica tema guardado al inicio de la aplicación
   - `IsCurrentlyDarkTheme()`: Detecta tema actual por análisis de colores

**Características Clave**:

- ✅ **Cambio instantáneo**: Los colores se actualizan inmediatamente sin reiniciar la app
- ✅ **Persistencia**: Las preferencias se guardan y restauran entre sesiones
- ✅ **Detección inteligente**: El switch siempre refleja el estado actual del tema
- ✅ **40+ colores definidos**: Cobertura completa para todos los elementos UI
- ✅ **Logging completo**: Debug detallado para troubleshooting
- ✅ **Manejo de errores**: Fallbacks robustos y mensajes de error amigables

**Colores de Tema Implementados**:

```csharp
// Tema Oscuro
PageBackgroundColor: #121212
SidebarBackgroundColor: #1E1E1E
TextColor: #FFFFFF
ButtonBackgroundColor: #3A3A3A
// ... 40+ colores más

// Tema Claro  
PageBackgroundColor: #FFFFFF
SidebarBackgroundColor: #F8F9FA
TextColor: #212529
ButtonBackgroundColor: #F8F9FA
// ... 40+ colores más
```

**Uso en XAML**:

```xaml
<ContentPage BackgroundColor="{DynamicResource PageBackgroundColor}">
    <Button BackgroundColor="{DynamicResource ButtonBackgroundColor}"
            TextColor="{DynamicResource ButtonTextColor}" />
</ContentPage>
```

**Navegación a Settings**:

```csharp
// En MainPage.xaml.cs
private async void OnSettingsClicked(object sender, EventArgs e)
{
    await Navigation.PushAsync(new SettingsPage());
}
```

> **Nota Técnica**: La implementación evita las limitaciones de .NET MAUI con `ResourceDictionary.Source` aplicando colores directamente a `Application.Current.Resources`, lo que garantiza compatibilidad cross-platform y rendimiento óptimo.

### 3.2. Herramientas y modelo de datos

Para gestionar las herramientas de forma dinámica, se utilizan modelos fuertemente tipados y colecciones observables:

- `Models/ToolCategory.cs`: enum con categorías predefinidas (Utilities, Development, System, Network, FileManagement, Security, Productivity, Media).
- `Models/ToolInfo.cs`: modelo principal de herramienta con `INotifyPropertyChanged` para binding en MAUI.
- `Models/ToolCollection.cs`: gestión de colecciones, filtrado y búsqueda lista para enlazar a la UI.

Ejemplo de `ToolInfo` (simplificado):

```csharp
public class ToolInfo : INotifyPropertyChanged
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconPath { get; set; } = string.Empty;
    public ToolCategory Category { get; set; }
    public Type? TargetType { get; set; } // Tipo de la Page de la herramienta

    public event PropertyChangedEventHandler? PropertyChanged;
}
```

- Herramienta de ejemplo: `Tools/Calculator/` con `CalculatorPage.xaml` y `CalculatorViewModel.cs`.
- Enlace a UI: `ToolCollection` se usa como `ItemsSource` de la `CollectionView` y se filtra por `ToolCategory`.

```csharp
// En MainViewModel
public ToolCollection Tools { get; } = new();
public IReadOnlyList<ToolInfo> FilteredTools => Tools.FilterByCategory(SelectedCategory);
```

### 3.3. DI y registro de servicios y ViewModels

Registrar servicios, ViewModels y páginas en `MauiProgram.cs` para habilitar navegación y resolución de dependencias:

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
    });

// Servicios (interfaces definidas en Services/)
// builder.Services.AddSingleton<INavigationService, NavigationService>();
// builder.Services.AddSingleton<IToolManagementService, ToolManagementService>();

// ViewModels
builder.Services.AddSingleton<MainViewModel>();
builder.Services.AddTransient<Tools.Calculator.CalculatorViewModel>();

// Pages / Views
builder.Services.AddSingleton<Views.MainPage>();
builder.Services.AddTransient<Tools.Calculator.CalculatorPage>();

return builder.Build();
```

## 4. Diseño visual y branding

* Color principal: el color `#CC333B` se definirá en `App.xaml` como un recurso estático para ser utilizado en toda la aplicación (por ejemplo: resaltar el elemento activo en la barra lateral, botones principales y otros elementos de la marca).

```xaml
<Application.Resources>
    <ResourceDictionary>
        <Color x:Key="PrimaryRed">#CC333B</Color>
        <!-- Otros recursos -->
    </ResourceDictionary>
</Application.Resources>
```

## 5. Estructura del Proyecto y Próximos Pasos

Se adopta una estructura escalable alineada con MVVM, DI y una arquitectura de herramientas modular:

```text
RedNachoToolbox/
├─ Models/
│  ├─ ToolCategory.cs
│  ├─ ToolInfo.cs
│  └─ ToolCollection.cs
├─ Views/
│  ├─ MainPage.xaml
│  ├─ MainPage.xaml.cs
│  ├─ AppShell.xaml
│  └─ AppShell.xaml.cs
├─ ViewModels/
│  ├─ BaseViewModel.cs
│  └─ MainViewModel.cs
├─ Services/
│  ├─ INavigationService.cs
│  └─ IToolManagementService.cs
├─ Tools/
│  ├─ Base/
│  │  ├─ IToolPage.cs
│  │  └─ BaseToolPage.cs
│  └─ Calculator/
│     ├─ CalculatorPage.xaml
│     ├─ CalculatorPage.xaml.cs
│     └─ CalculatorViewModel.cs
├─ Converters/
│  ├─ InvertedBoolConverter.cs
│  └─ IsZeroConverter.cs
├─ Resources/
│  └─ Styles/
│     ├─ Colors.xaml
│     └─ Styles.xaml
├─ Platforms/
└─ Properties/
```

### Epic 3: Gestión de Temas - ✅ COMPLETADO

**Fecha de Finalización**: Septiembre 2025

**Resumen de Implementación**:

El Epic 3 se centró en implementar un sistema completo de gestión de temas (Light/Dark Mode) con las siguientes características:

**Archivos Implementados**:
- ✅ `SettingsPage.xaml` - Interfaz de configuración con switch de tema
- ✅ `SettingsPage.xaml.cs` - Lógica completa de gestión de temas
- ✅ Navegación desde `MainPage` a `SettingsPage`
- ✅ Integración con `App.xaml.cs` para aplicación de tema al inicio

**Funcionalidades Logradas**:
- ✅ Cambio instantáneo de tema sin reinicio de aplicación
- ✅ Persistencia de preferencias del usuario entre sesiones
- ✅ Detección inteligente del tema actual
- ✅ 40+ colores definidos para cobertura completa de UI
- ✅ Logging detallado para debugging y troubleshooting
- ✅ Manejo robusto de errores con fallbacks

**Desafíos Técnicos Resueltos**:
- ❌ **Problema**: "Source can only be set from XAML" - Limitación de .NET MAUI
- ✅ **Solución**: Aplicación directa de colores a `Application.Current.Resources`
- ❌ **Problema**: Switch no sincronizado al re-entrar a Settings
- ✅ **Solución**: Detección de tema por análisis de colores aplicados

**Archivos Limpiados**:
- 🗑️ Eliminados: `DarkTheme.xaml`, `LightTheme.xaml` (ya no necesarios)
- 🔧 Actualizado: `App.xaml` (removida referencia a archivos XAML de tema)

### Próximos Pasos

**Epic 4: Herramientas y Funcionalidad Core**:
* Integrar `ToolCategory` y `ToolCollection` en `MainViewModel` y bindings de `MainPage`
* Registrar ViewModels y páginas en `MauiProgram.cs` vía DI
* Implementar herramientas de ejemplo bajo `Tools/` (Calculator, etc.)
* Definir servicios de navegación y gestión de herramientas (`Services/`)

**Mejoras de UI/UX**:
* Implementar animaciones suaves para transiciones de tema
* Agregar más opciones de configuración en SettingsPage
* Mejorar feedback visual durante cambios de tema

**Optimizaciones**:
* Implementar lazy loading para herramientas
* Agregar tests unitarios para gestión de temas
* Optimizar rendimiento de aplicación de colores

## Referencias

1. [What is .NET MAUI? - .NET MAUI | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui?view=net-maui-9.0) — acceso: 8 septiembre 2025.
2. [DevToys - A Swiss Army knife for developers](https://devtoys.app/) — acceso: 8 septiembre 2025.
3. [.NET MAUI - Card Views Using Border Control - C# Corner](https://www.c-sharpcorner.com/article/net-maui-card-views-using-border-control/) — acceso: 8 septiembre 2025.
4. [Dark/Light Theme in .NET MAUI - Dinesh Falwadiya - Medium](https://dineshphalwadiya1995.medium.com/dark-light-theme-in-net-maui-6a64b5a965a2) — acceso: 8 septiembre 2025.
5. [Switching MAUI Themes at Runtime - Grial UI Kit](https://grialkit.com/blog/switching-maui-themes-at-runtime) — acceso: 8 septiembre 2025.
6. [Theme an app - .NET MAUI | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/theming?view=net-maui-9.0) — acceso: 8 septiembre 2025.
