# Guía de Colores y Estilos (Tokens, Bordes, Tipografía, Estados)

Esta guía complementa `Docs/GuiaColores.md` y formaliza cómo usar colores y estilos en la UI. Cubre tokens de color, estilos recomendados (bordes, radios, sombras), tipografía, iconografía, y estados (hover/pressed/focus/selected) en .NET MAUI.

> Para comprender el sistema de tokens y cómo se aplican en runtime (limitación de MAUI con `ResourceDictionary.Source`), lee primero `Docs/GuiaColores.md`.

---

## 1) Principios Generales

- Usa siempre `DynamicResource` para colores en XAML.
- Evita valores hex “hardcoded”. Si necesitas un color nuevo, crea un token.
- Mantén equivalencia entre Claro/Oscuro en `SettingsPage.ApplyThemeColors*`.
- Prefiere `Border` en lugar de `Frame` cuando solo necesitas fondo/borde/radio.
- Para animaciones, prefiere `Opacity` (`FadeTo`) sobre cambiar `WidthRequest/HeightRequest`.

---

## 2) Tokens de Color – Recomendaciones de uso

Los tokens ya definidos están documentados en `Docs/GuiaColores.md`. Algunas recomendaciones prácticas:

- `PageBackgroundColor`: Fondo de toda la página (columna derecha principal en `MainPage.xaml`).
- `SidebarBackgroundColor`: Fondo del sidebar (columna izquierda).
- `CardBackgroundColor`: Fondo de tarjetas / contenedores.
- `BorderColor*`: Líneas divisorias y contornos sutiles.
- `TextColor*`: Tipografía (primario/secundario/terciario).
- `Navigation*`/`Settings*`: TONOS específicos para esas vistas.
- `Interactive*`: Azules de interacción (hover, focus, primario, etc.).
- `PrimaryRed`: Acento de marca (indicadores). Úsalo con moderación y significado.

Ejemplo en XAML:
```xml
<Border BackgroundColor="{DynamicResource CardBackgroundColor}"
        Stroke="{DynamicResource BorderColorLight}"
        StrokeThickness="1"
        StrokeShape="RoundRectangle 8">
    <Label Text="Título" TextColor="{DynamicResource TextColor}" />
</Border>
```

---

## 3) Estilos de Bordes, Radios y Sombras

- Radio de esquina recomendado: 8 px (coherente en tarjetas/botones).
- Sombras: En MAUI/WinUI el sombreado pesado incrementa costo de render. Preferir contraste por superficie y bordes sutiles.
- `Frame` vs `Border`:
  - `Frame` añade costos extra (sombra). Si usas `Frame`, desactiva sombras (`HasShadow="False"`).
  - `Border` es más ligero y suficiente para la mayoría de contenedores/botones.

Ejemplo de “botón” con `Border` y `TapGestureRecognizer`:
```xml
<Border x:Name="MyButton"
        BackgroundColor="Transparent"
        StrokeThickness="0"
        StrokeShape="RoundRectangle 8"
        Padding="12,8">
    <Border.GestureRecognizers>
        <TapGestureRecognizer Tapped="OnMyButtonTapped" />
    </Border.GestureRecognizers>
    <Grid ColumnDefinitions="Auto,*" ColumnSpacing="12">
        <Image Source="grid_outline_black.png" WidthRequest="20" HeightRequest="20"/>
        <Label Grid.Column="1" Text="Dashboard" TextColor="{DynamicResource TextColor}" />
    </Grid>
</Border>
```

---

## 4) Tipografía y Espaciado

- Tamaños de fuente: usa los estilos de `Resources/Styles/Styles.xaml` (si existen) o define `Style` compartidos.
- Colores de texto: `TextColor`, `TextColorSecondary`, `TextColorTertiary`.
- Jerarquía: títulos (primario), descripciones (secundario).
- Espaciado sugerido: múltiplos de 4 (4/8/12/16/24). Sidebar y tarjetas usan 12–16 px internos.

---

## 5) Iconografía y Tema

Tienes dos formas de resolver iconos Light/Dark:

1) AppThemeBinding inline (simple y directo):
```xml
<Image Source="{AppThemeBinding Light=grid_outline_black.png, Dark=grid_outline_white.png}" />
```

2) `ThemeIconConverter` (MultiBinding, cuando el nombre base cambia por tema):
```xml
<Image>
  <Image.Source>
    <MultiBinding Converter="{StaticResource ThemeIconConverter}">
      <Binding Path="DashboardIconBase" />
      <Binding Path="IsDarkTheme" />
    </MultiBinding>
  </Image.Source>
</Image>
```

- Coloca PNGs en `Resources/Images/` (no duplicar nombres con `Resources/Vector/`).
- Nomenclatura: `{base}_black.png` para claro, `{base}_white.png` para oscuro.

---

## 6) Estados (Hover/Pressed/Focus/Selected)

- Estados con color: usa tokens (`ButtonHoverBackgroundColor`, etc.) o tonos neutros.
- Evita bloquear UI con `await Task.Delay` en handlers. Usa helpers que no detengan la navegación.
- Para accesibilidad, agrega estado de `focus` visible en inputs/botones (borde azul interactivo).

Ejemplo simple de feedback no bloqueante (code-behind):
```csharp
private async Task PressFeedbackAsync(Border border)
{
    var hover = (Color)Application.Current.Resources["NavigationHoverBackgroundColor"];
    border.BackgroundColor = hover;
    await Task.Delay(80);
    border.BackgroundColor = Colors.Transparent; // o deja el color si el item queda activo
}
```

---

## 7) Buenas Prácticas

- Reutiliza `Style` para botones tipo sidebar (alto 48px, padding 12, icono 20px, gap 12).
- Mantén activos los indicadores (cápsula/punto) con animación `FadeTo` (más suave, sin relayout).
- Para listas largas, usa `CollectionView` en vez de `BindableLayout`.
- Colores y estilos centralizados; evita estilos locales duplicados.

---

## 8) Pitfalls

- No cambiar `ResourceDictionary.Source` en runtime (usa `ApplyThemeColors*`).
- No animar medidas para indicadores (genera jank). Usar `FadeTo`.
- No mezclar PNG/SVG con mismos nombres (Resizetizer falla). Preferir PNG.

---

## 9) Referencias

- `Docs/GuiaColores.md`
- `Docs/MejoraDiseñoUI.md`
- `Resources/Styles/Colors.xaml`
- `MainPage.xaml`, `MainPage.xaml.cs`
- `Converters/ThemeIconConverter.cs`
