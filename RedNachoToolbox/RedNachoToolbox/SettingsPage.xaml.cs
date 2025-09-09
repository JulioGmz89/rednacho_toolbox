using Microsoft.Maui.Controls;

namespace RedNachoToolbox;

public partial class SettingsPage : ContentPage
{
    private bool _isDarkTheme;

    public SettingsPage()
    {
        InitializeComponent();
        InitializeThemeState();
    }

    /// <summary>
    /// Gets or sets whether dark theme is currently active
    /// </summary>
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (_isDarkTheme != value)
            {
                _isDarkTheme = value;
                OnPropertyChanged();
            }
        }
    }

    #region Initialization

    /// <summary>
    /// Initialize the theme state based on current application resources
    /// </summary>
    private void InitializeThemeState()
    {
        System.Diagnostics.Debug.WriteLine("=== InitializeThemeState START ===");
        
        // Check if DarkTheme.xaml is currently loaded
        System.Diagnostics.Debug.WriteLine("Checking current theme state...");
        IsDarkTheme = IsCurrentlyDarkTheme();
        System.Diagnostics.Debug.WriteLine($"Current theme detected: {(IsDarkTheme ? "Dark" : "Light")}");
        
        System.Diagnostics.Debug.WriteLine($"Setting switch to: {IsDarkTheme}");
        ThemeSwitch.IsToggled = IsDarkTheme;
        System.Diagnostics.Debug.WriteLine($"Switch IsToggled set to: {ThemeSwitch.IsToggled}");
        
        // Verify theme files exist
        VerifyThemeFilesExist();
        
        System.Diagnostics.Debug.WriteLine("=== InitializeThemeState END ===");
    }

    /// <summary>
    /// Determines if the current theme is dark by checking applied color values
    /// </summary>
    private bool IsCurrentlyDarkTheme()
    {
        System.Diagnostics.Debug.WriteLine("=== IsCurrentlyDarkTheme START ===");
        
        try
        {
            var resources = Application.Current?.Resources;
            if (resources == null)
            {
                System.Diagnostics.Debug.WriteLine("No application resources available");
                return LoadThemePreference(); // Fallback to saved preference
            }

            // Check the current PageBackgroundColor to determine theme
            // Dark theme uses #121212, Light theme uses #FFFFFF
            if (resources.TryGetValue("PageBackgroundColor", out var bgColorObj) && bgColorObj is Color bgColor)
            {
                System.Diagnostics.Debug.WriteLine($"Current PageBackgroundColor: {bgColor}");
                
                // Dark theme background is very dark (#121212 = RGB(18,18,18))
                // Light theme background is white (#FFFFFF = RGB(255,255,255))
                bool isDark = bgColor.Red < 0.5f && bgColor.Green < 0.5f && bgColor.Blue < 0.5f;
                
                System.Diagnostics.Debug.WriteLine($"Theme detected by color analysis: {(isDark ? "Dark" : "Light")}");
                return isDark;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("PageBackgroundColor not found in resources, checking saved preference");
                return LoadThemePreference(); // Fallback to saved preference
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error detecting current theme: {ex.Message}");
            return LoadThemePreference(); // Fallback to saved preference
        }
    }

    /// <summary>
    /// Verifies that theme colors can be applied (no longer needs to check XAML files)
    /// </summary>
    private void VerifyThemeFilesExist()
    {
        System.Diagnostics.Debug.WriteLine("=== VerifyThemeColorsAvailable START ===");
        
        try
        {
            var resources = Application.Current?.Resources;
            if (resources == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ ERROR: Application resources not available");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine("✓ Application resources are accessible");
            System.Diagnostics.Debug.WriteLine("✓ Theme colors can be applied directly to resources");
            System.Diagnostics.Debug.WriteLine("✓ No XAML files needed - using direct color application");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ ERROR verifying theme capability: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Exception type: {ex.GetType().Name}");
        }
        
        System.Diagnostics.Debug.WriteLine("=== VerifyThemeColorsAvailable END ===");
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles theme switch toggle events
    /// </summary>
    private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("=== OnThemeSwitchToggled START ===");
        var isDarkMode = e.Value;
        System.Diagnostics.Debug.WriteLine($"Switch toggled to: {(isDarkMode ? "Dark" : "Light")} mode");
        
        IsDarkTheme = isDarkMode;
        System.Diagnostics.Debug.WriteLine($"IsDarkTheme property set to: {IsDarkTheme}");
        
        // Apply theme change
        System.Diagnostics.Debug.WriteLine("Calling ApplyTheme...");
        ApplyTheme(isDarkMode);
        System.Diagnostics.Debug.WriteLine("ApplyTheme completed");
        
        // Save theme preference
        System.Diagnostics.Debug.WriteLine("Saving theme preference...");
        SaveThemePreference(isDarkMode);
        System.Diagnostics.Debug.WriteLine("Theme preference saved");
        
        System.Diagnostics.Debug.WriteLine("=== OnThemeSwitchToggled END ===");
    }

    /// <summary>
    /// Handles back button click to return to main page
    /// </summary>
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            // Fallback navigation if Shell navigation fails
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            
            // Alternative navigation method
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }
    }

    #endregion

    #region Theme Management

    /// <summary>
    /// Applies the selected theme by updating application resource dictionaries
    /// </summary>
    private void ApplyTheme(bool isDarkTheme)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"=== ApplyTheme START: isDarkTheme={isDarkTheme} ===");
            
            // Check Application.Current
            if (Application.Current == null)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Application.Current is null");
                DisplayAlert("Theme Error", "Application not available. Please try again.", "OK");
                return;
            }
            System.Diagnostics.Debug.WriteLine("✓ Application.Current is available");

            // Check Resources
            if (Application.Current.Resources == null)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Application.Current.Resources is null");
                DisplayAlert("Theme Error", "Application resources not available. Please try again.", "OK");
                return;
            }
            System.Diagnostics.Debug.WriteLine("✓ Application.Current.Resources is available");

            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries == null)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: MergedDictionaries is null");
                DisplayAlert("Theme Error", "Resource dictionaries not available. Please try again.", "OK");
                return;
            }
            System.Diagnostics.Debug.WriteLine($"✓ MergedDictionaries available, count: {mergedDictionaries.Count}");

            // Log current dictionaries
            System.Diagnostics.Debug.WriteLine("Current merged dictionaries:");
            for (int i = 0; i < mergedDictionaries.Count; i++)
            {
                var dict = mergedDictionaries.ElementAt(i);
                var source = dict.Source?.OriginalString ?? "No source";
                System.Diagnostics.Debug.WriteLine($"  [{i}] {source}");
            }

            // Remove existing theme dictionaries
            System.Diagnostics.Debug.WriteLine("Removing existing theme dictionaries...");
            RemoveThemeDictionaries(mergedDictionaries);
            System.Diagnostics.Debug.WriteLine($"After removal, count: {mergedDictionaries.Count}");

            // Apply theme by updating individual resource values
            System.Diagnostics.Debug.WriteLine("Applying theme by updating resource values...");
            try
            {
                ApplyThemeColors(isDarkTheme);
                System.Diagnostics.Debug.WriteLine("✓ Theme colors applied successfully");
            }
            catch (Exception colorEx)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR applying theme colors: {colorEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {colorEx.StackTrace}");
                throw;
            }

            System.Diagnostics.Debug.WriteLine($"Final merged dictionaries count: {mergedDictionaries.Count}");
            System.Diagnostics.Debug.WriteLine($"✓ Theme applied successfully: {(isDarkTheme ? "Dark" : "Light")}");
            System.Diagnostics.Debug.WriteLine("=== ApplyTheme END ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"=== EXCEPTION in ApplyTheme ===");
            System.Diagnostics.Debug.WriteLine($"Exception Type: {ex.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
            }
            System.Diagnostics.Debug.WriteLine("=== END EXCEPTION ===");
            
            // Show user-friendly error message with more details
            DisplayAlert("Theme Error", 
                $"Unable to change theme: {ex.Message}\n\nCheck debug output for details.", 
                "OK");
        }
    }

    /// <summary>
    /// Removes existing theme dictionaries from merged dictionaries
    /// </summary>
    private void RemoveThemeDictionaries(ICollection<ResourceDictionary> mergedDictionaries)
    {
        System.Diagnostics.Debug.WriteLine("=== RemoveThemeDictionaries START ===");
        var themeDictionaries = new List<ResourceDictionary>();

        // Find theme dictionaries to remove
        System.Diagnostics.Debug.WriteLine("Searching for existing theme dictionaries...");
        foreach (var dictionary in mergedDictionaries)
        {
            if (dictionary.Source?.OriginalString != null)
            {
                var source = dictionary.Source.OriginalString;
                System.Diagnostics.Debug.WriteLine($"  Checking dictionary: {source}");
                if (source.Contains("LightTheme.xaml") || source.Contains("DarkTheme.xaml"))
                {
                    System.Diagnostics.Debug.WriteLine($"  ✓ Found theme dictionary to remove: {source}");
                    themeDictionaries.Add(dictionary);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("  Dictionary has no source");
            }
        }

        System.Diagnostics.Debug.WriteLine($"Found {themeDictionaries.Count} theme dictionaries to remove");

        // Remove found theme dictionaries
        foreach (var themeDict in themeDictionaries)
        {
            try
            {
                var source = themeDict.Source?.OriginalString ?? "Unknown";
                System.Diagnostics.Debug.WriteLine($"  Removing: {source}");
                mergedDictionaries.Remove(themeDict);
                System.Diagnostics.Debug.WriteLine($"  ✓ Removed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"  ERROR removing dictionary: {ex.Message}");
            }
        }
        System.Diagnostics.Debug.WriteLine("=== RemoveThemeDictionaries END ===");
    }

    /// <summary>
    /// Applies theme colors directly to application resources
    /// </summary>
    private void ApplyThemeColors(bool isDarkTheme)
    {
        System.Diagnostics.Debug.WriteLine($"=== ApplyThemeColors START: {(isDarkTheme ? "Dark" : "Light")} ===");
        
        var resources = Application.Current?.Resources;
        if (resources == null)
        {
            System.Diagnostics.Debug.WriteLine("ERROR: Application resources not available");
            return;
        }

        if (isDarkTheme)
        {
            // Apply Dark Theme Colors
            System.Diagnostics.Debug.WriteLine("Applying dark theme colors...");
            
            // Primary Colors
            resources["PrimaryRed"] = Color.FromArgb("#CC333B");
            
            // Background Colors
            resources["PageBackgroundColor"] = Color.FromArgb("#121212");
            resources["SidebarBackgroundColor"] = Color.FromArgb("#1E1E1E");
            resources["CardBackgroundColor"] = Color.FromArgb("#2A2A2A");
            resources["ContentBackgroundColor"] = Color.FromArgb("#1A1A1A");
            
            // Text Colors
            resources["TextColor"] = Color.FromArgb("#FFFFFF");
            resources["TextColorSecondary"] = Color.FromArgb("#B3B3B3");
            resources["TextColorTertiary"] = Color.FromArgb("#808080");
            resources["TextColorDisabled"] = Color.FromArgb("#4D4D4D");
            
            // Button Colors
            resources["ButtonBackgroundColor"] = Color.FromArgb("#3A3A3A");
            resources["ButtonTextColor"] = Color.FromArgb("#FFFFFF");
            resources["ButtonBorderColor"] = Color.FromArgb("#555555");
            resources["ButtonHoverBackgroundColor"] = Color.FromArgb("#4A4A4A");
            resources["ButtonPressedBackgroundColor"] = Color.FromArgb("#2A2A2A");
            
            // Primary Button Colors
            resources["PrimaryButtonBackgroundColor"] = Color.FromArgb("#CC333B");
            resources["PrimaryButtonTextColor"] = Color.FromArgb("#FFFFFF");
            resources["PrimaryButtonHoverBackgroundColor"] = Color.FromArgb("#B52D35");
            resources["PrimaryButtonPressedBackgroundColor"] = Color.FromArgb("#A02730");
            
            // Border Colors
            resources["BorderColorLight"] = Color.FromArgb("#444444");
            resources["BorderColorMedium"] = Color.FromArgb("#666666");
            resources["BorderColorDark"] = Color.FromArgb("#333333");
            
            // Input Colors
            resources["InputBackgroundColor"] = Color.FromArgb("#2A2A2A");
            resources["InputBorderColor"] = Color.FromArgb("#555555");
            resources["InputTextColor"] = Color.FromArgb("#FFFFFF");
            resources["InputPlaceholderColor"] = Color.FromArgb("#808080");
            resources["InputFocusBorderColor"] = Color.FromArgb("#CC333B");
            
            // Status Colors
            resources["SuccessColor"] = Color.FromArgb("#4CAF50");
            resources["WarningColor"] = Color.FromArgb("#FF9800");
            resources["ErrorColor"] = Color.FromArgb("#F44336");
            resources["InfoColor"] = Color.FromArgb("#2196F3");
            
            // Selection Colors
            resources["SelectionColor"] = Color.FromArgb("#CC333B");
            resources["SelectionBackgroundColor"] = Color.FromArgb("#3D1A1D");
            resources["HighlightColor"] = Color.FromArgb("#4D4D4D");
            
            // Navigation Colors
            resources["NavigationBackgroundColor"] = Color.FromArgb("#1E1E1E");
            resources["NavigationTextColor"] = Color.FromArgb("#FFFFFF");
            resources["NavigationSelectedBackgroundColor"] = Color.FromArgb("#CC333B");
            resources["NavigationSelectedTextColor"] = Color.FromArgb("#FFFFFF");
            resources["NavigationHoverBackgroundColor"] = Color.FromArgb("#3A3A3A");
            
            // Settings Colors
            resources["SettingsBackgroundColor"] = Color.FromArgb("#2A2A2A");
            resources["SettingsBorderColor"] = Color.FromArgb("#444444");
            resources["SettingsTextColor"] = Color.FromArgb("#FFFFFF");
            resources["SettingsSecondaryTextColor"] = Color.FromArgb("#B3B3B3");
        }
        else
        {
            // Apply Light Theme Colors
            System.Diagnostics.Debug.WriteLine("Applying light theme colors...");
            
            // Primary Colors
            resources["PrimaryRed"] = Color.FromArgb("#CC333B");
            
            // Background Colors
            resources["PageBackgroundColor"] = Color.FromArgb("#FFFFFF");
            resources["SidebarBackgroundColor"] = Color.FromArgb("#F8F9FA");
            resources["CardBackgroundColor"] = Color.FromArgb("#FFFFFF");
            resources["ContentBackgroundColor"] = Color.FromArgb("#FAFAFA");
            
            // Text Colors
            resources["TextColor"] = Color.FromArgb("#212529");
            resources["TextColorSecondary"] = Color.FromArgb("#6C757D");
            resources["TextColorTertiary"] = Color.FromArgb("#ADB5BD");
            resources["TextColorDisabled"] = Color.FromArgb("#CED4DA");
            
            // Button Colors
            resources["ButtonBackgroundColor"] = Color.FromArgb("#F8F9FA");
            resources["ButtonTextColor"] = Color.FromArgb("#212529");
            resources["ButtonBorderColor"] = Color.FromArgb("#DEE2E6");
            resources["ButtonHoverBackgroundColor"] = Color.FromArgb("#E9ECEF");
            resources["ButtonPressedBackgroundColor"] = Color.FromArgb("#DEE2E6");
            
            // Primary Button Colors
            resources["PrimaryButtonBackgroundColor"] = Color.FromArgb("#CC333B");
            resources["PrimaryButtonTextColor"] = Color.FromArgb("#FFFFFF");
            resources["PrimaryButtonHoverBackgroundColor"] = Color.FromArgb("#B52D35");
            resources["PrimaryButtonPressedBackgroundColor"] = Color.FromArgb("#A02730");
            
            // Border Colors
            resources["BorderColorLight"] = Color.FromArgb("#DEE2E6");
            resources["BorderColorMedium"] = Color.FromArgb("#ADB5BD");
            resources["BorderColorDark"] = Color.FromArgb("#6C757D");
            
            // Input Colors
            resources["InputBackgroundColor"] = Color.FromArgb("#FFFFFF");
            resources["InputBorderColor"] = Color.FromArgb("#CED4DA");
            resources["InputTextColor"] = Color.FromArgb("#212529");
            resources["InputPlaceholderColor"] = Color.FromArgb("#6C757D");
            resources["InputFocusBorderColor"] = Color.FromArgb("#CC333B");
            
            // Status Colors
            resources["SuccessColor"] = Color.FromArgb("#198754");
            resources["WarningColor"] = Color.FromArgb("#FFC107");
            resources["ErrorColor"] = Color.FromArgb("#DC3545");
            resources["InfoColor"] = Color.FromArgb("#0DCAF0");
            
            // Selection Colors
            resources["SelectionColor"] = Color.FromArgb("#CC333B");
            resources["SelectionBackgroundColor"] = Color.FromArgb("#FFF2F2");
            resources["HighlightColor"] = Color.FromArgb("#E9ECEF");
            
            // Navigation Colors
            resources["NavigationBackgroundColor"] = Color.FromArgb("#F8F9FA");
            resources["NavigationTextColor"] = Color.FromArgb("#212529");
            resources["NavigationSelectedBackgroundColor"] = Color.FromArgb("#CC333B");
            resources["NavigationSelectedTextColor"] = Color.FromArgb("#FFFFFF");
            resources["NavigationHoverBackgroundColor"] = Color.FromArgb("#E9ECEF");
            
            // Settings Colors
            resources["SettingsBackgroundColor"] = Color.FromArgb("#FFFFFF");
            resources["SettingsBorderColor"] = Color.FromArgb("#DEE2E6");
            resources["SettingsTextColor"] = Color.FromArgb("#212529");
            resources["SettingsSecondaryTextColor"] = Color.FromArgb("#6C757D");
        }
        
        System.Diagnostics.Debug.WriteLine($"✓ {(isDarkTheme ? "Dark" : "Light")} theme colors applied successfully");
        System.Diagnostics.Debug.WriteLine("=== ApplyThemeColors END ===");
    }

    #endregion

    #region Preferences Management

    /// <summary>
    /// Saves the user's theme preference to application preferences
    /// </summary>
    private void SaveThemePreference(bool isDarkTheme)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"=== SaveThemePreference: {(isDarkTheme ? "Dark" : "Light")} ===");
            Preferences.Set("IsDarkTheme", isDarkTheme);
            System.Diagnostics.Debug.WriteLine($"✓ Theme preference saved successfully: {(isDarkTheme ? "Dark" : "Light")}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR saving theme preference: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Loads the user's saved theme preference
    /// </summary>
    public static bool LoadThemePreference()
    {
        try
        {
            return Preferences.Get("IsDarkTheme", false); // Default to light theme
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading theme preference: {ex.Message}");
            return false; // Default to light theme on error
        }
    }

    /// <summary>
    /// Applies the saved theme preference at application startup
    /// </summary>
    public static void ApplySavedTheme()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("=== ApplySavedTheme START ===");
            
            bool isDarkTheme = LoadThemePreference();
            System.Diagnostics.Debug.WriteLine($"Loaded theme preference: {(isDarkTheme ? "Dark" : "Light")}");

            var resources = Application.Current?.Resources;
            if (resources == null)
            {
                System.Diagnostics.Debug.WriteLine("Error: Unable to access application resources during startup");
                return;
            }

            // Apply theme colors directly to resources
            ApplyThemeColorsStatic(isDarkTheme, resources);

            System.Diagnostics.Debug.WriteLine($"✓ Saved theme applied successfully: {(isDarkTheme ? "Dark" : "Light")}");
            System.Diagnostics.Debug.WriteLine("=== ApplySavedTheme END ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying saved theme: {ex.Message}");
        }
    }

    /// <summary>
    /// Static version of ApplyThemeColors for use during app startup
    /// </summary>
    private static void ApplyThemeColorsStatic(bool isDarkTheme, ResourceDictionary resources)
    {
        System.Diagnostics.Debug.WriteLine($"Applying {(isDarkTheme ? "dark" : "light")} theme colors at startup...");

        if (isDarkTheme)
        {
            // Apply Dark Theme Colors
            resources["PrimaryRed"] = Color.FromArgb("#CC333B");
            resources["PageBackgroundColor"] = Color.FromArgb("#121212");
            resources["SidebarBackgroundColor"] = Color.FromArgb("#1E1E1E");
            resources["CardBackgroundColor"] = Color.FromArgb("#2A2A2A");
            resources["ContentBackgroundColor"] = Color.FromArgb("#1A1A1A");
            resources["TextColor"] = Color.FromArgb("#FFFFFF");
            resources["TextColorSecondary"] = Color.FromArgb("#B3B3B3");
            resources["TextColorTertiary"] = Color.FromArgb("#808080");
            resources["TextColorDisabled"] = Color.FromArgb("#4D4D4D");
            resources["ButtonBackgroundColor"] = Color.FromArgb("#3A3A3A");
            resources["ButtonTextColor"] = Color.FromArgb("#FFFFFF");
            resources["ButtonBorderColor"] = Color.FromArgb("#555555");
            resources["ButtonHoverBackgroundColor"] = Color.FromArgb("#4A4A4A");
            resources["ButtonPressedBackgroundColor"] = Color.FromArgb("#2A2A2A");
            resources["PrimaryButtonBackgroundColor"] = Color.FromArgb("#CC333B");
            resources["PrimaryButtonTextColor"] = Color.FromArgb("#FFFFFF");
            resources["PrimaryButtonHoverBackgroundColor"] = Color.FromArgb("#B52D35");
            resources["PrimaryButtonPressedBackgroundColor"] = Color.FromArgb("#A02730");
            resources["BorderColorLight"] = Color.FromArgb("#444444");
            resources["BorderColorMedium"] = Color.FromArgb("#666666");
            resources["BorderColorDark"] = Color.FromArgb("#333333");
            resources["InputBackgroundColor"] = Color.FromArgb("#2A2A2A");
            resources["InputBorderColor"] = Color.FromArgb("#555555");
            resources["InputTextColor"] = Color.FromArgb("#FFFFFF");
            resources["InputPlaceholderColor"] = Color.FromArgb("#808080");
            resources["InputFocusBorderColor"] = Color.FromArgb("#CC333B");
            resources["SuccessColor"] = Color.FromArgb("#4CAF50");
            resources["WarningColor"] = Color.FromArgb("#FF9800");
            resources["ErrorColor"] = Color.FromArgb("#F44336");
            resources["InfoColor"] = Color.FromArgb("#2196F3");
            resources["SelectionColor"] = Color.FromArgb("#CC333B");
            resources["SelectionBackgroundColor"] = Color.FromArgb("#3D1A1D");
            resources["HighlightColor"] = Color.FromArgb("#4D4D4D");
            resources["NavigationBackgroundColor"] = Color.FromArgb("#1E1E1E");
            resources["NavigationTextColor"] = Color.FromArgb("#FFFFFF");
            resources["NavigationSelectedBackgroundColor"] = Color.FromArgb("#CC333B");
            resources["NavigationSelectedTextColor"] = Color.FromArgb("#FFFFFF");
            resources["NavigationHoverBackgroundColor"] = Color.FromArgb("#3A3A3A");
            resources["SettingsBackgroundColor"] = Color.FromArgb("#2A2A2A");
            resources["SettingsBorderColor"] = Color.FromArgb("#444444");
            resources["SettingsTextColor"] = Color.FromArgb("#FFFFFF");
            resources["SettingsSecondaryTextColor"] = Color.FromArgb("#B3B3B3");
        }
        else
        {
            // Apply Light Theme Colors
            resources["PrimaryRed"] = Color.FromArgb("#CC333B");
            resources["PageBackgroundColor"] = Color.FromArgb("#FFFFFF");
            resources["SidebarBackgroundColor"] = Color.FromArgb("#F8F9FA");
            resources["CardBackgroundColor"] = Color.FromArgb("#FFFFFF");
            resources["ContentBackgroundColor"] = Color.FromArgb("#FAFAFA");
            resources["TextColor"] = Color.FromArgb("#212529");
            resources["TextColorSecondary"] = Color.FromArgb("#6C757D");
            resources["TextColorTertiary"] = Color.FromArgb("#ADB5BD");
            resources["TextColorDisabled"] = Color.FromArgb("#CED4DA");
            resources["ButtonBackgroundColor"] = Color.FromArgb("#F8F9FA");
            resources["ButtonTextColor"] = Color.FromArgb("#212529");
            resources["ButtonBorderColor"] = Color.FromArgb("#DEE2E6");
            resources["ButtonHoverBackgroundColor"] = Color.FromArgb("#E9ECEF");
            resources["ButtonPressedBackgroundColor"] = Color.FromArgb("#DEE2E6");
            resources["PrimaryButtonBackgroundColor"] = Color.FromArgb("#CC333B");
            resources["PrimaryButtonTextColor"] = Color.FromArgb("#FFFFFF");
            resources["PrimaryButtonHoverBackgroundColor"] = Color.FromArgb("#B52D35");
            resources["PrimaryButtonPressedBackgroundColor"] = Color.FromArgb("#A02730");
            resources["BorderColorLight"] = Color.FromArgb("#DEE2E6");
            resources["BorderColorMedium"] = Color.FromArgb("#ADB5BD");
            resources["BorderColorDark"] = Color.FromArgb("#6C757D");
            resources["InputBackgroundColor"] = Color.FromArgb("#FFFFFF");
            resources["InputBorderColor"] = Color.FromArgb("#CED4DA");
            resources["InputTextColor"] = Color.FromArgb("#212529");
            resources["InputPlaceholderColor"] = Color.FromArgb("#6C757D");
            resources["InputFocusBorderColor"] = Color.FromArgb("#CC333B");
            resources["SuccessColor"] = Color.FromArgb("#198754");
            resources["WarningColor"] = Color.FromArgb("#FFC107");
            resources["ErrorColor"] = Color.FromArgb("#DC3545");
            resources["InfoColor"] = Color.FromArgb("#0DCAF0");
            resources["SelectionColor"] = Color.FromArgb("#CC333B");
            resources["SelectionBackgroundColor"] = Color.FromArgb("#FFF2F2");
            resources["HighlightColor"] = Color.FromArgb("#E9ECEF");
            resources["NavigationBackgroundColor"] = Color.FromArgb("#F8F9FA");
            resources["NavigationTextColor"] = Color.FromArgb("#212529");
            resources["NavigationSelectedBackgroundColor"] = Color.FromArgb("#CC333B");
            resources["NavigationSelectedTextColor"] = Color.FromArgb("#FFFFFF");
            resources["NavigationHoverBackgroundColor"] = Color.FromArgb("#E9ECEF");
            resources["SettingsBackgroundColor"] = Color.FromArgb("#FFFFFF");
            resources["SettingsBorderColor"] = Color.FromArgb("#DEE2E6");
            resources["SettingsTextColor"] = Color.FromArgb("#212529");
            resources["SettingsSecondaryTextColor"] = Color.FromArgb("#6C757D");
        }
    }

    #endregion
}
