namespace RedNachoToolbox.Constants;

/// <summary>
/// Defines constant keys for application preferences to avoid magic strings
/// and ensure type safety across the application.
/// </summary>
public static class PreferenceKeys
{
    /// <summary>
    /// Key for storing sidebar collapsed state preference.
    /// Type: bool (default: false - expanded)
 /// </summary>
    public const string IsSidebarCollapsed = nameof(IsSidebarCollapsed);

    /// <summary>
    /// Key for storing theme mode preference.
    /// Type: string (values: "System", "Light", "Dark", default: "System")
  /// </summary>
    public const string ThemeMode = nameof(ThemeMode);

    /// <summary>
    /// Legacy key for storing dark theme preference (kept for backward compatibility).
 /// Type: bool (default: false)
    /// </summary>
    public const string IsDarkTheme = nameof(IsDarkTheme);

    /// <summary>
    /// Key for storing recently used tools.
    /// Type: string (JSON serialized list)
    /// </summary>
    public const string RecentlyUsedTools = nameof(RecentlyUsedTools);

    /// <summary>
    /// Key for storing last used tool category filter.
    /// Type: string (enum value)
    /// </summary>
    public const string LastUsedCategory = nameof(LastUsedCategory);
}
