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

### 3.1. Gestión de Temas (Light/Dark Mode)

La solución actual utiliza AppThemeBinding y recursos en `Resources/Styles/Colors.xaml` y `Resources/Styles/Styles.xaml` para soportar modo claro/oscuro sin cambiar diccionarios en tiempo de ejecución.

Pasos recomendados:

1. Definir colores temáticos con `AppThemeBinding` en `Colors.xaml`.
2. Referenciar colores desde `Styles.xaml` y la UI con `{DynamicResource}`.
3. (Opcional) Permitir que el usuario fuerce el tema estableciendo `Application.Current.UserAppTheme`.

Ejemplo XAML en `Colors.xaml`:

```xaml
<!-- Colors.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Color x:Key="PageBackgroundColor">
        <AppThemeColor Light="#FFFFFF" Dark="#1E1E1E" />
    </Color>
    <Color x:Key="SidebarBackgroundColor">
        <AppThemeColor Light="#F5F5F5" Dark="#2A2A2A" />
    </Color>
    <Color x:Key="CardBorderColor">
        <AppThemeColor Light="#DDDDDD" Dark="#3A3A3A" />
    </Color>
</ResourceDictionary>
```

Uso en XAML (por ejemplo en `MainPage.xaml`):

```xaml
<ContentPage BackgroundColor="{DynamicResource PageBackgroundColor}">
    <!-- contenido -->
</ContentPage>
```

Alternar tema desde C# sin recargar diccionarios:

```csharp
private bool isDarkMode;

private void OnChangeThemeClicked(object sender, EventArgs e)
{
    Application.Current!.UserAppTheme = isDarkMode ? AppTheme.Light : AppTheme.Dark;
    isDarkMode = !isDarkMode;
}
```

> Nota: todos los colores en la UI deben usar `{DynamicResource}` para que se actualicen automáticamente al cambiar el tema y reflejen el tema del sistema.

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

### Próximos Pasos

* Ajustar la UI principal para el layout final: sidebar fija a 250 px y área de contenido flexible (como en `Views/MainPage.xaml`).
* Consolidar temas en `Resources/Styles/Colors.xaml` y `Resources/Styles/Styles.xaml` usando `{DynamicResource}`.
* Integrar `ToolCategory` y `ToolCollection` en `MainViewModel` y bindings de `MainPage`.
* Registrar ViewModels y páginas en `MauiProgram.cs` vía DI.
* Registrar y validar la herramienta de ejemplo `Calculator` bajo `Tools/Calculator/`.
* Definir servicios de navegación y gestión de herramientas (`Services/`) y su registro cuando las implementaciones estén listas.

## Referencias

1. [What is .NET MAUI? - .NET MAUI | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui?view=net-maui-9.0) — acceso: 8 septiembre 2025.
2. [DevToys - A Swiss Army knife for developers](https://devtoys.app/) — acceso: 8 septiembre 2025.
3. [.NET MAUI - Card Views Using Border Control - C# Corner](https://www.c-sharpcorner.com/article/net-maui-card-views-using-border-control/) — acceso: 8 septiembre 2025.
4. [Dark/Light Theme in .NET MAUI - Dinesh Falwadiya - Medium](https://dineshphalwadiya1995.medium.com/dark-light-theme-in-net-maui-6a64b5a965a2) — acceso: 8 septiembre 2025.
5. [Switching MAUI Themes at Runtime - Grial UI Kit](https://grialkit.com/blog/switching-maui-themes-at-runtime) — acceso: 8 septiembre 2025.
6. [Theme an app - .NET MAUI | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/theming?view=net-maui-9.0) — acceso: 8 septiembre 2025.
