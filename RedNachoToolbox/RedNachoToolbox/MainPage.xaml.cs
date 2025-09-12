using RedNachoToolbox.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using RedNachoToolbox.Views;
using RedNachoToolbox.Models;
using RedNachoToolbox.Tools.MarkdownToPdf;

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
        
        // Initialize main content with Dashboard
        MainContentHost.Content = new DashboardView { BindingContext = ViewModel };
        
        // Subscribe to tool open messages from views
        try
        {
            MessagingCenter.Subscribe<DashboardView, ToolInfo>(this, "OpenTool", (sender, tool) => ShowTool(tool));
            MessagingCenter.Subscribe<AllToolsView, ToolInfo>(this, "OpenTool", (sender, tool) => ShowTool(tool));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error subscribing to OpenTool: {ex.Message}");
        }
        
        // Subscribe to appearing event to refresh ViewModel state
        this.Appearing += OnPageAppearing;
    }

    private void ShowTool(ToolInfo tool)
    {
        try
        {
            // Map known tools to ContentViews (keep sidebar)
            ContentView? view = null;
            if (tool.TargetType == typeof(MarkdownToPdfView) || tool.Name.Contains("Markdown", StringComparison.OrdinalIgnoreCase))
            {
                view = new MarkdownToPdfView();
            }

            if (view != null)
            {
                // Keep binding to MainViewModel for shared theme state if needed
                // Tool view manages its own internal ViewModel
                MainContentHost.Content = view;
                System.Diagnostics.Debug.WriteLine($"Hosted tool in content area: {tool.Name}");
            }
            else
            {
                // Fallback: show dashboard if not mapped
                MainContentHost.Content = new DashboardView { BindingContext = ViewModel };
                System.Diagnostics.Debug.WriteLine($"No mapping for tool '{tool.Name}', showing Dashboard");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error hosting tool: {ex.Message}");
        }
    }

    private void OnPageAppearing(object? sender, EventArgs e)
    {
        // Refresh ViewModel state when page appears (e.g., returning from Settings)
        RefreshViewModelState();
        
        // Update active button backgrounds to fix initial color bug and theme change bug
        UpdateActiveButtonBackgrounds();
        
        System.Diagnostics.Debug.WriteLine("Page appearing - ViewModel refreshed and active button backgrounds updated");
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

    // Dashboard Hover Events - No hover effect when already active
    private void OnDashboardPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            // Don't show hover effect if button is already active (same color)
            if (ViewModel.IsDashboardActive)
            {
                System.Diagnostics.Debug.WriteLine("Dashboard hover entered - Button is active, no hover effect needed");
                return;
            }
            
            // Theme-aware hover color (same as active color)
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
            // Don't change color if button is active (should stay active color)
            if (ViewModel.IsDashboardActive)
            {
                System.Diagnostics.Debug.WriteLine("Dashboard hover exited - Button is active, maintaining active color");
                return;
            }
            
            frame.BackgroundColor = Colors.Transparent;
            System.Diagnostics.Debug.WriteLine("Dashboard hover exited - Returned to transparent");
        }
    }

    // Documentation Hover Events - No hover effect when already active
    private void OnDocumentationPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            // Don't show hover effect if button is already active (same color)
            if (ViewModel.IsDocumentationActive)
            {
                System.Diagnostics.Debug.WriteLine("Documentation hover entered - Button is active, no hover effect needed");
                return;
            }
            
            // Theme-aware hover color (same as active color)
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
            // Don't change color if button is active (should stay active color)
            if (ViewModel.IsDocumentationActive)
            {
                System.Diagnostics.Debug.WriteLine("Documentation hover exited - Button is active, maintaining active color");
                return;
            }
            
            frame.BackgroundColor = Colors.Transparent;
            System.Diagnostics.Debug.WriteLine("Documentation hover exited - Returned to transparent");
        }
    }

    // Settings Hover Events - Using more contrasting colors for better visibility
    private void OnSettingsPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            // More contrasting hover color for Settings (since it has different background)
            var hoverColor = ViewModel.IsDarkTheme 
                ? Color.FromArgb("#404040") // Lighter gray for dark theme (more contrast)
                : Color.FromArgb("#E0E0E0"); // Darker gray for light theme (more contrast)
            
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
            
            // Animate previous active indicators to deactivate (if any)
            if (ViewModel.IsDocumentationActive)
            {
                _ = AnimateIndicatorTransition(DocumentationIndicatorCapsule, false);
                _ = AnimateCollapsedDotTransition(DocumentationIndicatorDotCollapsed, false);
            }
            
            // Set Dashboard as active page
            ViewModel.SetActivePage("Dashboard");
            
            // Animate new active indicators (both expanded capsule and collapsed dot)
            var capsuleTask = AnimateIndicatorTransition(DashboardIndicatorCapsule, true);
            var dotTask = AnimateCollapsedDotTransition(DashboardIndicatorDotCollapsed, true);
            await Task.WhenAll(capsuleTask, dotTask);
            
            // Update active button backgrounds
            UpdateActiveButtonBackgrounds();
            
            // Clear any active filters to show all applications (Dashboard view)
            ViewModel.ClearFilters();
            System.Diagnostics.Debug.WriteLine("Dashboard view activated - filters cleared, set as active page");
            
            // Swap main content to Dashboard view
            MainContentHost.Content = new DashboardView { BindingContext = ViewModel };
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
            
            // Enhanced visual feedback for Frame-based button with theme-aware colors
            if (sender is Frame frame)
            {
                System.Diagnostics.Debug.WriteLine("OnDocumentationClicked - Frame found, applying visual states");
                
                // Pressed state - theme-aware pressed color
                var pressedColor = ViewModel.IsDarkTheme 
                    ? Color.FromArgb("#404040") // Darker gray for dark theme
                    : Color.FromArgb("#E0E0E0"); // Light gray for light theme
                
                frame.BackgroundColor = pressedColor;
                frame.Opacity = 0.9;
                System.Diagnostics.Debug.WriteLine($"OnDocumentationClicked - Applied pressed state ({pressedColor})");
                await Task.Delay(150); // Quick pressed feedback
                
                // Return to hover state briefly
                var hoverColor = ViewModel.IsDarkTheme 
                    ? Color.FromArgb("#2A2A2A") // Dark gray for dark theme
                    : Color.FromArgb("#F5F5F5"); // Light gray for light theme
                
                frame.BackgroundColor = hoverColor;
                frame.Opacity = 1.0;
                System.Diagnostics.Debug.WriteLine($"OnDocumentationClicked - Returned to hover state ({hoverColor})");
                await Task.Delay(100); // Brief hover feedback
                
                // Return to normal
                frame.BackgroundColor = Colors.Transparent;
                System.Diagnostics.Debug.WriteLine("OnDocumentationClicked - Returned to normal state");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OnDocumentationClicked - Sender is not a Frame!");
            }
            
            // Animate previous active indicators to deactivate (if any)
            if (ViewModel.IsDashboardActive)
            {
                _ = AnimateIndicatorTransition(DashboardIndicatorCapsule, false);
                _ = AnimateCollapsedDotTransition(DashboardIndicatorDotCollapsed, false);
            }
            
            // Set Documentation as active page
            ViewModel.SetActivePage("Documentation");
            
            // Animate new active indicators (both expanded capsule and collapsed dot)
            var capsuleTask = AnimateIndicatorTransition(DocumentationIndicatorCapsule, true);
            var dotTask = AnimateCollapsedDotTransition(DocumentationIndicatorDotCollapsed, true);
            await Task.WhenAll(capsuleTask, dotTask);
            
            // Update active button backgrounds
            UpdateActiveButtonBackgrounds();
            
            // Swap main content to AllTools view (acts as documentation/catalog)
            MainContentHost.Content = new AllToolsView { BindingContext = ViewModel };
            
            System.Diagnostics.Debug.WriteLine("Documentation view selected - set as active page");
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
            
            // Enhanced visual feedback for Frame-based button with theme-aware colors
            if (sender is Frame frame)
            {
                System.Diagnostics.Debug.WriteLine("OnSettingsClicked - Frame found, applying visual states");
                
                // Pressed state - theme-aware pressed color
                var pressedColor = ViewModel.IsDarkTheme 
                    ? Color.FromArgb("#404040") // Darker gray for dark theme
                    : Color.FromArgb("#E0E0E0"); // Light gray for light theme
                
                frame.BackgroundColor = pressedColor;
                frame.Opacity = 0.9;
                System.Diagnostics.Debug.WriteLine($"OnSettingsClicked - Applied pressed state ({pressedColor})");
                await Task.Delay(150); // Quick pressed feedback
                
                // Return to hover state briefly
                var hoverColor = ViewModel.IsDarkTheme 
                    ? Color.FromArgb("#2A2A2A") // Dark gray for dark theme
                    : Color.FromArgb("#F5F5F5"); // Light gray for light theme
                
                frame.BackgroundColor = hoverColor;
                frame.Opacity = 1.0;
                System.Diagnostics.Debug.WriteLine($"OnSettingsClicked - Returned to hover state ({hoverColor})");
                await Task.Delay(100); // Brief hover feedback
                
                // Return to normal
                frame.BackgroundColor = Colors.Transparent;
                System.Diagnostics.Debug.WriteLine("OnSettingsClicked - Returned to normal state");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OnSettingsClicked - Sender is not a Frame!");
            }
            
            // Note: Settings doesn't change active page state since it navigates to separate view
            // The previous active page (Dashboard/Documentation) should remain active when returning

            // Prefer Shell navigation with registered route; fallback to PushAsync if needed
            try
            {
                System.Diagnostics.Debug.WriteLine("Attempting Shell navigation to SettingsPage route...");
                await Shell.Current.GoToAsync(nameof(SettingsPage));
                System.Diagnostics.Debug.WriteLine("Shell navigation to Settings succeeded.");
            }
            catch (Exception shellEx)
            {
                System.Diagnostics.Debug.WriteLine($"Shell navigation failed: {shellEx}");
                try
                {
                    System.Diagnostics.Debug.WriteLine("Falling back to Navigation.PushAsync(new SettingsPage())...");
                    var settingsPage = new SettingsPage();
                    await Navigation.PushAsync(settingsPage);
                    System.Diagnostics.Debug.WriteLine("PushAsync to Settings succeeded.");
                }
                catch (Exception pushEx)
                {
                    // Throw aggregate so outer catch can show full details
                    throw new AggregateException("Navigation to Settings failed using both Shell and PushAsync.", shellEx, pushEx);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle navigation error gracefully
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
            var errorDetails = $"{ex.GetType().Name}: {ex.Message}";
            if (ex is AggregateException agg && agg.InnerExceptions?.Count > 0)
            {
                int idx = 1;
                foreach (var ie in agg.InnerExceptions)
                {
                    errorDetails += $"\nInner[{idx}]: {ie.GetType().Name}: {ie.Message}";
                    idx++;
                }
            }
            else if (ex.InnerException != null)
            {
                errorDetails += $"\nInner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}";
            }
            await DisplayAlert("Navigation Error", $"Unable to open Settings page.\n\n{errorDetails}", "OK");
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Updates the active background colors for all buttons based on current active page
    /// Active background color matches hover color for consistency
    /// </summary>
    private void UpdateActiveButtonBackgrounds()
    {
        try
        {
            // Active background color matches hover color (same as hover state)
            var activeColor = ViewModel.IsDarkTheme 
                ? Color.FromArgb("#2A2A2A") // Same as hover - Dark gray for dark theme
                : Color.FromArgb("#F5F5F5"); // Same as hover - Light gray for light theme
            
            var transparentColor = Colors.Transparent;
            
            // Update expanded buttons
            if (DashboardButtonFrame != null)
            {
                DashboardButtonFrame.BackgroundColor = ViewModel.IsDashboardActive ? activeColor : transparentColor;
            }
            
            if (DocumentationButtonFrame != null)
            {
                DocumentationButtonFrame.BackgroundColor = ViewModel.IsDocumentationActive ? activeColor : transparentColor;
            }
            
            // Update collapsed buttons
            if (DashboardButtonFrameCollapsed != null)
            {
                DashboardButtonFrameCollapsed.BackgroundColor = ViewModel.IsDashboardActive ? activeColor : transparentColor;
            }
            
            if (DocumentationButtonFrameCollapsed != null)
            {
                DocumentationButtonFrameCollapsed.BackgroundColor = ViewModel.IsDocumentationActive ? activeColor : transparentColor;
            }
            
            System.Diagnostics.Debug.WriteLine($"Updated active button backgrounds (matching hover colors) - Active page: {ViewModel.ActivePage}, Theme: {(ViewModel.IsDarkTheme ? "Dark" : "Light")}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating active button backgrounds: {ex.Message}");
        }
    }

    private void ResetButtonStyles()
    {
        // Note: With Frame-based buttons using TapGestureRecognizer, 
        // visual feedback is now handled individually in each event handler
        // This method is kept for compatibility but no longer needed
        System.Diagnostics.Debug.WriteLine("ResetButtonStyles called - using Frame-based buttons now");
    }

    /// <summary>
    /// Animates the collapsed dot indicator transition from small dot to full size
    /// </summary>
    /// <param name="targetDot">The Ellipse element to animate</param>
    /// <param name="isActivating">True if activating (small to full), false if deactivating</param>
    private async Task AnimateCollapsedDotTransition(Ellipse? targetDot, bool isActivating)
    {
        if (targetDot == null) return;

        try
        {
            const uint animationDuration = 250; // milliseconds - slightly faster than capsule
            const double smallDotSize = 2.0; // Very small starting size
            const double fullDotSize = 8.0; // Current full size

            if (isActivating)
            {
                // Animation from small dot to full size
                targetDot.WidthRequest = smallDotSize;
                targetDot.HeightRequest = smallDotSize;
                targetDot.IsVisible = true;

                // Create custom animation for width
                var widthAnimation = new Animation(
                    v => targetDot.WidthRequest = v,
                    smallDotSize,
                    fullDotSize,
                    Easing.CubicOut
                );

                // Create custom animation for height
                var heightAnimation = new Animation(
                    v => targetDot.HeightRequest = v,
                    smallDotSize,
                    fullDotSize,
                    Easing.CubicOut
                );

                // Combine animations
                var parentAnimation = new Animation();
                parentAnimation.Add(0, 1, widthAnimation);
                parentAnimation.Add(0, 1, heightAnimation);

                // Start animation
                parentAnimation.Commit(targetDot, "DotActivation", length: animationDuration);
                await Task.Delay((int)animationDuration);
            }
            else
            {
                // Animation from full size to small dot, then hide
                var widthAnimation = new Animation(
                    v => targetDot.WidthRequest = v,
                    fullDotSize,
                    smallDotSize,
                    Easing.CubicIn
                );

                var heightAnimation = new Animation(
                    v => targetDot.HeightRequest = v,
                    fullDotSize,
                    smallDotSize,
                    Easing.CubicIn
                );

                // Combine animations
                var parentAnimation = new Animation();
                parentAnimation.Add(0, 1, widthAnimation);
                parentAnimation.Add(0, 1, heightAnimation);

                // Start animation
                parentAnimation.Commit(targetDot, "DotDeactivation", length: animationDuration);
                await Task.Delay((int)animationDuration);
                
                targetDot.IsVisible = false;
            }

            System.Diagnostics.Debug.WriteLine($"Collapsed dot animation completed - Activating: {isActivating}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error animating collapsed dot transition: {ex.Message}");
        }
    }

    /// <summary>
    /// Animates the capsule indicator transition from dot to full capsule
    /// </summary>
    /// <param name="targetIndicator">The Border element to animate</param>
    /// <param name="isActivating">True if activating (dot to capsule), false if deactivating</param>
    private async Task AnimateIndicatorTransition(Border? targetIndicator, bool isActivating)
    {
        if (targetIndicator == null) return;

        try
        {
            const uint animationDuration = 300; // milliseconds
            const double dotSize = 4.0;
            const double capsuleHeight = 20.0;

            if (isActivating)
            {
                // Animation from dot to capsule: start as dot, grow to capsule
                targetIndicator.HeightRequest = dotSize;
                targetIndicator.WidthRequest = dotSize;
                targetIndicator.IsVisible = true;

                // Create custom animation for height
                var heightAnimation = new Animation(
                    v => targetIndicator.HeightRequest = v,
                    dotSize,
                    capsuleHeight,
                    Easing.CubicOut
                );

                // Create custom animation for width (stays same)
                var widthAnimation = new Animation(
                    v => targetIndicator.WidthRequest = v,
                    dotSize,
                    dotSize,
                    Easing.CubicOut
                );

                // Combine animations
                var parentAnimation = new Animation();
                parentAnimation.Add(0, 1, heightAnimation);
                parentAnimation.Add(0, 1, widthAnimation);

                // Start animation
                parentAnimation.Commit(targetIndicator, "CapsuleActivation", length: animationDuration);
                await Task.Delay((int)animationDuration);
            }
            else
            {
                // Animation from capsule to dot: shrink to dot, then hide
                var heightAnimation = new Animation(
                    v => targetIndicator.HeightRequest = v,
                    capsuleHeight,
                    dotSize,
                    Easing.CubicIn
                );

                var widthAnimation = new Animation(
                    v => targetIndicator.WidthRequest = v,
                    dotSize,
                    dotSize,
                    Easing.CubicIn
                );

                // Combine animations
                var parentAnimation = new Animation();
                parentAnimation.Add(0, 1, heightAnimation);
                parentAnimation.Add(0, 1, widthAnimation);

                // Start animation
                parentAnimation.Commit(targetIndicator, "CapsuleDeactivation", length: animationDuration);
                await Task.Delay((int)animationDuration);
                
                targetIndicator.IsVisible = false;
            }

            System.Diagnostics.Debug.WriteLine($"Indicator animation completed - Activating: {isActivating}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error animating indicator transition: {ex.Message}");
        }
    }

    #endregion
}
