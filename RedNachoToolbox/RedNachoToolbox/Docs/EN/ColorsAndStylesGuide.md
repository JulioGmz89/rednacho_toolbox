# Colors and Styles Guide (Tokens, Borders, Typography, States)

This guide complements `Docs/GuiaColores.md` and formalizes how to use colors and styles in the UI. It covers color tokens, recommended styles (borders, corner radius, shadows), typography, iconography, and interaction states (hover/pressed/focus/selected) in .NET MAUI.

> To understand the token system and how colors are applied at runtime (MAUI limitation with `ResourceDictionary.Source`), read `Docs/GuiaColores.md` first.

---

## 1) General Principles

- Always use `DynamicResource` for colors in XAML.
- Avoid hardcoded hex values. If you need a new color, create a token.
- Keep Light/Dark equivalents in `SettingsPage.ApplyThemeColors*` in sync.
- Prefer `Border` over `Frame` when you only need background/border/radius.
- For animations, prefer `Opacity` (`FadeTo`) instead of changing `WidthRequest/HeightRequest`.

---

## 2) Color Tokens – Usage Recommendations

Tokens already defined are documented in `Docs/GuiaColores.md`. Practical mapping:

- `PageBackgroundColor`: Entire page background (right main column in `MainPage.xaml`).
- `SidebarBackgroundColor`: Sidebar background (left column).
- `CardBackgroundColor`: Cards / container backgrounds.
- `BorderColor*`: Subtle separators and outlines.
- `TextColor*`: Typography (primary/secondary/tertiary).
- `Navigation*` / `Settings*`: Tokens specific to those views.
- `Interactive*`: Blues for interactive states (hover, focus, primary).
- `PrimaryRed`: Brand accent (indicators). Use sparingly and with meaning.

XAML example:
```xml
<Border BackgroundColor="{DynamicResource CardBackgroundColor}"
        Stroke="{DynamicResource BorderColorLight}"
        StrokeThickness="1"
        StrokeShape="RoundRectangle 8">
    <Label Text="Title" TextColor="{DynamicResource TextColor}" />
</Border>
```

---

## 3) Borders, Corner Radius, and Shadows

- Recommended corner radius: 8 px (consistent across cards/buttons).
- Shadows: On MAUI/WinUI, heavy shadows increase rendering cost. Prefer surface contrast and subtle borders.
- `Frame` vs `Border`:
  - `Frame` adds extra cost (shadow pipeline). If you use `Frame`, disable shadows (`HasShadow="False"`).
  - `Border` is lighter and sufficient for most containers/buttons.

Example "button" using `Border` + `TapGestureRecognizer`:
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

## 4) Typography and Spacing

- Font sizes: use styles in `Resources/Styles/Styles.xaml` (if present) or define shared `Style`s.
- Text colors: `TextColor`, `TextColorSecondary`, `TextColorTertiary`.
- Hierarchy: titles (primary), descriptions (secondary).
- Suggested spacing: multiples of 4 (4/8/12/16/24). Sidebar and cards typically use 12–16 px padding.

---

## 5) Iconography and Theme

Two options to handle Light/Dark icon switching:

1) Inline AppThemeBinding (simple and direct):
```xml
<Image Source="{AppThemeBinding Light=grid_outline_black.png, Dark=grid_outline_white.png}" />
```

2) `ThemeIconConverter` (MultiBinding, when you use base name + theme state):
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

- Place PNGs under `Resources/Images/` (avoid duplicates with `Resources/Vector/`).
- Naming: `{base}_black.png` for light, `{base}_white.png` for dark.

---

## 6) States (Hover/Pressed/Focus/Selected)

- State colors: use tokens (`ButtonHoverBackgroundColor`, etc.) or neutral tones.
- Avoid blocking UI with `await Task.Delay` in UI handlers. Use helpers that do not block navigation.
- For accessibility, add visible `focus` state in inputs/buttons (interactive blue outline).

Simple non‑blocking feedback (code‑behind):
```csharp
private async Task PressFeedbackAsync(Border border)
{
    var hover = (Color)Application.Current.Resources["NavigationHoverBackgroundColor"];
    border.BackgroundColor = hover;
    await Task.Delay(80);
    border.BackgroundColor = Colors.Transparent; // or keep color if the item remains active
}
```

---

## 7) Best Practices

- Reuse a `Style` for sidebar‑like buttons (height 48px, padding 12, 20px icon, 12 gap).
- Keep active indicators (capsule/dot) with `FadeTo` animations (smoother, avoids relayout).
- For long lists, use `CollectionView` instead of `BindableLayout`.
- Centralize colors and styles; avoid duplicated local styles.

---

## 8) Pitfalls

- Do not change `ResourceDictionary.Source` at runtime (use `ApplyThemeColors*`).
- Do not animate sizes for indicators (causes jank). Use `FadeTo`.
- Do not mix PNG/SVG with the same names (Resizetizer fails). Prefer PNG.

---

## 9) References

- `Docs/GuiaColores.md`
- `Docs/MejoraDiseñoUI.md`
- `Resources/Styles/Colors.xaml`
- `MainPage.xaml`, `MainPage.xaml.cs`
- `Converters/ThemeIconConverter.cs`
