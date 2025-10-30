using RedNachoToolbox.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using RedNachoToolbox.Views;
using RedNachoToolbox.Models;
using RedNachoToolbox.Tools.MarkdownToPdf;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using RedNachoToolbox.Messaging;
using RedNachoToolbox.Services; // added for DI resolution

namespace RedNachoToolbox;

public partial class MainPage : ContentPage
{
    /// <summary>
    /// Gets the MainViewModel instance bound to this page
    /// </summary>
    public MainViewModel ViewModel { get; private set; }

    // Cached content views to avoid re-creating and measuring on every navigation
    private DashboardView? _dashboardView;
    private ProductivityView? _productivityView;
    private MarkdownToPdfView? _markdownToPdfView;

    // Prevent overlapping navigation actions that can cause flicker
    private bool _navBusy;

    // Debounce for search text changes
    private System.Threading.CancellationTokenSource? _searchCts;

    public MainPage(MainViewModel? vm = null)
    {
        InitializeComponent();
        // Resolver vía DI si no se inyectó explícitamente
        ViewModel = vm ?? ServiceHelper.GetRequiredService<MainViewModel>();
        BindingContext = ViewModel;
        // Initialize and cache main content views (lazy for Productivity)
        _dashboardView = new DashboardView { BindingContext = ViewModel };
        MainContentHost.Content = _dashboardView;

        // React to sidebar collapse/expand to keep parent active only when collapsed
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        // Subscribe to tool open messages from views via WeakReferenceMessenger
        try
        {
            WeakReferenceMessenger.Default.Register<OpenToolMessage>(this, (recipient, message) =>
            {
                ShowTool(message.Value);
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error subscribing to OpenTool: {ex.Message}");
        }

        // Subscribe to appearing event to refresh ViewModel state
        this.Appearing += OnPageAppearing;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsSidebarCollapsed))
        {
            if (ViewModel.IsSidebarCollapsed)
            {
                // Do not clear individual tool selection; in collapsed mode the list isn't visible.
                // Only ensure the parent (Productivity) is marked as active.
                if (ViewModel.ActivePage != "Productivity")
                {
                    ViewModel.SetActivePage("Productivity");
                }
                UpdateActiveButtonBackgrounds();
            }
        }
    }

