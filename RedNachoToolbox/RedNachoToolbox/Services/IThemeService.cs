namespace RedNachoToolbox.Services;

/// <summary>
/// Service for managing application theme (Light, Dark, System).
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets the current theme mode.
    /// </summary>
    ThemeMode CurrentTheme { get; }

    /// <summary>
    /// Gets whether the current effective theme is dark.
    /// </summary>
    bool IsDarkTheme { get; }

/// <summary>
    /// Applies the specified theme mode.
    /// </summary>
    /// <param name="theme">The theme mode to apply</param>
    void ApplyTheme(ThemeMode theme);

    /// <summary>
    /// Initializes the theme service and applies saved theme preference.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Event raised when the theme changes.
    /// </summary>
    event EventHandler<ThemeMode>? ThemeChanged;
}

/// <summary>
/// Represents the available theme modes.
/// </summary>
public enum ThemeMode
{
    /// <summary>
/// Follow system theme preference.
    /// </summary>
    System,

    /// <summary>
    /// Always use light theme.
    /// </summary>
    Light,

    /// <summary>
    /// Always use dark theme.
    /// </summary>
    Dark
}
