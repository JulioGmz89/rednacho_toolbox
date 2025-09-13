# Red Nacho ToolBox - Estructura de Carpetas Escalable

Este documento describe la estructura de carpetas escalable implementada para la aplicación .NET MAUI Red Nacho ToolBox. Esta organización garantiza mantenibilidad, escalabilidad y sigue patrones de arquitectura MVVM.

## 📁 Resumen de la Estructura del Proyecto

```
RedNachoToolbox/
├── 📁 Models/                    # Modelos de datos y entidades de negocio
│   ├── ToolCategory.cs          # Enum para categorías de tools
│   ├── ToolInfo.cs              # Modelo principal de tool
│   └── ToolCollection.cs        # Gestión de colecciones de tools
├── 📁 Views/                     # Páginas XAML e interfaz de usuario
│   ├── MainPage.xaml            # Página principal de la aplicación
│   ├── MainPage.xaml.cs         # Code-behind de página principal
│   ├── AppShell.xaml            # Navegación tipo Shell
│   └── AppShell.xaml.cs         # Code-behind de Shell
├── 📁 ViewModels/               # ViewModels (MVVM)
│   ├── BaseViewModel.cs         # Clase base para todos los ViewModels
│   └── MainViewModel.cs         # ViewModel para la página principal
├── 📁 Services/                 # Lógica de negocio y servicios de datos
│   ├── INavigationService.cs    # Interfaz de servicio de navegación
│   └── IToolManagementService.cs # Interfaz de gestión de tools
├── 📁 Tools/                    # Implementaciones individuales de tools
│   ├── 📁 Base/                 # Clases base para tools
│   │   ├── IToolPage.cs         # Interfaz para páginas de tools
│   │   └── BaseToolPage.cs      # Clase base para páginas de tools
│   └── 📁 Calculator/           # Tool de ejemplo (calculadora)
│       ├── CalculatorPage.xaml  # UI de Calculadora
│       ├── CalculatorPage.xaml.cs # Code-behind de Calculadora
│       └── CalculatorViewModel.cs # ViewModel de Calculadora
├── 📁 Converters/               # Convertidores de valores para XAML
│   ├── InvertedBoolConverter.cs # Conversor de inversión booleana
│   └── IsZeroConverter.cs       # Conversor para verificar cero
├── 📁 Resources/                # Recursos de la aplicación
│   ├── 📁 Styles/               # Estilos y temas XAML
│   │   ├── Colors.xaml          # Definiciones de color
│   │   └── Styles.xaml          # Estilos de UI
│   ├── 📁 Images/               # Recursos de imágenes
│   ├── 📁 Fonts/                # Fuentes
│   └── 📁 AppIcon/              # Iconos de la aplicación
├── 📁 Platforms/                # Código específico de plataforma
└── 📁 Properties/               # Propiedades del proyecto
```

## 🏗️ Patrones de Arquitectura

### MVVM (Model-View-ViewModel)
- **Models**: Estructuras de datos y entidades de negocio (`Models/`)
- **Views**: Páginas XAML y componentes de UI (`Views/`)
- **ViewModels**: Lógica de presentación y data binding (`ViewModels/`)

### Inyección de Dependencias
- Los servicios se registran en `MauiProgram.cs`
- ViewModels y Views usan inyección por constructor
- Fomenta bajo acoplamiento y testabilidad

### Arquitectura de Plugins de Tools
- Cada tool es autocontenida en su propia carpeta bajo `Tools/`
- Los tools heredan de `BaseToolPage` e implementan `IToolPage`
- Interfaz consistente para todos los tools

## 📋 Descripciones de Carpetas

### `/Models`
Contiene modelos de datos y entidades de negocio:
- **ToolCategory.cs**: Enum que define categorías de tools (Utilities, Development, etc.)
- **ToolInfo.cs**: Modelo principal de tool con propiedades como Name, Description, IconPath, Category y TargetType
- **ToolCollection.cs**: Gestión de colecciones con filtrado y capacidades de búsqueda

### `/Views`
Contiene páginas XAML y componentes de la interfaz de usuario:
- **MainPage.xaml**: Página principal que muestra los tools disponibles
- **AppShell.xaml**: Configuración de navegación con Shell
- Todas las vistas siguen MVVM con data binding apropiado

### `/ViewModels`
Contiene lógica de presentación y data binding:
- **BaseViewModel.cs**: Clase base con funcionalidad común (INotifyPropertyChanged, estados de ocupado, métodos de ciclo de vida)
- **MainViewModel.cs**: Administra la funcionalidad de la página principal y la colección de tools
- Todos los ViewModels heredan de BaseViewModel para consistencia

