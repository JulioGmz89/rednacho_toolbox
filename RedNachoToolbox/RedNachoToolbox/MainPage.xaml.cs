using RedNachoToolbox.ViewModels;

namespace RedNachoToolbox;

public partial class MainPage : ContentPage
{
    /// <summary>
    /// Gets the MainViewModel instance bound to this page
    /// </summary>
    public MainViewModel ViewModel { get; private set; }

    public MainPage()
    {
        InitializeComponent();
        
        // Initialize and set the ViewModel
        ViewModel = new MainViewModel();
        BindingContext = ViewModel;
        
        // Subscribe to appearing event to refresh ViewModel state
        this.Appearing += OnPageAppearing;
    }

    private void OnPageAppearing(object? sender, EventArgs e)
    {
        // Refresh ViewModel state when page appears (e.g., returning from Settings)
        RefreshViewModelState();
    }

    /// <summary>
    /// Refreshes the ViewModel state from saved preferences
    /// </summary>
    private void RefreshViewModelState()
    {
        // Update sidebar state from preferences
        var isSidebarCollapsed = SettingsPage.LoadSidebarPreference();
        ViewModel.UpdateSidebarState(isSidebarCollapsed);
        
        // Update theme state from current application resources
        var isDarkTheme = IsCurrentlyDarkTheme();
        ViewModel.UpdateThemeState(isDarkTheme);
        
        System.Diagnostics.Debug.WriteLine($"MainPage refreshed ViewModel state - Sidebar: {(isSidebarCollapsed ? "Collapsed" : "Expanded")}, Theme: {(isDarkTheme ? "Dark" : "Light")}");
    }

    /// <summary>
    /// Determines if the current theme is dark by checking applied color values
    /// </summary>
    private bool IsCurrentlyDarkTheme()
    {
        try
        {
            var resources = Application.Current?.Resources;
            if (resources == null) return false;

            // Check the current PageBackgroundColor to determine theme
            if (resources.TryGetValue("PageBackgroundColor", out var bgColorObj) && bgColorObj is Color bgColor)
            {
                // Dark theme background is very dark, light theme background is white
                return bgColor.Red < 0.5f && bgColor.Green < 0.5f && bgColor.Blue < 0.5f;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error detecting current theme in MainPage: {ex.Message}");
        }
        
        return false; // Default to light theme
    }

    #region Search Event Handlers
    
    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        if (sender is SearchBar searchBar)
        {
            var searchText = searchBar.Text?.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                // Update the ViewModel's search text
                ViewModel.SearchText = searchText;
                System.Diagnostics.Debug.WriteLine($"Search executed for: {searchText}");
                
                // Unfocus the search bar
                searchBar.Unfocus();
            }
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.Trim() ?? string.Empty;
        
        // Update the ViewModel's search text for real-time filtering
        ViewModel.SearchText = searchText;
        System.Diagnostics.Debug.WriteLine($"Search text changed: {searchText}");
    }

    #endregion

    #region Hover Event Handlers

    // Dashboard Hover Events
    private void OnDashboardPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            // Theme-aware hover color
            var hoverColor = ViewModel.IsDarkTheme 
                ? Color.FromArgb("#2A2A2A") // Dark gray for dark theme
                : Color.FromArgb("#F5F5F5"); // Light gray for light theme
            
