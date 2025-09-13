# Red Nacho ToolBox - Scalable Folder Structure

This document describes the scalable folder structure implemented for the Red Nacho ToolBox .NET MAUI application. This organization ensures maintainability, scalability, and follows MVVM architectural patterns.

## 📁 Project Structure Overview

```
RedNachoToolbox/
├── 📁 Models/                    # Data models and business entities
│   ├── ToolCategory.cs          # Enum for tool categories
│   ├── ToolInfo.cs              # Main tool data model
│   └── ToolCollection.cs        # Collection management for tools
├── 📁 Views/                     # XAML pages and user interface
│   ├── MainPage.xaml            # Main application page
│   ├── MainPage.xaml.cs         # Main page code-behind
│   ├── AppShell.xaml            # Application shell navigation
│   └── AppShell.xaml.cs         # Shell code-behind
├── 📁 ViewModels/               # MVVM ViewModels
│   ├── BaseViewModel.cs         # Base class for all ViewModels
│   └── MainViewModel.cs         # ViewModel for main page
├── 📁 Services/                 # Business logic and data services
│   ├── INavigationService.cs    # Navigation service interface
│   └── IToolManagementService.cs # Tool management service interface
├── 📁 Tools/                    # Individual tool implementations
│   ├── 📁 Base/                 # Base classes for tools
│   │   ├── IToolPage.cs         # Interface for tool pages
│   │   └── BaseToolPage.cs      # Base class for tool pages
│   └── 📁 Calculator/           # Example calculator tool
│       ├── CalculatorPage.xaml  # Calculator UI
│       ├── CalculatorPage.xaml.cs # Calculator code-behind
│       └── CalculatorViewModel.cs # Calculator ViewModel
├── 📁 Converters/               # XAML value converters
│   ├── InvertedBoolConverter.cs # Boolean inversion converter
│   └── IsZeroConverter.cs       # Zero value checker converter
├── 📁 Resources/                # Application resources
│   ├── 📁 Styles/               # XAML styles and themes
│   │   ├── Colors.xaml          # Color definitions
│   │   └── Styles.xaml          # UI styles
│   ├── 📁 Images/               # Image resources
│   ├── 📁 Fonts/                # Font resources
│   └── 📁 AppIcon/              # Application icons
├── 📁 Platforms/                # Platform-specific code
└── 📁 Properties/               # Project properties
```

## 🏗️ Architectural Patterns

### MVVM (Model-View-ViewModel)
- **Models**: Data structures and business entities (`Models/`)
- **Views**: XAML pages and UI components (`Views/`)
- **ViewModels**: Presentation logic and data binding (`ViewModels/`)

### Dependency Injection
- Services are registered in `MauiProgram.cs`
- ViewModels and Views use constructor injection
- Promotes loose coupling and testability

### Tool Plugin Architecture
- Each tool is self-contained in its own folder under `Tools/`
- Tools inherit from `BaseToolPage` and implement `IToolPage`
- Consistent interface for all tools

## 📋 Folder Descriptions

### `/Models`
Contains data models and business entities:
- **ToolCategory.cs**: Enum defining tool categories (Utilities, Development, etc.)
- **ToolInfo.cs**: Main model representing a tool with properties like Name, Description, IconPath, Category, and TargetType
- **ToolCollection.cs**: Collection management with filtering and search capabilities

### `/Views`
Contains XAML pages and user interface components:
- **MainPage.xaml**: Primary application page displaying available tools
- **AppShell.xaml**: Navigation shell configuration
- All views follow MVVM pattern with proper data binding

### `/ViewModels`
Contains presentation logic and data binding:
- **BaseViewModel.cs**: Base class providing common functionality (INotifyPropertyChanged, busy state, lifecycle methods)
- **MainViewModel.cs**: Manages main page functionality and tool collection
- All ViewModels inherit from BaseViewModel for consistency

### `/Services`
Contains business logic and data access services:
- **INavigationService.cs**: Interface for navigation management
- **IToolManagementService.cs**: Interface for tool registration and management
- Services are registered with dependency injection

### `/Tools`
Contains individual tool implementations:
- **Base/**: Common interfaces and base classes for all tools
  - **IToolPage.cs**: Interface defining tool contract
  - **BaseToolPage.cs**: Base implementation with common functionality
- **Calculator/**: Example tool implementation
  - Complete MVVM implementation with View, ViewModel, and code-behind
  - Demonstrates proper tool structure and patterns

### `/Converters`
Contains XAML value converters:
- **InvertedBoolConverter.cs**: Inverts boolean values for UI binding
- **IsZeroConverter.cs**: Checks if numeric values are zero
- Registered globally in App.xaml resources

### `/Resources`
Contains application resources organized by type:
- **Styles/**: XAML styles, colors, and themes
- **Images/**: Image assets and icons
- **Fonts/**: Custom fonts
- **AppIcon/**: Application icon resources

## 🔧 Implementation Guidelines

### Adding New Tools
1. Create a new folder under `Tools/` (e.g., `Tools/TextEditor/`)
2. Create the tool page inheriting from `BaseToolPage`
3. Implement the required properties: `ToolId`, `ToolName`, `ToolDescription`
4. Create a ViewModel inheriting from `BaseViewModel`
5. Design the XAML UI following MVVM patterns
6. Register the tool in the dependency injection container

### Naming Conventions
- **Folders**: PascalCase (e.g., `ViewModels`, `Calculator`)
- **Files**: PascalCase with appropriate suffix (e.g., `MainViewModel.cs`, `CalculatorPage.xaml`)
- **Namespaces**: Follow folder structure (`RedNachoToolbox.ViewModels`)
- **Classes**: PascalCase descriptive names
- **Properties**: PascalCase
- **Fields**: camelCase with underscore prefix (`_fieldName`)

### Code Organization Best Practices
1. **Single Responsibility**: Each class has one clear purpose
2. **Dependency Injection**: Use constructor injection for dependencies
3. **Interface Segregation**: Define clear interfaces for services
4. **Documentation**: Comprehensive XML documentation for all public members
5. **Async/Await**: Use async patterns for I/O operations
6. **Error Handling**: Implement proper exception handling and logging

## 🚀 Benefits of This Structure

### Scalability
- Easy to add new tools without affecting existing code
- Clear separation of concerns
- Modular architecture supports team development

### Maintainability
- Consistent patterns across the application
- Easy to locate and modify specific functionality
- Clear dependencies and relationships

### Testability
- Dependency injection enables easy unit testing
- ViewModels can be tested independently of UI
- Services can be mocked for testing

### Performance
- Efficient resource organization
- Lazy loading capabilities for tools
- Proper memory management with lifecycle methods

## 🔄 Future Enhancements

### Planned Additions
- **Plugins/**: Dynamic tool loading system
- **Themes/**: Advanced theming support
- **Localization/**: Multi-language support
- **Extensions/**: Extension methods and utilities
- **Tests/**: Unit and integration tests

### Migration Path
When adding new folders or restructuring:
1. Update this documentation
2. Update namespace references
3. Update dependency injection registrations
4. Test all navigation and binding

## 📝 Notes

- All classes implement proper nullable reference type support
- XAML files use proper data binding with `x:DataType`
- Resources are organized for easy theming and localization
- Platform-specific code is isolated in `Platforms/` folder

This structure provides a solid foundation for the Red Nacho ToolBox application that can scale from a simple tool collection to a comprehensive productivity suite.
