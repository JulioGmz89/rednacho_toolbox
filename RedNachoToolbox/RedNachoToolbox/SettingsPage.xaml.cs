using Microsoft.Maui.Controls;
using Microsoft.Maui;
using System.Threading.Tasks;

namespace RedNachoToolbox;

public partial class SettingsPage : ContentPage
{
    private bool _isDarkTheme;
    private bool _isSidebarCollapsed;
    private string _closeButtonState = "idle";

    public SettingsPage()
    {
        InitializeComponent();
        InitializeThemeState();
        InitializeSidebarState();

        // Ensure thumb colors refresh once handlers are created
        try
        {
            ThemeSwitch.Loaded += (s, e) => RefreshSwitchThumbColors();
            SidebarCollapseSwitch.Loaded += (s, e) => RefreshSwitchThumbColors();
        }
        catch { /* ignore if not yet available */ }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Run after layout to guarantee handler exists
        try
        {
            Dispatcher.Dispatch(() => RefreshSwitchThumbColors());
        }
        catch { /* ignore */ }
    }

    /// <summary>
    /// Updates the Close (X) button image based on current state (idle/hover/active) and theme
    /// </summary>
    /// <param name="state">One of: idle, hover, active</param>
    private void UpdateCloseButtonImage(string state)
    {
        try
        {
            if (CloseButtonImage == null) return;
            var theme = IsDarkTheme ? "dark" : "light";
            var safeState = (state == "hover" || state == "active") ? state : "idle";
            var source = $"close_settings_{safeState}_{theme}.png";
            CloseButtonImage.Source = source;
            System.Diagnostics.Debug.WriteLine($"Close button image set to: {source}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating close button image: {ex.Message}");
        }
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

    /// <summary>
    /// Updates specific resource keys in all merged dictionaries so DynamicResource lookups
    /// that were defined there (e.g., in Colors.xaml) get refreshed immediately.
    /// </summary>
    private static void PropagateThemeKeys(ResourceDictionary resources, params string[] keys)
    {
        try
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
                    catch { /* ignore per-dictionary errors */ }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PropagateThemeKeys error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets or sets whether sidebar is collapsed
    /// </summary>
    public bool IsSidebarCollapsed
    {
        get => _isSidebarCollapsed;
        set
        {
            if (_isSidebarCollapsed != value)
            {
                _isSidebarCollapsed = value;
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

        // Initialize close button visual for current theme
        UpdateCloseButtonImage("idle");

        // Ensure switch thumbs reflect current toggled state
        RefreshSwitchThumbColors();
    }

    /// <summary>
    /// Initialize the sidebar state based on saved preferences
    /// </summary>
    private void InitializeSidebarState()
    {
        System.Diagnostics.Debug.WriteLine("=== InitializeSidebarState START ===");
        
        // Load saved sidebar preference
        IsSidebarCollapsed = LoadSidebarPreference();
        System.Diagnostics.Debug.WriteLine($"Current sidebar state: {(IsSidebarCollapsed ? "Collapsed" : "Expanded")}");
        
        System.Diagnostics.Debug.WriteLine($"Setting sidebar switch to: {IsSidebarCollapsed}");
        SidebarCollapseSwitch.IsToggled = IsSidebarCollapsed;
        System.Diagnostics.Debug.WriteLine($"Sidebar switch IsToggled set to: {SidebarCollapseSwitch.IsToggled}");
        
        System.Diagnostics.Debug.WriteLine("=== InitializeSidebarState END ===");

        // Ensure switch thumbs reflect current toggled state
        RefreshSwitchThumbColors();
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
        // Notify views to refresh theme-dependent templates
        Microsoft.Maui.Controls.MessagingCenter.Send(this, "ThemeChanged");
        
        // Save theme preference
        System.Diagnostics.Debug.WriteLine("Saving theme preference...");
        SaveThemePreference(isDarkMode);
        System.Diagnostics.Debug.WriteLine("Theme preference saved");
        
        // Ensure switch thumbs reflect current toggled state
        RefreshSwitchThumbColors();
        
        System.Diagnostics.Debug.WriteLine("=== OnThemeSwitchToggled END ===");
    }

    /// <summary>
    /// Handles sidebar collapse switch toggle events
    /// </summary>
    private void OnSidebarCollapseSwitchToggled(object sender, ToggledEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("=== OnSidebarCollapseSwitchToggled START ===");
        var isCollapsed = e.Value;
        System.Diagnostics.Debug.WriteLine($"Sidebar switch toggled to: {(isCollapsed ? "Collapsed" : "Expanded")}");
        
        IsSidebarCollapsed = isCollapsed;
        System.Diagnostics.Debug.WriteLine($"IsSidebarCollapsed property set to: {IsSidebarCollapsed}");
        
        // Save sidebar preference
        System.Diagnostics.Debug.WriteLine("Saving sidebar preference...");
        SaveSidebarPreference(isCollapsed);
        System.Diagnostics.Debug.WriteLine("Sidebar preference saved");
        
        // Ensure switch thumbs reflect current toggled state
        RefreshSwitchThumbColors();

        System.Diagnostics.Debug.WriteLine("=== OnSidebarCollapseSwitchToggled END ===");
    }

    /// <summary>
    /// Pointer entered on the Close (X) button - show hover visual
    /// </summary>
    private void OnClosePointerEntered(object sender, PointerEventArgs e)
    {
        _closeButtonState = "hover";
        UpdateCloseButtonImage(_closeButtonState);
    }

    /// <summary>
    /// Pointer exited the Close (X) button - return to idle
    /// </summary>
    private void OnClosePointerExited(object sender, PointerEventArgs e)
    {
        _closeButtonState = "idle";
        UpdateCloseButtonImage(_closeButtonState);
    }

    /// <summary>
    /// Tapped the Close (X) button - show active visual and navigate back
    /// </summary>
    private async void OnCloseTapped(object sender, EventArgs e)
    {
        try
        {
            _closeButtonState = "active";
            UpdateCloseButtonImage(_closeButtonState);
            await Task.Delay(120);

            // Reuse back navigation with robust error handling
            OnBackButtonClicked(sender, EventArgs.Empty);
        }
        catch
        {
            // ignore
        }
        finally
        {
            _closeButtonState = "idle";
            UpdateCloseButtonImage(_closeButtonState);
        }
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
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
            var errorDetails = $"{ex.GetType().Name}: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorDetails += $"\nInner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}";
            }
            try
            {
                await DisplayAlert("Navigation Error", $"Unable to go back.\n\n{errorDetails}", "OK");
            }
            catch { /* ignore UI errors in exceptional states */ }
            
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
                // Ensure keys defined in merged dictionaries (e.g., Colors.xaml) are also updated
                PropagateThemeKeys(Application.Current.Resources,
                    "CardBackgroundColor", "CardAccentBackgroundColor", "CardShadowColor",
                    "TextColor", "TextColorSecondary", "TextColorTertiary", "HighlightColor",
                    "PrimaryRed", "InteractivePrimaryColor", "InteractiveSecondaryColor", "BorderInteractiveColor");
                // Set UserAppTheme to trigger control refreshes that listen to OS theme changes
                Application.Current.UserAppTheme = isDarkTheme ? AppTheme.Dark : AppTheme.Light;
                Application.Current.MainPage?.ForceLayout();
                // Notify views to refresh theme-dependent templates
                Microsoft.Maui.Controls.MessagingCenter.Send(this, "ThemeChanged");
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

            // Refresh close button visual for new theme
            UpdateCloseButtonImage(_closeButtonState);

            // Ensure switch thumbs reflect current toggled state
            RefreshSwitchThumbColors();
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
            
            // Brand / Primary Colors
            resources["PrimaryRed"] = Color.FromArgb("#E57373"); // Softer brand red for dark per MejoraDiseñoUI
            
            // Background Colors
            resources["PageBackgroundColor"] = Color.FromArgb("#121212");
            resources["SidebarBackgroundColor"] = Color.FromArgb("#1E1E1E");
            resources["CardBackgroundColor"] = Color.FromArgb("#2A2A2A");
            resources["CardAccentBackgroundColor"] = Color.FromArgb("#333333");
            resources["CardShadowColor"] = Color.FromArgb("#666666");
            resources["ContentBackgroundColor"] = Color.FromArgb("#1A1A1A");
            
            // Text Colors (ergonomic dark)
            resources["TextColor"] = Color.FromArgb("#E0E0E0");
            resources["TextColorSecondary"] = Color.FromArgb("#BDBDBD");
            resources["TextColorTertiary"] = Color.FromArgb("#757575"); // placeholder/tertiary
            resources["TextColorDisabled"] = Color.FromArgb("#616161");
            
            // Button Colors
            resources["ButtonBackgroundColor"] = Color.FromArgb("#3A3A3A");
            resources["ButtonTextColor"] = Color.FromArgb("#FFFFFF");
            resources["ButtonBorderColor"] = Color.FromArgb("#555555");
            resources["ButtonHoverBackgroundColor"] = Color.FromArgb("#4A4A4A");
            resources["ButtonPressedBackgroundColor"] = Color.FromArgb("#2A2A2A");
            // Interactive tokens (dark)
            resources["InteractivePrimaryColor"] = Color.FromArgb("#90CAF9");
            resources["InteractiveSecondaryColor"] = Color.FromArgb("#81D4FA");
            resources["BorderInteractiveColor"] = Color.FromArgb("#42A5F5");
            resources["TextInteractiveColor"] = Color.FromArgb("#121212");
            
            // Primary Button Colors (interactive blue in dark)
            resources["PrimaryButtonBackgroundColor"] = Color.FromArgb("#90CAF9");
            resources["PrimaryButtonTextColor"] = Color.FromArgb("#121212");
            resources["PrimaryButtonHoverBackgroundColor"] = Color.FromArgb("#81D4FA");
            resources["PrimaryButtonPressedBackgroundColor"] = Color.FromArgb("#42A5F5");
            
            // Border Colors (primary border ~ #3C3C3C)
            resources["BorderColorLight"] = Color.FromArgb("#3C3C3C");
            resources["BorderColorMedium"] = Color.FromArgb("#666666");
            resources["BorderColorDark"] = Color.FromArgb("#333333");
            
            // Input Colors (focus uses interactive border)
            resources["InputBackgroundColor"] = Color.FromArgb("#2A2A2A");
            resources["InputBorderColor"] = Color.FromArgb("#3C3C3C");
            resources["InputTextColor"] = Color.FromArgb("#E0E0E0");
            resources["InputPlaceholderColor"] = Color.FromArgb("#757575");
            resources["InputFocusBorderColor"] = Color.FromArgb("#42A5F5");
            
            // Status Colors
            resources["SuccessColor"] = Color.FromArgb("#4CAF50");
            resources["WarningColor"] = Color.FromArgb("#FF9800");
            resources["ErrorColor"] = Color.FromArgb("#F44336");
            resources["InfoColor"] = Color.FromArgb("#2196F3");
            
            // Selection / Hover Colors
            resources["SelectionColor"] = Color.FromArgb("#90CAF9");
            resources["SelectionBackgroundColor"] = Color.FromArgb("#263238");
            resources["HighlightColor"] = Color.FromRgba(255,255,255,0.08);
            
            // Navigation Colors
            resources["NavigationBackgroundColor"] = Color.FromArgb("#1E1E1E");
            resources["NavigationTextColor"] = Color.FromArgb("#E0E0E0");
            resources["NavigationSelectedBackgroundColor"] = Color.FromArgb("#CC333B");
            resources["NavigationSelectedTextColor"] = Color.FromArgb("#FFFFFF");
            resources["NavigationHoverBackgroundColor"] = Color.FromArgb("#3A3A3A");
            
            // Settings Colors
            resources["SettingsBackgroundColor"] = Color.FromArgb("#2A2A2A");
            resources["SettingsBorderColor"] = Color.FromArgb("#3C3C3C");
            resources["SettingsTextColor"] = Color.FromArgb("#E0E0E0");
            resources["SettingsSecondaryTextColor"] = Color.FromArgb("#BDBDBD");
        }
        else
        {
            // Apply Light Theme Colors
            System.Diagnostics.Debug.WriteLine("Applying light theme colors...");
            
            // Primary Colors
            resources["PrimaryRed"] = Color.FromArgb("#D32F2F");
            
            // Background Colors
            resources["PageBackgroundColor"] = Color.FromArgb("#F5F5F5");
            resources["SidebarBackgroundColor"] = Color.FromArgb("#F8F9FA");
            resources["CardBackgroundColor"] = Color.FromArgb("#FFFFFF");
            resources["CardAccentBackgroundColor"] = Color.FromArgb("#F2F2F2");
            resources["CardShadowColor"] = Color.FromArgb("#ACACAC");
            resources["ContentBackgroundColor"] = Color.FromArgb("#FAFAFA");
            
            // Text Colors (refined light)
            resources["TextColor"] = Color.FromArgb("#212121");
            resources["TextColorSecondary"] = Color.FromArgb("#616161");
            resources["TextColorTertiary"] = Color.FromArgb("#9E9E9E");
            resources["TextColorDisabled"] = Color.FromArgb("#BDBDBD");
            
            // Button Colors
            resources["ButtonBackgroundColor"] = Color.FromArgb("#F8F9FA");
            resources["ButtonTextColor"] = Color.FromArgb("#212529");
            resources["ButtonBorderColor"] = Color.FromArgb("#DEE2E6");
            resources["ButtonHoverBackgroundColor"] = Color.FromArgb("#E9ECEF");
            resources["ButtonPressedBackgroundColor"] = Color.FromArgb("#DEE2E6");
            // Interactive tokens (light)
            resources["InteractivePrimaryColor"] = Color.FromArgb("#1976D2");
            resources["InteractiveSecondaryColor"] = Color.FromArgb("#0288D1");
            resources["BorderInteractiveColor"] = Color.FromArgb("#2196F3");
            resources["TextInteractiveColor"] = Color.FromArgb("#FFFFFF");
            
            // Primary Button Colors (interactive blue)
            resources["PrimaryButtonBackgroundColor"] = Color.FromArgb("#1976D2");
            resources["PrimaryButtonTextColor"] = Color.FromArgb("#FFFFFF");
            resources["PrimaryButtonHoverBackgroundColor"] = Color.FromArgb("#0288D1");
            resources["PrimaryButtonPressedBackgroundColor"] = Color.FromArgb("#1565C0");
            
            // Border Colors
            resources["BorderColorLight"] = Color.FromArgb("#DEE2E6");
            resources["BorderColorMedium"] = Color.FromArgb("#ADB5BD");
            resources["BorderColorDark"] = Color.FromArgb("#6C757D");
            
            // Input Colors (focus uses interactive border)
            resources["InputBackgroundColor"] = Color.FromArgb("#FFFFFF");
            resources["InputBorderColor"] = Color.FromArgb("#CED4DA");
            resources["InputTextColor"] = Color.FromArgb("#212121");
            resources["InputPlaceholderColor"] = Color.FromArgb("#9E9E9E");
            resources["InputFocusBorderColor"] = Color.FromArgb("#2196F3");
            
            // Status Colors
            resources["SuccessColor"] = Color.FromArgb("#198754");
            resources["WarningColor"] = Color.FromArgb("#FFC107");
            resources["ErrorColor"] = Color.FromArgb("#DC3545");
            resources["InfoColor"] = Color.FromArgb("#0DCAF0");
            
            // Selection / Hover Colors
            resources["SelectionColor"] = Color.FromArgb("#1976D2");
            resources["SelectionBackgroundColor"] = Color.FromArgb("#E8F0FE");
            resources["HighlightColor"] = Color.FromRgba(0,0,0,0.04);
            
            // Navigation Colors
            resources["NavigationBackgroundColor"] = Color.FromArgb("#F8F9FA");
            resources["NavigationTextColor"] = Color.FromArgb("#212121");
            resources["NavigationSelectedBackgroundColor"] = Color.FromArgb("#E8F0FE");
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

    /// <summary>
    /// Forces the correct thumb colors for switches based on their toggled state.
    /// Ensures the ON state uses a white thumb even on first load and after theme changes.
    /// </summary>
    private void RefreshSwitchThumbColors()
    {
        try
        {
            // Use dispatcher to ensure UI thread and that handlers are ready
            Dispatcher.Dispatch(() =>
            {
                if (ThemeSwitch != null)
                {
                    if (ThemeSwitch.IsToggled)
                        ThemeSwitch.ThumbColor = Color.FromArgb("#FFFFFF");
                    else
                        ThemeSwitch.ClearValue(Switch.ThumbColorProperty);
                }

                if (SidebarCollapseSwitch != null)
                {
                    if (SidebarCollapseSwitch.IsToggled)
                        SidebarCollapseSwitch.ThumbColor = Color.FromArgb("#FFFFFF");
                    else
                        SidebarCollapseSwitch.ClearValue(Switch.ThumbColorProperty);
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RefreshSwitchThumbColors error: {ex.Message}");
        }
    }

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
    /// Saves the user's sidebar preference to application preferences
    /// </summary>
    private void SaveSidebarPreference(bool isSidebarCollapsed)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"=== SaveSidebarPreference: {(isSidebarCollapsed ? "Collapsed" : "Expanded")} ===");
            Preferences.Set("IsSidebarCollapsed", isSidebarCollapsed);
            System.Diagnostics.Debug.WriteLine($"✓ Sidebar preference saved successfully: {(isSidebarCollapsed ? "Collapsed" : "Expanded")}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR saving sidebar preference: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Loads the user's saved sidebar preference
    /// </summary>
    public static bool LoadSidebarPreference()
    {
        try
        {
            return Preferences.Get("IsSidebarCollapsed", false); // Default to expanded sidebar
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading sidebar preference: {ex.Message}");
            return false; // Default to expanded sidebar on error
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
            // Ensure keys defined in merged dictionaries (e.g., Colors.xaml) are also updated
            PropagateThemeKeys(resources,
                "CardBackgroundColor", "CardAccentBackgroundColor", "CardShadowColor",
                "TextColor", "TextColorSecondary", "TextColorTertiary", "HighlightColor",
                "PrimaryRed", "InteractivePrimaryColor", "InteractiveSecondaryColor", "BorderInteractiveColor");
            // Also set UserAppTheme to ensure consistent visual refresh across controls
            Application.Current.UserAppTheme = isDarkTheme ? AppTheme.Dark : AppTheme.Light;
            Application.Current.MainPage?.ForceLayout();

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
            resources["HighlightColor"] = Color.FromRgba(255,255,255,0.08);
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
        else
        {
            // Apply Light Theme Colors
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
            resources["HighlightColor"] = Color.FromRgba(0,0,0,0.04);
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
    }

    #endregion
}
