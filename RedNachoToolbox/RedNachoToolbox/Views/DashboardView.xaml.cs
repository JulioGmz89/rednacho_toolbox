using Microsoft.Maui.Controls;
using RedNachoToolbox.ViewModels;
using RedNachoToolbox.Models;

namespace RedNachoToolbox.Views;

/// <summary>
/// Dashboard view that displays recently used tools and all available applications.
/// </summary>
public partial class DashboardView : ContentView
{
    /// <summary>
    /// Gets the MainViewModel from the binding context.
    /// </summary>
    private MainViewModel? ViewModel => BindingContext as MainViewModel;

    /// <summary>
    /// Initializes a new instance of the DashboardView class.
    /// </summary>
    public DashboardView()
    {
        InitializeComponent();
        // Subscribe to theme change notifications to refresh templates
        try
        {
            MessagingCenter.Subscribe<SettingsPage>(this, "ThemeChanged", (sender) =>
            {
                RefreshTemplates();
            });
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error subscribing to ThemeChanged: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the search button pressed event.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The event arguments</param>
    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        try
        {
            if (sender is SearchBar searchBar && ViewModel != null)
            {
                System.Diagnostics.Debug.WriteLine($"Search executed for: '{searchBar.Text}'");
                
                // The search is automatically handled by the SearchText binding in MainViewModel
                // Additional search logic can be added here if needed
                
                // Provide user feedback
                if (!string.IsNullOrWhiteSpace(searchBar.Text))
                {
                    DisplayAlert("Search", $"Searching for: {searchBar.Text}", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnSearchButtonPressed: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the search text changed event for real-time filtering.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The text changed event arguments</param>
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (sender is SearchBar searchBar && ViewModel != null)
            {
                System.Diagnostics.Debug.WriteLine($"Search text changed to: '{e.NewTextValue}'");
                
                // The filtering is automatically handled by the SearchText property binding
                // in the MainViewModel through the ApplyFilters method
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnSearchTextChanged: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the clear filters button click event.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The event arguments</param>
    private void OnClearFiltersClicked(object sender, EventArgs e)
    {
        try
        {
            if (ViewModel != null)
            {
                System.Diagnostics.Debug.WriteLine("Clearing all filters");
                
                ViewModel.ClearFilters();
                
                // Provide user feedback
                DisplayAlert("Filters Cleared", "All search filters have been cleared.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnClearFiltersClicked: {ex.Message}");
            DisplayAlert("Error", "Failed to clear filters. Please try again.", "OK");
        }
    }

    /// <summary>
    /// Handles the tool selection changed event for the main tools collection.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The selection changed event arguments</param>
    private async void OnToolSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection?.FirstOrDefault() is ToolInfo selectedTool && ViewModel != null)
            {
                System.Diagnostics.Debug.WriteLine($"Tool selected: {selectedTool.Name}");
                
                // Add to recently used tools
                ViewModel.AddToRecentlyUsed(selectedTool);

                // Publish message to host the tool inside MainPage content area (keep sidebar)
                try
                {
                    MessagingCenter.Send(this, "OpenTool", selectedTool);
                }
                catch (Exception msgEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error sending OpenTool message: {msgEx.Message}");
                }

                // Clear selection to allow reselection
                if (sender is CollectionView collectionView)
                {
                    collectionView.SelectedItem = null;
                }
                
                // Navigation handled by MainPage via MessagingCenter
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Failed to open the selected tool. Please try again.", "OK");
        }
    }

    /// <summary>
    /// Handles the recently used tool selection changed event.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The selection changed event arguments</param>
    private void OnRecentToolSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection?.FirstOrDefault() is ToolInfo selectedTool && ViewModel != null)
            {
                System.Diagnostics.Debug.WriteLine($"Recent tool selected: {selectedTool.Name}");
                
                // Update the recently used order (move to top)
                ViewModel.AddToRecentlyUsed(selectedTool);
                
                // Clear selection to allow reselection
                if (sender is CollectionView collectionView)
                {
                    collectionView.SelectedItem = null;
                }
                
                // Provide user feedback and future navigation placeholder
                DisplayAlert("Recent Tool Selected", $"Opening {selectedTool.Name}...\n\n{selectedTool.Description}", "OK");
                
                // TODO: Navigate to the specific tool page
                // await Shell.Current.GoToAsync($"tool?name={selectedTool.Name}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnRecentToolSelectionChanged: {ex.Message}");
            DisplayAlert("Error", "Failed to open the selected recent tool. Please try again.", "OK");
        }
    }

    /// <summary>
    /// Handles the add tool button click event (future feature).
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The event arguments</param>
    private void OnAddToolClicked(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Add tool button clicked");
            
            // Future feature: Open add tool dialog or page
            DisplayAlert("Add Tool", "This feature will be available in a future update.", "OK");
            
            // TODO: Implement add tool functionality
            // await Shell.Current.GoToAsync("addtool");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnAddToolClicked: {ex.Message}");
        }
    }

    /// <summary>
    /// Helper method to display alerts safely.
    /// </summary>
    /// <param name="title">The alert title</param>
    /// <param name="message">The alert message</param>
    /// <param name="cancel">The cancel button text</param>
    private async void DisplayAlert(string title, string message, string cancel)
    {
        try
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, cancel);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error displaying alert: {ex.Message}");
        }
    }

    /// <summary>
    /// Force-refreshes the ItemTemplate of the collection views so any DynamicResource
    /// based colors inside the DataTemplate re-evaluate after theme changes.
    /// </summary>
    private void RefreshTemplates()
    {
        try
        {
            var template = Application.Current?.Resources?["ToolCardTemplate"] as DataTemplate;

            if (template != null)
            {
                if (AllToolsCollectionView != null)
                {
                    var old = AllToolsCollectionView.ItemTemplate;
                    AllToolsCollectionView.ItemTemplate = null;
                    AllToolsCollectionView.ItemTemplate = template;
                }

                if (RecentToolsCollectionView != null)
                {
                    var old = RecentToolsCollectionView.ItemTemplate;
                    RecentToolsCollectionView.ItemTemplate = null;
                    RecentToolsCollectionView.ItemTemplate = template;
                }
            }
            // Invalidate measure to ensure visual update
            this.InvalidateMeasure();
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error refreshing templates: {ex.Message}");
        }
    }
}