### `/Services`
Contiene lógica de negocio y servicios de acceso a datos:
- **INavigationService.cs**: Interfaz para gestión de navegación
- **IToolManagementService.cs**: Interfaz para registro y gestión de tools
- Los servicios se registran con inyección de dependencias

### `/Tools`
Contiene implementaciones individuales de tools:
- **Base/**: Interfaces y clases base comunes
  - **IToolPage.cs**: Interfaz que define el contrato del tool
  - **BaseToolPage.cs**: Implementación base con funcionalidad común
- **Calculator/**: Implementación de un tool de ejemplo
  - Implementación MVVM completa con View, ViewModel y code-behind
  - Demuestra estructura y patrones adecuados

### `/Converters`
Contiene convertidores de valores para XAML:
- **InvertedBoolConverter.cs**: Invierte valores booleanos para binding en UI
- **IsZeroConverter.cs**: Verifica si un valor numérico es cero
- Registrados globalmente en los recursos de App.xaml

### `/Resources`
Contiene recursos de la aplicación organizados por tipo:
- **Styles/**: Estilos, colores y temas XAML
- **Images/**: Activos de imagen e íconos
- **Fonts/**: Fuentes personalizadas
- **AppIcon/**: Recursos de ícono de aplicación

## 🔧 Lineamientos de Implementación

### Agregar Nuevos Tools
1. Crea una nueva carpeta en `Tools/` (por ejemplo, `Tools/TextEditor/`)
2. Crea la página del tool heredando de `BaseToolPage`
3. Implementa las propiedades requeridas: `ToolId`, `ToolName`, `ToolDescription`
4. Crea un ViewModel que herede de `BaseViewModel`
5. Diseña la UI XAML siguiendo patrones MVVM
6. Registra el tool en el contenedor de inyección de dependencias

### Convenciones de Nombres
- **Carpetas**: PascalCase (p. ej., `ViewModels`, `Calculator`)
- **Archivos**: PascalCase con sufijo apropiado (p. ej., `MainViewModel.cs`, `CalculatorPage.xaml`)
- **Namespaces**: Siguen la estructura de carpetas (`RedNachoToolbox.ViewModels`)
- **Clases**: Nombres descriptivos en PascalCase
- **Propiedades**: PascalCase
- **Campos**: camelCase con prefijo de guion bajo (`_miCampo`)

### Buenas Prácticas de Organización de Código
1. **Responsabilidad Única**: Cada clase con un propósito claro
2. **Inyección de Dependencias**: Usa inyección por constructor
3. **Segregación de Interfaces**: Define interfaces claras para los servicios
4. **Documentación**: XML docs para miembros públicos
5. **Async/Await**: Usa patrones async para operaciones de E/S
6. **Manejo de Errores**: Excepciones y logging adecuados

## 🚀 Beneficios de esta Estructura

### Escalabilidad
- Fácil agregar nuevos tools sin afectar código existente
- Separación clara de responsabilidades
- Arquitectura modular soporta desarrollo en equipo

### Mantenibilidad
- Patrones consistentes en toda la aplicación
- Fácil localizar y modificar funcionalidad específica
- Dependencias y relaciones claras

### Testeabilidad
- Inyección de dependencias facilita pruebas unitarias
- ViewModels testeables independientemente de la UI
- Servicios pueden ser mockeados para testing

### Rendimiento
- Organización eficiente de recursos
- Capacidades de carga diferida (lazy) para tools
- Gestión de memoria adecuada con métodos de ciclo de vida

## 🔄 Mejoras Futuras

### Incorporaciones Planeadas
- **Plugins/**: Sistema de carga dinámica de tools
- **Themes/**: Soporte avanzado de temas
- **Localization/**: Multi‑idioma
- **Extensions/**: Métodos de extensión y utilidades
- **Tests/**: Pruebas unitarias e integración

### Ruta de Migración
Al añadir nuevas carpetas o reestructurar:
1. Actualiza esta documentación
2. Actualiza referencias de namespaces
3. Actualiza registros en el contenedor de inyección
4. Prueba toda la navegación y bindings

## 📝 Notas

- Todas las clases soportan correctamente tipos anulables (nullable reference types)
- Archivos XAML usan data binding apropiado con `x:DataType`
- Recursos organizados para facilitar temas y localización
- Código específico de plataforma aislado en la carpeta `Platforms/`

Esta estructura provee una base sólida para Red Nacho ToolBox que puede escalar desde una colección simple de tools hasta una suite de productividad completa.