    private void ShowTool(ToolInfo tool)
    {
        try
        {
            // Map known tools to ContentViews (keep sidebar)
            ContentView? view = null;
            if (tool.TargetType == typeof(MarkdownToPdfView) || tool.Name.Contains("Markdown", StringComparison.OrdinalIgnoreCase))
            {
                // Cache heavy WebView-based view to avoid re-initialization
                view = _markdownToPdfView ??= new MarkdownToPdfView();
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

            // Sync sidebar selection when a Productivity tool is opened from cards/views
            try
            {
                if (tool.Category == ToolCategory.Productivity)
                {
                    // Mark parent Productivity as active
                    if (ViewModel.ActivePage != "Productivity")
                    {
                        // Deactivate Dashboard indicators if needed
                        if (ViewModel.IsDashboardActive)
                        {
                            _ = AnimateIndicatorTransition(DashboardIndicatorCapsule, false);
                            _ = AnimateCollapsedDotTransition(DashboardIndicatorDotCollapsed, false);
                        }

                        ViewModel.SetActivePage("Productivity");
                        _ = AnimateIndicatorTransition(ProductivityIndicatorCapsule, true);
                        _ = AnimateCollapsedDotTransition(ProductivityIndicatorDotCollapsed, true);
                        UpdateActiveButtonBackgrounds();
                    }

                    // Only reflect child selection visually when expanded
                    if (!ViewModel.IsSidebarCollapsed)
                    {
                        _ = SelectProductivityToolInSidebarAsync(tool);
                    }
                }
            }
            catch (Exception syncEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error syncing sidebar selection for tool '{tool.Name}': {syncEx.Message}");
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
        // Debounce to avoid filtering on every keystroke
        _searchCts?.Cancel();
        _searchCts = new System.Threading.CancellationTokenSource();
        var token = _searchCts.Token;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(250, token);
                await Dispatcher.DispatchAsync(() =>
                {
                    ViewModel.SearchText = searchText;
                    System.Diagnostics.Debug.WriteLine($"Search text debounced -> {searchText}");
                });
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Debounced search error: {ex.Message}");
            }
        });
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

    // Productivity Hover Events (legacy alias) - mapped to Productivity state
    private void OnDocumentationPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            // Don't show hover effect if button is already active (same color)
            if (ViewModel.IsProductivityActive)
            {
                System.Diagnostics.Debug.WriteLine("Productivity (alias) hover entered - Button is active, no hover effect needed");
                return;
            }

            // Theme-aware hover color (same as active color)
            var hoverColor = ViewModel.IsDarkTheme
                ? Color.FromArgb("#2A2A2A") // Dark gray for dark theme
                : Color.FromArgb("#F5F5F5"); // Light gray for light theme

            frame.BackgroundColor = hoverColor;
            System.Diagnostics.Debug.WriteLine($"Productivity (alias) hover entered - Theme: {(ViewModel.IsDarkTheme ? "Dark" : "Light")}, Color: {hoverColor}");
        }
    }

    private void OnDocumentationPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            // Don't change color if button is active (should stay active color)
            if (ViewModel.IsProductivityActive)
            {
                System.Diagnostics.Debug.WriteLine("Productivity (alias) hover exited - Button is active, maintaining active color");
                return;
            }

            frame.BackgroundColor = Colors.Transparent;
            System.Diagnostics.Debug.WriteLine("Productivity (alias) hover exited - Returned to transparent");
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
            if (_navBusy)
            {
                if (sender is Frame f1) _ = PressFeedbackAsync(f1);
                return;
            }
            if (ViewModel.IsDashboardActive)
            {
                if (sender is Frame f2) _ = PressFeedbackAsync(f2);
                return;
            }
            _navBusy = true;

            // Enhanced visual feedback for Frame-based button with theme-aware colors
            if (sender is Frame frame) _ = PressFeedbackAsync(frame);

            // Animate previous active indicators to deactivate (if any)
            if (ViewModel.IsProductivityActive)
            {
                _ = AnimateIndicatorTransition(ProductivityIndicatorCapsule, false);
                _ = AnimateCollapsedDotTransition(ProductivityIndicatorDotCollapsed, false);
            }

            // Clear selection from Productivity tool list
            ClearAllProductivityToolActiveFlags();
            ClearProductivityListActiveStateUI();

            // Set Dashboard as active page
            ViewModel.SetActivePage("Dashboard");

            // Animate new active indicators (both expanded capsule and collapsed dot)
            _ = AnimateIndicatorTransition(DashboardIndicatorCapsule, true);
            _ = AnimateCollapsedDotTransition(DashboardIndicatorDotCollapsed, true);

            // Update active button backgrounds
            UpdateActiveButtonBackgrounds();

            // Clear any active filters to show all applications (Dashboard view)
            ViewModel.ClearFilters();
            System.Diagnostics.Debug.WriteLine("Dashboard view activated - filters cleared, set as active page");

            // Swap main content to Dashboard view
            if (_dashboardView == null)
                _dashboardView = new DashboardView { BindingContext = ViewModel };
            MainContentHost.Content = _dashboardView;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Dashboard navigation error: {ex.Message}");
            await DisplayAlert("Error", "Could not access Dashboard.", "OK");
        }
        finally
        {
            _navBusy = false;
        }
    }

    private async void OnProductivityClicked(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("OnProductivityClicked - Event handler called");
            if (_navBusy)
            {
                if (sender is Frame f1) _ = PressFeedbackAsync(f1);
                return;
            }
            if (ViewModel.IsProductivityActive)
            {
                if (sender is Frame f2) _ = PressFeedbackAsync(f2);
                return;
            }
            _navBusy = true;

            // Enhanced visual feedback for Frame-based button with theme-aware colors
            if (sender is Frame frame) _ = PressFeedbackAsync(frame);

            // Animate previous active indicators to deactivate (if any)
            if (ViewModel.IsDashboardActive)
            {
                _ = AnimateIndicatorTransition(DashboardIndicatorCapsule, false);
                _ = AnimateCollapsedDotTransition(DashboardIndicatorDotCollapsed, false);
            }

            // Clear selection from Productivity tool list (navigating to category root)
            ClearAllProductivityToolActiveFlags();
            ClearProductivityListActiveStateUI();

            // Set Productivity as active page
            ViewModel.SetActivePage("Productivity");

            // Animate new active indicators (both expanded capsule and collapsed dot)
            _ = AnimateIndicatorTransition(ProductivityIndicatorCapsule, true);
            _ = AnimateCollapsedDotTransition(ProductivityIndicatorDotCollapsed, true);

            // Update active button backgrounds
            UpdateActiveButtonBackgrounds();

            // Swap main content to Productivity view (lists only Productivity tools)
            // Ensure category is set to Productivity before hosting the view
            ViewModel.SelectedCategory = ToolCategory.Productivity;
            if (_productivityView == null)
                _productivityView = new ProductivityView { BindingContext = ViewModel };
            MainContentHost.Content = _productivityView;

            System.Diagnostics.Debug.WriteLine("Productivity view selected - set as active page and filtered by Productivity");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Productivity navigation error: {ex.Message}");
            await DisplayAlert("Error", "Could not access Productivity.", "OK");
        }
        finally
        {
            _navBusy = false;
        }
    }

    // Productivity Hover Events - No hover effect when already active
    private void OnProductivityPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            // Don't show hover effect if button is already active (same color)
            if (ViewModel.IsProductivityActive)
            {
                System.Diagnostics.Debug.WriteLine("Productivity hover entered - Button is active, no hover effect needed");
                return;
            }
            var hoverColor = ViewModel.IsDarkTheme
                ? Color.FromArgb("#2A2A2A") // Dark gray for dark theme
                : Color.FromArgb("#F5F5F5"); // Light gray for light theme
            frame.BackgroundColor = hoverColor;
            System.Diagnostics.Debug.WriteLine($"Productivity hover entered - Theme: {(ViewModel.IsDarkTheme ? "Dark" : "Light")}, Color: {hoverColor}");
        }
    }

    private void OnProductivityPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            // Don't change color if button is active (should stay active color)
            if (ViewModel.IsProductivityActive)
            {
                System.Diagnostics.Debug.WriteLine("Productivity hover exited - Button is active, maintaining active color");
                return;
            }
            frame.BackgroundColor = Colors.Transparent;
            System.Diagnostics.Debug.WriteLine("Productivity hover exited - Returned to transparent");
        }
    }

    // Chevron toggle for Productivity category list in sidebar
    private async void OnProductivityChevronTapped(object sender, EventArgs e)
    {
        try
        {
            bool expand = !ViewModel.IsProductivityExpanded;
            ViewModel.IsProductivityExpanded = expand;

            // Rotate chevron
            if (ProductivityChevronImage != null)
            {
                await ProductivityChevronImage.RotateTo(expand ? 180 : 0, 150, Easing.CubicInOut);
            }

            // Fade list in/out
            if (ProductivityListContainer != null)
            {
                if (expand)
                {
                    ProductivityListContainer.Opacity = 0;
                    ProductivityListContainer.IsVisible = true;
                    await ProductivityListContainer.FadeTo(1, 150, Easing.CubicInOut);
                }
                else
                {
                    await ProductivityListContainer.FadeTo(0, 150, Easing.CubicInOut);
                    ProductivityListContainer.IsVisible = false;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error toggling Productivity chevron: {ex.Message}");
        }
    }

    // Handle taps on individual Productivity tools inside the expanded sidebar list
    private void OnProductivitySidebarToolTapped(object sender, EventArgs e)
    {
        try
        {
            // Apply pressed visual feedback similar to other sidebar buttons
            if (sender is Frame frame) _ = PressFeedbackAsync(frame);

            if (sender is Element el && el.BindingContext is ToolInfo tool)
            {
                // Activate this tool, deactivate others (UI + data)
                ClearAllProductivityToolActiveFlags();
                ClearProductivityListActiveStateUI();

                tool.IsActive = true;

                // Animate the tool's capsule indicator if accessible
                if (sender is Frame toolFrame)
                {
                    var capsule = toolFrame.FindByName<Border>("ToolIndicatorCapsule");
                    _ = AnimateIndicatorTransition(capsule, true);
                    // Do not keep background; only capsule indicates active
                    toolFrame.BackgroundColor = Colors.Transparent;
                }

                // Add to recently used, then open tool just like from Dashboard
                ViewModel?.AddToRecentlyUsed(tool);
                ShowTool(tool);
                System.Diagnostics.Debug.WriteLine($"Opened tool from Productivity sidebar list: {tool.Name}");

                // Ensure parent remains active (for collapsed state indicators)
                if (ViewModel != null && ViewModel.ActivePage != "Productivity")
                {
                    // Deactivate Dashboard indicators if needed
                    if (ViewModel.IsDashboardActive)
                    {
                        _ = AnimateIndicatorTransition(DashboardIndicatorCapsule, false);
                        _ = AnimateCollapsedDotTransition(DashboardIndicatorDotCollapsed, false);
                    }

                    ViewModel.SetActivePage("Productivity");
                    // Animate parent Productivity indicators to active
                    _ = AnimateIndicatorTransition(ProductivityIndicatorCapsule, true);
                    _ = AnimateCollapsedDotTransition(ProductivityIndicatorDotCollapsed, true);
                    UpdateActiveButtonBackgrounds();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling Productivity sidebar tool tap: {ex.Message}");
        }
    }

    private void OnProductivitySidebarToolPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            var hoverColor = ViewModel.IsDarkTheme
                ? Color.FromArgb("#2A2A2A") // Dark gray for dark theme
                : Color.FromArgb("#F5F5F5"); // Light gray for light theme
            frame.BackgroundColor = hoverColor;
        }
    }

    private void OnProductivitySidebarToolPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            frame.BackgroundColor = Colors.Transparent;
        }
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("OnSettingsClicked - Event handler called");

            // Enhanced visual feedback for Frame-based button with theme-aware colors
            if (sender is Frame frame) _ = PressFeedbackAsync(frame);

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
    /// Lightweight, non-blocking press feedback for sidebar frames.
    /// Runs asynchronously and does not block navigation logic.
    /// </summary>
    private async Task PressFeedbackAsync(Frame frame)
    {
        try
        {
            var pressedColor = ViewModel.IsDarkTheme
                ? Color.FromArgb("#404040")
                : Color.FromArgb("#E0E0E0");
            var hoverColor = ViewModel.IsDarkTheme
                ? Color.FromArgb("#2A2A2A")
                : Color.FromArgb("#F5F5F5");

            frame.BackgroundColor = pressedColor;
            frame.Opacity = 0.9;
            await Task.Delay(120);

            frame.BackgroundColor = hoverColor;
            frame.Opacity = 1.0;
            await Task.Delay(80);

            // If button becomes active, UpdateActiveButtonBackgrounds will set final color.
            // Otherwise, return to transparent.
            if (!(ReferenceEquals(frame, DashboardButtonFrame) && ViewModel.IsDashboardActive)
                && !(ReferenceEquals(frame, ProductivityButtonFrame) && ViewModel.IsProductivityActive))
            {
                frame.BackgroundColor = Colors.Transparent;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PressFeedbackAsync error: {ex.Message}");
        }
    }

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

            if (ProductivityButtonFrame != null)
            {
                ProductivityButtonFrame.BackgroundColor = ViewModel.IsProductivityActive ? activeColor : transparentColor;
            }

            // Update collapsed buttons
            if (DashboardButtonFrameCollapsed != null)
            {
                DashboardButtonFrameCollapsed.BackgroundColor = ViewModel.IsDashboardActive ? activeColor : transparentColor;
            }

            if (ProductivityButtonFrameCollapsed != null)
            {
                ProductivityButtonFrameCollapsed.BackgroundColor = ViewModel.IsProductivityActive ? activeColor : transparentColor;
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

    private async Task SelectProductivityToolInSidebarAsync(ToolInfo tool)
    {
        try
        {
            // Expand the Productivity list if not already
            if (!ViewModel.IsProductivityExpanded)
            {
                ViewModel.IsProductivityExpanded = true;
                // Allow UI to create item views
                await Task.Delay(50);
            }

            // Clear current selection and UI, then set the matching tool active
            ClearAllProductivityToolActiveFlags();
            ClearProductivityListActiveStateUI();
            var tools = ViewModel.ProductivityTools?.ToList() ?? new List<ToolInfo>();
            var index = tools.FindIndex(t => string.Equals(t.Name, tool.Name, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                tools[index].IsActive = true;

                // Try to update the visuals for the corresponding item
                _ = Dispatcher.DispatchAsync(async () =>
                {
                    try
                    {
                        // Ensure children are realized
                        if (ProductivityListContainer?.Children?.Count <= index)
                        {
                            await Task.Delay(50);
                        }

                        if (ProductivityListContainer?.Children != null && ProductivityListContainer.Children.Count > index)
                        {
                            if (ProductivityListContainer.Children[index] is Frame itemFrame)
                            {
                                // Do not keep background; only capsule indicates active
                                itemFrame.BackgroundColor = Colors.Transparent;

                                var cap = itemFrame.FindByName<Border>("ToolIndicatorCapsule");
                                if (cap != null)
                                {
                                    _ = AnimateIndicatorTransition(cap, true);
                                }
                            }
                        }
                    }
                    catch (Exception uiex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error updating sidebar visuals for tool '{tool.Name}': {uiex.Message}");
                    }
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error selecting Productivity tool in sidebar: {ex.Message}");
        }
    }

    private void ClearAllProductivityToolActiveFlags()
    {
        try
        {
            foreach (var t in ViewModel.ProductivityTools.ToList())
            {
                if (t.IsActive)
                    t.IsActive = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error clearing Productivity tool active flags: {ex.Message}");
        }
    }

    private void ClearProductivityListActiveStateUI()
    {
        try
        {
            if (ProductivityListContainer?.Children == null) return;
            foreach (var child in ProductivityListContainer.Children)
            {
                if (child is Frame f)
                {
                    f.BackgroundColor = Colors.Transparent;
                    var cap = f.FindByName<Border>("ToolIndicatorCapsule");
                    if (cap != null) cap.IsVisible = false;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error clearing Productivity list UI states: {ex.Message}");
        }
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
            const uint animationDuration = 150; // faster, smoother

            if (isActivating)
            {
                targetDot.Opacity = 0;
                targetDot.IsVisible = true;
                await targetDot.FadeTo(1, animationDuration, Easing.CubicOut);
            }
            else
            {
                await targetDot.FadeTo(0, animationDuration, Easing.CubicIn);
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
            const uint animationDuration = 180; // faster fade for responsiveness

            if (isActivating)
            {
                targetIndicator.Opacity = 0;
                targetIndicator.IsVisible = true;
                await targetIndicator.FadeTo(1, animationDuration, Easing.CubicOut);
            }
            else
            {
                await targetIndicator.FadeTo(0, animationDuration, Easing.CubicIn);
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