            frame.BackgroundColor = hoverColor;
            System.Diagnostics.Debug.WriteLine($"Dashboard hover entered - Theme: {(ViewModel.IsDarkTheme ? "Dark" : "Light")}, Color: {hoverColor}");
        }
    }

    private void OnDashboardPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            frame.BackgroundColor = Colors.Transparent;
            System.Diagnostics.Debug.WriteLine("Dashboard hover exited - Returned to transparent");
        }
    }

    // Documentation Hover Events
    private void OnDocumentationPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            // Theme-aware hover color
            var hoverColor = ViewModel.IsDarkTheme 
                ? Color.FromArgb("#2A2A2A") // Dark gray for dark theme
                : Color.FromArgb("#F5F5F5"); // Light gray for light theme
            
            frame.BackgroundColor = hoverColor;
            System.Diagnostics.Debug.WriteLine($"Documentation hover entered - Theme: {(ViewModel.IsDarkTheme ? "Dark" : "Light")}, Color: {hoverColor}");
        }
    }

    private void OnDocumentationPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            frame.BackgroundColor = Colors.Transparent;
            System.Diagnostics.Debug.WriteLine("Documentation hover exited - Returned to transparent");
        }
    }

    // Settings Hover Events
    private void OnSettingsPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            // Theme-aware hover color
            var hoverColor = ViewModel.IsDarkTheme 
                ? Color.FromArgb("#2A2A2A") // Dark gray for dark theme
                : Color.FromArgb("#F5F5F5"); // Light gray for light theme
            
            frame.BackgroundColor = hoverColor;
            System.Diagnostics.Debug.WriteLine($"Settings hover entered - Theme: {(ViewModel.IsDarkTheme ? "Dark" : "Light")}, Color: {hoverColor}");
        }
    }

    private void OnSettingsPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            frame.BackgroundColor = Colors.Transparent;
            System.Diagnostics.Debug.WriteLine("Settings hover exited - Returned to transparent");
        }
    }

    #endregion

    #region Navigation Event Handlers

    private async void OnDashboardClicked(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("OnDashboardClicked - Event handler called");
            
            // Enhanced visual feedback for Frame-based button with theme-aware colors
            if (sender is Frame frame)
            {
                System.Diagnostics.Debug.WriteLine("OnDashboardClicked - Frame found, applying visual states");
                
                // Pressed state - theme-aware pressed color
                var pressedColor = ViewModel.IsDarkTheme 
                    ? Color.FromArgb("#404040") // Darker gray for dark theme
                    : Color.FromArgb("#E0E0E0"); // Light gray for light theme
                
                frame.BackgroundColor = pressedColor;
                frame.Opacity = 0.9;
                System.Diagnostics.Debug.WriteLine($"OnDashboardClicked - Applied pressed state ({pressedColor})");
                await Task.Delay(150); // Quick pressed feedback
                
                // Return to hover state briefly
                var hoverColor = ViewModel.IsDarkTheme 
                    ? Color.FromArgb("#2A2A2A") // Dark gray for dark theme
                    : Color.FromArgb("#F5F5F5"); // Light gray for light theme
                
                frame.BackgroundColor = hoverColor;
                frame.Opacity = 1.0;
                System.Diagnostics.Debug.WriteLine($"OnDashboardClicked - Returned to hover state ({hoverColor})");
                await Task.Delay(100); // Brief hover feedback
                
                // Return to normal
                frame.BackgroundColor = Colors.Transparent;
                System.Diagnostics.Debug.WriteLine("OnDashboardClicked - Returned to normal state");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OnDashboardClicked - Sender is not a Frame!");
            }
            
            // Clear any active filters to show all applications (Dashboard view)
            ViewModel.ClearFilters();
            System.Diagnostics.Debug.WriteLine("Dashboard view activated - filters cleared");
            
            // TODO: Navigate to Dashboard/All Applications view
            await DisplayAlert("Dashboard", "Dashboard functionality will be implemented here.", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Dashboard navigation error: {ex.Message}");
            await DisplayAlert("Error", "Could not access Dashboard.", "OK");
        }
    }

    private async void OnDocumentationClicked(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("OnDocumentationClicked - Event handler called");
            
            // Enhanced visual feedback for Frame-based button with more visible colors
            if (sender is Frame frame)
            {
                System.Diagnostics.Debug.WriteLine("OnDocumentationClicked - Frame found, applying visual states");
                
                // Pressed state - very visible green for testing
                frame.BackgroundColor = Colors.Green;
                frame.Opacity = 0.8;
                System.Diagnostics.Debug.WriteLine("OnDocumentationClicked - Applied pressed state (Green)");
                await Task.Delay(200); // Longer delay for visibility
                
                // Hover state - very visible orange for testing
                frame.BackgroundColor = Colors.Orange;
                frame.Opacity = 0.9;
                System.Diagnostics.Debug.WriteLine("OnDocumentationClicked - Applied hover state (Orange)");
                await Task.Delay(200); // Longer delay for visibility
                
                // Return to normal
                frame.BackgroundColor = Colors.Transparent;
                frame.Opacity = 1.0;
                System.Diagnostics.Debug.WriteLine("OnDocumentationClicked - Returned to normal state");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OnDocumentationClicked - Sender is not a Frame!");
            }
            
            // TODO: Navigate to Documentation view
            // This will be connected to the ViewModel and navigation service in future iterations
            await DisplayAlert("Documentation", "Documentation functionality will be implemented here.", "OK");
            
            System.Diagnostics.Debug.WriteLine("Documentation view selected");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Documentation navigation error: {ex.Message}");
            await DisplayAlert("Error", "Could not access Documentation.", "OK");
        }
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("OnSettingsClicked - Event handler called");
            
            // Enhanced visual feedback for Frame-based button with more visible colors
            if (sender is Frame frame)
            {
                System.Diagnostics.Debug.WriteLine("OnSettingsClicked - Frame found, applying visual states");
                
                // Pressed state - very visible purple for testing
                frame.BackgroundColor = Colors.Purple;
                frame.Opacity = 0.8;
                System.Diagnostics.Debug.WriteLine("OnSettingsClicked - Applied pressed state (Purple)");
                await Task.Delay(200); // Longer delay for visibility
                
                // Hover state - very visible yellow for testing
                frame.BackgroundColor = Colors.Yellow;
                frame.Opacity = 0.9;
                System.Diagnostics.Debug.WriteLine("OnSettingsClicked - Applied hover state (Yellow)");
                await Task.Delay(200); // Longer delay for visibility
                
                // Return to normal
                frame.BackgroundColor = Colors.Transparent;
                frame.Opacity = 1.0;
                System.Diagnostics.Debug.WriteLine("OnSettingsClicked - Returned to normal state");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OnSettingsClicked - Sender is not a Frame!");
            }
            
            // Navigate to SettingsPage
            var settingsPage = new SettingsPage();
            await Navigation.PushAsync(settingsPage);
            
            System.Diagnostics.Debug.WriteLine("Navigated to Settings page");
        }
        catch (Exception ex)
        {
            // Handle navigation error gracefully
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            await DisplayAlert("Navigation Error", "Unable to open Settings page. Please try again.", "OK");
        }
    }

    #endregion

    #region Helper Methods

    private void ResetButtonStyles()
    {
        // Note: With Frame-based buttons using TapGestureRecognizer, 
        // visual feedback is now handled individually in each event handler
        // This method is kept for compatibility but no longer needed
        System.Diagnostics.Debug.WriteLine("ResetButtonStyles called - using Frame-based buttons now");
    }

    #endregion
}
