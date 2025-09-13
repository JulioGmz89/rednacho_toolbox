# Red Nacho ToolBox

Cross‑platform productivity toolkit built with .NET MAUI. Clean, modern UI with a collapsible sidebar, runtime theme switching, a dashboard with "Recently used" tools, and a solid MVVM architecture.


## ✨ Highlights

- Collapsible sidebar with theme‑aware branding and icons
- Dashboard with Recently Used (last 3 tools) and debounced search
- Runtime theme switching (light/dark) with persisted preferences
- Semantic color tokens applied via `DynamicResource`
- Active page indicators (brand red capsule and collapsed‑state dot)
- Example tool included: Markdown → PDF with live HTML preview (WebView2 on Windows)
- Multi‑platform targets: Windows, Android, iOS, macOS (Mac Catalyst)


## 🧱 Architecture

- Pattern: MVVM
- Key folders: `Models/`, `Views/`, `ViewModels/`, `Tools/`, `Converters/`, `Resources/Styles/`, `Docs/`
- Messaging: CommunityToolkit.Mvvm `WeakReferenceMessenger`
- Iconography: PNG assets with light/dark variants (no SVG resizetizer conflicts)

See full structure and rationale in:
- `RedNachoToolbox/FOLDER_STRUCTURE.md`


## 📚 Documentation

English:
- `RedNachoToolbox/Docs/Overview.en.md` — Overview, Architecture, Build/Publish, Troubleshooting
- `RedNachoToolbox/Docs/ColorsAndStylesGuide.en.md` — Color tokens, borders, typography, states
- `RedNachoToolbox/Docs/ToolsAndCategoriesGuide.en.md` — Create tools & categories, patterns, examples

Spanish:
- `RedNachoToolbox/Docs/Overview.md` — Documentación general
- `RedNachoToolbox/Docs/GuiaDeColoresYEstilos.md` — Colores y estilos
- `RedNachoToolbox/Docs/Guia_Creacion_Tools_y_Categorias.md` — Creación de tools y categorías
- `RedNachoToolbox/Docs/GuiaColores.md` — Guía de colores/temas (tokens y aplicación)
- `RedNachoToolbox/MejoraDiseñoUI.md` — Informe de rediseño de sistema de color


## 🚀 Quick Start

1) Prerequisites
- .NET 8 SDK
- MAUI workloads: `dotnet workload install maui`
- Visual Studio 2022 (Windows/macOS) or JetBrains Rider

2) Build & Run
- Open `RedNachoToolbox.sln`
- Select a target (Windows, Android, iOS, Mac Catalyst)
- Build and run from the IDE

CLI:
```bash
# Build Debug
dotnet build

# Build Release
dotnet build -c Release
```


## 📦 Publish (per platform)

Refer to `RedNachoToolbox/Docs/Overview.en.md` for complete commands and platform notes. Examples:

Windows (MSIX):
```bash
dotnet publish -f net8.0-windows10.0.19041.0 -c Release -p:WindowsPackageType=Msix
```

Android (APK/AAB):
```bash
# APK quick build
dotnet publish -f net8.0-android -c Release -p:AndroidPackageFormat=apk

# AAB for Play Store (with signing)
dotnet publish -f net8.0-android -c Release -p:AndroidPackageFormat=aab \
  -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=your.keystore \
  -p:AndroidSigningKeyAlias=alias -p:AndroidSigningKeyPass=password -p:AndroidSigningStorePass=password
```

iOS (.ipa on macOS):
```bash
dotnet publish -f net8.0-ios -c Release \
  -p:ArchiveOnBuild=true -p:CodesignKey="Apple Distribution: YOUR ORG" -p:CodesignProvision="YOUR PROFILE"
```

macOS (Mac Catalyst):
```bash
dotnet publish -f net8.0-maccatalyst -c Release
```


## ⚡ Performance Notes

- Non‑blocking click/hover feedback (avoid chaining `Task.Delay` on the UI thread)
- Use `FadeTo` (Opacity) instead of animating size (prevents layout thrash)
- Cached heavy views (e.g., WebView‑based) to reduce initialization cost
- Debounced search to avoid filtering on every keystroke


## 🔌 Tech & Packages

- .NET 8, .NET MAUI
- CommunityToolkit.Mvvm (MVVM, Messaging)
- Markdig (Markdown → HTML)
- HtmlRendererCore.PdfSharp + PdfSharpCore (PDF generation)
- WebView2 (Windows preview rendering)


## 🤝 Contributing

- Follow the MVVM structure and naming conventions (see `FOLDER_STRUCTURE.md`)
- Use color tokens via `DynamicResource`
- Keep UI feedback lightweight and non‑blocking
- Open an issue or PR with a clear description and reproduction steps


## 🪪 License

Specify your license of choice here (e.g., MIT). If absent, this project remains All Rights Reserved by the owner.
