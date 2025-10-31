using Microsoft.Extensions.Logging;
using RedNachoToolbox.Constants;
using CommunityToolkit.Mvvm.Messaging;
using RedNachoToolbox.Messaging;

namespace RedNachoToolbox.Services;

/// <summary>
/// Default implementation of IThemeService for managing application themes.
/// </summary>
public class ThemeService : IThemeService
{
    private readonly IPreferencesService _preferencesService;
    private readonly ILogger<ThemeService> _logger;
    private ThemeMode _currentTheme;
    private bool _isSystemThemeListenerActive;

    public ThemeService(IPreferencesService preferencesService, ILogger<ThemeService> logger)
    {
        _preferencesService = preferencesService ?? throw new ArgumentNullException(nameof(preferencesService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
   _logger.LogDebug("ThemeService created");
    }

    /// <inheritdoc/>
    public ThemeMode CurrentTheme => _currentTheme;

    /// <inheritdoc/>
    public bool IsDarkTheme => DetermineIfDarkTheme();

    /// <inheritdoc/>
    public event EventHandler<ThemeMode>? ThemeChanged;

    /// <inheritdoc/>
    public void Initialize()
    {
  _logger.LogInformation("Initializing ThemeService");

        // Load saved theme preference
     var savedTheme = _preferencesService.Get(PreferenceKeys.ThemeMode, "System");
        _currentTheme = ParseThemeMode(savedTheme);

   _logger.LogDebug("Loaded theme preference: {ThemeMode}", _currentTheme);

  // Apply the theme
   ApplyTheme(_currentTheme);
    }

    /// <inheritdoc/>
    public void ApplyTheme(ThemeMode theme)
    {
      try
        {
   _logger.LogInformation("Applying theme: {ThemeMode}", theme);

            if (Application.Current == null)
       {
             _logger.LogWarning("Application.Current is null, cannot apply theme");
       return;
        }

      // Unsubscribe from system theme changes if switching away from System mode
            if (_isSystemThemeListenerActive && theme != ThemeMode.System)
          {
       Application.Current.RequestedThemeChanged -= OnSystemThemeChanged;
    _isSystemThemeListenerActive = false;
     _logger.LogDebug("Unsubscribed from system theme changes");
            }

            _currentTheme = theme;

            switch (theme)
{
           case ThemeMode.Light:
   ApplyThemeColors(false);
    Application.Current.UserAppTheme = AppTheme.Light;
          break;

      case ThemeMode.Dark:
          ApplyThemeColors(true);
   Application.Current.UserAppTheme = AppTheme.Dark;
  break;

             case ThemeMode.System:
     // Follow system preference
         Application.Current.UserAppTheme = AppTheme.Unspecified;
         var isSystemDark = Application.Current.RequestedTheme == AppTheme.Dark;
             ApplyThemeColors(isSystemDark);

  // Subscribe to system theme changes
          if (!_isSystemThemeListenerActive)
   {
      Application.Current.RequestedThemeChanged += OnSystemThemeChanged;
         _isSystemThemeListenerActive = true;
            _logger.LogDebug("Subscribed to system theme changes");
        }
            break;
 }

   // Save preference
            _preferencesService.Set(PreferenceKeys.ThemeMode, theme.ToString());

     // Propagate theme colors to merged dictionaries
PropagateThemeKeys(Application.Current.Resources,
           "CardBackgroundColor", "CardAccentBackgroundColor", "CardShadowColor",
       "TextColor", "TextColorSecondary", "TextColorTertiary", "HighlightColor",
                "PrimaryRed", "InteractivePrimaryColor", "InteractiveSecondaryColor", "BorderInteractiveColor");

  // Force layout refresh
  Application.Current.MainPage?.ForceLayout();

  // Notify subscribers
     ThemeChanged?.Invoke(this, theme);
            WeakReferenceMessenger.Default.Send(new ThemeChangedMessage(IsDarkTheme));

          _logger.LogInformation("Theme applied successfully: {ThemeMode}", theme);
        }
        catch (Exception ex)
  {
          _logger.LogError(ex, "Error applying theme: {ThemeMode}", theme);
    }
    }

    private void OnSystemThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
   if (_currentTheme != ThemeMode.System)
        {
return; // Only respond if in System mode
   }

   var isDark = e.RequestedTheme == AppTheme.Dark;
        _logger.LogInformation("System theme changed to {Theme}", isDark ? "Dark" : "Light");

        ApplyThemeColors(isDark);

 if (Application.Current != null)
        {
       PropagateThemeKeys(Application.Current.Resources,
           "CardBackgroundColor", "CardAccentBackgroundColor", "CardShadowColor",
         "TextColor", "TextColorSecondary", "TextColorTertiary", "HighlightColor",
              "PrimaryRed", "InteractivePrimaryColor", "InteractiveSecondaryColor", "BorderInteractiveColor");
 Application.Current.MainPage?.ForceLayout();
        }

        WeakReferenceMessenger.Default.Send(new ThemeChangedMessage(isDark));
    }

    private void ApplyThemeColors(bool isDarkTheme)
    {
  var resources = Application.Current?.Resources;
        if (resources == null)
        {
            _logger.LogWarning("Application resources not available");
   return;
 }

 _logger.LogDebug("Applying {Theme} theme colors", isDarkTheme ? "dark" : "light");

        if (isDarkTheme)
   {
            ApplyDarkTheme(resources);
        }
  else
        {
          ApplyLightTheme(resources);
        }
  }

 private void ApplyDarkTheme(ResourceDictionary resources)
    {
      resources["PrimaryRed"] = Color.FromArgb("#E57373");
        resources["PageBackgroundColor"] = Color.FromArgb("#121212");
        resources["SidebarBackgroundColor"] = Color.FromArgb("#1E1E1E");
        resources["CardBackgroundColor"] = Color.FromArgb("#2A2A2A");
        resources["CardAccentBackgroundColor"] = Color.FromArgb("#333333");
        resources["CardShadowColor"] = Color.FromArgb("#666666");
      resources["ContentBackgroundColor"] = Color.FromArgb("#1A1A1A");
        resources["TextColor"] = Color.FromArgb("#E0E0E0");
        resources["TextColorSecondary"] = Color.FromArgb("#BDBDBD");
        resources["TextColorTertiary"] = Color.FromArgb("#757575");
        resources["TextColorDisabled"] = Color.FromArgb("#616161");
        resources["ButtonBackgroundColor"] = Color.FromArgb("#3A3A3A");
        resources["ButtonTextColor"] = Color.FromArgb("#FFFFFF");
        resources["ButtonBorderColor"] = Color.FromArgb("#555555");
        resources["ButtonHoverBackgroundColor"] = Color.FromArgb("#4A4A4A");
     resources["ButtonPressedBackgroundColor"] = Color.FromArgb("#2A2A2A");
        resources["InteractivePrimaryColor"] = Color.FromArgb("#90CAF9");
     resources["InteractiveSecondaryColor"] = Color.FromArgb("#81D4FA");
    resources["BorderInteractiveColor"] = Color.FromArgb("#42A5F5");
        resources["TextInteractiveColor"] = Color.FromArgb("#121212");
        resources["PrimaryButtonBackgroundColor"] = Color.FromArgb("#90CAF9");
        resources["PrimaryButtonTextColor"] = Color.FromArgb("#121212");
        resources["PrimaryButtonHoverBackgroundColor"] = Color.FromArgb("#81D4FA");
   resources["PrimaryButtonPressedBackgroundColor"] = Color.FromArgb("#42A5F5");
        resources["BorderColorLight"] = Color.FromArgb("#3C3C3C");
        resources["BorderColorMedium"] = Color.FromArgb("#666666");
    resources["BorderColorDark"] = Color.FromArgb("#333333");
      resources["InputBackgroundColor"] = Color.FromArgb("#2A2A2A");
        resources["InputBorderColor"] = Color.FromArgb("#3C3C3C");
        resources["InputTextColor"] = Color.FromArgb("#E0E0E0");
        resources["InputPlaceholderColor"] = Color.FromArgb("#757575");
    resources["InputFocusBorderColor"] = Color.FromArgb("#42A5F5");
  resources["SuccessColor"] = Color.FromArgb("#4CAF50");
        resources["WarningColor"] = Color.FromArgb("#FF9800");
        resources["ErrorColor"] = Color.FromArgb("#F44336");
   resources["InfoColor"] = Color.FromArgb("#2196F3");
  resources["SelectionColor"] = Color.FromArgb("#90CAF9");
        resources["SelectionBackgroundColor"] = Color.FromArgb("#263238");
        resources["HighlightColor"] = Color.FromRgba(255, 255, 255, 0.08);
 resources["NavigationBackgroundColor"] = Color.FromArgb("#1E1E1E");
        resources["NavigationTextColor"] = Color.FromArgb("#E0E0E0");
        resources["NavigationSelectedBackgroundColor"] = Color.FromArgb("#CC333B");
        resources["NavigationSelectedTextColor"] = Color.FromArgb("#FFFFFF");
        resources["NavigationHoverBackgroundColor"] = Color.FromArgb("#3A3A3A");
        resources["SettingsBackgroundColor"] = Color.FromArgb("#2A2A2A");
        resources["SettingsBorderColor"] = Color.FromArgb("#3C3C3C");
        resources["SettingsTextColor"] = Color.FromArgb("#E0E0E0");
        resources["SettingsSecondaryTextColor"] = Color.FromArgb("#BDBDBD");
    }

    private void ApplyLightTheme(ResourceDictionary resources)
  {
  resources["PrimaryRed"] = Color.FromArgb("#D32F2F");
   resources["PageBackgroundColor"] = Color.FromArgb("#F5F5F5");
 resources["SidebarBackgroundColor"] = Color.FromArgb("#F8F9FA");
    resources["CardBackgroundColor"] = Color.FromArgb("#FFFFFF");
        resources["CardAccentBackgroundColor"] = Color.FromArgb("#F2F2F2");
      resources["CardShadowColor"] = Color.FromArgb("#ACACAC");
        resources["ContentBackgroundColor"] = Color.FromArgb("#FAFAFA");
        resources["TextColor"] = Color.FromArgb("#212121");
    resources["TextColorSecondary"] = Color.FromArgb("#616161");
   resources["TextColorTertiary"] = Color.FromArgb("#9E9E9E");
        resources["TextColorDisabled"] = Color.FromArgb("#BDBDBD");
    resources["ButtonBackgroundColor"] = Color.FromArgb("#F8F9FA");
        resources["ButtonTextColor"] = Color.FromArgb("#212529");
   resources["ButtonBorderColor"] = Color.FromArgb("#DEE2E6");
        resources["ButtonHoverBackgroundColor"] = Color.FromArgb("#E9ECEF");
        resources["ButtonPressedBackgroundColor"] = Color.FromArgb("#DEE2E6");
   resources["InteractivePrimaryColor"] = Color.FromArgb("#1976D2");
  resources["InteractiveSecondaryColor"] = Color.FromArgb("#0288D1");
   resources["BorderInteractiveColor"] = Color.FromArgb("#2196F3");
      resources["TextInteractiveColor"] = Color.FromArgb("#FFFFFF");
 resources["PrimaryButtonBackgroundColor"] = Color.FromArgb("#1976D2");
        resources["PrimaryButtonTextColor"] = Color.FromArgb("#FFFFFF");
        resources["PrimaryButtonHoverBackgroundColor"] = Color.FromArgb("#0288D1");
        resources["PrimaryButtonPressedBackgroundColor"] = Color.FromArgb("#1565C0");
        resources["BorderColorLight"] = Color.FromArgb("#DEE2E6");
        resources["BorderColorMedium"] = Color.FromArgb("#ADB5BD");
        resources["BorderColorDark"] = Color.FromArgb("#6C757D");
        resources["InputBackgroundColor"] = Color.FromArgb("#FFFFFF");
  resources["InputBorderColor"] = Color.FromArgb("#CED4DA");
        resources["InputTextColor"] = Color.FromArgb("#212121");
        resources["InputPlaceholderColor"] = Color.FromArgb("#9E9E9E");
        resources["InputFocusBorderColor"] = Color.FromArgb("#2196F3");
  resources["SuccessColor"] = Color.FromArgb("#198754");
     resources["WarningColor"] = Color.FromArgb("#FFC107");
        resources["ErrorColor"] = Color.FromArgb("#DC3545");
        resources["InfoColor"] = Color.FromArgb("#0DCAF0");
        resources["SelectionColor"] = Color.FromArgb("#1976D2");
        resources["SelectionBackgroundColor"] = Color.FromArgb("#E8F0FE");
resources["HighlightColor"] = Color.FromRgba(0, 0, 0, 0.04);
        resources["NavigationBackgroundColor"] = Color.FromArgb("#F8F9FA");
        resources["NavigationTextColor"] = Color.FromArgb("#212121");
        resources["NavigationSelectedBackgroundColor"] = Color.FromArgb("#E8F0FE");
        resources["NavigationSelectedTextColor"] = Color.FromArgb("#FFFFFF");
        resources["NavigationHoverBackgroundColor"] = Color.FromArgb("#E9ECEF");
        resources["SettingsBackgroundColor"] = Color.FromArgb("#FFFFFF");
        resources["SettingsBorderColor"] = Color.FromArgb("#DEE2E6");
  resources["SettingsTextColor"] = Color.FromArgb("#212529");
        resources["SettingsSecondaryTextColor"] = Color.FromArgb("#6C757D");
    }

    private static void PropagateThemeKeys(ResourceDictionary resources, params string[] keys)
    {
        if (resources == null || keys == null || keys.Length == 0)
            return;

     foreach (var key in keys)
        {
            if (!resources.ContainsKey(key))
                continue;

    var value = resources[key];

       foreach (var dict in resources.MergedDictionaries)
            {
      try
       {
     if (dict.ContainsKey(key))
     {
        dict[key] = value;
              }
          }
    catch
       {
        // Ignore per-dictionary errors
      }
         }
        }
    }

    private bool DetermineIfDarkTheme()
    {
        try
      {
          var resources = Application.Current?.Resources;
     if (resources == null) return false;

         if (resources.TryGetValue("PageBackgroundColor", out var bgObj) && bgObj is Color bgColor)
        {
       return bgColor.Red < 0.5f && bgColor.Green < 0.5f && bgColor.Blue < 0.5f;
       }
    }
        catch (Exception ex)
        {
 _logger.LogError(ex, "Error determining if theme is dark");
      }

   return false;
    }

    private static ThemeMode ParseThemeMode(string value)
    {
        return value switch
     {
        "Light" => ThemeMode.Light,
   "Dark" => ThemeMode.Dark,
            _ => ThemeMode.System
      };
    }
}
